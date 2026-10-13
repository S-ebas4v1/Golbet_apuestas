using GolBet.Entities.Common;
using GolBet.Entities.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace GolBet.Entities
{
    public class Bet : AuditableEntity
    {
        [Column(TypeName = "decimal(12,2)")]
        public decimal Amount { get; set; }

        // Cuota congelada al momento de apostar: si el admin la cambia después, esta apuesta no se ve afectada.
        [Column(TypeName = "decimal(5,2)")]
        public decimal OddsAtPlacement { get; set; }

        public BetPick Pick { get; set; }

        public BetStatus Status { get; set; } = BetStatus.Pending;

        public int MatchId { get; set; }
        public Match Match { get; set; } = null!;

        // Dueño de la apuesta. FK de tipo string porque la PK de Identity (AspNetUsers) lo es.
        public string UserId { get; set; } = null!;
        public AppUser User { get; set; } = null!;
    }
}
