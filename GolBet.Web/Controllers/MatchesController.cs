using GolBet.Services;
using Microsoft.AspNetCore.Mvc;

namespace GolBet.Web.Controllers
{
    public class MatchesController : Controller
    {
        private readonly IMatchService _matchService;

        public MatchesController(IMatchService matchService)
        {
            _matchService = matchService;
        }

        public async Task<IActionResult> Index()
        {
            var matches = await _matchService.GetBoardAsync();
            return View(matches);
        }
    }
}
