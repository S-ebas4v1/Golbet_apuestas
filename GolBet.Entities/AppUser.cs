using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GolBet.Entities
{
    // Usuario de la aplicación. No hereda de AuditableEntity (C# no tiene herencia múltiple)
    // e IdentityUser ya trae su propio Id de tipo string.
    public class AppUser : IdentityUser
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; } = null!;

        // Saldo en FutCoins, la moneda virtual con la que se apuesta.
        public decimal Balance { get; set; }

        public ICollection<Bet> Bets { get; set; } = new List<Bet>();
    }
}
