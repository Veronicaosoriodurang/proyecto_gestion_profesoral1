-- ============================================================
-- DATOS DEMO - GESTIÓN PROFESORAL
-- ============================================================
-- Este archivo contiene información 100% FICTICIA para demostración.
-- No modifica el esquema ni reemplaza los datos de referencia del curso.
-- Puede ejecutarse después de levantar la base gestion_local.
--
-- Objetivo: permitir que la interfaz tenga suficiente información para
-- mostrar docentes, formación, trayectoria, redes, apoyos y becas.
-- ============================================================

USE gestion_local;
GO

SET NOCOUNT ON;
GO

-- ============================================================
-- 1. LÍNEAS DE INVESTIGACIÓN
-- ============================================================
SET IDENTITY_INSERT linea_investigacion ON;

IF NOT EXISTS (SELECT 1 FROM linea_investigacion WHERE id = 9101)
INSERT INTO linea_investigacion (id, nombre, descripcion, activo)
VALUES (9101, 'Ingeniería de Software', 'Desarrollo, calidad y arquitectura de sistemas de software.', 1);

IF NOT EXISTS (SELECT 1 FROM linea_investigacion WHERE id = 9102)
INSERT INTO linea_investigacion (id, nombre, descripcion, activo)
VALUES (9102, 'Ciencia de Datos', 'Analítica, visualización y procesamiento de información.', 1);

IF NOT EXISTS (SELECT 1 FROM linea_investigacion WHERE id = 9103)
INSERT INTO linea_investigacion (id, nombre, descripcion, activo)
VALUES (9103, 'Automatización', 'Automatización de procesos y sistemas inteligentes.', 1);

IF NOT EXISTS (SELECT 1 FROM linea_investigacion WHERE id = 9104)
INSERT INTO linea_investigacion (id, nombre, descripcion, activo)
VALUES (9104, 'Educación y Tecnología', 'Uso de herramientas digitales en procesos educativos.', 1);

SET IDENTITY_INSERT linea_investigacion OFF;
GO

-- ============================================================
-- 2. PROGRAMAS
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM programa WHERE id = 9101)
INSERT INTO programa (id,nombre,tipo,nivel,fecha_creacion,fecha_cierre,numero_cohortes,cant_graduados,fecha_actualizacion,ciudad,facultad,activo)
VALUES (9101,'Tecnología en Desarrollo de Software','Académico','Tecnológico','2015-01-20',NULL,'18','640','2026-08-01','Medellín',1,1);

IF NOT EXISTS (SELECT 1 FROM programa WHERE id = 9102)
INSERT INTO programa (id,nombre,tipo,nivel,fecha_creacion,fecha_cierre,numero_cohortes,cant_graduados,fecha_actualizacion,ciudad,facultad,activo)
VALUES (9102,'Ingeniería de Sistemas','Académico','Profesional','2010-02-01',NULL,'28','1100','2026-08-01','Medellín',1,1);

IF NOT EXISTS (SELECT 1 FROM programa WHERE id = 9103)
INSERT INTO programa (id,nombre,tipo,nivel,fecha_creacion,fecha_cierre,numero_cohortes,cant_graduados,fecha_actualizacion,ciudad,facultad,activo)
VALUES (9103,'Ingeniería Mecatrónica','Académico','Profesional','2012-07-15',NULL,'22','760','2026-08-01','Medellín',1,1);

IF NOT EXISTS (SELECT 1 FROM programa WHERE id = 9104)
INSERT INTO programa (id,nombre,tipo,nivel,fecha_creacion,fecha_cierre,numero_cohortes,cant_graduados,fecha_actualizacion,ciudad,facultad,activo)
VALUES (9104,'Maestría en Gestión de la Innovación','Posgrado','Maestría','2018-01-10',NULL,'10','180','2026-08-01','Medellín',2,1);
GO

-- ============================================================
-- 3. REDES ACADÉMICAS
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM red WHERE idr = 9101)
INSERT INTO red (idr,nombre,url,pais,activo)
VALUES (9101,'Red Colombiana de Software','https://demo.local/red-software','Colombia',1);

IF NOT EXISTS (SELECT 1 FROM red WHERE idr = 9102)
INSERT INTO red (idr,nombre,url,pais,activo)
VALUES (9102,'Red Académica de Datos','https://demo.local/red-datos','Colombia',1);

IF NOT EXISTS (SELECT 1 FROM red WHERE idr = 9103)
INSERT INTO red (idr,nombre,url,pais,activo)
VALUES (9103,'Comunidad de Innovación Educativa','https://demo.local/red-educacion','Colombia',1);
GO

-- ============================================================
-- 4. TÉRMINOS CLAVE
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM termino_clave WHERE termino = 'Inteligencia artificial')
INSERT INTO termino_clave (termino,termino_ingles,activo) VALUES ('Inteligencia artificial','Artificial intelligence',1);

IF NOT EXISTS (SELECT 1 FROM termino_clave WHERE termino = 'Software')
INSERT INTO termino_clave (termino,termino_ingles,activo) VALUES ('Software','Software',1);

IF NOT EXISTS (SELECT 1 FROM termino_clave WHERE termino = 'Analítica de datos')
INSERT INTO termino_clave (termino,termino_ingles,activo) VALUES ('Analítica de datos','Data analytics',1);

IF NOT EXISTS (SELECT 1 FROM termino_clave WHERE termino = 'Automatización')
INSERT INTO termino_clave (termino,termino_ingles,activo) VALUES ('Automatización','Automation',1);

IF NOT EXISTS (SELECT 1 FROM termino_clave WHERE termino = 'Educación digital')
INSERT INTO termino_clave (termino,termino_ingles,activo) VALUES ('Educación digital','Digital education',1);
GO

-- ============================================================
-- 5. DOCENTES FICTICIOS
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM docente WHERE cedula = 91000001)
INSERT INTO docente VALUES (91000001,'Laura','Gómez Restrepo','Femenino','Docente','1988-04-14','laura.gomez@demo.edu.co','3000000001','https://demo.local/cvlac/laura','2026-09-01','Asistente','Docente de desarrollo de software y arquitectura de aplicaciones.','Junior','2025','Colombiana',9101,1);

IF NOT EXISTS (SELECT 1 FROM docente WHERE cedula = 91000002)
INSERT INTO docente VALUES (91000002,'Mateo','Ramírez Vélez','Masculino','Docente','1985-11-03','mateo.ramirez@demo.edu.co','3000000002','https://demo.local/cvlac/mateo','2026-09-01','Asociado','Investigador en ciencia de datos y analítica aplicada.','Asociado','2025','Colombiana',9102,1);

IF NOT EXISTS (SELECT 1 FROM docente WHERE cedula = 91000003)
INSERT INTO docente VALUES (91000003,'Sofía','Martínez Londoño','Femenino','Docente','1991-06-21','sofia.martinez@demo.edu.co','3000000003','https://demo.local/cvlac/sofia','2026-09-01','Auxiliar','Docente de programación, bases de datos y servicios web.','Junior','2025','Colombiana',9101,1);

IF NOT EXISTS (SELECT 1 FROM docente WHERE cedula = 91000004)
INSERT INTO docente VALUES (91000004,'Daniel','Castaño Ruiz','Masculino','Docente','1983-02-09','daniel.castano@demo.edu.co','3000000004','https://demo.local/cvlac/daniel','2026-09-01','Titular','Profesor de automatización, robótica y sistemas de control.','Senior','2025','Colombiana',9103,1);

IF NOT EXISTS (SELECT 1 FROM docente WHERE cedula = 91000005)
INSERT INTO docente VALUES (91000005,'Valentina','Torres Mejía','Femenino','Docente','1990-09-18','valentina.torres@demo.edu.co','3000000005','https://demo.local/cvlac/valentina','2026-09-01','Asistente','Docente interesada en innovación educativa y aprendizaje digital.','Junior','2025','Colombiana',9104,1);

IF NOT EXISTS (SELECT 1 FROM docente WHERE cedula = 91000006)
INSERT INTO docente VALUES (91000006,'Samuel','Restrepo Arias','Masculino','Docente','1987-12-07','samuel.restrepo@demo.edu.co','3000000006','https://demo.local/cvlac/samuel','2026-09-01','Asociado','Desarrollador e investigador en ingeniería de software.','Asociado','2025','Colombiana',9101,1);

IF NOT EXISTS (SELECT 1 FROM docente WHERE cedula = 91000007)
INSERT INTO docente VALUES (91000007,'Mariana','Ospina Álvarez','Femenino','Docente','1992-01-29','mariana.ospina@demo.edu.co','3000000007','https://demo.local/cvlac/mariana','2026-09-01','Auxiliar','Docente de estadística, visualización y analítica de información.','Junior','2025','Colombiana',9102,1);

IF NOT EXISTS (SELECT 1 FROM docente WHERE cedula = 91000008)
INSERT INTO docente VALUES (91000008,'Nicolás','Herrera Cano','Masculino','Docente','1989-07-12','nicolas.herrera@demo.edu.co','3000000008','https://demo.local/cvlac/nicolas','2026-09-01','Asistente','Profesor de sistemas inteligentes y automatización.','Junior','2025','Colombiana',9103,1);

IF NOT EXISTS (SELECT 1 FROM docente WHERE cedula = 91000009)
INSERT INTO docente VALUES (91000009,'Isabella','Ríos Cardona','Femenino','Docente','1986-05-30','isabella.rios@demo.edu.co','3000000009','https://demo.local/cvlac/isabella','2026-09-01','Asociado','Investigadora en educación, innovación y transformación digital.','Asociado','2025','Colombiana',9104,1);

IF NOT EXISTS (SELECT 1 FROM docente WHERE cedula = 91000010)
INSERT INTO docente VALUES (91000010,'Sebastián','Quintero Gil','Masculino','Docente','1984-10-16','sebastian.quintero@demo.edu.co','3000000010','https://demo.local/cvlac/sebastian','2026-09-01','Titular','Profesor de arquitectura de software y servicios distribuidos.','Senior','2025','Colombiana',9101,1);
GO

-- ============================================================
-- 6. ESTUDIOS REALIZADOS
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM estudios_realizados WHERE id = 9201)
INSERT INTO estudios_realizados VALUES (9201,'Maestría en Ingeniería de Software','Universidad Demo','2018-12-10','Maestría','Medellín',91000001,1,'Presencial','Arquitectura y calidad de software.','Colombia',1);
IF NOT EXISTS (SELECT 1 FROM estudios_realizados WHERE id = 9202)
INSERT INTO estudios_realizados VALUES (9202,'Maestría en Ciencia de Datos','Universidad Demo','2019-06-20','Maestría','Bogotá',91000002,1,'Presencial','Analítica avanzada y modelos de datos.','Colombia',1);
IF NOT EXISTS (SELECT 1 FROM estudios_realizados WHERE id = 9203)
INSERT INTO estudios_realizados VALUES (9203,'Especialización en Desarrollo Web','Institución Demo','2020-11-14','Especialización','Medellín',91000003,1,'Virtual','Desarrollo de aplicaciones y servicios web.','Colombia',1);
IF NOT EXISTS (SELECT 1 FROM estudios_realizados WHERE id = 9204)
INSERT INTO estudios_realizados VALUES (9204,'Doctorado en Automatización','Universidad Demo','2017-08-25','Doctorado','Cali',91000004,1,'Presencial','Automatización y control de sistemas.','Colombia',1);
IF NOT EXISTS (SELECT 1 FROM estudios_realizados WHERE id = 9205)
INSERT INTO estudios_realizados VALUES (9205,'Maestría en Educación Digital','Universidad Demo','2021-04-16','Maestría','Medellín',91000005,1,'Virtual','Diseño de experiencias de aprendizaje digital.','Colombia',1);
IF NOT EXISTS (SELECT 1 FROM estudios_realizados WHERE id = 9206)
INSERT INTO estudios_realizados VALUES (9206,'Maestría en Ingeniería','Universidad Demo','2019-09-12','Maestría','Medellín',91000006,1,'Presencial','Ingeniería y construcción de software.','Colombia',1);
IF NOT EXISTS (SELECT 1 FROM estudios_realizados WHERE id = 9207)
INSERT INTO estudios_realizados VALUES (9207,'Maestría en Estadística Aplicada','Universidad Demo','2020-03-03','Maestría','Bogotá',91000007,1,'Presencial','Estadística y analítica de datos.','Colombia',1);
IF NOT EXISTS (SELECT 1 FROM estudios_realizados WHERE id = 9208)
INSERT INTO estudios_realizados VALUES (9208,'Maestría en Automatización','Universidad Demo','2018-07-19','Maestría','Medellín',91000008,1,'Presencial','Automatización y sistemas inteligentes.','Colombia',1);
IF NOT EXISTS (SELECT 1 FROM estudios_realizados WHERE id = 9209)
INSERT INTO estudios_realizados VALUES (9209,'Doctorado en Educación','Universidad Demo','2016-10-21','Doctorado','Manizales',91000009,1,'Presencial','Educación e innovación tecnológica.','Colombia',1);
IF NOT EXISTS (SELECT 1 FROM estudios_realizados WHERE id = 9210)
INSERT INTO estudios_realizados VALUES (9210,'Doctorado en Ingeniería de Sistemas','Universidad Demo','2015-11-05','Doctorado','Medellín',91000010,1,'Presencial','Arquitectura de sistemas distribuidos.','Colombia',1);
GO

-- ============================================================
-- 7. VINCULACIÓN DOCENTE - PROGRAMA
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM docente_departamento WHERE docente=91000001 AND departamento=9101)
INSERT INTO docente_departamento VALUES (91000001,9101,'Tiempo completo','Presencial','2020-01-15',NULL,1);
IF NOT EXISTS (SELECT 1 FROM docente_departamento WHERE docente=91000002 AND departamento=9102)
INSERT INTO docente_departamento VALUES (91000002,9102,'Tiempo completo','Presencial','2019-02-01',NULL,1);
IF NOT EXISTS (SELECT 1 FROM docente_departamento WHERE docente=91000003 AND departamento=9101)
INSERT INTO docente_departamento VALUES (91000003,9101,'Tiempo completo','Mixta','2021-07-12',NULL,1);
IF NOT EXISTS (SELECT 1 FROM docente_departamento WHERE docente=91000004 AND departamento=9103)
INSERT INTO docente_departamento VALUES (91000004,9103,'Tiempo completo','Presencial','2018-01-22',NULL,1);
IF NOT EXISTS (SELECT 1 FROM docente_departamento WHERE docente=91000005 AND departamento=9104)
INSERT INTO docente_departamento VALUES (91000005,9104,'Medio tiempo','Virtual','2022-02-03',NULL,1);
IF NOT EXISTS (SELECT 1 FROM docente_departamento WHERE docente=91000006 AND departamento=9102)
INSERT INTO docente_departamento VALUES (91000006,9102,'Tiempo completo','Presencial','2020-08-10',NULL,1);
IF NOT EXISTS (SELECT 1 FROM docente_departamento WHERE docente=91000007 AND departamento=9102)
INSERT INTO docente_departamento VALUES (91000007,9102,'Medio tiempo','Mixta','2022-01-17',NULL,1);
IF NOT EXISTS (SELECT 1 FROM docente_departamento WHERE docente=91000008 AND departamento=9103)
INSERT INTO docente_departamento VALUES (91000008,9103,'Tiempo completo','Presencial','2021-01-25',NULL,1);
IF NOT EXISTS (SELECT 1 FROM docente_departamento WHERE docente=91000009 AND departamento=9104)
INSERT INTO docente_departamento VALUES (91000009,9104,'Tiempo completo','Virtual','2019-07-08',NULL,1);
IF NOT EXISTS (SELECT 1 FROM docente_departamento WHERE docente=91000010 AND departamento=9101)
INSERT INTO docente_departamento VALUES (91000010,9101,'Tiempo completo','Presencial','2017-02-06',NULL,1);
GO

-- ============================================================
-- 8. INTERESES FUTUROS
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM intereses_futuros WHERE docente=91000001 AND termino_clave='Software') INSERT INTO intereses_futuros VALUES (91000001,'Software',1);
IF NOT EXISTS (SELECT 1 FROM intereses_futuros WHERE docente=91000002 AND termino_clave='Analítica de datos') INSERT INTO intereses_futuros VALUES (91000002,'Analítica de datos',1);
IF NOT EXISTS (SELECT 1 FROM intereses_futuros WHERE docente=91000003 AND termino_clave='Inteligencia artificial') INSERT INTO intereses_futuros VALUES (91000003,'Inteligencia artificial',1);
IF NOT EXISTS (SELECT 1 FROM intereses_futuros WHERE docente=91000004 AND termino_clave='Automatización') INSERT INTO intereses_futuros VALUES (91000004,'Automatización',1);
IF NOT EXISTS (SELECT 1 FROM intereses_futuros WHERE docente=91000005 AND termino_clave='Educación digital') INSERT INTO intereses_futuros VALUES (91000005,'Educación digital',1);
IF NOT EXISTS (SELECT 1 FROM intereses_futuros WHERE docente=91000006 AND termino_clave='Software') INSERT INTO intereses_futuros VALUES (91000006,'Software',1);
IF NOT EXISTS (SELECT 1 FROM intereses_futuros WHERE docente=91000007 AND termino_clave='Analítica de datos') INSERT INTO intereses_futuros VALUES (91000007,'Analítica de datos',1);
IF NOT EXISTS (SELECT 1 FROM intereses_futuros WHERE docente=91000008 AND termino_clave='Inteligencia artificial') INSERT INTO intereses_futuros VALUES (91000008,'Inteligencia artificial',1);
IF NOT EXISTS (SELECT 1 FROM intereses_futuros WHERE docente=91000009 AND termino_clave='Educación digital') INSERT INTO intereses_futuros VALUES (91000009,'Educación digital',1);
IF NOT EXISTS (SELECT 1 FROM intereses_futuros WHERE docente=91000010 AND termino_clave='Software') INSERT INTO intereses_futuros VALUES (91000010,'Software',1);
GO

-- ============================================================
-- 9. EVALUACIONES
-- ============================================================
SET IDENTITY_INSERT evaluacion_docente ON;
IF NOT EXISTS (SELECT 1 FROM evaluacion_docente WHERE id=9301) INSERT INTO evaluacion_docente (id,calificacion,semestre,docente,activo) VALUES (9301,4.8,'2026-1',91000001,1);
IF NOT EXISTS (SELECT 1 FROM evaluacion_docente WHERE id=9302) INSERT INTO evaluacion_docente (id,calificacion,semestre,docente,activo) VALUES (9302,4.7,'2026-1',91000002,1);
IF NOT EXISTS (SELECT 1 FROM evaluacion_docente WHERE id=9303) INSERT INTO evaluacion_docente (id,calificacion,semestre,docente,activo) VALUES (9303,4.9,'2026-1',91000003,1);
IF NOT EXISTS (SELECT 1 FROM evaluacion_docente WHERE id=9304) INSERT INTO evaluacion_docente (id,calificacion,semestre,docente,activo) VALUES (9304,4.6,'2026-1',91000004,1);
IF NOT EXISTS (SELECT 1 FROM evaluacion_docente WHERE id=9305) INSERT INTO evaluacion_docente (id,calificacion,semestre,docente,activo) VALUES (9305,4.8,'2026-1',91000005,1);
IF NOT EXISTS (SELECT 1 FROM evaluacion_docente WHERE id=9306) INSERT INTO evaluacion_docente (id,calificacion,semestre,docente,activo) VALUES (9306,4.5,'2026-1',91000006,1);
IF NOT EXISTS (SELECT 1 FROM evaluacion_docente WHERE id=9307) INSERT INTO evaluacion_docente (id,calificacion,semestre,docente,activo) VALUES (9307,4.7,'2026-1',91000007,1);
IF NOT EXISTS (SELECT 1 FROM evaluacion_docente WHERE id=9308) INSERT INTO evaluacion_docente (id,calificacion,semestre,docente,activo) VALUES (9308,4.6,'2026-1',91000008,1);
IF NOT EXISTS (SELECT 1 FROM evaluacion_docente WHERE id=9309) INSERT INTO evaluacion_docente (id,calificacion,semestre,docente,activo) VALUES (9309,4.9,'2026-1',91000009,1);
IF NOT EXISTS (SELECT 1 FROM evaluacion_docente WHERE id=9310) INSERT INTO evaluacion_docente (id,calificacion,semestre,docente,activo) VALUES (9310,4.8,'2026-1',91000010,1);
SET IDENTITY_INSERT evaluacion_docente OFF;
GO

-- ============================================================
-- 10. RECONOCIMIENTOS
-- ============================================================
SET IDENTITY_INSERT reconocimiento ON;
IF NOT EXISTS (SELECT 1 FROM reconocimiento WHERE id=9401) INSERT INTO reconocimiento (id,tipo,fecha,institucion,nombre,ambito,docente,activo) VALUES (9401,'Académico','2025-11-20','Institución Demo','Excelencia docente','Institucional',91000001,1);
IF NOT EXISTS (SELECT 1 FROM reconocimiento WHERE id=9402) INSERT INTO reconocimiento (id,tipo,fecha,institucion,nombre,ambito,docente,activo) VALUES (9402,'Investigación','2025-09-12','Institución Demo','Proyecto destacado','Nacional',91000002,1);
IF NOT EXISTS (SELECT 1 FROM reconocimiento WHERE id=9403) INSERT INTO reconocimiento (id,tipo,fecha,institucion,nombre,ambito,docente,activo) VALUES (9403,'Innovación','2026-02-18','Institución Demo','Innovación en el aula','Institucional',91000005,1);
IF NOT EXISTS (SELECT 1 FROM reconocimiento WHERE id=9404) INSERT INTO reconocimiento (id,tipo,fecha,institucion,nombre,ambito,docente,activo) VALUES (9404,'Investigación','2026-03-15','Institución Demo','Trayectoria investigativa','Nacional',91000010,1);
SET IDENTITY_INSERT reconocimiento OFF;
GO

-- ============================================================
-- 11. EXPERIENCIA
-- ============================================================
SET IDENTITY_INSERT experiecia ON;
IF NOT EXISTS (SELECT 1 FROM experiecia WHERE id=9501) INSERT INTO experiecia (id,nombre_cargo,institucion,tipo,fecha_inicio,fecha_fin,docente,activo) VALUES (9501,'Ingeniera de software','Empresa Demo','Profesional','2016-01-15','2019-12-20',91000001,1);
IF NOT EXISTS (SELECT 1 FROM experiecia WHERE id=9502) INSERT INTO experiecia (id,nombre_cargo,institucion,tipo,fecha_inicio,fecha_fin,docente,activo) VALUES (9502,'Analista de datos','Empresa Demo','Profesional','2015-03-02','2018-11-30',91000002,1);
IF NOT EXISTS (SELECT 1 FROM experiecia WHERE id=9503) INSERT INTO experiecia (id,nombre_cargo,institucion,tipo,fecha_inicio,fecha_fin,docente,activo) VALUES (9503,'Desarrolladora web','Empresa Demo','Profesional','2018-06-11','2021-06-30',91000003,1);
IF NOT EXISTS (SELECT 1 FROM experiecia WHERE id=9504) INSERT INTO experiecia (id,nombre_cargo,institucion,tipo,fecha_inicio,fecha_fin,docente,activo) VALUES (9504,'Ingeniero de automatización','Empresa Demo','Profesional','2012-02-01','2017-12-15',91000004,1);
IF NOT EXISTS (SELECT 1 FROM experiecia WHERE id=9505) INSERT INTO experiecia (id,nombre_cargo,institucion,tipo,fecha_inicio,fecha_fin,docente,activo) VALUES (9505,'Coordinadora académica','Institución Demo','Académica','2017-01-20','2021-12-10',91000009,1);
SET IDENTITY_INSERT experiecia OFF;
GO

-- ============================================================
-- 12. REDES DE DOCENTES
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM red_docente WHERE red=9101 AND docente=91000001) INSERT INTO red_docente VALUES (9101,91000001,'2022-02-01',NULL,'Participación en encuentros de arquitectura de software.',1);
IF NOT EXISTS (SELECT 1 FROM red_docente WHERE red=9102 AND docente=91000002) INSERT INTO red_docente VALUES (9102,91000002,'2021-08-15',NULL,'Proyectos colaborativos de analítica de datos.',1);
IF NOT EXISTS (SELECT 1 FROM red_docente WHERE red=9101 AND docente=91000003) INSERT INTO red_docente VALUES (9101,91000003,'2023-01-20',NULL,'Talleres sobre servicios web y buenas prácticas.',1);
IF NOT EXISTS (SELECT 1 FROM red_docente WHERE red=9103 AND docente=91000005) INSERT INTO red_docente VALUES (9103,91000005,'2022-06-08',NULL,'Diseño de estrategias de educación digital.',1);
IF NOT EXISTS (SELECT 1 FROM red_docente WHERE red=9103 AND docente=91000009) INSERT INTO red_docente VALUES (9103,91000009,'2020-03-05',NULL,'Investigación sobre innovación educativa.',1);
GO

-- ============================================================
-- 13. ESTUDIO - ÁREA DE CONOCIMIENTO
-- Se usan códigos que ya existen en los datos de referencia.
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM estudio_ac WHERE estudio=9201 AND area_conocimiento='2B04') INSERT INTO estudio_ac VALUES (9201,'2B04',1);
IF NOT EXISTS (SELECT 1 FROM estudio_ac WHERE estudio=9202 AND area_conocimiento='1B01') INSERT INTO estudio_ac VALUES (9202,'1B01',1);
IF NOT EXISTS (SELECT 1 FROM estudio_ac WHERE estudio=9203 AND area_conocimiento='1B01') INSERT INTO estudio_ac VALUES (9203,'1B01',1);
IF NOT EXISTS (SELECT 1 FROM estudio_ac WHERE estudio=9204 AND area_conocimiento='2B03') INSERT INTO estudio_ac VALUES (9204,'2B03',1);
IF NOT EXISTS (SELECT 1 FROM estudio_ac WHERE estudio=9206 AND area_conocimiento='2B04') INSERT INTO estudio_ac VALUES (9206,'2B04',1);
IF NOT EXISTS (SELECT 1 FROM estudio_ac WHERE estudio=9207 AND area_conocimiento='1A03') INSERT INTO estudio_ac VALUES (9207,'1A03',1);
IF NOT EXISTS (SELECT 1 FROM estudio_ac WHERE estudio=9208 AND area_conocimiento='2B03') INSERT INTO estudio_ac VALUES (9208,'2B03',1);
IF NOT EXISTS (SELECT 1 FROM estudio_ac WHERE estudio=9210 AND area_conocimiento='2B04') INSERT INTO estudio_ac VALUES (9210,'2B04',1);
GO

-- ============================================================
-- 14. APOYO PROFESORAL
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM apoyo_profesoral WHERE estudios=9201) INSERT INTO apoyo_profesoral VALUES (9201,1,'Institución Demo','Comisión de estudios',1);
IF NOT EXISTS (SELECT 1 FROM apoyo_profesoral WHERE estudios=9202) INSERT INTO apoyo_profesoral VALUES (9202,1,'Institución Demo','Apoyo económico',1);
IF NOT EXISTS (SELECT 1 FROM apoyo_profesoral WHERE estudios=9204) INSERT INTO apoyo_profesoral VALUES (9204,1,'Institución Demo','Comisión doctoral',1);
IF NOT EXISTS (SELECT 1 FROM apoyo_profesoral WHERE estudios=9205) INSERT INTO apoyo_profesoral VALUES (9205,1,'Institución Demo','Apoyo académico',1);
IF NOT EXISTS (SELECT 1 FROM apoyo_profesoral WHERE estudios=9209) INSERT INTO apoyo_profesoral VALUES (9209,1,'Institución Demo','Comisión de estudios',1);
GO

-- ============================================================
-- 15. BECAS
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM beca WHERE estudios=9201) INSERT INTO beca VALUES (9201,'Beca de posgrado','Institución Demo','2017-01-20','2018-12-20',1);
IF NOT EXISTS (SELECT 1 FROM beca WHERE estudios=9202) INSERT INTO beca VALUES (9202,'Beca académica','Fundación Demo','2018-01-15','2019-06-20',1);
IF NOT EXISTS (SELECT 1 FROM beca WHERE estudios=9204) INSERT INTO beca VALUES (9204,'Beca doctoral','Institución Demo','2014-08-01','2017-08-25',1);
IF NOT EXISTS (SELECT 1 FROM beca WHERE estudios=9209) INSERT INTO beca VALUES (9209,'Beca doctoral','Fundación Demo','2013-01-10','2016-10-21',1);
GO

PRINT 'Datos demo cargados correctamente.';
PRINT 'Docentes ficticios: 10';
PRINT 'Programas demo: 4';
PRINT 'La información de este archivo es exclusivamente de demostración.';
GO
