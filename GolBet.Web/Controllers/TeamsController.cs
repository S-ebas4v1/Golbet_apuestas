using GolBet.Entities;
using GolBet.Services;
using GolBet.Services.DTOs;
using GolBet.Services.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GolBet.Web.Controllers
{
    // Toda la gestión de equipos es exclusiva del administrador.
    [Authorize(Roles = Roles.Admin)]
    public class TeamsController : Controller
    {
        private readonly ITeamService _teamService;

        public TeamsController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        public async Task<IActionResult> Index()
        {
            var teams = await _teamService.GetAllAsync();
            return View(teams);
        }

        public IActionResult Create() => View(new TeamFormDto());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeamFormDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                await _teamService.CreateAsync(dto);
            }
            catch (BusinessRuleException ex)
            {
                ModelState.AddModelError(nameof(dto.Name), ex.Message);
                return View(dto);
            }

            // PRG: tras un POST exitoso se redirige, así F5 no vuelve a crear el equipo.
            TempData["Success"] = $"Equipo «{dto.Name}» creado.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _teamService.GetForEditAsync(id);
            if (dto is null)
                return NotFound();

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TeamFormDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                await _teamService.UpdateAsync(dto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (BusinessRuleException ex)
            {
                ModelState.AddModelError(nameof(dto.Name), ex.Message);
                return View(dto);
            }

            TempData["Success"] = $"Equipo «{dto.Name}» actualizado.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            try
            {
                await _teamService.DeactivateAsync(id);
                TempData["Success"] = "Equipo desactivado.";
            }
            catch (BusinessRuleException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
