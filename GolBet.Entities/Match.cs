using GolBet.Entities.Common;
using GolBet.Entities.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace GolBet.Entities
{
    public class Match : AuditableEntity
    {
        public DateTime Date { get; set; }

        public MatchStatus Status { get; set; } = MatchStatus.Scheduled;

        public int? HomeGoals { get; set; }
        public int? AwayGoals { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal HomeOdds { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal DrawOdds { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal AwayOdds { get; set; }

        // Doble relación con Team: el mismo equipo puede aparecer como local o visitante.
        public int HomeTeamId { get; set; }
        public Team HomeTeam { get; set; } = null!;

        public int AwayTeamId { get; set; }
        public Team AwayTeam { get; set; } = null!;

        public ICollection<Bet> Bets { get; set; } = new List<Bet>();
    }
}
