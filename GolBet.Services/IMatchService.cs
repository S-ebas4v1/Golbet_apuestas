using GolBet.Entities.Enums;
using GolBet.Services.DTOs;

namespace GolBet.Services
{
    public interface IMatchService
    {
        Task<IEnumerable<MatchDto>> GetBoardAsync(MatchStatus? status = null);

        Task<MatchDto?> GetByIdAsync(int id);
    }
}
