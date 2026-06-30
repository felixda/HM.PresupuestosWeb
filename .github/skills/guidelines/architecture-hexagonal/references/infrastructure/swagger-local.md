# Guideline: Swagger Local (HM.Presupuestos.Api)

> Guia operativa para usar Swagger en local con autenticacion JWT de desarrollo.
> Aplica al proyecto `HM.Presupuestos.Api`.

---

## 1. Objetivo

Facilitar pruebas manuales de endpoints REST desde Swagger UI sin romper el flujo real de autenticacion por Bearer.

---

## 2. Script oficial

Usar el script del repositorio:

- `RunSwaggerWithToken.ps1`

Este script:

1. Lee la clave de firma JWT con precedencia:
   - `Auth:SigningKey`
   - fallback a `Jwt:Clave`
2. Genera un token JWT de desarrollo.
3. Guarda el token en:
   - `HM.Presupuestos.Api/bin/Debug/net10.0/swagger-dev.jwt`
4. Copia el token al portapapeles.
5. Arranca la API local (si no esta ya activa).
6. Abre Swagger UI.
7. Verifica autenticacion contra un endpoint de control (`api/v1/maestros/tipologias`).

---

## 3. Uso rapido

Desde la raiz del repo:

```powershell
.\RunSwaggerWithToken.ps1
```

Opciones utiles:

```powershell
# No abrir navegador
.\RunSwaggerWithToken.ps1 -NoBrowser

# No arrancar API automaticamente (si ya esta levantada)
.\RunSwaggerWithToken.ps1 -NoStartApi

# Cambiar URL base de Swagger
.\RunSwaggerWithToken.ps1 -SwaggerBaseUrl "http://localhost:5078"
```

---

## 4. Como autorizar en Swagger UI

1. Abrir `http://localhost:5078/swagger` (o la URL configurada).
2. Pulsar `Authorize`.
3. Pegar solo el token (sin prefijo `Bearer `).
4. Ejecutar endpoints protegidos.

---

## 5. Diagnostico de problemas comunes

### 401 Unauthorized en Swagger

Verificar:

1. La API esta usando la misma signing key que el script.
2. El token no esta expirado (`exp`).
3. El proceso de API no esta bloqueando binarios antiguos.

Indicador tipico de bloqueo:

- errores `MSB3027/MSB3021` al compilar por DLL bloqueadas en `C:\publish\net10.0`.

Accion recomendada:

1. Parar proceso en ejecucion de `HM.Presupuestos.Api`.
2. Recompilar.
3. Reejecutar `RunSwaggerWithToken.ps1`.

### El endpoint de verificacion no responde 200

El script prueba por defecto:

- `GET /api/v1/maestros/tipologias`

Si falla, revisar logs de API y la resolucion de usuario desde token/claims en el pipeline de autenticacion.

---

## 6. Reglas de mantenimiento

1. Si se cambia la configuracion JWT de la API, actualizar tambien `RunSwaggerWithToken.ps1`.
2. Mantener la verificacion automatica del script para detectar regresiones de auth temprano.
3. No introducir hacks de autenticacion solo para Swagger: mantener coherencia con Bearer real.
