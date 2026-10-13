using GolBet.Entities;
using GolBet.Entities.Enums;

namespace GolBet.Repositories
{
    public interface IMatchRepository : IGenericRepository<Match>
    {
        // Cartelera: partidos activos con sus equipos ya cargados, filtrando en la BD
        // (y no en memoria) cuando se pide un estado concreto.
        Task<IEnumerable<Match>> GetAllWithTeamsAsync(MatchStatus? status = null);

        // Detalle: además de los equipos, trae las apuestas para el contador de la vista.
        Task<Match?> GetByIdWithDetailsAsync(int id);

        // ¿El equipo tiene partidos activos aún por jugarse o en juego? Se usa para
        // impedir desactivar un equipo que dejaría huérfana la cartelera.
        Task<bool> TeamHasPendingMatchesAsync(int teamId);
    }
}
