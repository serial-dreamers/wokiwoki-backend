using MediatR; 
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Features.Users.Commands.SaveUserPreferences
{
	public record SaveUserPreferencesCommand : IRequest<Result>
	{
		public string? Location { get; set; }
		public List<UserPreferenceDto> Preferences { get; set; } = new();
	}

	public record UserPreferenceDto
	{
		public Guid CategoryId { get; set; }
		public List<Guid> TagIds { get; set; } = new();
	}

	public class SaveUserPreferencesCommandHandler : IRequestHandler<SaveUserPreferencesCommand, Result>
	{
		private readonly IIdentityService _identityService;
		private readonly IUserContext _userContext;
		private readonly IUserTagPreferenceRepository _userTagPreferenceRepository;
		private readonly IGoongMapService _goongMapService;
		private readonly ICategoryRepository _categoryRepository;
		private readonly ITagRepository _tagRepository;

		public SaveUserPreferencesCommandHandler(
			IIdentityService identityService,
			IUserContext userContext,
			IUserTagPreferenceRepository userTagPreferenceRepository,
			IGoongMapService goongMapService,
			ICategoryRepository categoryRepository,
			ITagRepository tagRepository)
		{
			_identityService = identityService;
			_userContext = userContext;
			_userTagPreferenceRepository = userTagPreferenceRepository;
			_goongMapService = goongMapService;
			_categoryRepository = categoryRepository;
			_tagRepository = tagRepository;
		}

		public async Task<Result> Handle(SaveUserPreferencesCommand request, CancellationToken cancellationToken)
		{
			var userId = _userContext.UserId;

			try
			{
				// Update user location if provided
				double? latitude = null;
				double? longitude = null;

				if (!string.IsNullOrEmpty(request.Location))
				{
					// Get coordinates from location using GoongMapService
					var coordinates = await _goongMapService.GetCoordinatesAsync(request.Location);
					if (coordinates.HasValue)
					{
						latitude = coordinates.Value.lat;
						longitude = coordinates.Value.lng;
					}

					// Update user location via IdentityService
					var locationResult = await _identityService.UpdateUserLocationAsync(userId, request.Location, latitude, longitude);
					if (!locationResult.Succeeded)
					{
						return locationResult;
					}
				}

				// Delete existing tag preferences
				var existingPreferences = await _userTagPreferenceRepository.GetByUserIdAsync(userId, cancellationToken);

				foreach (var pref in existingPreferences)
				{
					await _userTagPreferenceRepository.DeleteAsync(pref.Id, cancellationToken);
				}

				// Save new tag preferences
				foreach (var preference in request.Preferences)
				{
					// Validate category exists
					var category = await _categoryRepository.GetByIdAsync(preference.CategoryId, cancellationToken);
					if (category == null)
						continue;

					// Get all tags for this category to validate
					var categoryTags = await _tagRepository.GetTagsByCategory(preference.CategoryId, cancellationToken);
					var validTagIds = categoryTags.Select(t => t.Id).ToHashSet();

					foreach (var tagId in preference.TagIds)
					{
						// Validate tag exists and belongs to category
						if (!validTagIds.Contains(tagId))
							continue;

						var userTagPreference = new UserTagPreference
						{
							UserId = userId,
							CategoryId = preference.CategoryId,
							TagId = tagId
						};

						await _userTagPreferenceRepository.CreateAsync(userTagPreference, cancellationToken);
					}
				}

			return Result.Success();
			}
			catch (Exception ex)
			{
				return Result.Failure(new[] { $"Error saving preferences: {ex.Message}" });
			}
		}
	}
}

