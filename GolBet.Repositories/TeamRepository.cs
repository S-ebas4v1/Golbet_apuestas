using GolBet.Entities;
using GolBet.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace GolBet.Repositories
{
    public class TeamRepository : GenericRepository<Team>, ITeamRepository
    {
        public TeamRepository(AppDbContext context) : base(context) { }

        // La columna Name usa la collation golbet_ci_ai (ver AppDbContext), así que esta
        // igualdad ya ignora mayúsculas y acentos: "millonarios" choca con "Millonarios".
        public async Task<bool> NameExistsAsync(string name, int excludeId = 0) =>
            await Context.Teams.AnyAsync(t => t.Name == name.Trim() && t.Id != excludeId);
    }
}
