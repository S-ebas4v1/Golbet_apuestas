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

            CreateMap<Team, TeamDto>();

            // Formulario -> entidad: solo los campos editables. Id, auditoría, IsActive,
            // estado, goles y navegaciones nunca se toman del formulario.
            CreateMap<Team, TeamFormDto>();
            CreateMap<TeamFormDto, Team>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.CreatedDate, o => o.Ignore())
                .ForMember(d => d.ModifiedDate, o => o.Ignore())
                .ForMember(d => d.IsActive, o => o.Ignore())
                .ForMember(d => d.CrestUrl, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.CrestUrl) ? null : s.CrestUrl.Trim()));

            CreateMap<Match, MatchFormDto>();
            CreateMap<MatchFormDto, Match>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.CreatedDate, o => o.Ignore())
                .ForMember(d => d.ModifiedDate, o => o.Ignore())
                .ForMember(d => d.IsActive, o => o.Ignore())
                .ForMember(d => d.Status, o => o.Ignore())
                .ForMember(d => d.HomeGoals, o => o.Ignore())
                .ForMember(d => d.AwayGoals, o => o.Ignore())
                .ForMember(d => d.HomeTeam, o => o.Ignore())
                .ForMember(d => d.AwayTeam, o => o.Ignore())
                .ForMember(d => d.Bets, o => o.Ignore());
        }
    }
}
