using AutoMapper;
using Lumina.DTO.JournalEntry;
using Lumina.Models;

namespace Lumina.Services.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Create and Update mappings
            CreateMap<CreateJournalEntryDto, JournalEntry>();
            CreateMap<UpdateJournalEntryDto, JournalEntry>();

            // Read mapping: JournalEntry -> JournalEntryDto
            CreateMap<JournalEntry, JournalEntryDto>()
                .ForMember(dest => dest.PrimaryMoodName, opt => opt.MapFrom(src => src.PrimaryMood.Name))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src =>
                    src.Tags.Select(jt => jt.Tag.Name).ToList()))
                .ForMember(dest => dest.SecondaryEmotions, opt => opt.MapFrom(src =>
                    src.SecondaryEmotions.Select(js => js.Emotion.Name).ToList()));
        }
        
    }
}
