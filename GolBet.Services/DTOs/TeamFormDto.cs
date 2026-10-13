using System.ComponentModel.DataAnnotations;

namespace GolBet.Services.DTOs
{
    // Un solo DTO para crear y editar: Id = 0 significa "nuevo".
    public class TeamFormDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(80, ErrorMessage = "El nombre admite máximo 80 caracteres.")]
        [Display(Name = "Nombre")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "La ciudad es obligatoria.")]
        [MaxLength(60, ErrorMessage = "La ciudad admite máximo 60 caracteres.")]
        [Display(Name = "Ciudad")]
        public string City { get; set; } = null!;

        [Url(ErrorMessage = "Escribe una URL válida (https://...).")]
        [MaxLength(300, ErrorMessage = "La URL admite máximo 300 caracteres.")]
        [Display(Name = "URL del escudo (opcional)")]
        public string? CrestUrl { get; set; }
    }
}
