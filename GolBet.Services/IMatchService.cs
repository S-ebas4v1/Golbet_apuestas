using GolBet.Entities.Enums;
using GolBet.Services.DTOs;

namespace GolBet.Services
{
    public interface IMatchService
    {
        Task<IEnumerable<MatchDto>> GetBoardAsync(MatchStatus? status = null, string? search = null);

        Task<MatchDetailDto?> GetByIdAsync(int id);

        Task<MatchFormDto?> GetForEditAsync(int id);

        Task CreateAsync(MatchFormDto dto);

        Task UpdateAsync(MatchFormDto dto);

        Task DeactivateAsync(int id);

        // Máquina de estados: Programado -> En juego (cierra las apuestas).
        Task StartMatchAsync(int id);

        // En juego -> Finalizado: registra el marcador y resuelve y paga las apuestas.
        Task<ResolutionSummaryDto> CloseMatchAsync(MatchResultFormDto dto);
    }
}
