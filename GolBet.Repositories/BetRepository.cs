using GolBet.Entities;
using GolBet.Entities.Enums;
using GolBet.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace GolBet.Repositories
{
    public class BetRepository : GenericRepository<Bet>, IBetRepository
    {
        public BetRepository(AppDbContext context) : base(context) { }

        public async Task<bool> ExistsAsync(int matchId, string userId, BetPick pick) =>
            await Context.Bets.AnyAsync(b =>
                b.MatchId == matchId &&
                b.UserId == userId &&
                b.Pick == pick);

        public async Task<IEnumerable<Bet>> GetByUserAsync(string userId) =>
            await Context.Bets
                .AsNoTracking()
                .Include(b => b.Match).ThenInclude(m => m.HomeTeam)
                .Include(b => b.Match).ThenInclude(m => m.AwayTeam)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync();

        public async Task<AppUser?> GetUserForUpdateAsync(string userId) =>
            await Context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        // SIN AsNoTracking a propósito: no es una lectura para mostrar, es el inicio de una
        // modificación. Solo trae las Pending (las ya resueltas nunca se vuelven a pagar) y
        // NO incluye b.Match: el servicio ya tiene ese partido rastreado, y una segunda
        // instancia de la misma fila daría el error "already being tracked".
        public async Task<List<Bet>> GetPendingByMatchAsync(int matchId) =>
            await Context.Bets
                .Include(b => b.User)
                .Where(b => b.MatchId == matchId && b.Status == BetStatus.Pending)
                .ToListAsync();

        public async Task<bool> HasPendingByMatchAsync(int matchId) =>
            await Context.Bets.AnyAsync(b => b.MatchId == matchId && b.Status == BetStatus.Pending);
    }
}
