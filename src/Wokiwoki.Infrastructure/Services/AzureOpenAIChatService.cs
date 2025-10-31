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

		// Cập nhật signature: Chỉ nhận userMessage và userId (để query/load/save conversation)
		// Interface cũng cần cập nhật tương ứng: Task<ChatResponse> ChatAsync(string userMessage, string userId);
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

			// 2. Load history từ DB (chuyển sang List<ChatMessageDto>)
			var history = conversation.MessagesChats
				.Select(m => new ChatMessageDto { Role = m.Role, Content = m.Content })
				.ToList();

			// 3. Định nghĩa các functions với schema đầy đủ
			var tools = new List<ChatTool>
			{
				CreateSearchWorkshopsFunction(),
				CreateSearchOrganizationsFunction(),
				CreateGetWorkshopDetailsFunction(),
				CreateSearchByTagsFunction()
			};

			// 4. System prompt với schema database
			var systemMessage = new SystemChatMessage(GetSystemPromptWithSchema());

			// 5. Build conversation history
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

			// 6. Gọi AI với function calling
			var response = await _chatClient.CompleteChatAsync(messages, options);

			// 7. Xử lý function calls (có thể gọi nhiều lần)
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

			// 8. Lưu messages mới vào DB
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
				CreatedBy = "AI"  // Hoặc userId nếu cần
			};
			_dbContext.MessageChats.Add(aiMsg);

			// Cập nhật conversation
			conversation.LastModified = DateTime.UtcNow;
			conversation.LastModifiedBy = userId;

			// Auto-gen title nếu chưa có (dựa trên message đầu tiên hoặc userMessage)
			if (string.IsNullOrEmpty(conversation.Title) && conversation.MessagesChats.Count == 0)
			{
				conversation.Title = $"Trò chuyện về: {userMessage.Substring(0, Math.Min(50, userMessage.Length))}...";
			}

			await _dbContext.SaveChangesAsync();

			// 9. Return response
			return new ChatResponse
			{
				Message = aiResponse,
				Markdown = aiResponse
			};
		}

		// ==================== SYSTEM PROMPT WITH SCHEMA ====================
		private string GetSystemPromptWithSchema()
		{
			return @"You are an intelligent AI assistant that helps users find suitable workshops on the Wokiwoki platform.

DATABASE SCHEMA:
You have access to the following tables:

**workshops** - List of workshops  
- id (uuid), title (string), summary (string), description (text)  
- delivery_type (Online/Offline/Hybrid), schedule_type (Recurring/OneTime)  
- starting_price (decimal), status (Draft/Published/Hidden/Cancelled)  
- organization_id (uuid), category_id (uuid)  
- like_count, total_bookings, review_count, average_rating  
- duration_minutes, default_capacity, display_address, latitude, longitude  

**organizations** - Workshop organizers  
- id (uuid), name (string), description (text)  
- logo_url, contact_email, contact_phone  
- street, commune, province  

**categories** - Workshop categories (Programming, Design, Marketing, etc.)  
- id (uuid), name (string), description (string)  

**tags** - Tags attached to workshops (JavaScript, Figma, SEO, etc.)  
- id (uuid), name (string), description (string), icon_url  

**workshop_schedules** - Recurring schedules for workshops  
- id, workshop_id, recurrence_type (Daily/Weekly/Monthly/Yearly/None)  
- days_of_week, start_time, end_time, valid_from, valid_until  

**workshop_sessions** - Specific sessions of a workshop  
- id, workshop_id, schedule_id, title, description  
- start_time, end_time, capacity, booked_count  
- street, commune, province, age_restriction_type  

**workshop_schedule_tickets** - Ticket types for schedules  
- id, workshop_schedule_id, name (""Adult"", ""Child"")  
- price, max_quantity  

**reviews** - Workshop reviews  
- id, workshop_id, user_id, rating (1-5), comment  

RELATIONSHIPS:
- Workshop belongs to Organization and Category  
- Workshop has many Tags (many-to-many)  
- Workshop has many Schedules (recurring pattern)  
- Workshop has many Sessions (specific dates)  
- Schedule has many ScheduleTickets (pricing tiers)  

TASKS:
1. Understand the user's intent (what kind of workshop they are looking for, location, price, time, etc.)  
2. Call the appropriate function to query the database  
3. Return the result in a clean, readable **Markdown** format  

RETURN FORMAT:
## 🎯 Suitable Workshops

### 1. **[Workshop Title]**  
*[Organization Name]*  

**Description:** [Short Summary]  

**Location:** [DisplayAddress or Online/Hybrid]  
**Starting Price:** [StartingPrice] VND  
**Duration:** [DurationMinutes] minutes  
⭐ **Rating:** [AverageRating]/5 ([ReviewCount] reviews)  
**Bookings:** [TotalBookings] people  

**Tags:** [Tag1], [Tag2], [Tag3]  

**Sessions:**  
- [Session 1: StartTime - EndTime]  
- [Session 2: StartTime - EndTime]  

[Additional description if needed]  

---

### 2. **[Second Workshop]**
...

---

*Found X workshops - You can ask for more details or refine your search!*  

IF NO RESULTS:
- Explain why no results were found  
- Suggest similar workshops or alternative criteria  
- Always be helpful and friendly  

NOTES:
- ALWAYS return responses in **Markdown format**  
- ONLY use data from function results  
- NEVER fabricate or invent information not found in the database  

⚠️ **IMPORTANT:** Although this system prompt is written in English, you must always respond in **Vietnamese**.";
		}

		// ==================== FUNCTION DEFINITIONS ====================
		private ChatTool CreateSearchWorkshopsFunction()
		{
			return ChatTool.CreateFunctionTool(
				functionName: "search_workshops",
				functionDescription: "Tìm workshop theo tiêu chí: keyword, category, tag, location, price, delivery type, rating",
				functionParameters: BinaryData.FromString("""
            {
                "type": "object",
                "properties": {
                    "keyword": {
                        "type": "string",
                        "description": "Từ khóa tìm trong title, summary, description"
                    },
                    "categoryName": {
                        "type": "string",
                        "description": "Tên category: 'Lập trình', 'Thiết kế', 'Marketing'"
                    },
                    "tagNames": {
                        "type": "array",
                        "items": {"type": "string"},
                        "description": "Danh sách tag: ['JavaScript', 'React', 'Figma']"
                    },
                    "deliveryType": {
                        "type": "string",
                        "enum": ["Online", "Offline", "Hybrid"],
                        "description": "Hình thức: Online/Offline/Hybrid"
                    },
                    "location": {
                        "type": "string",
                        "description": "Địa điểm (province): 'Hà Nội', 'TP.HCM'"
                    },
                    "maxPrice": {
                        "type": "number",
                        "description": "Giá tối đa"
                    },
                    "minRating": {
                        "type": "number",
                        "description": "Đánh giá tối thiểu (1-5)"
                    }
                }
            }
            """)
			);
		}

		private ChatTool CreateSearchOrganizationsFunction()
		{
			return ChatTool.CreateFunctionTool(
				functionName: "search_organizations",
				functionDescription: "Tìm tổ chức tổ chức workshop",
				functionParameters: BinaryData.FromString("""
            {
                "type": "object",
                "properties": {
                    "name": {"type": "string", "description": "Tên tổ chức"},
                    "location": {"type": "string", "description": "Địa điểm"}
                }
            }
            """)
			);
		}

		private ChatTool CreateGetWorkshopDetailsFunction()
		{
			return ChatTool.CreateFunctionTool(
				functionName: "get_workshop_details",
				functionDescription: "Lấy chi tiết đầy đủ của 1 workshop bao gồm sessions, schedules, tickets",
				functionParameters: BinaryData.FromString("""
            {
                "type": "object",
                "properties": {
                    "workshopId": {"type": "string", "description": "ID của workshop"}
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
				functionDescription: "Tìm workshop theo tag cụ thể",
				functionParameters: BinaryData.FromString("""
            {
                "type": "object",
                "properties": {
                    "tagNames": {
                        "type": "array",
                        "items": {"type": "string"},
                        "description": "Danh sách tag names"
                    }
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
				.Include(w => w.WorkshopSessions.OrderBy(s => s.StartTime).Take(3))
				.Where(w => w.IsActive && w.Status == WorkshopStatus.Published)
				.AsQueryable();

			// Filter by keyword
			if (!string.IsNullOrEmpty(args.Keyword))
			{
				var keyword = args.Keyword.ToLower();
				query = query.Where(w =>
					w.Title.ToLower().Contains(keyword) ||
					w.Summary.ToLower().Contains(keyword) ||
					w.Description.ToLower().Contains(keyword));
			}

			// Filter by category
			if (!string.IsNullOrEmpty(args.CategoryName))
			{
				query = query.Where(w => w.Category.Name.Contains(args.CategoryName));
			}

			// Filter by tags
			if (args.TagNames?.Any() == true)
			{
				query = query.Where(w => w.Tags.Any(t => args.TagNames.Contains(t.Name)));
			}

			// Filter by delivery type
			if (!string.IsNullOrEmpty(args.DeliveryType))
			{
				if (Enum.TryParse<WorkshopDeliveryType>(args.DeliveryType, out var deliveryType))
				{
					query = query.Where(w => w.DeliveryType == deliveryType);
				}
			}

			// Filter by location (province)
			if (!string.IsNullOrEmpty(args.Location))
			{
				query = query.Where(w =>
					w.DisplayAddress != null && w.DisplayAddress.Contains(args.Location));
			}

			// Filter by price
			if (args.MaxPrice.HasValue)
			{
				query = query.Where(w => w.StartingPrice <= args.MaxPrice.Value);
			}

			// Filter by rating
			if (args.MinRating.HasValue)
			{
				query = query.Where(w => w.AverageRating >= args.MinRating.Value);
			}

			var workshops = await query
				.OrderByDescending(w => w.AverageRating)
				.ThenByDescending(w => w.TotalBookings)
				.Take(10)
				.ToListAsync();

			return JsonSerializer.Serialize(workshops.Select(w => new
			{
				w.Id,
				w.Title,
				w.Summary,
				w.Description,
				w.StartingPrice,
				w.DurationMinutes,
				w.DeliveryType,
				w.DisplayAddress,
				w.AverageRating,
				w.ReviewCount,
				w.TotalBookings,
				w.LikeCount,
				Organization = new { w.Organization.Id, w.Organization.Name },
				Category = new { w.Category.Id, w.Category.Name },
				Tags = w.Tags.Select(t => t.Name).ToList(),
				UpcomingSessions = w.WorkshopSessions
					.Where(s => s.StartTime > DateTime.UtcNow && s.IsActive)
					.OrderBy(s => s.StartTime)
					.Take(3)
					.Select(s => new
					{
						s.Id,
						s.Title,
						s.StartTime,
						s.EndTime,
						s.Capacity,
						s.BookedCount,
						AvailableSeats = s.Capacity - s.BookedCount
					})
			}), new JsonSerializerOptions { WriteIndented = true });
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

			var orgs = await query.Take(10).ToListAsync();

			return JsonSerializer.Serialize(orgs.Select(o => new
			{
				o.Id,
				o.Name,
				o.Description,
				o.LogoUrl,
				o.ContactEmail,
				o.ContactPhone,
				Location = $"{o.Street}, {o.Commune}, {o.Province}"
			}));
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
				.Include(w => w.Reviews.OrderByDescending(r => r.Created).Take(5))
				.FirstOrDefaultAsync(w => w.Id == Guid.Parse(args.WorkshopId));

			if (workshop == null)
				return JsonSerializer.Serialize(new { error = "Workshop not found" });

			return JsonSerializer.Serialize(new
			{
				workshop.Id,
				workshop.Title,
				workshop.Summary,
				workshop.Description,
				workshop.StartingPrice,
				workshop.DurationMinutes,
				workshop.DeliveryType,
				workshop.ScheduleType,
				workshop.DisplayAddress,
				workshop.OnlineEventUrl,
				workshop.AverageRating,
				workshop.ReviewCount,
				workshop.TotalBookings,
				Organization = workshop.Organization,
				Category = new { workshop.Category.Id, workshop.Category.Name },
				Tags = workshop.Tags.Select(t => new { t.Id, t.Name }).ToList(),
				Schedules = workshop.Schedules.Select(s => new
				{
					s.Id,
					s.RecurrenceType,
					s.StartTime,
					s.EndTime,
					s.ValidFrom,
					s.ValidUntil,
					Tickets = s.Tickets.Select(t => new { t.Name, t.Price, t.MaxQuantity })
				}),
				Sessions = workshop.WorkshopSessions
					.Where(s => s.IsActive)
					.Select(s => new
					{
						s.Id,
						s.Title,
						s.StartTime,
						s.EndTime,
						s.Capacity,
						s.BookedCount,
						Location = $"{s.Street}, {s.Commune}, {s.Province}"
					}),
				RecentReviews = workshop.Reviews.Select(r => new
				{
					r.Rating,
					r.Comment,
					r.Created
				})
			}, new JsonSerializerOptions { WriteIndented = true });
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
				.Take(10)
				.ToListAsync();

			return JsonSerializer.Serialize(workshops.Select(w => new
			{
				w.Id,
				w.Title,
				w.Summary,
				w.StartingPrice,
				w.AverageRating,
				Organization = w.Organization.Name,
				Category = w.Category.Name,
				Tags = w.Tags.Select(t => t.Name).ToList()
			}));
		}
	}
}