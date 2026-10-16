#!/usr/bin/env python3
"""Genera comprobaciones SQL de conservación física a partir de una corrida HTTP."""
import argparse,json,pathlib
p=argparse.ArgumentParser();p.add_argument('--informe',required=True);p.add_argument('--salida',required=True);a=p.parse_args()
r=pathlib.Path(__file__).resolve().parents[1];meta=json.loads((r/'db/v2_recursos.json').read_text());report=json.loads(pathlib.Path(a.informe).read_text());sql=['SET NOCOUNT ON;']
for t,m in meta.items():
 clauses=[]
 for c in m['cols']:
  if c['col'] not in m['keys']:continue
  value=report['filas'][t][c['prop'][0].lower()+c['prop'][1:]]
  literal=str(value) if isinstance(value,int) else "N'"+str(value).replace("'","''")+"'"
  clauses.append(c['col']+'='+literal)
 sql.append(f"IF NOT EXISTS(SELECT 1 FROM dbo.{t} WHERE "+' AND '.join(clauses)+f" AND activo=0) THROW 51110, N'No se conservó retirado: {t}', 1;")
sql.append("PRINT N'OK: las 16 filas retiradas permanecen físicamente en SQL Server.';")
pathlib.Path(a.salida).write_text('\n'.join(sql))
