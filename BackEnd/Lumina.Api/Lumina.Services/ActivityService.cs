using AutoMapper;
using Lumina.DTO.Activity;
using Lumina.DTO.Tag;
using Lumina.Models;
using Lumina.Repository;
using Lumina.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var activities = await _dbContext.Tags
                //.Where(t => t.IsSystemDefined)
                //.OrderBy(t => t.SortOrder)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ActivityDto>>(activities);
        }

        public async Task<IEnumerable<UserActivityDto>> GetAllUserActivitiesAsync(string userId)
        {
            throw new NotImplementedException();
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
                IsActive = true
            };

            _dbContext.UserActivities.Add(userActivity);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<UserActivityDto>(userActivity);
        }

        public async Task DeleteUserActivityAsync(int activityId, string userId)
        {
            throw new NotImplementedException();
        }

        

        public async Task<UserActivityDto?> UpdateUserActivityAsync(int id, UpdateUserActivityDto dto, string userId)
        {
            throw new NotImplementedException();
        }
    }
}
