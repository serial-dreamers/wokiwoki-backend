using Azure;
using Azure.AI.OpenAI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using System.Text.Json;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.DTOs;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Infrastructure.Services
{
	public class AzureOpenAIChatService : IAzureOpenAIChatService
	{
		private readonly AzureOpenAIClient _aiClient;
		private readonly ChatClient _chatClient;
		private readonly WokiwokiDbContext _dbContext;

		public AzureOpenAIChatService(WokiwokiDbContext dbContext, IConfiguration config)
		{
			var endpoint = new Uri(config["AzureOpenAI:Endpoint"]!);
			var apiKey = config["AzureOpenAI:ApiKey"]!;
			var deploymentName = "gpt-4o-mini";

			_aiClient = new AzureOpenAIClient(endpoint, new AzureKeyCredential(apiKey));
			_chatClient = _aiClient.GetChatClient(deploymentName);
			_dbContext = dbContext;
		}

		public async Task<ChatResponse> ChatAsync(string userMessage, string userId)
		{
			var conversation = await _dbContext.ConversationChats
				.Include(c => c.MessagesChats.OrderBy(m => m.Created))
				.FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive);

			if (conversation == null)
			{
				conversation = new ConversationChat
				{
					UserId = userId,
					Title = "Cuộc trò chuyện mới",
					Created = DateTime.UtcNow,
					CreatedBy = userId,
					LastModified = DateTime.UtcNow,
					LastModifiedBy = userId
				};
				_dbContext.ConversationChats.Add(conversation);
				await _dbContext.SaveChangesAsync();
			}

			var history = conversation.MessagesChats
				.Select(m => new ChatMessageDto { Role = m.Role, Content = m.Content })
				.ToList();

			var tools = new List<ChatTool>
			{
				CreateSearchWorkshopsFunction(),
				CreateSearchOrganizationsFunction(),
				CreateGetWorkshopDetailsFunction(),
				CreateSearchByTagsFunction()
			};

			var systemMessage = new SystemChatMessage(GetSystemPromptWithSchema());

			var messages = new List<ChatMessage> { systemMessage };
			messages.AddRange(history.Select(h => h.Role == "user"
				? (ChatMessage)new UserChatMessage(h.Content)
				: (ChatMessage)new AssistantChatMessage(h.Content)));
			messages.Add(new UserChatMessage(userMessage));

			var options = new ChatCompletionOptions
			{
				Temperature = 0.7f,
				MaxOutputTokenCount = 2000
			};

			foreach (var tool in tools)
			{
				options.Tools.Add(tool);
			}

			var response = await _chatClient.CompleteChatAsync(messages, options);

			int maxIterations = 5;
			int iteration = 0;

			while (response.Value.FinishReason == ChatFinishReason.ToolCalls && iteration < maxIterations)
			{
				messages.Add(new AssistantChatMessage(response.Value));

				foreach (var toolCall in response.Value.ToolCalls)
				{
					var argsString = toolCall.FunctionArguments.ToString();
					var functionResult = await ExecuteFunctionAsync(toolCall.FunctionName, argsString);
					messages.Add(new ToolChatMessage(toolCall.Id, functionResult));
				}

				response = await _chatClient.CompleteChatAsync(messages, options);
				iteration++;
			}

			var aiResponse = response.Value.Content[0].Text;

			var userMsg = new MessageChat
			{
				ConversationId = conversation.Id,
				Role = "user",
				Content = userMessage,
				Created = DateTime.UtcNow,
				CreatedBy = userId
			};
			_dbContext.MessageChats.Add(userMsg);

			var aiMsg = new MessageChat
			{
				ConversationId = conversation.Id,
				Role = "assistant",
				Content = aiResponse,
				Created = DateTime.UtcNow,
				CreatedBy = "AI"
			};
			_dbContext.MessageChats.Add(aiMsg);

			conversation.LastModified = DateTime.UtcNow;
			conversation.LastModifiedBy = userId;

			if (string.IsNullOrEmpty(conversation.Title) && conversation.MessagesChats.Count == 0)
			{
				conversation.Title = $"Trò chuyện về: {userMessage.Substring(0, Math.Min(50, userMessage.Length))}...";
			}

			await _dbContext.SaveChangesAsync();

			return new ChatResponse
			{
				Message = aiResponse,
				Markdown = aiResponse
			};
		}

		// ==================== SYSTEM PROMPT ====================
		private string GetSystemPromptWithSchema()
		{
			return @"You are an intelligent AI assistant for the Wokiwoki workshop platform.

DATABASE SCHEMA:
- **workshops**: id, title, summary, image_url, delivery_type, starting_price, organization_id
- **organizations**: id, name, logo_url, contact_email, location
- **categories**: id, name
- **tags**: id, name
- **workshop_sessions**: id, workshop_id, start_time, end_time

CRITICAL INSTRUCTIONS:

1. **LANGUAGE DETECTION**: 
   - Detect the user's language from their message
   - Respond in the SAME language (Vietnamese, English, etc.)
   - Default to Vietnamese if unclear

2. **WHEN TO CALL FUNCTIONS**:
   ✅ CALL functions when user asks:
   - ""Tìm workshop về...""
   - ""Show me workshops about...""
   - ""Có tổ chức nào...""
   - ""Chi tiết workshop [name/id]""
   - ""List all organizations""
   
   ❌ DO NOT call functions for casual chat:
   - Greetings: ""Xin chào"", ""Hello"", ""Hi""
   - General questions: ""Bạn có thể làm gì?"", ""What can you do?""
   - Clarifications: ""Ý bạn là sao?"", ""Can you explain?""
   - Feedback: ""Cảm ơn"", ""Thanks"", ""OK""

3. **RESPONSE FORMAT**:

   **For CASUAL CHAT** (no function call):
   - Just respond naturally in 1-3 sentences
   - Example: ""Xin chào! Tôi có thể giúp bạn tìm workshop phù hợp. Bạn quan tâm loại workshop nào?""

   **For LIST RESULTS** (after function call):
   
   **WORKSHOPS FORMAT:**
   ```markdown
   ## 🎯 Workshops Found (X results)

   [Write 1-2 sentences introducing the workshops - explain what makes them suitable for the user's query, highlight common themes, or provide helpful context]

   ### 1. [Title](workshop-id-here)
   ![Workshop Image](image-url-here)  
   **Price:** XXX VND | **Type:** Online/Offline/Hybrid  
   **Location:** [DisplayAddress]  
   **Organizer:** [Name]  
   [Summary]

   ---

   ### 2. [Title](workshop-id-here)
   ...

   ---

   [Write 1-2 sentences as closing remarks - suggest next steps, offer to help with more details, or provide additional recommendations]
   ```

   **ORGANIZATIONS FORMAT:**
   ```markdown
   ## 🏢 Organizations Found (X results)

   [Write 1-2 sentences introducing the organizations]

   ### 1. [Name](organization-id-here)
   ![Logo](logo-url-here)  
   **Contact:** [Email] | [Phone]  
   **Location:** [Address]  
   **Description:** [Description]

   ---

   ### 2. [Name](organization-id-here)
   ...

   ---

   [Write 1-2 sentences as closing remarks]
   ```

   **For MIXED RESULTS** (workshops + organizations):
   Show both sections, 5 items max per section.

4. **DATA LIMITS & REQUIRED FIELDS**:
   - Return maximum 5 workshops
   - Return maximum 5 organizations
   - **CRITICAL**: Always include these fields:
     * Workshops: Id, Title, ImageUrl, Price, Type, Location, Organizer, Summary
     * Organizations: Id, Name, LogoUrl, Email, Phone, Location, Description

5. **SMART FILTERING**:
   - If user asks ""workshop về lập trình"" → call search_workshops with keyword=""lập trình""
   - If user asks ""tổ chức nào ở TP.HCM"" → call search_organizations with location=""TP.HCM""
   - If user asks both → call both functions

6. **IMPORTANT MARKDOWN FORMAT RULES**:
   - Keep responses SHORT and READABLE
   - **Workshop title format**: `[Title](id)` → Creates clickable link with workshop ID
   - **Organization name format**: `[Name](id)` → Creates clickable link with organization ID
   - **Image format**: `![Alt Text](url)` → Shows image from URL
   - **Example**: `### 1. [Làm Nước Hoa](cc4743e8-6db5-4ff0-a51f-d1df7da406d4)`
   - **Example**: `![Workshop](https://wokiwoki.blob.core.windows.net/images/workshop.jpg)`
   - Never fabricate data
   - If no results, suggest alternatives

Examples:

**User:** ""Xin chào""  
**Assistant:** ""Xin chào! Tôi là trợ lý tìm workshop của Wokiwoki. Bạn đang tìm workshop về chủ đề gì?""

**User:** ""Tìm workshop về lập trình""  
**Assistant:** [Calls search_workshops] → Returns:
```markdown
## 🎯 Workshops Found (2 results)

Đây là các workshop lập trình phổ biến hiện tại, phù hợp cho cả người mới và người đã có kinh nghiệm.

### 1. [Python cho Beginners](abc-123-def)
![Workshop](https://example.com/python.jpg)  
**Price:** 500000 VND | **Type:** Online  
**Location:** Online Event  
**Organizer:** TechAcademy  
Khóa học Python cơ bản với dự án thực tế.

---

### 2. [Web Development Bootcamp](xyz-456-ghi)
![Workshop](https://example.com/web.jpg)  
**Price:** 1200000 VND | **Type:** Offline  
**Location:** 123 Nguyễn Huệ, Q.1, TP.HCM  
**Organizer:** CodeSchool  
Học HTML, CSS, JavaScript từ zero đến hero.

---

Bạn muốn xem chi tiết workshop nào? Hoặc cần tìm thêm khóa học khác?
```

**User:** ""Show me organizations in Ho Chi Minh""  
**Assistant:** [Calls search_organizations] → Returns:
```markdown
## 🏢 Organizations Found (2 results)

Các tổ chức đào tạo uy tín tại TP.HCM với nhiều workshop chất lượng.

### 1. [TechAcademy Vietnam](org-abc-123)
![Logo](https://example.com/logo1.jpg)  
**Contact:** contact@techacademy.vn | 0909123456  
**Location:** 123 Lê Lợi, Q.1, TP.HCM  
**Description:** Trung tâm đào tạo công nghệ hàng đầu với 10+ năm kinh nghiệm.

---

### 2. [Creative Hub Studio](org-xyz-456)
![Logo](https://example.com/logo2.jpg)  
**Contact:** info@creativehub.vn | 0912345678  
**Location:** 456 Nguyễn Trãi, Q.5, TP.HCM  
**Description:** Studio chuyên về thiết kế và nghệ thuật sáng tạo.

---

Bạn muốn xem thêm thông tin tổ chức nào?
```

**User:** ""Thanks!""  
**Assistant:** ""You're welcome! Feel free to ask if you need anything else! 😊""";
		}

		// ==================== FUNCTION DEFINITIONS ====================
		private ChatTool CreateSearchWorkshopsFunction()
		{
			return ChatTool.CreateFunctionTool(
				functionName: "search_workshops",
				functionDescription: "Search workshops by keyword, category, tags, location, price, delivery type, rating",
				functionParameters: BinaryData.FromString("""
            {
                "type": "object",
                "properties": {
                    "keyword": {"type": "string"},
                    "categoryName": {"type": "string"},
                    "tagNames": {"type": "array", "items": {"type": "string"}},
                    "deliveryType": {"type": "string", "enum": ["Online", "Offline", "Hybrid"]},
                    "location": {"type": "string"},
                    "maxPrice": {"type": "number"},
                    "minRating": {"type": "number"}
                }
            }
            """)
			);
		}

		private ChatTool CreateSearchOrganizationsFunction()
		{
			return ChatTool.CreateFunctionTool(
				functionName: "search_organizations",
				functionDescription: "Search organizations by name or location",
				functionParameters: BinaryData.FromString("""
            {
                "type": "object",
                "properties": {
                    "name": {"type": "string"},
                    "location": {"type": "string"}
                }
            }
            """)
			);
		}

		private ChatTool CreateGetWorkshopDetailsFunction()
		{
			return ChatTool.CreateFunctionTool(
				functionName: "get_workshop_details",
				functionDescription: "Get full details of a specific workshop including sessions, schedules, tickets",
				functionParameters: BinaryData.FromString("""
            {
                "type": "object",
                "properties": {
                    "workshopId": {"type": "string"}
                },
                "required": ["workshopId"]
            }
            """)
			);
		}

		private ChatTool CreateSearchByTagsFunction()
		{
			return ChatTool.CreateFunctionTool(
				functionName: "search_by_tags",
				functionDescription: "Search workshops by specific tags",
				functionParameters: BinaryData.FromString("""
            {
                "type": "object",
                "properties": {
                    "tagNames": {"type": "array", "items": {"type": "string"}}
                },
                "required": ["tagNames"]
            }
            """)
			);
		}

		// ==================== FUNCTION EXECUTION ====================
		private async Task<string> ExecuteFunctionAsync(string functionName, string arguments)
		{
			try
			{
				return functionName switch
				{
					"search_workshops" => await SearchWorkshopsAsync(arguments),
					"search_organizations" => await SearchOrganizationsAsync(arguments),
					"get_workshop_details" => await GetWorkshopDetailsAsync(arguments),
					"search_by_tags" => await SearchByTagsAsync(arguments),
					_ => JsonSerializer.Serialize(new { error = "Function not found" })
				};
			}
			catch (Exception ex)
			{
				return JsonSerializer.Serialize(new { error = ex.Message });
			}
		}

		private async Task<string> SearchWorkshopsAsync(string arguments)
		{
			var args = JsonSerializer.Deserialize<SearchWorkshopArgs>(arguments);

			var query = _dbContext.Workshops
				.Include(w => w.Organization)
				.Include(w => w.Category)
				.Include(w => w.Tags)
				.Where(w => w.IsActive && w.Status == WorkshopStatus.Published)
				.AsQueryable();

			if (!string.IsNullOrEmpty(args.Keyword))
			{
				var keyword = args.Keyword.ToLower();
				query = query.Where(w =>
					w.Title.ToLower().Contains(keyword) ||
					w.Summary.ToLower().Contains(keyword) ||
					w.Description.ToLower().Contains(keyword));
			}

			if (!string.IsNullOrEmpty(args.CategoryName))
			{
				query = query.Where(w => w.Category.Name.Contains(args.CategoryName));
			}

			if (args.TagNames?.Any() == true)
			{
				query = query.Where(w => w.Tags.Any(t => args.TagNames.Contains(t.Name)));
			}

			if (!string.IsNullOrEmpty(args.DeliveryType))
			{
				if (Enum.TryParse<WorkshopDeliveryType>(args.DeliveryType, out var deliveryType))
				{
					query = query.Where(w => w.DeliveryType == deliveryType);
				}
			}

			if (!string.IsNullOrEmpty(args.Location))
			{
				query = query.Where(w =>
					w.DisplayAddress != null && w.DisplayAddress.Contains(args.Location));
			}

			if (args.MaxPrice.HasValue)
			{
				query = query.Where(w => w.StartingPrice <= args.MaxPrice.Value);
			}

			if (args.MinRating.HasValue)
			{
				query = query.Where(w => w.AverageRating >= args.MinRating.Value);
			}

			var workshops = await query
				.OrderByDescending(w => w.AverageRating)
				.ThenByDescending(w => w.TotalBookings)
				.Take(5)
				.ToListAsync();

			// Return ALL required fields including ImageUrl
			return JsonSerializer.Serialize(workshops.Select(w => new
			{
				w.Id,
				w.Title,
				w.Summary,
				w.ImageUrl,  // CRITICAL: Must include for frontend
				w.StartingPrice,
				w.DeliveryType,
				w.DisplayAddress,
				Organization = new { w.Organization.Name },
				Tags = w.Tags.Select(t => t.Name).Take(3).ToList()
			}), new JsonSerializerOptions { WriteIndented = false });
		}

		private async Task<string> SearchOrganizationsAsync(string arguments)
		{
			var args = JsonSerializer.Deserialize<SearchOrgArgs>(arguments);

			var query = _dbContext.Organizations.AsQueryable();

			if (!string.IsNullOrEmpty(args.Name))
			{
				query = query.Where(o => o.Name.Contains(args.Name));
			}

			if (!string.IsNullOrEmpty(args.Location))
			{
				query = query.Where(o => o.Province != null && o.Province.Contains(args.Location));
			}

			var orgs = await query.Take(5).ToListAsync();

			// Return ALL required fields including Id and LogoUrl
			return JsonSerializer.Serialize(orgs.Select(o => new
			{
				o.Id,  // CRITICAL: Must include for frontend
				o.Name,
				o.Description,
				o.LogoUrl,  // CRITICAL: Must include for frontend
				o.ContactEmail,
				o.ContactPhone,
				Location = $"{o.Street}, {o.Commune}, {o.Province}"
			}), new JsonSerializerOptions { WriteIndented = false });
		}

		private async Task<string> GetWorkshopDetailsAsync(string arguments)
		{
			var args = JsonSerializer.Deserialize<GetDetailsArgs>(arguments);

			var workshop = await _dbContext.Workshops
				.Include(w => w.Organization)
				.Include(w => w.Category)
				.Include(w => w.Tags)
				.Include(w => w.Schedules)
					.ThenInclude(s => s.Tickets)
				.Include(w => w.WorkshopSessions.OrderBy(s => s.StartTime))
				.Include(w => w.Reviews.OrderByDescending(r => r.Created).Take(3))
				.FirstOrDefaultAsync(w => w.Id == Guid.Parse(args.WorkshopId));

			if (workshop == null)
				return JsonSerializer.Serialize(new { error = "Workshop not found" });

			return JsonSerializer.Serialize(new
			{
				workshop.Id,
				workshop.Title,
				workshop.Summary,
				workshop.Description,
				workshop.ImageUrl,
				workshop.StartingPrice,
				workshop.DurationMinutes,
				workshop.DeliveryType,
				workshop.ScheduleType,
				workshop.DisplayAddress,
				workshop.OnlineEventUrl,
				workshop.AverageRating,
				workshop.ReviewCount,
				workshop.TotalBookings,
				Organization = new
				{
					workshop.Organization.Id,
					workshop.Organization.Name,
					workshop.Organization.LogoUrl,
					workshop.Organization.ContactEmail
				},
				Category = new { workshop.Category.Id, workshop.Category.Name },
				Tags = workshop.Tags.Select(t => new { t.Id, t.Name }).ToList(),
				Schedules = workshop.Schedules.Select(s => new
				{
					s.Id,
					s.RecurrenceType,
					s.StartTime,
					s.EndTime,
					Tickets = s.Tickets.Select(t => new { t.Name, t.Price, t.MaxQuantity })
				}),
				UpcomingSessions = workshop.WorkshopSessions
					.Where(s => s.IsActive && s.StartTime > DateTime.UtcNow)
					.OrderBy(s => s.StartTime)
					.Take(5)
					.Select(s => new
					{
						s.Id,
						s.Title,
						s.StartTime,
						s.EndTime,
						s.Capacity,
						s.BookedCount,
						AvailableSeats = s.Capacity - s.BookedCount
					}),
				RecentReviews = workshop.Reviews.Select(r => new
				{
					r.Rating,
					r.Comment,
					r.Created
				})
			}, new JsonSerializerOptions { WriteIndented = false });
		}

		private async Task<string> SearchByTagsAsync(string arguments)
		{
			var args = JsonSerializer.Deserialize<SearchByTagsArgs>(arguments);

			var workshops = await _dbContext.Workshops
				.Include(w => w.Organization)
				.Include(w => w.Category)
				.Include(w => w.Tags)
				.Where(w => w.IsActive &&
							w.Status == WorkshopStatus.Published &&
							w.Tags.Any(t => args.TagNames.Contains(t.Name)))
				.Take(5)
				.ToListAsync();

			// Return ALL required fields including ImageUrl
			return JsonSerializer.Serialize(workshops.Select(w => new
			{
				w.Id,
				w.Title,
				w.Summary,
				w.ImageUrl,  // CRITICAL: Must include for frontend
				w.StartingPrice,
				w.DeliveryType,
				w.DisplayAddress,
				Organization = w.Organization.Name,
				Tags = w.Tags.Select(t => t.Name).Take(3).ToList()
			}), new JsonSerializerOptions { WriteIndented = false });
		}
	} 
}