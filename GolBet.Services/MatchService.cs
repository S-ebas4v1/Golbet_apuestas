using AutoMapper;
using GolBet.Entities;
using GolBet.Entities.Enums;
using GolBet.Repositories;
using GolBet.Services.DTOs;
using GolBet.Services.Exceptions;

namespace GolBet.Services
{
    public class MatchService : IMatchService
    {
        private readonly IMatchRepository _matchRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IMapper _mapper;

        public MatchService(IMatchRepository matchRepository, ITeamRepository teamRepository, IMapper mapper)
        {
            _matchRepository = matchRepository;
            _teamRepository = teamRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MatchDto>> GetBoardAsync(MatchStatus? status = null)
        {
            var matches = await _matchRepository.GetAllWithTeamsAsync(status);
            return _mapper.Map<IEnumerable<MatchDto>>(matches);
        }

        public async Task<MatchDetailDto?> GetByIdAsync(int id)
        {
            var match = await _matchRepository.GetByIdWithDetailsAsync(id);
            return match is null ? null : _mapper.Map<MatchDetailDto>(match);
        }

        public async Task<MatchFormDto?> GetForEditAsync(int id)
        {
            var match = await _matchRepository.GetByIdAsync(id);
            return match is null || !match.IsActive ? null : _mapper.Map<MatchFormDto>(match);
        }

        public async Task CreateAsync(MatchFormDto dto)
        {
            await ValidateTeamsAsync(dto);
            EnsureFutureDate(dto.Date);

            await _matchRepository.AddAsync(_mapper.Map<Match>(dto));
            await _matchRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(MatchFormDto dto)
        {
            var match = await _matchRepository.GetByIdAsync(dto.Id);
            if (match is null || !match.IsActive)
                throw new KeyNotFoundException($"No existe el partido {dto.Id}.");

            await ValidateTeamsAsync(dto);

            // Solo se exige fecha futura si se está reprogramando: así un partido ya
            // jugado puede corregir sus cuotas sin chocar con esta regla.
            if (dto.Date != match.Date)
                EnsureFutureDate(dto.Date);

            _mapper.Map(dto, match);
            await _matchRepository.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id)
        {
            if (await _matchRepository.DeactivateAsync(id))
                await _matchRepository.SaveChangesAsync();
        }

        private async Task ValidateTeamsAsync(MatchFormDto dto)
        {
            if (dto.HomeTeamId == dto.AwayTeamId)
                throw new BusinessRuleException("El equipo local y el visitante no pueden ser el mismo.");

            var home = await _teamRepository.GetByIdAsync(dto.HomeTeamId);
            var away = await _teamRepository.GetByIdAsync(dto.AwayTeamId);

            if (home is not { IsActive: true } || away is not { IsActive: true })
                throw new BusinessRuleException("Ambos equipos deben existir y estar activos.");
        }

        // Match.Date es hora local (ver AppDbContext), por eso se compara contra DateTime.Now.
        private static void EnsureFutureDate(DateTime date)
        {
            if (date <= DateTime.Now)
                throw new BusinessRuleException("La fecha del partido debe ser futura.");
        }
    }
}
