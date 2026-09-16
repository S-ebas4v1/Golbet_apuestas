using GolBet.Entities.Enums;
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

        public async Task<IActionResult> Index(MatchStatus? status)
        {
            var matches = await _matchService.GetBoardAsync(status);
            ViewBag.CurrentStatus = status;
            return View(matches);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var match = await _matchService.GetByIdAsync(id);
            if (match is null)
                return NotFound();

            return View(match);
        }
    }
}
