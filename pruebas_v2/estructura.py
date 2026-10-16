#!/usr/bin/env python3
"""Verifica el contrato SQL versionado sin simular una ejecución de SQL Server."""
import json,pathlib,re
root=pathlib.Path(__file__).resolve().parents[1]
meta=json.loads((root/'db/v2_recursos.json').read_text());sql=(root/'db/version_2.sql').read_text();checks=0
for tabla,m in meta.items():
 for op in ['listar','consultar','crear','actualizar','eliminar']:
  assert re.search(r'CREATE OR ALTER PROCEDURE dbo.sp_'+op+'_'+tabla+r'\b',sql),(tabla,op);checks+=1
 repo=(root/f'api_gestion/Repositorios/Repositorio{m["name"]}SqlServer.cs').read_text()
 assert not re.search(r'\b(SELECT|INSERT|UPDATE|DELETE)\s',repo,re.I),tabla;checks+=1
 assert 'CommandType.StoredProcedure' in repo,tabla;checks+=1
for name in ['1_constitution','2_spec','3_plan','4_research','5_data_model','6_contracts','7_quickstart','8_tasks','9_checklist','GUIA_IA2','PROMPTS']:
 assert (root/f'docs/spec_kit/versiones/v2_relaciones/{name}.md').is_file(),name;checks+=1
assert re.search(r'AFTER INSERT, UPDATE, DELETE',sql);checks+=1
assert 'DROP TABLE' not in sql and 'TRUNCATE' not in sql;checks+=1
print(f'{checks} comprobaciones de estructura aprobadas. Esto no sustituye las pruebas de SQL Server.')
