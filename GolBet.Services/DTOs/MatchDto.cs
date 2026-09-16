using GolBet.Entities.Enums;

namespace GolBet.Services.DTOs
{
    // DTO plano para la cartelera: nada de navegación a Team, solo lo que la vista necesita.
    public class MatchDto
    {
        public int Id { get; set; }

        // Hora local del partido, tal como quedó guardada (ver AppDbContext: no es UTC).
        public DateTime Date { get; set; }

        public MatchStatus Status { get; set; }

        public int? HomeGoals { get; set; }
        public int? AwayGoals { get; set; }

        public decimal HomeOdds { get; set; }
        public decimal DrawOdds { get; set; }
        public decimal AwayOdds { get; set; }

        public string HomeTeamName { get; set; } = null!;
        public string? HomeTeamCrestUrl { get; set; }

        public string AwayTeamName { get; set; } = null!;
        public string? AwayTeamCrestUrl { get; set; }
    }
}
