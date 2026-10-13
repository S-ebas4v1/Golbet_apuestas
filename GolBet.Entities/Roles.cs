namespace GolBet.Entities
{
    // Nombres de rol como constantes: un error de tipeo en [Authorize(Roles = ...)]
    // no compila, en vez de dejar una página abierta o cerrada en silencio.
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Bettor = "Apostador";
    }
}
