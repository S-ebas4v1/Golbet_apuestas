# GolBet

Aplicación GolBet — Curso de Desarrollo de Software, .NET 8, arquitectura N-capas
(`GolBet.Entities`, `GolBet.Repositories`, `GolBet.Services`, `GolBet.Web`).

## Base de datos: PostgreSQL

Este proyecto usa **PostgreSQL + Npgsql** (herramienta de administración recomendada:
DBeaver).

### Puesta en marcha desde cero

1. Tener PostgreSQL 15+ corriendo localmente.
2. Editar `db/01_bootstrap.sql` y reemplazar `CAMBIA_ESTA_CLAVE` por una contraseña real.
3. Ejecutarlo conectado como superusuario `postgres` (en DBeaver, en modo autocommit,
   sentencia por sentencia). Crea el rol `golbet_app` y la base `golbet`.
4. Crear `GolBet.Web/appsettings.Development.json` (no está en el repo, cada quien pone
   la suya) con la misma contraseña:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=golbet;Username=golbet_app;Password=TU_CLAVE"
     }
   }
   ```
5. Ejecutar la aplicación (`dotnet run` desde `GolBet.Web`, o F5). Al arrancar aplica las
   migraciones pendientes y siembra 8 equipos y 6 partidos si la base está vacía (no
   duplica en arranques posteriores).

### Reiniciar desde cero

Borra la base `golbet` (en DBeaver o con `DROP DATABASE`), vuelve a correr
`db/01_bootstrap.sql` y arranca la aplicación de nuevo.

## Compilar

```
dotnet build GolBet.sln
```
