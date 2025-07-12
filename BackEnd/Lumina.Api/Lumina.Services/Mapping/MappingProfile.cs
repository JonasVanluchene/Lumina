using AutoMapper;
using Lumina.DTO.JournalEntry;
using Lumina.DTO.Tag;
using Lumina.Models;

namespace Lumina.Services.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Create and Update mappings

            //JournalEntry
            CreateMap<CreateJournalEntryDto, JournalEntry>();
            CreateMap<UpdateJournalEntryDto, JournalEntry>();

            //Tag
            CreateMap<CreateUserTagDto, UserTag>();
            CreateMap<UpdateUserTagDto, UserTag>();




            // Read mapping

            // JournalEntry -> JournalEntryDto
            CreateMap<JournalEntry, JournalEntryDto>()
                .ForMember(dest => dest.PrimaryMoodName, opt => opt.MapFrom(src => src.PrimaryMood.Name))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src =>
                    src.Tags.Where(jt => jt.Tag != null).Select(jt => jt.Tag.Name).ToList()))
                .ForMember(dest => dest.UserTags, opt => opt.MapFrom(src =>
                    src.Tags.Where(jt => jt.UserTag != null).Select(jt => jt.UserTag.Name).ToList()))
                .ForMember(dest => dest.SecondaryEmotions, opt => opt.MapFrom(src =>
                    src.SecondaryEmotions.Select(js => js.Emotion.Name).ToList()));

            //Tag
            CreateMap<UserTag, UserTagDto>();
            CreateMap<Tag, TagDto>();
        }
        
    }
}
