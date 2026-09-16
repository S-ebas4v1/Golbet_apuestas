using GolBet.Entities;
using GolBet.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace GolBet.Repositories
{
    public class MatchRepository : GenericRepository<Match>, IMatchRepository
    {
        public MatchRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Match>> GetAllWithTeamsAsync() =>
            await Context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .OrderBy(m => m.Date)
                .ToListAsync();

        public async Task<Match?> GetByIdWithDetailsAsync(int id) =>
            await Context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Include(m => m.Bets)
                .FirstOrDefaultAsync(m => m.Id == id);
    }
}
