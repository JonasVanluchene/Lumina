using AutoMapper;
using Lumina.DTO.Tag;
using Lumina.Models;
using Lumina.Repository;
using Lumina.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Lumina.Services
{
    public class TagService : ITagService
    {
        private readonly LuminaDbContext _dbContext;
        private readonly IMapper _mapper;

        public TagService(LuminaDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }


        public async Task<IEnumerable<TagDto>> GetAllSystemTagsAsync()
        {
            var tags = await _dbContext.Tags
                //.Where(t => t.IsSystemDefined)
                //.OrderBy(t => t.SortOrder)
                .ToListAsync();

            return _mapper.Map<IEnumerable<TagDto>>(tags);
        }

        public async Task<IEnumerable<UserTagDto>> GetUserTagsAsync(string userId)
        {
            var tags = await _dbContext.UserTags
                .Where(t => t.UserId == userId && t.IsActive)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserTagDto>>(tags);
        }

        public async Task<UserTagDto> CreateUserTagAsync(CreateUserTagDto dto, string userId)
        {
            var name = dto.Name.Trim();

            bool exists = await _dbContext.UserTags
                .AnyAsync(t => t.UserId == userId &&
                               EF.Functions.Like(t.Name, name) &&
                               t.IsActive);
            if (exists)
            {
                throw new InvalidOperationException("You already have a tag with this name.");
            }

            var userTag = new UserTag
            {
                Name = name,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _dbContext.UserTags.Add(userTag);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<UserTagDto>(userTag);
        }
        

        public async Task DeleteUserTagAsync(int tagId, string userId)
        {
            var tag = await _dbContext.UserTags.FirstOrDefaultAsync(t => t.Id == tagId && t.UserId == userId);
            if (tag == null)
            {
                throw new Exception("Tag not found");
            }

            _dbContext.UserTags.Remove(tag);
            await _dbContext.SaveChangesAsync();
        }

        
    }
}
