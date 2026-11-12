using System.ComponentModel.DataAnnotations;

namespace GolBet.Services.DTOs
{
    // El marcador final que envía el Admin para cerrar un partido.
    public class MatchResultFormDto
    {
        public int MatchId { get; set; }

        [Range(0, 99, ErrorMessage = "Los goles deben estar entre 0 y 99.")]
        [Display(Name = "Goles del local")]
        public int HomeGoals { get; set; }

        [Range(0, 99, ErrorMessage = "Los goles deben estar entre 0 y 99.")]
        [Display(Name = "Goles del visitante")]
        public int AwayGoals { get; set; }
    }
}
