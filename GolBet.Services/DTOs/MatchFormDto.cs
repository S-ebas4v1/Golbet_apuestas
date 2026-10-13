using System.ComponentModel.DataAnnotations;

namespace GolBet.Services.DTOs
{
    // Formulario de creación/edición de partidos. Las reglas que cruzan campos o
    // consultan la BD (equipos distintos y activos, fecha futura) viven en MatchService.
    public class MatchFormDto
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona el equipo local.")]
        [Display(Name = "Equipo local")]
        public int HomeTeamId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona el equipo visitante.")]
        [Display(Name = "Equipo visitante")]
        public int AwayTeamId { get; set; }

        // Hora local del partido, igual que Match.Date (no se convierte a UTC).
        [Required(ErrorMessage = "La fecha es obligatoria.")]
        [Display(Name = "Fecha y hora")]
        public DateTime Date { get; set; } = DateTime.Today.AddDays(7).AddHours(19);

        [Range(typeof(decimal), "1.01", "999.99", ErrorMessage = "La cuota debe estar entre 1.01 y 999.99.")]
        [Display(Name = "Cuota local (1)")]
        public decimal HomeOdds { get; set; } = 2.00m;

        [Range(typeof(decimal), "1.01", "999.99", ErrorMessage = "La cuota debe estar entre 1.01 y 999.99.")]
        [Display(Name = "Cuota empate (X)")]
        public decimal DrawOdds { get; set; } = 3.00m;

        [Range(typeof(decimal), "1.01", "999.99", ErrorMessage = "La cuota debe estar entre 1.01 y 999.99.")]
        [Display(Name = "Cuota visitante (2)")]
        public decimal AwayOdds { get; set; } = 3.50m;
    }
}
