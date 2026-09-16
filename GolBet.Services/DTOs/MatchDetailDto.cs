namespace GolBet.Services.DTOs
{
    // Todo lo del MatchDto de la cartelera, más lo que solo necesita la página de detalle.
    public class MatchDetailDto : MatchDto
    {
        public int BetsCount { get; set; }
    }
}
