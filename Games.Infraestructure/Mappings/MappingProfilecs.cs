using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using AutoMapper;
using Gamess.Core.Entities;

using Gamess.Infraestructure.DTOs;

namespace Games.Infrastructure.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Game, GameDto>().ReverseMap();
            CreateMap<Review, ReviewDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Security, SecurityDto>().ReverseMap();
            CreateMap<Game, GameDto>()

    .ForMember(d => d.AverageScore,
        o => o.MapFrom(s => s.Reviews.Count == 0 ? (double?)null : s.Reviews.Average(r => r.Score)))
    .ForMember(d => d.ReviewsCount,
        o => o.MapFrom(s => s.Reviews.Count));

        }
        
    }

}

