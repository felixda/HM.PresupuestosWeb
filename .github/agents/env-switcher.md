---
description: Cambia los user-secrets del proyecto al entorno indicado (DEV, PRU, PRE, PRO). Úsalo cuando quieras arrancar la aplicación apuntando a un entorno concreto.
tools: Bash
---

# Env Switcher — HM.Presupuestos

Cambia los `dotnet user-secrets` del proyecto al entorno solicitado por el usuario.

## Prerrequisito

El fichero `secrets.local.json` debe existir en la raíz del repositorio y **no está en git**.
Si no existe, indicar al usuario que lo cree a partir de la plantilla:

```powershell
cp secrets.local.template.json secrets.local.json
# A continuación, editar secrets.local.json con los valores reales de cada entorno.
```

## Pasos

### 1. Identificar el entorno

Leer el mensaje del usuario y extraer el entorno. Valores válidos: `DEV`, `PRU`, `PRE`, `PRO`.
Si no se indica o no es válido, preguntar antes de continuar.

### 2. Ejecutar el script

```powershell
.\Switch-Env.ps1 -Env <ENTORNO>
```

### 3. Interpretar el resultado

- Si termina con `OK — N secretos aplicados`: confirmar al usuario qué entorno está activo.
- Si el error es `No se encontro 'secrets.local.json'`: explicar que debe copiar la plantilla y rellenar los valores reales.
- Si el error es `no existe en secrets.local.json`: el JSON no tiene la sección del entorno pedido.
- Si cualquier otro error: mostrar el mensaje de error literal y sugerir revisar el fichero.

## Formato de respuesta

Responder en una sola línea confirmando el cambio, por ejemplo:

> Entorno cambiado a **PRU**. Ya puedes arrancar la aplicación con `dotnet run --project HM.Presupuestos.Web`.
