using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Lumina.DTO.JournalEntry;
using Lumina.Models;

namespace Lumina.Services.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateJournalEntryDto, JournalEntry>();
            CreateMap<JournalEntry, JournalEntryDto>()
                .ForMember(dest => dest.PrimaryMoodName, opt => opt.MapFrom(src => src.PrimaryMood.Name));
        }
        
    }
}
