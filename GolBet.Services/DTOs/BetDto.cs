using GolBet.Entities.Enums;

namespace GolBet.Services.DTOs
{
    // Fila de "Mis apuestas": plano, con los datos del partido ya aplanados.
    public class BetDto
    {
        public int Id { get; set; }

        public string HomeTeamName { get; set; } = null!;
        public string AwayTeamName { get; set; } = null!;

        // Hora local del partido, igual que Match.Date.
        public DateTime MatchDate { get; set; }

        public BetPick Pick { get; set; }
        public decimal Amount { get; set; }
        public decimal OddsAtPlacement { get; set; }

        // Lo que recibiría si acierta: monto x cuota congelada.
        public decimal PotentialPayout { get; set; }

        public BetStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
