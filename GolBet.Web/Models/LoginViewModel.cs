using System.ComponentModel.DataAnnotations;

namespace GolBet.Web.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "Escribe un correo válido.")]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = null!;

    [Display(Name = "Recordarme")]
    public bool RememberMe { get; set; }

    // Página protegida que pidió el usuario antes de ser enviado al login.
    public string? ReturnUrl { get; set; }
}
