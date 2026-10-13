namespace GolBet.Services.DTOs
{
    // Fila del listado de equipos del administrador.
    public class TeamDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string City { get; set; } = null!;
        public string? CrestUrl { get; set; }
    }
}
