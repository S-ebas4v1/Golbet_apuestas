using AutoMapper;
using GolBet.Entities;
using GolBet.Repositories;
using GolBet.Services.DTOs;
using GolBet.Services.Exceptions;

namespace GolBet.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly IMapper _mapper;

        public TeamService(ITeamRepository teamRepository, IMatchRepository matchRepository, IMapper mapper)
        {
            _teamRepository = teamRepository;
            _matchRepository = matchRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TeamDto>> GetAllAsync()
        {
            var teams = await _teamRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<TeamDto>>(teams.OrderBy(t => t.Name));
        }

        public async Task<TeamFormDto?> GetForEditAsync(int id)
        {
            var team = await _teamRepository.GetByIdAsync(id);
            return team is null || !team.IsActive ? null : _mapper.Map<TeamFormDto>(team);
        }

        public async Task CreateAsync(TeamFormDto dto)
        {
            await EnsureUniqueNameAsync(dto);

            await _teamRepository.AddAsync(_mapper.Map<Team>(dto));
            await _teamRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(TeamFormDto dto)
        {
            var team = await _teamRepository.GetByIdAsync(dto.Id);
            if (team is null || !team.IsActive)
                throw new KeyNotFoundException($"No existe el equipo {dto.Id}.");

            await EnsureUniqueNameAsync(dto);

            // Copia los valores del formulario sobre la entidad que EF ya está rastreando.
            _mapper.Map(dto, team);
            await _teamRepository.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id)
        {
            if (await _matchRepository.TeamHasPendingMatchesAsync(id))
                throw new BusinessRuleException(
                    "No se puede desactivar: el equipo tiene partidos programados o en juego.");

            if (await _teamRepository.DeactivateAsync(id))
                await _teamRepository.SaveChangesAsync();
        }

        // Se valida antes de guardar para mostrar un mensaje claro en el formulario
        // en lugar de dejar que el índice único reviente con una DbUpdateException.
        private async Task EnsureUniqueNameAsync(TeamFormDto dto)
        {
            dto.Name = dto.Name.Trim();
            dto.City = dto.City.Trim();

            if (await _teamRepository.NameExistsAsync(dto.Name, dto.Id))
                throw new BusinessRuleException($"Ya existe un equipo llamado «{dto.Name}».");
        }
    }
}
