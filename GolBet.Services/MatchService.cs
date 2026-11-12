using AutoMapper;
using GolBet.Entities;
using GolBet.Entities.Enums;
using GolBet.Repositories;
using GolBet.Services.DTOs;
using GolBet.Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace GolBet.Services
{
    public class MatchService : IMatchService
    {
        private readonly IMatchRepository _matchRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IBetRepository _betRepository;
        private readonly IMapper _mapper;

        public MatchService(
            IMatchRepository matchRepository,
            ITeamRepository teamRepository,
            IBetRepository betRepository,
            IMapper mapper)
        {
            _matchRepository = matchRepository;
            _teamRepository = teamRepository;
            _betRepository = betRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MatchDto>> GetBoardAsync(MatchStatus? status = null, string? search = null)
        {
            var matches = await _matchRepository.GetAllWithTeamsAsync(status, search);
            return _mapper.Map<IEnumerable<MatchDto>>(matches);
        }

        public async Task<MatchDetailDto?> GetByIdAsync(int id)
        {
            var match = await _matchRepository.GetByIdWithDetailsAsync(id);
            return match is null ? null : _mapper.Map<MatchDetailDto>(match);
        }

        public async Task<MatchFormDto?> GetForEditAsync(int id)
        {
            var match = await _matchRepository.GetByIdAsync(id);
            return match is null || !match.IsActive ? null : _mapper.Map<MatchFormDto>(match);
        }

        public async Task CreateAsync(MatchFormDto dto)
        {
            await ValidateTeamsAsync(dto);
            EnsureFutureDate(dto.Date);

            await _matchRepository.AddAsync(_mapper.Map<Match>(dto));
            await _matchRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(MatchFormDto dto)
        {
            var match = await _matchRepository.GetByIdAsync(dto.Id);
            if (match is null || !match.IsActive)
                throw new KeyNotFoundException($"No existe el partido {dto.Id}.");

            // Un partido En juego o Finalizado ya no se edita: cambiarle equipos o cuotas
            // después de cerrar las apuestas (o de pagarlas) no tiene sentido.
            if (match.Status != MatchStatus.Scheduled)
                throw new BusinessRuleException("Solo un partido programado puede editarse.");

            await ValidateTeamsAsync(dto);

            // Solo se exige fecha futura si se está reprogramando: así se pueden ajustar
            // las cuotas de un partido programado sin tocar su fecha.
            if (dto.Date != match.Date)
                EnsureFutureDate(dto.Date);

            _mapper.Map(dto, match);
            await _matchRepository.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id)
        {
            // Si quedan apuestas sin resolver, desactivar el partido dejaría ese dinero atrapado.
            if (await _betRepository.HasPendingByMatchAsync(id))
                throw new BusinessRuleException(
                    "No se puede desactivar: el partido tiene apuestas pendientes. Ciérralo para resolverlas.");

            if (await _matchRepository.DeactivateAsync(id))
                await _matchRepository.SaveChangesAsync();
        }

        public async Task StartMatchAsync(int id)
        {
            var match = await GetActiveMatchAsync(id);

            if (match.Status != MatchStatus.Scheduled)
                throw new BusinessRuleException("Solo un partido programado puede iniciarse.");

            // Al pasar a En juego, BetService rechaza nuevas apuestas (regla 1).
            match.Status = MatchStatus.InProgress;
            await _matchRepository.SaveChangesAsync();
        }

        public async Task<ResolutionSummaryDto> CloseMatchAsync(MatchResultFormDto dto)
        {
            var match = await GetActiveMatchAsync(dto.MatchId);

            // Guardias de la máquina de estados. El de Finalizado es el primer cinturón
            // de la idempotencia: un partido cerrado no se vuelve a pagar.
            if (match.Status == MatchStatus.Finished)
                throw new BusinessRuleException("Este partido ya fue cerrado y sus apuestas resueltas.");

            if (match.Status != MatchStatus.InProgress)
                throw new BusinessRuleException("El partido debe estar «En juego» para cerrarse. Inícialo primero.");

            // 1. Marcador final y nuevo estado.
            match.HomeGoals = dto.HomeGoals;
            match.AwayGoals = dto.AwayGoals;
            match.Status = MatchStatus.Finished;

            // 2. Qué selección ganó según el marcador.
            var winningPick = GetWinningPick(dto.HomeGoals, dto.AwayGoals);

            // 3. Resolver cada apuesta Pendiente. Segundo cinturón de la idempotencia:
            //    la consulta solo trae Pending, así que las ya resueltas nunca llegan aquí.
            var pendingBets = await _betRepository.GetPendingByMatchAsync(match.Id);
            var summary = new ResolutionSummaryDto { ResolvedBets = pendingBets.Count };

            foreach (var bet in pendingBets)
            {
                if (bet.Pick == winningPick)
                {
                    // Se paga con la cuota CONGELADA al apostar, nunca con la cuota actual del partido.
                    var payout = bet.Amount * bet.OddsAtPlacement;

                    bet.Status = BetStatus.Won;
                    bet.User.Balance += payout;

                    // Mismo cinturón de concurrencia del Módulo 8: si otro cierre simultáneo
                    // ya pagó a este usuario, este SaveChanges falla y se revierte entero.
                    bet.User.ConcurrencyStamp = Guid.NewGuid().ToString();

                    summary.WonBets++;
                    summary.TotalPaid += payout;
                }
                else
                {
                    // Lo apostado ya se descontó al apostar: perder es simplemente no cobrar.
                    bet.Status = BetStatus.Lost;
                }
            }

            try
            {
                // 4. Un solo SaveChanges = una sola transacción: el partido, todas las apuestas
                //    y todos los saldos se guardan juntos, o no se guarda nada.
                await _matchRepository.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // La transacción se revirtió completa: nada quedó a medias.
                throw new BusinessRuleException(
                    "Otra operación modificó este partido o el saldo de un apostador al mismo tiempo. " +
                    "No se guardó nada; revisa el estado del partido e intenta cerrarlo de nuevo.");
            }

            return summary;
        }

        // ¿Ganó el local? ¿ganó el visitante? Si no, fue empate.
        private static BetPick GetWinningPick(int homeGoals, int awayGoals) =>
            homeGoals > awayGoals ? BetPick.Home
            : homeGoals < awayGoals ? BetPick.Away
            : BetPick.Draw;

        private async Task<Match> GetActiveMatchAsync(int id)
        {
            var match = await _matchRepository.GetByIdAsync(id);
            if (match is null || !match.IsActive)
                throw new KeyNotFoundException($"No existe el partido {id}.");

            return match;
        }

        private async Task ValidateTeamsAsync(MatchFormDto dto)
        {
            if (dto.HomeTeamId == dto.AwayTeamId)
                throw new BusinessRuleException("El equipo local y el visitante no pueden ser el mismo.");

            var home = await _teamRepository.GetByIdAsync(dto.HomeTeamId);
            var away = await _teamRepository.GetByIdAsync(dto.AwayTeamId);

            if (home is not { IsActive: true } || away is not { IsActive: true })
                throw new BusinessRuleException("Ambos equipos deben existir y estar activos.");
        }

        // Match.Date es hora local (ver AppDbContext), por eso se compara contra DateTime.Now.
        private static void EnsureFutureDate(DateTime date)
        {
            if (date <= DateTime.Now)
                throw new BusinessRuleException("La fecha del partido debe ser futura.");
        }
    }
}
