using GolBet.Services.DTOs;

namespace GolBet.Services
{
    public interface ITeamService
    {
        Task<IEnumerable<TeamDto>> GetAllAsync();

        Task<TeamFormDto?> GetForEditAsync(int id);

        Task CreateAsync(TeamFormDto dto);

        Task UpdateAsync(TeamFormDto dto);

        Task DeactivateAsync(int id);
    }
}
