using Microsoft.AspNetCore.Identity;

namespace GolBet.Web.Helpers;

// Identity trae sus mensajes en inglés; aquí se traducen los que puede ver un usuario
// al registrarse o iniciar sesión. Los demás conservan el texto por defecto.
public class SpanishIdentityErrorDescriber : IdentityErrorDescriber
{
    // El correo también es el nombre de usuario, así que ambos duplicados dicen lo mismo
    // (el controlador descarta el mensaje repetido).
    public override IdentityError DuplicateUserName(string userName) =>
        new() { Code = nameof(DuplicateUserName), Description = $"Ya existe una cuenta con el correo {userName}." };

    public override IdentityError DuplicateEmail(string email) =>
        new() { Code = nameof(DuplicateEmail), Description = $"Ya existe una cuenta con el correo {email}." };

    public override IdentityError InvalidEmail(string? email) =>
        new() { Code = nameof(InvalidEmail), Description = $"El correo {email} no es válido." };

    public override IdentityError PasswordTooShort(int length) =>
        new() { Code = nameof(PasswordTooShort), Description = $"La contraseña debe tener al menos {length} caracteres." };

    public override IdentityError PasswordRequiresDigit() =>
        new() { Code = nameof(PasswordRequiresDigit), Description = "La contraseña debe tener al menos un número (0-9)." };

    public override IdentityError PasswordRequiresLower() =>
        new() { Code = nameof(PasswordRequiresLower), Description = "La contraseña debe tener al menos una letra minúscula." };
}
