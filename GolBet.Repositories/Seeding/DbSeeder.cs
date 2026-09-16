using GolBet.Entities;
using GolBet.Entities.Enums;
using GolBet.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace GolBet.Repositories.Seeding
{
    public static class DbSeeder
    {
        // Idempotente: si ya hay equipos, se asume que la BD ya fue sembrada.
        // Así correr la app varias veces nunca duplica datos.
        public static async Task SeedAsync(AppDbContext context)
        {
            await context.Database.MigrateAsync();

            if (await context.Teams.AnyAsync())
                return;

            var teams = new List<Team>
            {
                new() { Name = "Millonarios FC",         City = "Bogotá" },
                new() { Name = "Atlético Nacional",      City = "Medellín" },
                new() { Name = "América de Cali",        City = "Cali" },
                new() { Name = "Junior FC",              City = "Barranquilla" },
                new() { Name = "Independiente Santa Fe", City = "Bogotá" },
                new() { Name = "Deportivo Cali",         City = "Cali" },
                new() { Name = "Once Caldas",            City = "Manizales" },
                new() { Name = "Deportes Tolima",        City = "Ibagué" },
            };

            await context.Teams.AddRangeAsync(teams);
            await context.SaveChangesAsync();

            var today = DateTime.Today;

            var matches = new List<Match>
            {
                new() { HomeTeam = teams[0], AwayTeam = teams[1], Date = today.AddDays(2).AddHours(20), Status = MatchStatus.Scheduled, HomeOdds = 2.10m, DrawOdds = 3.05m, AwayOdds = 3.20m },
                new() { HomeTeam = teams[2], AwayTeam = teams[3], Date = today.AddDays(3).AddHours(18), Status = MatchStatus.Scheduled, HomeOdds = 1.95m, DrawOdds = 3.10m, AwayOdds = 3.60m },
                new() { HomeTeam = teams[4], AwayTeam = teams[5], Date = today.AddHours(15), Status = MatchStatus.InProgress, HomeGoals = 1, AwayGoals = 1, HomeOdds = 2.40m, DrawOdds = 2.90m, AwayOdds = 2.75m },
                new() { HomeTeam = teams[6], AwayTeam = teams[7], Date = today.AddDays(-1).AddHours(20), Status = MatchStatus.Finished, HomeGoals = 2, AwayGoals = 0, HomeOdds = 2.05m, DrawOdds = 3.15m, AwayOdds = 3.50m },
                new() { HomeTeam = teams[1], AwayTeam = teams[3], Date = today.AddDays(-3).AddHours(19), Status = MatchStatus.Finished, HomeGoals = 1, AwayGoals = 1, HomeOdds = 1.85m, DrawOdds = 3.20m, AwayOdds = 4.10m },
                new() { HomeTeam = teams[0], AwayTeam = teams[5], Date = today.AddDays(-7).AddHours(16), Status = MatchStatus.Finished, HomeGoals = 3, AwayGoals = 1, HomeOdds = 1.70m, DrawOdds = 3.40m, AwayOdds = 4.80m },
            };

            await context.Matches.AddRangeAsync(matches);
            await context.SaveChangesAsync();
        }
    }
}
