using Lumina.DTO.Activity;

namespace Lumina.Services.Interfaces
{
    public interface IActivityService
    {
        Task<IEnumerable<ActivityDto>> GetAllSystemActivitiesAsync();
        Task<IEnumerable<UserActivityDto>> GetAllUserActivitiesAsync(string userId);
        Task<UserActivityDto> CreateUserActivityAsync(CreateUserActivityDto dto, string userId);
        Task<UserActivityDto?> UpdateUserActivityAsync(int id, UpdateUserActivityDto dto, string userId);
        Task DeleteUserActivityAsync(int activityId, string userId);
    }
}
