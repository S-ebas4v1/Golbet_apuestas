using GolBet.Entities;
using GolBet.Entities.Enums;

namespace GolBet.Repositories
{
    public interface IBetRepository : IGenericRepository<Bet>
    {
        // Regla de negocio: un usuario no repite la misma selección en el mismo partido.
        Task<bool> ExistsAsync(int matchId, string userId, BetPick pick);

        // "Mis apuestas": historial del usuario, la más reciente primero, con partido y equipos.
        Task<IEnumerable<Bet>> GetByUserAsync(string userId);

        // El apostador se carga con seguimiento (tracking) en el mismo contexto que la apuesta,
        // así el descuento del saldo y la apuesta nueva se guardan en un solo SaveChanges.
        Task<AppUser?> GetUserForUpdateAsync(string userId);
    }
}
