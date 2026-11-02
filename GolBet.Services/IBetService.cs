using GolBet.Services.DTOs;

namespace GolBet.Services
{
    public interface IBetService
    {
        // Registra la apuesta del usuario; lanza BusinessRuleException si viola una regla.
        Task PlaceBetAsync(string userId, BetFormDto dto);

        Task<IEnumerable<BetDto>> GetMyBetsAsync(string userId);
    }
}
