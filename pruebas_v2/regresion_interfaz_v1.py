#!/usr/bin/env python3
"""Crear, editar por ambos botones y retirar los cinco recursos V1 desde Blazor."""
import argparse,json,pathlib,secrets,re
from datetime import datetime,timezone
from playwright.sync_api import sync_playwright,expect
p=argparse.ArgumentParser();p.add_argument('--front',default='http://localhost:8075');p.add_argument('--chromium',default='/usr/bin/chromium');p.add_argument('--confirmar-base-pruebas',action='store_true',required=True);p.add_argument('--salida',default='/tmp/gestion-v2-v1-interfaz.json');a=p.parse_args()
mark='RV'+secrets.token_hex(2);key=str(800_000_000+secrets.randbelow(100_000_000));checks=[]
cases=[
 ('programas',{'Código':key,'Nombre':mark,'Tipo':'Pregrado','Nivel':'Profesional','Fecha de creación':'2026','Número de cohortes':'1','Cantidad de graduados':'0','Fecha de actualización':'2026','Ciudad':'Medellín','Facultad':'1'},'Ciudad','Cali','Bogotá'),
 ('redes',{'Código':key,'Nombre':mark,'URL':'https://example.edu','País':'Colombia'},'País','Chile','Perú'),
 ('areas-conocimiento',{'Código':mark,'Gran área':mark,'Área':'Pruebas','Disciplina':'Disciplina de prueba'},'Disciplina','Disciplina completa','Disciplina parcial'),
 ('terminos-clave',{'Término':mark,'Término en inglés':'Test'},'Término en inglés','Complete','Partial'),
 ('lineas-investigacion',{'Nombre':mark,'Descripción':'Descripción de prueba'},'Descripción','Descripción completa','Descripción parcial')]
error=None
try:
 with sync_playwright() as pw:
  b=pw.chromium.launch(executable_path=a.chromium,headless=True,args=['--no-sandbox']);page=b.new_page();page.set_default_timeout(30000);page.on('dialog',lambda d:d.accept())
  for route,fields,editable,full,partial in cases:
   page.goto(a.front+'/'+route,wait_until='domcontentloaded');page.wait_for_function('window.Blazor !== undefined');page.wait_for_timeout(800)
   page.get_by_role('button',name='Agregar',exact=True).click()
   form=page.locator('.tarjeta-formulario').first;expect(form).to_be_visible()
   def fill(label,value):
    exact=form.get_by_label(label,exact=True)
    (exact if exact.count() else form.get_by_label(label,exact=False)).first.fill(value)
   for label,value in fields.items():fill(label,value)
   form.get_by_role('button',name='Agregar',exact=True).click()
   row=page.get_by_role('row').filter(has_text=mark);expect(row).to_have_count(1);checks.append({'prueba':route+': crear desde interfaz','paso':True})
   row.get_by_role('button',name='Editar',exact=True).click();fill(editable,full)
   form.get_by_role('button',name='Guardar la ficha completa',exact=True).click();expect(row).to_contain_text(full);checks.append({'prueba':route+': reemplazo completo desde interfaz','paso':True})
   row.get_by_role('button',name='Editar',exact=True).click();fill(editable,partial)
   form.get_by_role('button',name='Guardar solo lo que cambié',exact=True).click();expect(row).to_contain_text(partial);checks.append({'prueba':route+': actualización parcial desde interfaz','paso':True})
   row.get_by_role('button',name='Retirar',exact=True).click();expect(row).to_have_count(0);checks.append({'prueba':route+': retiro desde interfaz','paso':True})
  b.close()
except Exception as e:
 import traceback;traceback.print_exc();error=str(e)
finally:
 pathlib.Path(a.salida).write_text(json.dumps({'fecha':datetime.now(timezone.utc).isoformat(),'comprobaciones':checks,'error':error},ensure_ascii=False,indent=2))
 print(f'{len(checks)}/20 pasos de regresión V1 por interfaz; informe: {a.salida}')
 if error:raise SystemExit(1)
