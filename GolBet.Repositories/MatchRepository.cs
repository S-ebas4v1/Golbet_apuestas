using GolBet.Entities;
using GolBet.Entities.Enums;
using GolBet.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace GolBet.Repositories
{
    public class MatchRepository : GenericRepository<Match>, IMatchRepository
    {
        public MatchRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Match>> GetAllWithTeamsAsync(MatchStatus? status = null)
        {
            var query = Context.Matches
                .AsNoTracking()
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => m.IsActive);

            if (status.HasValue)
                query = query.Where(m => m.Status == status.Value);

            return await query.OrderBy(m => m.Date).ToListAsync();
        }

        // Un partido desactivado se comporta como inexistente para el público (404).
        public async Task<Match?> GetByIdWithDetailsAsync(int id) =>
            await Context.Matches
                .AsNoTracking()
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Include(m => m.Bets)
                .FirstOrDefaultAsync(m => m.Id == id && m.IsActive);

        public async Task<bool> TeamHasPendingMatchesAsync(int teamId) =>
            await Context.Matches.AnyAsync(m =>
                m.IsActive &&
                m.Status != MatchStatus.Finished &&
                (m.HomeTeamId == teamId || m.AwayTeamId == teamId));
    }
}
