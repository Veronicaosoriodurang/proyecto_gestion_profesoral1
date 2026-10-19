#!/usr/bin/env python3
"""Blazor real en Chromium: docente con varios estudios, rollback y trigger por UI."""
import argparse,json,pathlib,re,secrets,urllib.request,urllib.error
from datetime import datetime,timezone
from playwright.sync_api import sync_playwright,expect
p=argparse.ArgumentParser();p.add_argument('--front',default='http://localhost:8075');p.add_argument('--api',default='http://localhost:8074');p.add_argument('--chromium',default='/usr/bin/chromium');p.add_argument('--confirmar-base-pruebas',action='store_true',required=True);p.add_argument('--salida',default='/tmp/gestion-v2-interfaz.json');a=p.parse_args()
base=700_000_000+secrets.randbelow(100_000_000);marca='UI'+secrets.token_hex(2);checks=[]
def api(path):
 try:
  with urllib.request.urlopen(a.api+path) as r:return r.status,json.load(r)
 except urllib.error.HTTPError as e:return e.code,json.load(e)
def check(value,text):
 checks.append({'prueba':text,'paso':bool(value)})
 if not value:raise AssertionError(text)
error=None
try:
 with sync_playwright() as pw:
  browser=pw.chromium.launch(executable_path=a.chromium,headless=True,args=['--no-sandbox']);page=browser.new_page(viewport={'width':1440,'height':1100});page.set_default_timeout(45000);page.set_default_navigation_timeout(45000)
  pageerrors=[];page.on('pageerror',lambda e:pageerrors.append(str(e)))
  routes=['programas','redes','areas-conocimiento','terminos-clave','lineas-investigacion','docentes','estudios-realizados','docente-departamento','intereses-futuros','evaluacion-docente','reconocimientos','experiencia','red-docente','estudio-ac','apoyo-profesoral','beca']
  # Actual page routes are taken from the versioned Blazor files, avoiding guessed URLs.
  root=pathlib.Path(__file__).resolve().parents[1]
  routes=[]
  for file in (root/'front_blazor/Components/Pages').glob('*.razor'):
   match=re.search(r'@page "([^"]+)"',file.read_text(encoding='utf-8-sig'))
   if match and match[1]!='/':routes.append(match[1])
  for route in routes:
   response=page.goto(a.front+route,wait_until="domcontentloaded");check(response.status==200,'Pantalla responde: '+route)
   expect(page.locator('h1')).to_be_visible()
  page.goto(a.front+'/docentes');page.wait_for_function('window.Blazor !== undefined');page.wait_for_timeout(1000)
  page.get_by_role('button',name='Agregar',exact=True).click();expect(page.get_by_role('heading',name='Expediente del docente')).to_be_visible()
  fields={'Cédula':str(base),'Nombres':marca,'Apellidos':'Prueba interfaz','Género':'Otro','Cargo':'Docente','Fecha de nacimiento':'1990-01-01','Correo':marca+'@example.edu','Teléfono':'3001234567','URL CvLAC':'https://example.edu/cvlac','Fecha de actualización':'2026-10-16','Escalafón':'Prueba','Perfil':'Perfil de prueba','Convocatoria Minciencias':'Prueba','Nacionalidad':'Colombia'}
  for label,value in fields.items():page.get_by_label(label,exact=False).first.fill(value)
  page.get_by_label('Línea de investigación principal').select_option('')
  section=page.locator('section').filter(has=page.get_by_role('heading',name='Expediente del docente',exact=True)).first
  studies=section.locator('details').filter(has=page.locator('summary',has_text=re.compile('^Estudios realizados'))).first
  studies.locator('summary').first.click()
  def add_study(code,title):
   values={'Código del estudio':str(code),'Título':title,'Universidad':'Universidad de prueba','Fecha':'2026-01-01','Tipo':'Maestría','Ciudad':'Medellín','Metodología':'Presencial','Perfil de egreso':'Perfil','País':'Colombia'}
   for label,value in values.items():studies.get_by_label(label,exact=True).fill(value)
   studies.get_by_label('Institución acreditada').select_option('1')
   studies.get_by_role('button',name='Preparar renglón',exact=True).click()
  add_study(base,marca+' A');add_study(base,marca+' Duplicado')
  section.get_by_role('button',name='Guardar expediente completo',exact=True).first.click()
  expect(section.get_by_text('No se guardó ningún renglón',exact=False).first).to_be_visible()
  check(api('/api/docente/'+str(base))[0]==404,'UI: error en segundo renglón revierte maestro')
  check(api('/api/estudios-realizados/'+str(base))[0]==404,'UI: error en segundo renglón revierte primer detalle')
  studies.get_by_role('button',name='Quitar del envío',exact=True).last.click()
  add_study(base+1,marca+' B')
  section.get_by_role('button',name='Guardar expediente completo',exact=True).first.click()
  row=page.get_by_role('row').filter(has_text=marca)
  expect(row).to_be_visible();check(api('/api/docente/'+str(base))[1]['totalEstudios']==2,'UI: guardado maestro y dos estudios actualiza trigger')
  check(api('/api/docente/'+str(base))[1]['lineaInvestigacionPrincipal'] is None,'UI: opción sin línea envía null')
  row.get_by_role('button',name='Editar',exact=True).click()
  expect(page.get_by_text('Estudios activos: 2',exact=True)).to_be_visible()
  section=page.locator('section').filter(has=page.get_by_role('heading',name='Expediente del docente',exact=True)).first
  studies=section.locator('details').filter(has=page.locator('summary',has_text=re.compile('^Estudios realizados'))).first
  studies.locator('summary').first.click()
  expect(studies.get_by_text(marca+' A',exact=False).first).to_be_visible()
  check(True,'UI: detalle visible desde maestro sin volver a elegir docente')
  # Study-specific master-detail UI, including optional dates and API-backed area catalog.
  child=studies.locator('li').filter(has_text=marca+' A').first
  childsection=child.locator('section').first
  areas=childsection.locator('details').filter(has=page.locator('summary',has_text=re.compile('^Áreas de conocimiento'))).first
  areas.locator('summary').click();area_select=areas.get_by_label('Área de conocimiento',exact=False)
  expect(area_select.locator('option').nth(1)).to_be_attached()
  check(area_select.locator('option').count()>1,'UI: desplegable de áreas cargado desde API')
  option=area_select.locator('option').nth(1);areaid=option.get_attribute('value');area_select.select_option(areaid)
  areas.get_by_role('button',name='Preparar renglón',exact=True).click()
  childsection.get_by_role('button',name='Guardar expediente completo',exact=True).click()
  expect(childsection.get_by_text('Se guardó el expediente completo.',exact=True)).to_be_visible()
  check(api('/api/estudios-realizados/'+str(base)+'/expediente')[1]['datos']['detalles']['areas'][0]['areaConocimiento']==areaid,'UI: puente guardado desde estudio')
  areas.get_by_role('button',name='Retirar',exact=True).first.click()
  expect(areas.locator('summary')).to_contain_text('0 guardados')
  child.get_by_role('button',name='Retirar',exact=True).first.click()
  expect(page.get_by_text('Estudios activos: 1',exact=True)).to_be_visible()
  check(api('/api/docente/'+str(base))[1]['totalEstudios']==1,'UI: retirar estudio actualiza contador sin enviarlo')
  remaining=studies.locator('li').filter(has_text=marca+' B').first
  remaining.get_by_role('button',name='Retirar',exact=True).first.click()
  expect(page.get_by_text('Estudios activos: 0',exact=True)).to_be_visible()
  check(api('/api/docente/'+str(base))[1]['totalEstudios']==0,'UI: segundo retiro deja contador cero')
  page.get_by_role('button',name='Cancelar',exact=True).click()
  page.on('dialog',lambda dialog:dialog.accept())
  page.get_by_role('row').filter(has_text=marca).get_by_role('button',name='Retirar',exact=True).click()
  expect(page.get_by_role('row').filter(has_text=marca)).to_have_count(0)
  check(api('/api/docente/'+str(base))[0]==404,'UI: docente retirado desaparece')
  check(not pageerrors,'Chromium no registró errores de JavaScript')
  browser.close()
except Exception as e:
 import traceback;traceback.print_exc();error=str(e)
finally:
 pathlib.Path(a.salida).write_text(json.dumps({'fecha':datetime.now(timezone.utc).isoformat(),'identificador':base,'comprobaciones':checks,'error':error},ensure_ascii=False,indent=2))
 print(f'{sum(x["paso"] for x in checks)}/{len(checks)} comprobaciones de interfaz; informe: {a.salida}')
 if error:raise SystemExit(1)
