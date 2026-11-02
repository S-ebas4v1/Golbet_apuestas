using GolBet.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace GolBet.Services.DTOs
{
    // Lo que envía el panel de apuesta. Quién apuesta NO viaja en el formulario:
    // sale de la cookie de sesión, para que nadie pueda apostar a nombre de otro.
    public class BetFormDto
    {
        public int MatchId { get; set; }

        public BetPick Pick { get; set; }

        [Range(typeof(decimal), "1", "1000000000", ErrorMessage = "El monto debe ser mayor que cero.")]
        [Display(Name = "Monto")]
        public decimal Amount { get; set; }
    }
}
