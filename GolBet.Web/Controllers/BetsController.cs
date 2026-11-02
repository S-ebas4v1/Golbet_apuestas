using GolBet.Entities;
using GolBet.Services;
using GolBet.Services.DTOs;
using GolBet.Services.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GolBet.Web.Controllers
{
    // Solo los apostadores apuestan; el administrador gestiona la cartelera.
    [Authorize(Roles = Roles.Bettor)]
    public class BetsController : Controller
    {
        private readonly IBetService _betService;
        private readonly UserManager<AppUser> _userManager;

        public BetsController(IBetService betService, UserManager<AppUser> userManager)
        {
            _betService = betService;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Place(BetFormDto dto)
        {
            if (!ModelState.IsValid)
            {
                // Monto fuera de rango o una selección que no existe en BetPick.
                TempData["Error"] = "Revisa el monto y la selección de la apuesta.";
                return RedirectToAction("Detail", "Matches", new { id = dto.MatchId });
            }

            // El Id sale de la cookie de sesión, nunca del formulario.
            var userId = _userManager.GetUserId(User)!;

            try
            {
                await _betService.PlaceBetAsync(userId, dto);
                TempData["Success"] = $"¡Apuesta registrada! {dto.Amount:N0} FutCoins en juego.";
            }
            catch (BusinessRuleException ex)
            {
                TempData["Error"] = ex.Message;
            }

            // PRG: siempre se vuelve al detalle del partido; el layout muestra el mensaje.
            return RedirectToAction("Detail", "Matches", new { id = dto.MatchId });
        }

        public async Task<IActionResult> MyBets()
        {
            var userId = _userManager.GetUserId(User)!;
            var bets = await _betService.GetMyBetsAsync(userId);
            return View(bets);
        }
    }
}
