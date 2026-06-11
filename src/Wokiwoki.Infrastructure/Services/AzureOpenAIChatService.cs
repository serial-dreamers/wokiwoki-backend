using Azure;
using Azure.AI.OpenAI;
using Google.Apis.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using System.Text.Json;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.DTOs;

namespace Wokiwoki.Infrastructure.Services
{
	public class AzureOpenAIChatService : IAzureOpenAIChatService
	{
		private readonly ChatClient _gptClient;
		private readonly HttpClient _httpClient;
		private readonly string _geminiApiKey;
		private readonly WokiwokiDbContext _dbContext;
		private readonly IGoongMapService _goongMapService;
		private readonly IConfiguration _configuration;


		public AzureOpenAIChatService(WokiwokiDbContext dbContext, IConfiguration config, IGoongMapService goongMapService)
		{
			_configuration = config;
			var endpoint = new Uri(config["AzureOpenAI:Endpoint"]!);
			var apiKey = config["AzureOpenAI:ApiKey"]!;
			var azureClient = new AzureOpenAIClient(endpoint, new AzureKeyCredential(apiKey));
			_gptClient = azureClient.GetChatClient("gpt-4o-mini");

			_geminiApiKey = config["Gemini:ApiKey"]!;
			_httpClient = new HttpClient();
			_goongMapService = goongMapService;

			_dbContext = dbContext;
		}
		 
		public async Task<ChatResponse> ChatAsync(string userMessage, string? userId)
		{
			Console.WriteLine($"\n{new string('=', 60)}");
			Console.WriteLine($"USER: {userMessage}");
			Console.WriteLine($"UserId: {userId ?? "Anonymous"}");
			Console.WriteLine($"{new string('=', 60)}\n");
			 
			List<ChatMessageDto> history = new List<ChatMessageDto>();
			ConversationChat? conversation = null;
			 
			if (!string.IsNullOrEmpty(userId))
			{
				conversation = await LoadOrCreateConversationAsync(userId);
				history = conversation.MessagesChats
					.OrderByDescending(m => m.Created)
					.Take(3)
					.OrderBy(m => m.Created) // Đảo lại thứ tự đúng
					.Select(m => new ChatMessageDto { Role = m.Role, Content = m.Content })
					.ToList();
			}
			else
			{
				Console.WriteLine($" Anonymous user - no history loaded");
			}

			// ==================== STEP 1: AI EXTRACTS LOCATION ====================
			var locationInfo = await ExtractLocationWithAI(userMessage);

			if (locationInfo != null)
			{
				Console.WriteLine($"Location: {locationInfo.LocationText} ({locationInfo.SearchType})");
			}

			// ==================== STEP 2: DETECT SEARCH TARGET ====================
			var searchTarget = DetectSearchTarget(userMessage);

			if (searchTarget == "general" || searchTarget == "booking_guide")
			{
				var response = await FormatWithGemini(userMessage, "", null, history, searchTarget);
				if (!string.IsNullOrEmpty(userId) && conversation != null)
					await SaveMessagesAsync(conversation, userId, userMessage, response);
				return new ChatResponse { Message = response, Markdown = response };
			}

			var gptPrompt = BuildGPTSqlPrompt(userMessage, history, locationInfo, searchTarget);
			var gptMessages = new List<ChatMessage>
	{
		new SystemChatMessage(GetDatabaseSchema()),
		new SystemChatMessage(gptPrompt)
	};

			// ✅ Only add history if user is logged in AND has history
			const int CONTEXT_MESSAGE_COUNT = 6; // 3 cặp hỏi-đáp

			if (!string.IsNullOrEmpty(userId) && history.Any())
			{
				var recentMessages = history
					.TakeLast(CONTEXT_MESSAGE_COUNT)
					.ToList();

				Console.WriteLine($"📤 Sending {recentMessages.Count} context messages to GPT");

				foreach (var msg in recentMessages)
				{
					// Truncate dài hơn: 500 chars thay vì 200
					var shortContent = msg.Content.Length > 500
						? msg.Content.Substring(0, 497) + "..."
						: msg.Content;

					// Remove [SEARCH_TYPE: xxx] tag from assistant messages
					if (msg.Role == "assistant")
					{
						shortContent = System.Text.RegularExpressions.Regex
							.Replace(shortContent, @"^\[SEARCH_TYPE:\s*\w+\]\s*", "")
							.Trim();
					}

					gptMessages.Add(msg.Role == "user"
						? new UserChatMessage(shortContent)
						: new AssistantChatMessage(shortContent));
				}
			}

			gptMessages.Add(new UserChatMessage(userMessage));

			var gptOptions = new ChatCompletionOptions
			{
				Temperature = 0.2f,
				MaxOutputTokenCount = 1500
			};

			var gptResponse = await _gptClient.CompleteChatAsync(gptMessages, gptOptions);
			var gptOutput = gptResponse.Value.Content[0].Text;
			Console.WriteLine($"GPT Output:\n{gptOutput}\n");

			var sqlQuery = ExtractSqlFromResponse(gptOutput);

			if (string.IsNullOrEmpty(sqlQuery))
			{
				bool shouldHaveSQL = searchTarget == "organization" ||
						 searchTarget == "workshop" ||
						 searchTarget == "mixed";

				if (searchTarget == "booking_guide")
				{
					Console.WriteLine("Detected: Booking guide request");
					var bookingResponse = await FormatWithGemini(userMessage, gptOutput, null, history, "booking_guide");

					if (!string.IsNullOrEmpty(userId) && conversation != null)
					{
						await SaveMessagesAsync(conversation, userId, userMessage, bookingResponse);
					}

					return new ChatResponse { Message = bookingResponse, Markdown = bookingResponse };
				}
				 
				if (searchTarget == "general")
				{
					Console.WriteLine("Detected: General conversation");
					var generalResponse = await FormatWithGemini(userMessage, gptOutput, null, history, "general");

					if (!string.IsNullOrEmpty(userId) && conversation != null)
					{
						await SaveMessagesAsync(conversation, userId, userMessage, generalResponse);
					}

					return new ChatResponse { Message = generalResponse, Markdown = generalResponse };
				}

				if (shouldHaveSQL)
				{
					sqlQuery = searchTarget == "organization"
						? "SELECT o.id, o.name, o.description, o.logourl, o.contactemail, o.contactphone, o.street, o.commune, o.province FROM organization o WHERE o.isactive = true LIMIT 3"
						: "SELECT w.id, w.title, w.summary, w.imageurl, w.startingprice, w.deliverytype, w.displayaddress, w.organizationid FROM workshop w WHERE w.isactive = true AND w.status = 2 LIMIT 3";

					// Apply location filter if exists
					if (locationInfo != null)
					{
						sqlQuery = ApplyLocationFilter(sqlQuery, locationInfo, searchTarget);
					}
				}
				else
				{
					Console.WriteLine("Confirmed: Casual conversation (no search keywords)");
					var casualResponse = await FormatWithGemini(userMessage, gptOutput, null, history, "casual");

					// ✅ Only save if userId is provided
					if (!string.IsNullOrEmpty(userId) && conversation != null)
					{
						await SaveMessagesAsync(conversation, userId, userMessage, casualResponse);
					}

					return new ChatResponse { Message = casualResponse, Markdown = casualResponse };
				}
			}

			// ==================== STEP 4: APPLY LOCATION FILTER ====================
			if (locationInfo != null)
			{
				sqlQuery = ApplyLocationFilter(sqlQuery, locationInfo, searchTarget);
				Console.WriteLine($"📍 Applied {locationInfo.SearchType} filter");
			}

			Console.WriteLine($"Final SQL:\n{sqlQuery}\n");

			// ==================== STEP 5: EXECUTE SQL ====================
			string jsonResult;
			try
			{
				jsonResult = await ExecuteSqlQueryAsync(sqlQuery);
				Console.WriteLine($" Query executed");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"SQL Error: {ex.Message}");
				jsonResult = JsonSerializer.Serialize(new { error = ex.Message });
			}

			// ==================== STEP 6: FORMAT WITH GEMINI ====================
			Console.WriteLine("Formatting...");
			var finalResponse = await FormatWithGemini(userMessage, gptOutput, jsonResult, history, searchTarget);

			// ✅ Only save if userId is provided
			if (!string.IsNullOrEmpty(userId) && conversation != null)
			{
				await SaveMessagesAsync(conversation, userId, userMessage, finalResponse);
			}
			else
			{
				Console.WriteLine($"💬 Anonymous chat - messages not saved");
			}

			return new ChatResponse
			{
				Message = finalResponse,
				Markdown = finalResponse
			};
		}

		// ==================== DETECT SEARCH TARGET ====================
		private string DetectSearchTarget(string userMessage)
		{
			var lowerMsg = userMessage.ToLower();

			// Organization keywords
			var orgKeywords = new[] {
		"tổ chức", "to chuc", "organization", "organizer",
		"công ty", "cong ty", "trung tâm", "trung tam",
		"center", "đơn vị", "don vi",
		"nhà tổ chức", "nha to chuc",
		"doanh nghiệp", "doanh nghiep"  // ← THÊM
    };

			// Workshop keywords
			var workshopKeywords = new[] {
				"workshop", "khóa học", "khoa hoc", "lớp học", "lop hoc",
				"course", "class", "khóa", "lớp", "buổi học"
			};

			var bookingKeywords = new[] {
		"đặt vé", "dat ve", "mua vé", "mua ve", "booking",
		"đăng ký", "dang ky", "register", "tham gia",
		"cách đặt", "cach dat", "how to book", "hướng dẫn đặt",
		"huong dan dat", "thanh toán", "thanh toan", "payment"
			};

			var generalKeywords = new[] {
		"xin chào", "chao", "hello", "hi", "hey",
		"cảm ơn", "cam on", "thank", "thanks",
		"bạn là ai", "ban la ai", "who are you",
		"giúp", "giup", "help", "hỗ trợ", "ho tro",
		"wokiwoki là gì", "wokiwoki la gi"
	};

			var hasOrgKeyword = orgKeywords.Any(k => lowerMsg.Contains(k));
			var hasWorkshopKeyword = workshopKeywords.Any(k => lowerMsg.Contains(k));
			var hasBookingKeyword = bookingKeywords.Any(k => lowerMsg.Contains(k));
			var hasGeneralKeyword = generalKeywords.Any(k => lowerMsg.Contains(k));

			if (hasBookingKeyword)
				return "booking_guide";

			if (hasGeneralKeyword && !hasWorkshopKeyword && !hasOrgKeyword)
				return "general";


			if (hasOrgKeyword && !hasWorkshopKeyword)
				return "organization";

			if (hasWorkshopKeyword)
				return "workshop";

			if (hasOrgKeyword && hasWorkshopKeyword)
				return "mixed";

			// Mixed search if both or neither
			var searchIndicators = new[] { "tìm", "tim", "find", "search", "ở", "tại", "near", "gần" };
			if (searchIndicators.Any(k => lowerMsg.Contains(k)))
				return "workshop";

			return "general";
		}

		// ==================== AI LOCATION EXTRACTION ====================
		private async Task<LocationInfo?> ExtractLocationWithAI(string userMessage)
		{ 

			var prompt = $@"Extract location from Vietnamese text. Return ONLY JSON.

TEXT: ""{userMessage}""

RULES:
1. Extract MOST SPECIFIC location (landmark > street > district > city)
2. Normalize: ""Sài Gòn""/""HCM"" → ""TP Hồ Chí Minh"", ""Q1"" → ""Quận 1""
3. For districts, add "", TP Hồ Chí Minh""
4. Keep landmark names as-is

JSON FORMAT:
{{""hasLocation"":true/false,""locationQuery"":""search string"",""locationType"":""landmark|street|district|city"",""confidence"":""high|medium|low""}}

EXAMPLES:
""workshop ở quận 1"" → {{""hasLocation"":true,""locationQuery"":""Quận 1, TP Hồ Chí Minh"",""locationType"":""district"",""confidence"":""high""}}
""gần chợ bến thành"" → {{""hasLocation"":true,""locationQuery"":""Chợ Bến Thành, Quận 1, TP Hồ Chí Minh"",""locationType"":""landmark"",""confidence"":""high""}}
""tìm workshop online"" → {{""hasLocation"":false}}";

			try
			{
				var geminiResponse = await CallGeminiApiAsync(prompt);
				var cleanJson = geminiResponse.Trim().Replace("```json", "").Replace("```", "").Trim();

				var extraction = JsonSerializer.Deserialize<LocationExtractionResult>(cleanJson,
					new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

				if (extraction?.HasLocation == true && !string.IsNullOrEmpty(extraction.LocationQuery))
				{
					Console.WriteLine($"📍 Extracted: {extraction.LocationQuery}");

					// Get coordinates from Goong Maps
					var coordinates = await _goongMapService.GetCoordinatesAsync(extraction.LocationQuery);

					if (coordinates == null)
					{
						Console.WriteLine($"⚠️ No coordinates, using text search");
						return new LocationInfo
						{
							LocationText = extraction.LocationQuery,
							LocationType = extraction.LocationType ?? "address",
							SearchType = "text_search",
							Latitude = null,
							Longitude = null
						};
					}

					var (lat, lng) = coordinates.Value;
					Console.WriteLine($"Coordinates: {lat}, {lng}");

					var searchType = DetermineSearchType(extraction.LocationType, extraction.Confidence);

					return new LocationInfo
					{
						LocationText = extraction.LocationQuery,
						LocationType = extraction.LocationType ?? "address",
						SearchType = searchType,
						Latitude = lat,
						Longitude = lng
					};
				}

				return null;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Location extraction failed: {ex.Message}");
				return null;
			}
		}

		private string DetermineSearchType(string? locationType, string? confidence)
		{
			return locationType?.ToLower() switch
			{
				"landmark" => "coordinate_tight",      // 2km
				"street" => "coordinate_tight",        // 2km
				"district" => "coordinate_medium",     // 5km
				"city" => "coordinate_wide",           // 10km
				_ => confidence == "low" ? "text_search" : "coordinate_medium"
			};
		}

		// ==================== APPLY LOCATION FILTER ====================
		private string ApplyLocationFilter(string sqlQuery, LocationInfo locationInfo, string searchTarget)
		{
			if (!sqlQuery.Contains("[LOCATION_FILTER]"))
			{
				var whereIndex = sqlQuery.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase);
				if (whereIndex != -1)
				{
					var insertPoint = sqlQuery.IndexOf("GROUP BY", whereIndex, StringComparison.OrdinalIgnoreCase);
					if (insertPoint == -1) insertPoint = sqlQuery.IndexOf("ORDER BY", whereIndex, StringComparison.OrdinalIgnoreCase);
					if (insertPoint == -1) insertPoint = sqlQuery.IndexOf("LIMIT", whereIndex, StringComparison.OrdinalIgnoreCase);

					if (insertPoint != -1)
					{
						sqlQuery = sqlQuery.Insert(insertPoint, "\n    [LOCATION_FILTER]\n");
					}
				}
			}

			string locationFilter;

			if (locationInfo.Latitude.HasValue && locationInfo.Longitude.HasValue)
			{
				var radiusKm = locationInfo.SearchType switch
				{
					"coordinate_tight" => 2.0,
					"coordinate_medium" => 5.0,
					"coordinate_wide" => 10.0,
					_ => 5.0
				};

				// For workshop search, filter by workshop location
				if (searchTarget == "workshop" || searchTarget == "mixed")
				{
					locationFilter = $@"AND w.latitude IS NOT NULL 
    AND w.longitude IS NOT NULL
    AND (6371 * acos(
        cos(radians({locationInfo.Latitude.Value})) * cos(radians(w.latitude)) * 
        cos(radians(w.longitude) - radians({locationInfo.Longitude.Value})) + 
        sin(radians({locationInfo.Latitude.Value})) * sin(radians(w.latitude))
    )) <= {radiusKm}";
				}
				// For organization search, filter by organization location (use province for now)
				else
				{
					locationFilter = $"AND o.province ILIKE '%{SanitizeInput(locationInfo.LocationText)}%'";
				}
			}
			else
			{
				// Text search fallback
				if (searchTarget == "workshop" || searchTarget == "mixed")
				{
					locationFilter = $"AND w.displayaddress ILIKE '%{locationInfo.LocationText}%'";
				}
				else
				{
					locationFilter = $"AND (o.street ILIKE '%{locationInfo.LocationText}%' OR o.commune ILIKE '%{locationInfo.LocationText}%' OR o.province ILIKE '%{locationInfo.LocationText}%')";
				}
			}

			return sqlQuery.Replace("[LOCATION_FILTER]", locationFilter);
		}

		private string SanitizeInput(string input)
		{
			return input.Replace("'", "''").Replace(";", "").Replace("--", "");
		}


		// ==================== DATABASE SCHEMA ====================
		private static readonly string _cachedDatabaseSchema = @"# DATABASE SCHEMA

## Table: workshop
- id (uuid, PK)
- title (varchar)
- summary (text)
- imageurl (varchar)
- deliverytype (int4: 0=Online, 1=Offline, 2=Hybrid)
- startingprice (decimal)
- displayaddress (varchar)
- organizationid (uuid, FK)
- categoryid (uuid, FK)
- averagerating (decimal)
- isactive (boolean)
- status (int4: 0=Draft, 1=PendingReview, 2=Published, 3=Hidden, 4=Cancelled)
- latitude (numeric)
- longitude (numeric)

## Table: organization
- id (uuid, PK)
- name (varchar)
- description (text)
- logourl (varchar)
- contactemail (varchar)
- contactphone (varchar)
- street (varchar)
- commune (varchar)
- province (varchar)
- isactive (boolean)

## Table: category
- id (uuid, PK)
- name (varchar)
- description (text)

## Table: tag
- id (uuid, PK)
- name (varchar)

## Table: workshop_tag (many-to-many)
- WorkshopsId (uuid, FK)
- TagsId (uuid, FK)

## Table: workshop_schedule
- id (uuid, PK)
- workshopid (uuid, FK)
- recurrencetype (enum: 0=OneTime, 1=Daily, 2=Weekly, 3=Monthly)
- starttime (time)
- endtime (time)

## Table: review
- id (uuid, PK)
- workshopid (uuid, FK)
- userid (varchar)
- rating (int, 1-5)
- comment (text)
- created (timestamp)

**NOTES:**
- Active workshops: isactive = true AND status = 2
- Active organizations: isactive = true
- Location search uses [LOCATION_FILTER] placeholder
- No JOIN organization for workshop queries
- ⚠️ NEVER SELECT description column from workshop table (it's too long and wastes tokens)";

		private string GetDatabaseSchema() => _cachedDatabaseSchema;

		// ==================== GPT SQL PROMPT ====================
		private string BuildGPTSqlPrompt(string userMessage, List<ChatMessageDto> history, LocationInfo? locationInfo, string searchTarget)
		{

			if (searchTarget == "general" || searchTarget == "booking_guide")
			{
				return $@"# CONVERSATIONAL ASSISTANT

USER MESSAGE: {userMessage}
QUERY TYPE: {searchTarget.ToUpper()}

This is NOT a database search request. This is a {searchTarget} conversation.

OUTPUT:
ANALYSIS: This is a {searchTarget} query, no SQL needed";
			}


			var needsContext = new[] { "còn", "thêm", "khác", "nữa", "more", "other" }
				.Any(kw => userMessage.ToLower().Contains(kw));

			var recentContext = needsContext && history.Any()
				? string.Join("\n", history.TakeLast(2).Select(h =>
				{
					var shortContent = h.Content.Length > 100
						? h.Content.Substring(0, 97) + "..."
						: h.Content;
					return $"{h.Role}: {shortContent}";
				}))
				: "No previous context";

			var locationContext = locationInfo != null
				? $"\n📍 LOCATION: {locationInfo.LocationText} ({locationInfo.SearchType}) - Use [LOCATION_FILTER]"
				: "";

			var templateSection = searchTarget switch
			{
				"organization" => @"
**CRITICAL: THIS IS A DATABASE SEARCH REQUEST - YOU MUST GENERATE SQL!**
User is asking for organizations from the DATABASE. This is NOT casual chat!

**ORGANIZATION SEARCH - MANDATORY SQL OUTPUT:**
YOU MUST generate this exact SQL structure:
```sql
SELECT o.id, o.name, o.description, o.logourl, o.contactemail, 
       o.contactphone, o.street, o.commune, o.province
FROM organization o
WHERE o.isactive = true
    [LOCATION_FILTER]
ORDER BY o.name ASC
LIMIT 3;
```",
				"mixed" => @"
**MIXED SEARCH (Workshops + Organizations):**
Generate 2 separate queries:
1. Workshop query (limit 3)
2. Organization query (limit 3)
Return both with clear labels: WORKSHOP_QUERY: and ORGANIZATION_QUERY:",
				_ => @"
**WORKSHOP SEARCH TEMPLATE:**
CRITICAL RULES:
1. Table name is 'workshop' (NOT 'workshops' - no 's'!)
2. MUST include: isactive = true AND status = 2
3. MUST use exact column names from schema
```sql
SELECT w.id, w.title, w.summary, w.imageurl, w.startingprice, 
       w.deliverytype, w.displayaddress, w.organizationid
FROM workshop w
WHERE w.isactive = true AND w.status = 2
    [LOCATION_FILTER]
ORDER BY w.averagerating DESC
LIMIT 3;
```

- Do NOT select w.description - it's too long and wastes tokens!
- Do NOT JOIN organization - just return organizationid"
			};

			return $@"# SQL QUERY GENERATOR

CONTEXT: {recentContext}{locationContext}
SEARCH TARGET: {searchTarget.ToUpper()}

TASK: Generate SQL for {searchTarget} search. ALWAYS LIMIT to 3 results max.
{templateSection}

FILTERS:
- Keyword: AND (column ILIKE '%kw%')
- Category: JOIN category c ON w.categoryid = c.id WHERE c.name ILIKE '%category%'
- Tag: JOIN workshop_tag wt ON w.id = wt.workshopsid JOIN tag t ON wt.tagsid = t.id WHERE t.name ILIKE '%tag%'
- Price (workshops): AND w.startingprice <= 500000
- Type (workshops): AND w.deliverytype = 0 (0=Online, 1=Offline, 2=Hybrid)
- Location: Use [LOCATION_FILTER] placeholder

OUTPUT:
ANALYSIS: [1 sentence]
SQL:
```sql
[query]
```

For casual chat: Return 'ANALYSIS: Casual chat, no SQL needed'";
		}

		// ==================== EXTRACT SQL ====================
		private string ExtractSqlFromResponse(string gptOutput)
		{
			if (gptOutput.Contains("Casual chat, no SQL needed", StringComparison.OrdinalIgnoreCase))
			{
				return string.Empty;
			}

			var sqlStart = gptOutput.IndexOf("```sql", StringComparison.OrdinalIgnoreCase);
			if (sqlStart == -1) return string.Empty;

			sqlStart += 6;
			var sqlEnd = gptOutput.IndexOf("```", sqlStart);
			if (sqlEnd == -1) return string.Empty;

			return gptOutput.Substring(sqlStart, sqlEnd - sqlStart).Trim();
		}

		// ==================== EXECUTE SQL ====================
		private async Task<string> ExecuteSqlQueryAsync(string sqlQuery)
		{
			try
			{
				var connection = _dbContext.Database.GetDbConnection();
				await connection.OpenAsync();

				using var command = connection.CreateCommand();
				command.CommandText = sqlQuery;

				using var reader = await command.ExecuteReaderAsync();
				var results = new List<Dictionary<string, object>>();

				while (await reader.ReadAsync())
				{
					var row = new Dictionary<string, object>();
					for (int i = 0; i < reader.FieldCount; i++)
					{
						var value = reader.GetValue(i);
						row[reader.GetName(i)] = value is DBNull ? null : value;
					}
					results.Add(row);
				}

				await connection.CloseAsync();

				return JsonSerializer.Serialize(results, new JsonSerializerOptions
				{
					WriteIndented = false,
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase
				});
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException($"SQL execution failed: {ex.Message}", ex);
			}
		}

		// ==================== GEMINI FORMATTING ====================
		private async Task<string> FormatWithGemini(string userMessage, string gptAnalysis, string? jsonData, List<ChatMessageDto> history, string searchTarget)
		{
			var isVietnamese = IsVietnameseQuery(userMessage);
			var hasData = !string.IsNullOrEmpty(jsonData) && !jsonData.Contains("\"error\"");



			var cleanAnalysis = ExtractAnalysisOnly(gptAnalysis);
			var limitedJsonData = hasData ? LimitJsonSize(jsonData) : "No data";

			var formatInstructions = searchTarget switch
			{
				"workshop" => @"
**CRITICAL: THIS IS A DATABASE SEARCH REQUEST - YOU MUST GENERATE SQL!**
**WORKSHOP FORMAT:**
**START YOUR RESPONSE WITH THIS EXACT TAG (for parsing):** [SEARCH_TYPE: workshop]
```markdown
## 🎯 Workshops Found (X results)
[Write 1-2 sentences introducing the workshops - explain what makes them suitable, highlight themes, or provide context]

### 1. [Title](workshop-id)
![Image](imageurl)

**Price:** XXX VND | **Type:** Online/Offline/Hybrid

**Location:** [displayaddress]

[summary]

---

### 2. [Next workshop...]

---

[Write 1-2 sentences as closing remarks with helpful suggestions]
```

**DATA FIELDS:**
- id, title, summary, imageurl, startingprice, deliverytype, displayaddress, organization_name
- deliverytype: 0=Online, 1=Offline, 2=Hybrid
- Note: organizationid is UUID only, do NOT try to display organization name",

				"organization" => @"
**ORGANIZATION FORMAT:**
**START YOUR RESPONSE WITH THIS EXACT TAG (for parsing):** [SEARCH_TYPE: organization]
```markdown
## 🏢 Organizations Found (X results)
[Write 1-2 sentences introducing the organizations]

### 1. [Name](organization-id)
![Logo](logourl)

**Contact:** [contactemail] | [contactphone]
**Location:** [street], [commune], [province]
**Description:** [description]

---

### 2. [Next organization...]

---

[Write 1-2 sentences as closing remarks]
```

**DATA FIELDS:**
- id, name, description, logourl, contactemail, contactphone, street, commune, province",

				"mixed" => @"
**CRITICAL: THIS IS A DATABASE SEARCH REQUEST - YOU MUST GENERATE 2 SQL QUERIES!**
**START YOUR RESPONSE WITH THIS EXACT TAG (for parsing):** [SEARCH_TYPE: mixed]
**MIXED FORMAT:**
Show BOTH sections:

## 🎓 Workshops Found (max 5)
[Workshop format...]

## 🏢 Organizations Found (max 5)
[Organization format...]
[Closing remarks]",

				"booking_guide" => @"
**BOOKING GUIDE FORMAT:**
**START YOUR RESPONSE WITH THIS EXACT TAG (for parsing):** [SEARCH_TYPE: booking_guide]
```markdown
## 📝 Hướng Dẫn Đặt Vé Workshop

Để đặt vé workshop trên Wokiwoki, bạn làm theo các bước sau:

### Bước 1: Tìm Workshop
1. Sử dụng thanh tìm kiếm hoặc duyệt danh mục.
2. Lọc theo địa điểm, giá, loại hình (Online/Offline).

### Bước 2: Xem Chi Tiết & Chọn Lịch
1. Click vào workshop bạn quan tâm.
2. Xem thông tin: nội dung, giá, địa điểm, lịch học.
3. Chọn buổi học phù hợp với lịch của bạn.
4. Chọn các loại giá tương ứng với buổi học đó, bạn có thể mua nhiều loại giá vé khác nhau tương ứng với buổi học bạn chọn.

### Bước 3: Đăng Ký & Thanh Toán
1. Click nút ""Mua ngay""
2. Điền thông tin cá nhân.
3. Kiểm tra và xác nhận lại các loại vé, số lượng và tổng giá tiền.
4. Bấm ""Tiếp tục"" hoặc ""Xác nhận"" để chuyển sang trang thanh toán.

### 4: Tiến Hành Thanh Toán
1. Tại trang thanh toán mở app ngân hàng hoặc ví điện tử để quét mã QR.
2. Xác nhận số tiền và hoàn tất giao dịch trên ứng dụng của bạn.

### 5: Hoàn Tất & Chuẩn Bị Tham Gia
1. Chờ hệ thống xác nhận thanh toán hoàn tất (thường mất vài giây).
2. Bạn sẽ được dẫn tới trang ""Vé đã mua"".
3. Khi tham gia workshop: Mở mục ""Vé đã mua"" để ban tổ chức check-in, hoặc nhờ họ kiểm tra tên trong danh sách tham dự.

💡 **Lưu ý:** 
- Kiểm tra chính sách hoàn hủy trước khi đặt
- Liên hệ nhà tổ chức nếu cần hỗ trợ
- Đặt sớm để có giá tốt!

Bạn cần hỗ trợ thêm không? 😊
```

**INSTRUCTIONS:**
- Be detailed and step-by-step
- Use friendly, encouraging tone
- Include tips and warnings
- Mention contact support if needed",

				"general" => @"
[SEARCH_TYPE: general]

Bạn là trợ lý thân thiện của Wokiwoki. Tất cả phản hồi phải bằng **tiếng Việt**, ngắn gọn (2–3 câu), tự nhiên và mang tinh thần thương hiệu.

**MỤC TIÊU THƯƠNG HIỆU:**
'Chúng tôi là cầu nối giúp bạn khám phá các khóa học và workshop chất lượng, đồng thời hỗ trợ doanh nghiệp tiếp cận đúng đối tượng muốn phát triển bản thân.'

**QUY TẮC PHẢN HỒI:**
- Nếu người dùng chào → chào lại ấm áp, giới thiệu ngắn:
  'Chào bạn! 👋 Mình là trợ lý Wokiwoki…' + slogan (rút gọn).
- Nếu người dùng cảm ơn → đáp lại lịch sự, mời họ hỏi thêm.
- Nếu người dùng hỏi 'bạn là ai' → nêu rõ bạn là trợ lý AI của Wokiwoki + slogan.
- Nếu người dùng hỏi cần giúp gì → liệt kê nhanh 2–3 khả năng:
  tìm workshop, gợi ý theo sở thích, hỗ trợ đặt vé, tìm nhà tổ chức.
- Luôn kết thúc bằng 1 câu hỏi gợi mở, ví dụ:
  'Bạn đang muốn tìm workshop về chủ đề nào?'

**GIỌNG VĂN:**
- Thân thiện, chuyên nghiệp, gọn – không sử dụng tiếng Anh trừ khi buộc phải có.
- Tránh máy móc, tránh lặp lại nguyên văn user.
- Dùng icon nhẹ nhàng nếu phù hợp (👋✨🎨).

",


				_ => @"
**START YOUR RESPONSE WITH THIS EXACT TAG (for parsing):** [SEARCH_TYPE: general]
Format as friendly conversation with markdown."
			};

			var prompt = $@"# WORKSHOP ASSISTANT

USER QUERY: {userMessage}
ANALYSIS: {cleanAnalysis}
RESULTS DATA: {limitedJsonData}
LANGUAGE: {(isVietnamese ? "VIETNAMESE" : "ENGLISH")}
SEARCH TYPE: {searchTarget.ToUpper()}

{formatInstructions}

INSTRUCTIONS:
- MANDATORY: ALWAYS start your ENTIRE response with the exact [SEARCH_TYPE: ...] tag as specified in the format above. Do NOT omit or change it.
- Format ALL results from the data (max 5 items)
- Use proper markdown with emojis
- Be friendly and helpful
- Include intro (1-2 sentences) and closing remarks (1-2 sentences)
- Follow the exact format structure above";

			//return await CallGeminiApiAsync(prompt);
			var geminiResponse = await CallGeminiApiAsync(prompt);
			var formattedResponse = EnsureTypeTag(geminiResponse, searchTarget ?? "general");
			return RemoveSqlBlocks(formattedResponse);
		}

		private string EnsureTypeTag(string response, string searchTarget)
		{
			var typeTag = $"[SEARCH_TYPE: {searchTarget}]";
			if (response.StartsWith(typeTag))
			{
				return response;
			}
			return $"{typeTag}\n{response}";
		}

		private string ExtractAnalysisOnly(string gptOutput)
		{
			var analysisStart = gptOutput.IndexOf("ANALYSIS:", StringComparison.OrdinalIgnoreCase);
			if (analysisStart == -1) return gptOutput.Length > 100 ? gptOutput.Substring(0, 100) : gptOutput;

			var analysisText = gptOutput.Substring(analysisStart + 9).Trim();
			var sqlStart = analysisText.IndexOf("```sql", StringComparison.OrdinalIgnoreCase);
			if (sqlStart != -1)
			{
				analysisText = analysisText.Substring(0, sqlStart).Trim();
			}

			return analysisText.Length > 150 ? analysisText.Substring(0, 147) + "..." : analysisText;
		}

		private string LimitJsonSize(string jsonData)
		{
			if (jsonData.Length <= 2000) return jsonData;

			var truncated = jsonData.Substring(0, 1900);
			var lastBrace = truncated.LastIndexOf('}');
			if (lastBrace > 1500)
			{
				return truncated.Substring(0, lastBrace + 1) + "...(truncated)";
			}
			return truncated + "...(truncated)";
		}

		private async Task<string> CallGeminiApiAsync(string prompt)
		{
			if (string.IsNullOrEmpty(_geminiApiKey))
			{
				return "Gemini API not configured.";
			}

			try
			{
				var requestBody = new
				{
					contents = new[]
					{
						new { parts = new[] { new { text = prompt } } }
					},
					generationConfig = new
					{
						temperature = 0.7,
						maxOutputTokens = 4096
					}
				};

				var jsonContent = JsonSerializer.Serialize(requestBody);
				var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

				var apikey = _configuration["Gemini:ApiKey"];
				var apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apikey}";

				var response = await _httpClient.PostAsync(apiUrl, content);

				if (!response.IsSuccessStatusCode)
				{
					return "Formatting service unavailable.";
				}

				var responseJson = await response.Content.ReadAsStringAsync();
				using var doc = JsonDocument.Parse(responseJson);

				var text = doc.RootElement
					.GetProperty("candidates")[0]
					.GetProperty("content")
					.GetProperty("parts")[0]
					.GetProperty("text")
					.GetString();

				return text ?? "Empty response";
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ Gemini error: {ex.Message}");
				return "Error formatting response.";
			}
		}

		// ==================== HELPERS ====================
		private bool IsVietnameseQuery(string query)
		{
			var vnKeywords = new[] { "tìm", "có", "workshop", "khóa", "học", "ở", "tại", "giá", "nào", "gì", "còn", "thêm", "tổ chức" };
			return vnKeywords.Count(k => query.ToLower().Contains(k)) >= 2;
		}

		private async Task<ConversationChat> LoadOrCreateConversationAsync(string userId)
		{
			var conversation = await _dbContext.ConversationChats
				.Include(c => c.MessagesChats.OrderBy(m => m.Created))
				.FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive);

			if (conversation == null)
			{
				conversation = new ConversationChat
				{
					UserId = userId,
					Title = "New Conversation",
					Created = DateTime.UtcNow,
					CreatedBy = userId,
					IsActive = true
				};
				_dbContext.ConversationChats.Add(conversation);
				await _dbContext.SaveChangesAsync();
			}

			return conversation;
		}

		private string RemoveSqlBlocks(string response)
		{
			// Remove ```sql ... ``` blocks
			var sqlPattern = @"```sql[\s\S]*?```\s*";
			return System.Text.RegularExpressions.Regex.Replace(response, sqlPattern, "").Trim();
		}

		private async Task SaveMessagesAsync(ConversationChat conversation, string userId, string userMessage, string aiResponse)
		{
			_dbContext.MessageChats.Add(new MessageChat
			{
				ConversationId = conversation.Id,
				Role = "user",
				Content = userMessage,
				Created = DateTime.UtcNow,
				CreatedBy = userId
			});

			_dbContext.MessageChats.Add(new MessageChat
			{
				ConversationId = conversation.Id,
				Role = "assistant",
				Content = aiResponse,
				Created = DateTime.UtcNow,
				CreatedBy = "AI"
			});

			conversation.LastModified = DateTime.UtcNow;

			if (conversation.MessagesChats.Count == 0)
			{
				conversation.Title = userMessage.Length > 50
					? userMessage.Substring(0, 47) + "..."
					: userMessage;
			}

			await _dbContext.SaveChangesAsync();
		}
	}

	// ==================== SUPPORTING CLASSES ====================
	public class LocationInfo
	{
		public string LocationText { get; set; } = "";
		public string LocationType { get; set; } = "";
		public string SearchType { get; set; } = "";
		public double? Latitude { get; set; }
		public double? Longitude { get; set; }
	}

	public class LocationExtractionResult
	{
		public bool HasLocation { get; set; }
		public string? LocationQuery { get; set; }
		public string? LocationType { get; set; }
		public string? Confidence { get; set; }
	}
}