-- Solo base aislada. Los registros sintéticos se revierten al finalizar.
SET XACT_ABORT ON;
IF DB_NAME() NOT IN ('gestion_v2_pruebas','gestion_v2_validacion') THROW 51100, N'Use exclusivamente una base aislada de pruebas V2.', 1;
BEGIN TRANSACTION;
INSERT INTO docente (cedula,nombres,apellidos,genero,cargo,fecha_nacimiento,correo,telefono,url_cvlac,fecha_actualizacion,escalafon,perfil,conv_minciencia,nacionalidaad)
VALUES (2000000001,'Prueba','Trigger','Otro','Docente','19900101','trigger@example.edu','1','https://example.edu','20260101','Prueba','Prueba','Prueba','Colombia'),
       (2000000002,'Prueba','Traslado','Otro','Docente','19900101','traslado@example.edu','1','https://example.edu','20260101','Prueba','Prueba','Prueba','Colombia');
INSERT INTO estudios_realizados (id,titulo,universidad,fecha,tipo,ciudad,docente,ins_acreditada,metodologia,perfil_egresado,pais)
VALUES (2000000001,'Prueba A','Universidad','20260101','Curso','Medellín',2000000001,1,'Presencial','Perfil','Colombia'),
       (2000000002,'Prueba B','Universidad','20260101','Curso','Medellín',2000000001,1,'Presencial','Perfil','Colombia');
IF (SELECT total_estudios FROM docente WHERE cedula=2000000001) <> 2 THROW 51101, N'Falló INSERT multirregistro.', 1;
UPDATE estudios_realizados SET docente=2000000002 WHERE id IN (2000000001,2000000002);
IF (SELECT total_estudios FROM docente WHERE cedula=2000000001) <> 0 THROW 51102, N'Falló maestro anterior.', 1;
IF (SELECT total_estudios FROM docente WHERE cedula=2000000002) <> 2 THROW 51103, N'Falló maestro nuevo.', 1;
UPDATE estudios_realizados SET activo=0 WHERE id=2000000001;
IF (SELECT total_estudios FROM docente WHERE cedula=2000000002) <> 1 THROW 51104, N'Falló retiro lógico.', 1;
UPDATE estudios_realizados SET activo=1 WHERE id=2000000001;
IF (SELECT total_estudios FROM docente WHERE cedula=2000000002) <> 2 THROW 51105, N'Falló reactivación.', 1;
-- El DELETE físico solo se usa aquí para probar ese evento del trigger.
DELETE FROM estudios_realizados WHERE id IN (2000000001,2000000002);
IF (SELECT total_estudios FROM docente WHERE cedula=2000000002) <> 0 THROW 51106, N'Falló DELETE multirregistro.', 1;
ROLLBACK TRANSACTION;
PRINT N'OK: INSERT múltiple, traslado, retiro lógico, reactivación y DELETE múltiple. Todo revertido.';
