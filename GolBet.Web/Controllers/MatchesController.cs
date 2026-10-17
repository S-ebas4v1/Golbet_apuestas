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
            await _matchService.DeactivateAsync(id);
            TempData["Success"] = "Partido desactivado.";
            return RedirectToAction(nameof(Index));
        }

        // Los combos de equipos solo ofrecen equipos activos.
        private async Task LoadTeamsAsync()
        {
            var teams = await _teamService.GetAllAsync();
            ViewBag.Teams = new SelectList(teams, nameof(TeamDto.Id), nameof(TeamDto.Name));
        }
    }
}
