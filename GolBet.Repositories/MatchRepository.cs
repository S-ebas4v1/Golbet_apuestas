using GolBet.Entities;
using GolBet.Entities.Enums;
using GolBet.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace GolBet.Repositories
{
    public class MatchRepository : GenericRepository<Match>, IMatchRepository
    {
        public MatchRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Match>> GetAllWithTeamsAsync(MatchStatus? status = null, string? search = null)
        {
            var query = Context.Matches
                .AsNoTracking()
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => m.IsActive);

            if (status.HasValue)
                query = query.Where(m => m.Status == status.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                // % y _ son comodines de LIKE: se escapan para buscarlos como texto literal.
                var pattern = $"%{EscapeLikePattern(search.Trim())}%";

                // Teams.Name usa la collation no determinística golbet_ci_ai y PostgreSQL no
                // permite LIKE/ILIKE sobre ella, por eso se compara con la collation "default".
                // ILIKE ignora mayúsculas; el OR cubre al equipo local y al visitante.
                // El carácter de escape se indica explícito: sin él, Npgsql genera ESCAPE ''
                // y la barra invertida de EscapeLikePattern dejaría de funcionar.
                query = query.Where(m =>
                    EF.Functions.ILike(EF.Functions.Collate(m.HomeTeam.Name, "default"), pattern, LikeEscape) ||
                    EF.Functions.ILike(EF.Functions.Collate(m.AwayTeam.Name, "default"), pattern, LikeEscape));
            }

            return await query.OrderBy(m => m.Date).ToListAsync();
        }

        private const string LikeEscape = @"\";

        private static string EscapeLikePattern(string text) =>
            text.Replace(LikeEscape, LikeEscape + LikeEscape)
                .Replace("%", LikeEscape + "%")
                .Replace("_", LikeEscape + "_");

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
