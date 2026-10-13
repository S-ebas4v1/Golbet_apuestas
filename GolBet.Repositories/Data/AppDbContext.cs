using GolBet.Entities;
using GolBet.Entities.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GolBet.Repositories.Data
{
    // IdentityDbContext agrega las tablas de usuarios, roles, claims y logins al mismo
    // modelo, así una apuesta puede tener FK real hacia su usuario.
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Team> Teams => Set<Team>();
        public DbSet<Match> Matches => Set<Match>();
        public DbSet<Bet> Bets => Set<Bet>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Identity fija sus tablas en PascalCase ("AspNetUsers") con ToTable explícito, y eso
            // le gana a UseSnakeCaseNamingConvention. Se renombran para que todo quede en snake_case.
            modelBuilder.Entity<AppUser>().ToTable("asp_net_users");
            modelBuilder.Entity<IdentityRole>().ToTable("asp_net_roles");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("asp_net_user_roles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("asp_net_user_claims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("asp_net_user_logins");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("asp_net_user_tokens");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("asp_net_role_claims");

            // El nombre del equipo debe ser único sin importar mayúsculas/acentos.
            // PostgreSQL no trae una collation "CI_AI" como SQL Server; el equivalente
            // es una collation ICU no determinística ("ks-level1" compara solo por
            // fuerza primaria, ignorando caso y acentos).
            modelBuilder.HasCollation(
                name: "golbet_ci_ai",
                locale: "und-u-ks-level1",
                provider: "icu",
                deterministic: false);

            modelBuilder.Entity<Team>()
                .Property(t => t.Name)
                .UseCollation("golbet_ci_ai");

            modelBuilder.Entity<Team>()
                .HasIndex(t => t.Name)
                .IsUnique();

            // Match.Date es la hora local del partido (no un instante UTC). Npgsql exige
            // Kind=Utc para "timestamp with time zone" (el tipo por defecto), así que se
            // mapea a "timestamp without time zone" para poder guardar la hora tal cual.
            modelBuilder.Entity<Match>()
                .Property(m => m.Date)
                .HasColumnType("timestamp without time zone");

            // El convention discovery de EF no puede resolver dos FKs de Match hacia Team,
            // así que cada relación se declara explícitamente. Restrict evita que borrar un
            // equipo elimine en cascada los partidos donde participó.
            modelBuilder.Entity<Match>()
                .HasOne(m => m.HomeTeam)
                .WithMany()
                .HasForeignKey(m => m.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.AwayTeam)
                .WithMany()
                .HasForeignKey(m => m.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            // Un partido con apuestas registradas no se puede borrar.
            modelBuilder.Entity<Bet>()
                .HasOne(b => b.Match)
                .WithMany(m => m.Bets)
                .OnDelete(DeleteBehavior.Restrict);

            // Tampoco se puede borrar un usuario que tenga apuestas: son su historial.
            modelBuilder.Entity<Bet>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bets)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Un usuario no puede repetir el mismo pronóstico (1, X o 2) en el mismo partido.
            modelBuilder.Entity<Bet>()
                .HasIndex(b => new { b.MatchId, b.UserId, b.Pick })
                .IsUnique();

            modelBuilder.Entity<AppUser>()
                .Property(u => u.Balance)
                .HasPrecision(12, 2);
        }

        // Estampa CreatedDate/ModifiedDate automáticamente para toda entidad auditable,
        // así ningún servicio tiene que acordarse de hacerlo a mano.
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var utcNow = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = utcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.ModifiedDate = utcNow;
                    entry.Property(e => e.CreatedDate).IsModified = false;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
