using GolBet.Entities;

namespace GolBet.Repositories
{
    public interface IMatchRepository : IGenericRepository<Match>
    {
        // Cartelera: partidos con sus equipos ya cargados, sin ida y vuelta extra a la BD.
        Task<IEnumerable<Match>> GetAllWithTeamsAsync();

        // Detalle: además de los equipos, trae las apuestas para el contador de la vista.
        Task<Match?> GetByIdWithDetailsAsync(int id);
    }
}
