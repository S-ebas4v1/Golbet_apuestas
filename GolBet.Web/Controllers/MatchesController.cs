using GolBet.Entities;
using GolBet.Entities.Enums;
using GolBet.Services;
using GolBet.Services.DTOs;
using GolBet.Services.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GolBet.Web.Controllers
{
    public class MatchesController : Controller
    {
        private readonly IMatchService _matchService;
        private readonly ITeamService _teamService;

        public MatchesController(IMatchService matchService, ITeamService teamService)
        {
            _matchService = matchService;
            _teamService = teamService;
        }

        // GET /Matches?status=Scheduled&search=nacional — ambos parámetros llegan por model binding.
        public async Task<IActionResult> Index(MatchStatus? status, string? search)
        {
            var matches = await _matchService.GetBoardAsync(status, search);

            // La vista los necesita para resaltar el filtro activo, rellenar el campo de búsqueda
            // y que filtros y buscador se conserven entre sí.
            ViewBag.CurrentStatus = status;
            ViewBag.CurrentSearch = search?.Trim();
            return View(matches);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var match = await _matchService.GetByIdAsync(id);
            if (match is null)
                return NotFound();

            return View(match);
        }

        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create()
        {
            await LoadTeamsAsync();
            return View(new MatchFormDto());
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MatchFormDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _matchService.CreateAsync(dto);
                    TempData["Success"] = "Partido creado.";
                    return RedirectToAction(nameof(Index));
                }
                catch (BusinessRuleException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            await LoadTeamsAsync();
            return View(dto);
        }

        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _matchService.GetForEditAsync(id);
            if (dto is null)
                return NotFound();

            await LoadTeamsAsync();
            return View(dto);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MatchFormDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _matchService.UpdateAsync(dto);
                    TempData["Success"] = "Partido actualizado.";
                    return RedirectToAction(nameof(Detail), new { id = dto.Id });
                }
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }
                catch (BusinessRuleException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            await LoadTeamsAsync();
            return View(dto);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            try
            {
                await _matchService.DeactivateAsync(id);
            }
            catch (BusinessRuleException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Detail), new { id });
            }

            TempData["Success"] = "Partido desactivado.";
            return RedirectToAction(nameof(Index));
        }

        // POST /Matches/Start/5 — cambia el estado, por eso es POST (con confirmación en la vista).
        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Start(int id)
        {
            try
            {
                await _matchService.StartMatchAsync(id);
                TempData["Success"] = "¡Partido en juego! Las apuestas quedaron cerradas.";
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (BusinessRuleException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Detail), new { id });
        }

        // GET /Matches/Close/5 — formulario del marcador. El GET no es el guardia:
        // las reglas de la máquina de estados las aplica el servicio al enviar.
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Close(int id)
        {
            var match = await _matchService.GetByIdAsync(id);
            if (match is null)
                return NotFound();

            // El modelo de la vista es el marcador; los datos del partido solo decoran el encabezado.
            ViewBag.Match = match;
            return View(new MatchResultFormDto { MatchId = id });
        }

        // POST /Matches/Close — cierra el partido y resuelve las apuestas.
        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Close(MatchResultFormDto dto)
        {
            if (!ModelState.IsValid)
            {
                var match = await _matchService.GetByIdAsync(dto.MatchId);
                if (match is null)
                    return NotFound();

                ViewBag.Match = match;
                return View(dto);
            }

            try
            {
                var summary = await _matchService.CloseMatchAsync(dto);

                // El recibo de la resolución es el mensaje de éxito del Admin.
                TempData["Success"] =
                    $"Partido cerrado {dto.HomeGoals}-{dto.AwayGoals}. " +
                    $"{summary.ResolvedBets} apuesta(s) resuelta(s): {summary.WonBets} ganada(s), " +
                    $"{summary.TotalPaid:N0} FutCoins pagados.";
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (BusinessRuleException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Detail), new { id = dto.MatchId });
        }

        // Los combos de equipos solo ofrecen equipos activos.
        private async Task LoadTeamsAsync()
        {
            var teams = await _teamService.GetAllAsync();
            ViewBag.Teams = new SelectList(teams, nameof(TeamDto.Id), nameof(TeamDto.Name));
        }
    }
}
