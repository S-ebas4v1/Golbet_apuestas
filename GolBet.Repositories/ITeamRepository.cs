using GolBet.Entities;

namespace GolBet.Repositories
{
    public interface ITeamRepository : IGenericRepository<Team>
    {
        // Revisa también equipos inactivos: el índice único sobre Name no distingue
        // activos de inactivos, así que el nombre sigue "ocupado" tras una baja lógica.
        Task<bool> NameExistsAsync(string name, int excludeId = 0);
    }
}
