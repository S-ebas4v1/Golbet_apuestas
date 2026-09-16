/* =====================================================================
   GolBet - Rol de aplicación y base de datos
   ---------------------------------------------------------------------
   Ejecutar conectado a PostgreSQL como el superusuario 'postgres',
   contra la base 'postgres' (CREATE DATABASE no puede correr dentro de
   una transacción, así que en DBeaver hay que estar en autocommit).

   ANTES DE EJECUTAR: reemplaza CAMBIA_ESTA_CLAVE por una contraseña real,
   y usa esa misma contraseña en GolBet.Web/appsettings.Development.json
   (ese archivo no se sube al repo).
   ===================================================================== */

-- Re-ejecutable: si el rol ya existe, solo le resetea la contraseña.
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'golbet_app') THEN
        ALTER ROLE golbet_app WITH LOGIN PASSWORD 'CAMBIA_ESTA_CLAVE';
    ELSE
        CREATE ROLE golbet_app WITH LOGIN PASSWORD 'CAMBIA_ESTA_CLAVE';
    END IF;
END
$$;

-- Falla con "already exists" si ya se creó antes; es inofensivo.
CREATE DATABASE golbet
    OWNER    golbet_app
    ENCODING 'UTF8';

-- Verificación: debe devolver una fila (golbet | golbet_app).
SELECT d.datname AS base, r.rolname AS dueno
FROM   pg_database d
JOIN   pg_roles    r ON r.oid = d.datdba
WHERE  d.datname = 'golbet';
