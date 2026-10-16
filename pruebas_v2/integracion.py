#!/usr/bin/env python3
"""Pruebas HTTP reales. Solo ejecutar contra una base de pruebas aislada.
No borra datos físicamente; retira únicamente las filas creadas en esta ejecución.
"""
import argparse, copy, json, pathlib, secrets, sys, traceback
import urllib.request, urllib.error, urllib.parse
from datetime import datetime, timezone

ROOT=pathlib.Path(__file__).resolve().parents[1]
META=json.loads((ROOT/'db/v2_recursos.json').read_text())
ROUTES=dict(zip(META,['area-conocimiento','termino-clave','linea-investigacion','programa','red','docente','estudios-realizados','docente-departamento','intereses-futuros','evaluacion-docente','reconocimiento','experiecia','red-docente','estudio-ac','apoyo-profesoral','beca']))
parser=argparse.ArgumentParser()
parser.add_argument('--api',default='http://localhost:8074')
parser.add_argument('--confirmar-base-pruebas',action='store_true',required=True)
parser.add_argument('--salida',default='/tmp/gestion-v2-resultados.json')
a=parser.parse_args()
base=secrets.randbelow(500_000_000)+100_000_000
mark='V'+secrets.token_hex(2).upper()
records=[]; asserts=[];rows={};paths={}

def check(cond,msg):
    asserts.append({'prueba':msg,'paso':bool(cond)})
    if not cond:raise AssertionError(msg)

def req(method,path,body=None,status=200):
    data=None if body is None else json.dumps(body).encode()
    request=urllib.request.Request(a.api+path,data=data,method=method,headers={'Content-Type':'application/json'})
    try:
        with urllib.request.urlopen(request,timeout=30) as response:code=response.status;raw=response.read().decode()
    except urllib.error.HTTPError as e:code=e.code;raw=e.read().decode()
    result=json.loads(raw) if raw else None
    records.append({'metodo':method,'ruta':path,'cuerpo':body,'esperado':status,'obtenido':code})
    check(code in status if isinstance(status,list) else code==status,f'{method} {path}: esperado {status}, recibido {code}; {raw[:250] if code>=500 else ""}')
    return result

def route(t):return '/api/'+ROUTES[t]
def keypath(t,row):return route(t)+'/'+ '/'.join(urllib.parse.quote(str(row[c['prop'][0].lower()+c['prop'][1:]]),safe='') for c in META[t]['cols'] if c['col'] in META[t]['keys'])
def fixture(t):
    body={}
    for c in META[t]['cols']:
        if c['identity']:continue
        p=c['prop'][0].lower()+c['prop'][1:]
        if not c['required']:v=None
        elif c['sql']=='DATE':v='2026-01-02'
        elif c['sql']=='TINYINT':v=1
        elif c['sql']=='REAL':v=4.5
        elif c['sql']=='INT':v=base
        else:v=mark if c['col']=='id' else 'Prueba '+mark
        if p=='correo':v=f'{mark}@universidad.edu'
        if p in ['url','urlCvlac']:v='https://universidad.edu/pruebas'
        body[p]=v
    for fk,parent,pk in META[t]['fks']:
        prop=next(c['prop'] for c in META[t]['cols'] if c['col']==fk);prop=prop[0].lower()+prop[1:]
        parentprop=next(c['prop'] for c in META[parent]['cols'] if c['col']==pk);parentprop=parentprop[0].lower()+parentprop[1:]
        body[prop]=rows[parent][parentprop]
    return body

def run():
    # v1 parents and every v2 resource, keeping parents active until regression is complete.
    for t,m in META.items():
        body=fixture(t)
        creada=req('POST',route(t),body)
        listed=req('GET',route(t)+'?limite=2147483647')['datos']
        candidates=[row for row in listed if all(row.get(k)==v for k,v in body.items())]
        check(len(candidates)==1,f'{t}: crear devuelve valores persistidos en listado')
        row=candidates[0];rows[t]=row;paths[t]=keypath(t,row)
        check(all(creada['datos'].get(k)==v for k,v in row.items()),f'{t}: respuesta contiene la fila persistida y su identidad')
        req('GET',paths[t])
        req('GET',route(t)+'?limite=0',status=400)
        req('POST',route(t),{},status=422)
        req('PUT',paths[t],{},status=200 if t == 'termino_clave' else 422)
        req('PATCH',paths[t],{},status=400)
        # Pure two-key bridges can replace themselves; others edit an independent field.
        edit=next((c for c in m['cols'] if c['col'] not in m['keys'] and c['col'] not in [f[0] for f in m['fks']] and not c['identity'] and c['sql'].startswith('VARCHAR')),None)
        patch={edit['prop'][0].lower()+edit['prop'][1:]:'Cambio '+mark} if edit else {next(iter(body)):next(iter(body.values()))}
        req('PATCH',paths[t],patch)
        actual=req('GET',paths[t]);check(all(actual.get(k)==v for k,v in patch.items()),f'{t}: modificación parcial persistida')
        req('PUT',paths[t],body)
        actual=req('GET',paths[t]);check(all(actual.get(k)==v for k,v in body.items()),f'{t}: reemplazo completo conserva valores y opcionales nulos')
        if not any(c['identity'] for c in m['cols']):req('POST',route(t),body,status=409)
        for fk,parent,pk in m['fks']:
            bad=copy.deepcopy(body)
            c=next(c for c in m['cols'] if c['col']==fk);bad[c['prop'][0].lower()+c['prop'][1:]]='NOEX' if c['sql'].startswith('VARCHAR') else -987654
            r=req('POST',route(t),bad,status=409);check(bool(r.get('mensaje')),f'{t}.{fk}: conflicto con mensaje en español')
    check(req('GET',paths['docente'])['totalEstudios']==1,'Trigger: crear estudio incrementa contador')
    req('POST',route('docente')+'/expediente',{'docente':rows['docente'],'detalles':{'estudios':[None]}},status=422)
    sinlinea=copy.deepcopy(rows['docente']);sinlinea['lineaInvestigacionPrincipal']=None
    req('PUT',paths['docente'],sinlinea)
    check(req('GET',paths['docente'])['lineaInvestigacionPrincipal'] is None,'PUT: limpiar la relación opcional conserva null')
    req('PUT',paths['docente'],rows['docente'])
    for t in ['programa','area_conocimiento','termino_clave','linea_investigacion','red','docente','estudios_realizados']:
        req('DELETE',paths[t],status=409)
    # Move an active study between teachers: both totals must change.
    teacher=copy.deepcopy(rows['docente']);teacher['cedula']=base+1;teacher.pop('totalEstudios',None)
    req('POST',route('docente'),teacher)
    req('PATCH',paths['estudios_realizados'],{'docente':base+1})
    check(req('GET',paths['docente'])['totalEstudios']==0,'Trigger: traslado decrementa maestro anterior')
    check(req('GET',route('docente')+'/'+str(base+1))['totalEstudios']==1,'Trigger: traslado incrementa maestro nuevo')
    req('PATCH',paths['estudios_realizados'],{'docente':base})
    req('DELETE',route('docente')+'/'+str(base+1))
    # All rows in a multi-row submission roll back, including trigger effects.
    study=copy.deepcopy(rows['estudios_realizados']);study['id']=base+2
    badstudy=copy.deepcopy(study)
    aggregate={'docente':copy.deepcopy(rows['docente']),'detalles':{'estudios':[study,badstudy]}}
    aggregate['docente']['nombres']='Cambio que debe revertirse'
    req('POST',paths['docente']+'/detalles',aggregate,status=409)
    req('GET',route('estudios_realizados')+'/'+str(base+2),status=404)
    check(req('GET',paths['docente'])['totalEstudios']==1,'Atomicidad: rollback también revierte contador')
    check(req('GET',paths['docente'])['nombres']==rows['docente']['nombres'],'Atomicidad: fallo del detalle revierte cambios de la ficha')
    aggregate['docente']=copy.deepcopy(rows['docente'])
    aggregate['detalles']['estudios'][1]['id']=base+3
    req('POST',paths['docente']+'/detalles',aggregate)
    check(req('GET',paths['docente'])['totalEstudios']==3,'Atomicidad: dos renglones guardados en un solo envío')
    expediente=req('GET',paths['docente']+'/expediente')['datos']
    check(len(expediente['detalles']['estudios'])==3,'Expediente muestra solo detalles del maestro')
    # Study detail transaction: support inserts before duplicate bridge must disappear on failure.
    nested=copy.deepcopy(rows['estudios_realizados']);nested['id']=base+4
    beca=copy.deepcopy(rows['beca']);beca['estudios']=base+4
    area={'estudio':base+4,'areaConocimiento':rows['area_conocimiento']['id']}
    studyagg={'estudio':nested,'detalles':{'becas':[beca],'areas':[area,area]}}
    req('POST',route('estudios_realizados')+'/expediente',studyagg,status=409)
    req('GET',route('estudios_realizados')+'/'+str(base+4),status=404)
    studyagg['detalles']['areas']=[area]
    req('POST',route('estudios_realizados')+'/expediente',studyagg)
    view=req('GET',route('estudios_realizados')+'/'+str(base+4)+'/expediente')['datos']
    check(len(view['detalles']['becas'])==1 and len(view['detalles']['areas'])==1,'Estudio maestro, beca y puente guardados juntos')
    # New teacher must roll back along with detail on invalid relation.
    newteacher=copy.deepcopy(rows['docente']);newteacher['cedula']=base+5
    interest={'docente':base+5,'terminoClave':'NO_EXISTE'}
    newagg={'docente':newteacher,'detalles':{'intereses':[interest]}}
    req('POST',route('docente')+'/expediente',newagg,status=409)
    req('GET',route('docente')+'/'+str(base+5),status=404)
    interest['terminoClave']=rows['termino_clave']['termino']
    req('POST',route('docente')+'/expediente',newagg)
    # Retire only our created data, children first. Physical persistence is checked separately in SQL.
    req('DELETE',route('intereses_futuros')+'/'+str(base+5)+'/'+urllib.parse.quote(interest['terminoClave']))
    req('DELETE',route('docente')+'/'+str(base+5))
    req('DELETE',route('estudio_ac')+'/'+str(base+4)+'/'+rows['area_conocimiento']['id'])
    req('DELETE',route('beca')+'/'+str(base+4))
    for i in [2,3,4]:req('DELETE',route('estudios_realizados')+'/'+str(base+i))
    for t in reversed(META):
        req('DELETE',paths[t])
        req('GET',paths[t],status=404)
        req('DELETE',paths[t],status=404)
        listed=req('GET',route(t),status=[200,204])
        if listed:check(not any(keypath(t,row)==paths[t] for row in listed['datos']),f'{t}: listado excluye retirados')
    check(req('GET',route('docente')+'/'+str(base),status=404)['estado']==404,'Docente retirado no consultable')
    # A logically retired parent must never be offered as a valid relationship.
    invalid=copy.deepcopy(rows['estudios_realizados']);invalid['id']=base+6
    req('POST',route('estudios_realizados'),invalid,status=409)

error=None
try:run()
except Exception as e:error=str(e);traceback.print_exc()
finally:
    report={'fecha':datetime.now(timezone.utc).isoformat(),'api':a.api,'identificador':base,'marca':mark,'filas':rows,'claves':paths,'comprobaciones':asserts,'peticiones':records,'error':error}
    pathlib.Path(a.salida).write_text(json.dumps(report,ensure_ascii=False,indent=2))
    collection={'info':{'name':'Gestión Profesoral V2 — peticiones reales de integración','schema':'https://schema.getpostman.com/json/collection/v2.1.0/collection.json'},'variable':[{'key':'baseUrl','value':a.api}], 'item':[]}
    for i,x in enumerate(records):
        request={'method':x['metodo'],'header':[{'key':'Content-Type','value':'application/json'}],'url':'{{baseUrl}}'+x['ruta']}
        if x['cuerpo'] is not None:request['body']={'mode':'raw','raw':json.dumps(x['cuerpo'],ensure_ascii=False,indent=2),'options':{'raw':{'language':'json'}}}
        expected=x['esperado'] if isinstance(x['esperado'],list) else [x['esperado']]
        collection['item'].append({'name':f'{i+1}. {x["metodo"]} {x["ruta"]}', 'request':request,'event':[{'listen':'test','script':{'type':'text/javascript','exec':[f'pm.test("Estado esperado", () => pm.expect({json.dumps(expected)}).to.include(pm.response.code));']}}]})
    pathlib.Path(a.salida).with_suffix('.postman.json').write_text(json.dumps(collection,ensure_ascii=False,indent=2))
    print(f'{sum(x["paso"] for x in asserts)}/{len(asserts)} comprobaciones; {len(records)} peticiones; informe: {a.salida}')
    sys.exit(1 if error else 0)
