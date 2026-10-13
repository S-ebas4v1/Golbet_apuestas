using GolBet.Entities;
using GolBet.Entities.Enums;
using GolBet.Repositories.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GolBet.Repositories.Seeding
{
    public static class DbSeeder
    {
        // Cuentas de demostración para revisar la app sin registrarse (ver README).
        public const string AdminEmail = "admin@golbet.com";
        public const string AdminPassword = "Admin123*";
        public const string BettorEmail = "apostador@golbet.com";
        public const string BettorPassword = "Apostador123*";

        public const decimal WelcomeBalance = 100_000m;

        // Idempotente: cada bloque revisa si sus datos ya existen antes de insertarlos.
        // Así correr la app varias veces nunca duplica datos.
        public static async Task SeedAsync(
            AppDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<AppUser> userManager)
        {
            await context.Database.MigrateAsync();

            await SeedDomainAsync(context);
            await SeedIdentityAsync(roleManager, userManager);
        }

        private static async Task SeedDomainAsync(AppDbContext context)
        {
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

        private static async Task SeedIdentityAsync(
            RoleManager<IdentityRole> roleManager,
            UserManager<AppUser> userManager)
        {
            foreach (var role in new[] { Roles.Admin, Roles.Bettor })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // El administrador gestiona la cartelera, no apuesta: arranca sin saldo.
            await EnsureUserAsync(userManager, AdminEmail, AdminPassword, "Administrador GolBet", 0m, Roles.Admin);
            await EnsureUserAsync(userManager, BettorEmail, BettorPassword, "Apostador Demo", WelcomeBalance, Roles.Bettor);
        }

        private static async Task EnsureUserAsync(
            UserManager<AppUser> userManager,
            string email, string password, string fullName, decimal balance, string role)
        {
            if (await userManager.FindByEmailAsync(email) is not null)
                return;

            var user = new AppUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = fullName,
                Balance = balance
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new InvalidOperationException(
                    $"No se pudo crear el usuario {email}: " +
                    string.Join("; ", result.Errors.Select(e => e.Description)));

            await userManager.AddToRoleAsync(user, role);
        }
    }
}
