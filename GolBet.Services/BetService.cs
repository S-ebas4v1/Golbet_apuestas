using AutoMapper;
using GolBet.Entities;
using GolBet.Entities.Enums;
using GolBet.Repositories;
using GolBet.Services.DTOs;
using GolBet.Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace GolBet.Services
{
    public class BetService : IBetService
    {
        private readonly IBetRepository _betRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly IMapper _mapper;

        public BetService(IBetRepository betRepository, IMatchRepository matchRepository, IMapper mapper)
        {
            _betRepository = betRepository;
            _matchRepository = matchRepository;
            _mapper = mapper;
        }

        public async Task PlaceBetAsync(string userId, BetFormDto dto)
        {
            if (!Enum.IsDefined(dto.Pick))
                throw new BusinessRuleException("Selección inválida.");

            // Regla 1: el partido existe, está activo, programado y aún no empieza.
            // Match.Date es hora local (ver AppDbContext), por eso se compara contra DateTime.Now.
            var match = await _matchRepository.GetByIdAsync(dto.MatchId);
            if (match is null || !match.IsActive)
                throw new BusinessRuleException("El partido no existe.");

            if (match.Status != MatchStatus.Scheduled || match.Date <= DateTime.Now)
                throw new BusinessRuleException("Este partido ya no acepta apuestas.");

            // Regla 2: la misma selección solo una vez por partido.
            if (await _betRepository.ExistsAsync(dto.MatchId, userId, dto.Pick))
                throw new BusinessRuleException("Ya tienes una apuesta con esta selección en este partido.");

            // Regla 3: el monto no puede superar el saldo.
            var user = await _betRepository.GetUserForUpdateAsync(userId)
                ?? throw new BusinessRuleException("Usuario no encontrado.");

            if (dto.Amount > user.Balance)
                throw new BusinessRuleException($"Saldo insuficiente: tienes {user.Balance:N0} FutCoins.");

            // Regla 4: la cuota se congela al apostar; si el admin la cambia después, esta apuesta no cambia.
            var oddsAtPlacement = dto.Pick switch
            {
                BetPick.Home => match.HomeOdds,
                BetPick.Draw => match.DrawOdds,
                _ => match.AwayOdds
            };

            user.Balance -= dto.Amount;

            // Identity usa ConcurrencyStamp como token de concurrencia: al renovarlo, si dos apuestas
            // del mismo usuario llegan a la vez, la segunda falla en vez de gastar saldo que ya no existe.
            user.ConcurrencyStamp = Guid.NewGuid().ToString();

            await _betRepository.AddAsync(new Bet
            {
                MatchId = dto.MatchId,
                UserId = userId,
                Pick = dto.Pick,
                Amount = dto.Amount,
                OddsAtPlacement = oddsAtPlacement,
                Status = BetStatus.Pending
            });

            try
            {
                // Un solo SaveChanges = una sola transacción: descuento del saldo + apuesta nueva.
                await _betRepository.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new BusinessRuleException("Tu saldo cambió mientras apostabas. Intenta de nuevo.");
            }
            catch (DbUpdateException)
            {
                // El índice único (partido, usuario, selección) frena un doble envío simultáneo.
                throw new BusinessRuleException("Ya tienes una apuesta con esta selección en este partido.");
            }
        }

        public async Task<IEnumerable<BetDto>> GetMyBetsAsync(string userId)
        {
            var bets = await _betRepository.GetByUserAsync(userId);
            return _mapper.Map<IEnumerable<BetDto>>(bets);
        }
    }
}
