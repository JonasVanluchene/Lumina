using AutoMapper;
using Lumina.DTO.Activity;
using Lumina.DTO.Tag;
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
            throw new NotImplementedException();
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
