using AutoMapper;
using Lumina.DTO.Activity;
using Lumina.Models;
using Lumina.Repository;
using Lumina.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Lumina.Services
{
    public class ActivityService : IActivityService
    {
        private readonly LuminaDbContext _dbContext;
        private readonly IMapper _mapper;


        public ActivityService(LuminaDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ActivityDto>> GetAllSystemActivitiesAsync()
        {
            var activities = await _dbContext.Activities
                //.Where(t => t.IsSystemDefined)
                //.OrderBy(t => t.SortOrder)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ActivityDto>>(activities);
        }

        public async Task<IEnumerable<UserActivityDto>> GetAllUserActivitiesAsync(string userId)
        {
            var activities = await _dbContext.UserActivities
                .Where(t => t.UserId == userId && t.IsActive)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserActivityDto>>(activities);
        }
        public async Task<UserActivityDto> CreateUserActivityAsync(CreateUserActivityDto dto, string userId)
        {
            var name = dto.Name.Trim();

            bool exists = await _dbContext.UserActivities
                .AnyAsync(t => t.UserId == userId &&
                               EF.Functions.Like(t.Name, name) &&
                               t.IsActive);
            if (exists)
            {
                throw new InvalidOperationException("You already have an activity with this name.");
            }

            var userActivity = new UserActivity
            {
                Name = name,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Category = dto.Category
            };

            _dbContext.UserActivities.Add(userActivity);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<UserActivityDto>(userActivity);
        }

               

        public async Task<UserActivityDto?> UpdateUserActivityAsync(int id, UpdateUserActivityDto dto, string userId)
        {
            var existingActivity = await _dbContext.UserActivities.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
            if (existingActivity is null)
            {
                return null;
            }

            _mapper.Map(dto, existingActivity);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<UserActivityDto>(existingActivity);
        }

        public async Task DeleteUserActivityAsync(int activityId, string userId)
        {
            var activity = await _dbContext.UserActivities.FirstOrDefaultAsync(a => a.Id == activityId && a.UserId == userId);
            if (activity == null)
            {
                throw new Exception("Activity not found");
            }

            _dbContext.UserActivities.Remove(activity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
