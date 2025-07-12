using Lumina.DTO.Tag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumina.Services.Interfaces
{
    public interface ITagService
    {
        Task<IEnumerable<TagDto>> GetAllSystemTagsAsync();
        Task<IEnumerable<UserTagDto>> GetUserTagsAsync(string userId);
        Task<UserTagDto> CreateUserTagAsync(CreateUserTagDto dto, string userId);
        Task DeleteUserTagAsync(int tagId, string userId);
    }
}
