-- ============================================================
-- REPARAR DATOS DEMO - GESTIÓN PROFESORAL
-- ============================================================
-- Elimina únicamente los registros ficticios creados por datos_demo.sql.
-- Después de ejecutar este archivo, vuelva a ejecutar datos_demo.sql
-- usando UTF-8 desde PowerShell.
-- ============================================================

USE gestion_local;
GO
SET NOCOUNT ON;
GO

DELETE FROM beca WHERE estudios BETWEEN 9201 AND 9210;
DELETE FROM apoyo_profesoral WHERE estudios BETWEEN 9201 AND 9210;
DELETE FROM estudio_ac WHERE estudio BETWEEN 9201 AND 9210;
DELETE FROM red_docente WHERE docente BETWEEN 91000001 AND 91000010;
DELETE FROM experiecia WHERE docente BETWEEN 91000001 AND 91000010;
DELETE FROM reconocimiento WHERE docente BETWEEN 91000001 AND 91000010;
DELETE FROM evaluacion_docente WHERE docente BETWEEN 91000001 AND 91000010;
DELETE FROM intereses_futuros WHERE docente BETWEEN 91000001 AND 91000010;
DELETE FROM docente_departamento WHERE docente BETWEEN 91000001 AND 91000010;
DELETE FROM estudios_realizados WHERE id BETWEEN 9201 AND 9210;
DELETE FROM docente WHERE cedula BETWEEN 91000001 AND 91000010;

DELETE FROM red WHERE idr BETWEEN 9101 AND 9103;
DELETE FROM programa WHERE id BETWEEN 9101 AND 9104;
DELETE FROM linea_investigacion WHERE id BETWEEN 9101 AND 9104;

DELETE FROM termino_clave
WHERE termino IN (
    'Inteligencia artificial',
    'Software',
    'Analítica de datos',
    'Automatización',
    'Educación digital'
);
GO

PRINT 'Datos demo anteriores eliminados.';
PRINT 'Ahora ejecute datos_demo.sql usando Get-Content -Encoding UTF8.';
GO
