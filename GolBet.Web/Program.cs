using GolBet.Entities;
using GolBet.Repositories;
using GolBet.Repositories.Data;
using GolBet.Repositories.Seeding;
using GolBet.Services;
using GolBet.Services.Mapping;
using GolBet.Web.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

// Fechas y textos en español de Colombia, pero con punto decimal: los <input type="number">
// envían "2.50" y con la coma de es-CO el model binder leería 250 en las cuotas.
var culture = (CultureInfo)CultureInfo.GetCultureInfo("es-CO").Clone();
culture.NumberFormat.NumberDecimalSeparator = ".";
culture.NumberFormat.NumberGroupSeparator = ",";
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// PostgreSQL con nombres de columnas/tablas en snake_case, para que las
// consultas escritas a mano en DBeaver no tengan que citar identificadores.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseSnakeCaseNamingConvention());

// Identity guarda usuarios y roles en el mismo AppDbContext y autentica con cookie.
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
    {
        // Política relajada para el entorno académico; en producción sería más estricta.
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddErrorDescriber<SpanishIdentityErrorDescriber>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";              // anónimo que pide algo protegido
    options.AccessDeniedPath = "/Account/AccessDenied"; // autenticado pero sin el rol
});

// Registro genérico abierto: cualquier IGenericRepository<T> se resuelve con GenericRepository<T>.
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<ITeamService, TeamService>();

var app = builder.Build();

// Aplica migraciones pendientes y siembra datos, roles y usuarios demo si faltan.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DbSeeder.SeedAsync(
        services.GetRequiredService<AppDbContext>(),
        services.GetRequiredService<RoleManager<IdentityRole>>(),
        services.GetRequiredService<UserManager<AppUser>>());
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// El orden importa: primero se identifica al usuario (cookie) y luego se evalúa [Authorize].
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
