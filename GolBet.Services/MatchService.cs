using AutoMapper;
using GolBet.Entities.Enums;
using GolBet.Repositories;
using GolBet.Services.DTOs;

namespace GolBet.Services
{
    public class MatchService : IMatchService
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IMapper _mapper;

        public MatchService(IMatchRepository matchRepository, IMapper mapper)
        {
            _matchRepository = matchRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MatchDto>> GetBoardAsync(MatchStatus? status = null)
        {
            var matches = await _matchRepository.GetAllWithTeamsAsync();

            if (status.HasValue)
                matches = matches.Where(m => m.Status == status.Value).ToList();

            return _mapper.Map<IEnumerable<MatchDto>>(matches);
        }

        public async Task<MatchDto?> GetByIdAsync(int id)
        {
            var match = await _matchRepository.GetByIdWithDetailsAsync(id);
            return match is null ? null : _mapper.Map<MatchDto>(match);
        }
    }
}
