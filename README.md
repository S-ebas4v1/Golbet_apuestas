# GolBet

Aplicación GolBet — Curso de Desarrollo de Software, .NET 8, arquitectura N-capas
(`GolBet.Entities`, `GolBet.Repositories`, `GolBet.Services`, `GolBet.Web`).

# Nota Aclaratoria sobre la Migración del Repositorio

# Nota para la evaluación:
Este proyecto fue desarrollado originalmente en mi cuenta de GitHub inscrita en mi empresa desde el inicio del curso. Debido a problemas de permisos y acceso técnico con los repositorios en dicha cuenta, realicé la migración del repositorio a mi cuenta personal para garantizar su visualización pública y libre acceso para la entrega.

# Detalles del procedimiento técnico de migración:

1. Para preservar la integridad del historial progresivo de commits, la estructura de ramas y las etiquetas (tags de los módulos 1 al 5), la migración se realizó mediante una clonación completa con el historial original intacto.

2. Se utilizó un Personal Access Token (Classic) para vincular de manera segura los permisos de lectura/escritura entre ambas cuentas.

3. El proceso se apoyó en la herramienta Claude Code como asistente CLI para auditar los comandos Git, asegurando que no se perdiera la cronología del proyecto ni se sobrescribieran commits previos.

## Nota Aclaratoria sobre la Migración del Repositorio

## Nota para la evaluación:

1. Este proyecto fue desarrollado originalmente en mi cuenta de GitHub inscrita en mi empresa desde el inicio del curso. Debido a problemas de permisos y acceso técnico con los repositorios en dicha cuenta, realicé la migración del repositorio a mi cuenta personal para garantizar su visualización pública y libre acceso para la entrega.

## Detalles del procedimiento técnico de migración:

1. Para preservar la integridad del historial progresivo de commits, la estructura de ramas y las etiquetas (tags de los módulos 1 al 5), la migración se realizó mediante una clonación completa con el historial original intacto.

2. Se utilizó un Personal Access Token (Classic) para vincular de manera segura los permisos de lectura/escritura entre ambas cuentas.

3. El proceso se apoyó en la herramienta Claude Code como asistente CLI para auditar los comandos Git, asegurando que no se perdiera la cronología del proyecto ni se sobrescribieran commits previos.
## Avance por módulo

| Módulo | Tag | Contenido |
|---|---|---|
| 1 | `modulo-1` | Esqueleto N-capas y marca GolBet |
| 2 | `modulo-2` | Entidades de dominio, `AppDbContext` Code First y migración inicial (PostgreSQL) |
| 3 | `modulo-3` | Repositorio genérico, `MatchRepository` y seeder idempotente |
| 4 | `modulo-4` | Servicios, DTOs, AutoMapper y cartelera |
| 5 | `modulo-5` | Filtros por estado, detalle de partido y 404 |
| 6 | `modulo-6` | CRUD de equipos y partidos: validación, PRG, baja lógica y reglas de negocio |
| 7 | `modulo-7` | ASP.NET Core Identity: registro, login/logout, roles Admin/Apostador y autorización |
| Taller | — | Buscador de partidos por equipo en la cartelera, combinable con el filtro de estado |

### Cuentas de demostración (Módulo 7)

El seeder las crea al arrancar la app si no existen:

| Rol | Correo | Contraseña |
|---|---|---|
| Admin | `admin@golbet.com` | `Admin123*` |
| Apostador | `apostador@golbet.com` | `Apostador123*` |

Todo usuario que se registra queda con rol **Apostador** y recibe 100,000 FutCoins de bienvenida.
Solo el **Admin** puede crear, editar o desactivar equipos y partidos.
