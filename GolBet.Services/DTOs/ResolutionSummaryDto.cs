namespace GolBet.Services.DTOs
{
    // El "recibo" del cierre: cuántas apuestas se resolvieron, cuántas ganaron y cuánto se pagó.
    public class ResolutionSummaryDto
    {
        public int ResolvedBets { get; set; }
        public int WonBets { get; set; }
        public decimal TotalPaid { get; set; }
    }
}
