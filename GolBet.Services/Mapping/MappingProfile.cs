using AutoMapper;
using GolBet.Entities;
using GolBet.Services.DTOs;

namespace GolBet.Services.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Match, MatchDto>()
                .ForMember(d => d.HomeTeamName, o => o.MapFrom(s => s.HomeTeam.Name))
                .ForMember(d => d.HomeTeamCrestUrl, o => o.MapFrom(s => s.HomeTeam.CrestUrl))
                .ForMember(d => d.AwayTeamName, o => o.MapFrom(s => s.AwayTeam.Name))
                .ForMember(d => d.AwayTeamCrestUrl, o => o.MapFrom(s => s.AwayTeam.CrestUrl));

            CreateMap<Match, MatchDetailDto>()
                .IncludeBase<Match, MatchDto>()
                .ForMember(d => d.BetsCount, o => o.MapFrom(s => s.Bets.Count));
        }
    }
}
