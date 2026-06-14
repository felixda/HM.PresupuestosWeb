# MCP Azure DevOps — Configuración y uso

## Cómo está configurado

### Fichero de configuración
`.vscode/mcp.json` (en el repositorio, commiteado):

```json
{
  "servers": {
    "azure-devops": {
      "type": "stdio",
      "command": "npx",
      "args": ["-y", "@tiberriver256/mcp-server-azure-devops"],
      "env": {
        "AZURE_DEVOPS_ORG_URL": "https://dev.azure.com/havasit-iberia",
        "AZURE_DEVOPS_DEFAULT_PROJECT": "ESFNN - España Resto Finanzas",
        "AZURE_DEVOPS_AUTH_METHOD": "pat"
      }
    }
  }
}
```

### Variable de entorno obligatoria (NO en el repositorio)
El PAT se define como variable de entorno del sistema operativo en la máquina del desarrollador:

```
AZURE_DEVOPS_PAT=<personal_access_token>
```

Sin esta variable el servidor arranca pero falla al autenticar.

### Paquete npm
`@tiberriver256/mcp-server-azure-devops` — se descarga automáticamente con `npx -y` al iniciar VS Code.

---

## Cómo crear un PAT

1. Ir a https://dev.azure.com/havasit-iberia
2. Click en el avatar (arriba derecha) → **Personal access tokens**
3. **New Token** con los scopes:
   - Work Items: **Read & Write**
   - Code: **Read** (opcional, para búsquedas de código)
4. Copiar el token generado
5. Definir la variable de entorno del sistema:
   - Windows: `setx AZURE_DEVOPS_PAT "el_token"` (o desde Panel de control → Variables de entorno)
6. **Reiniciar VS Code** para que el servidor MCP recoja la variable

---

## Convenciones de uso

- Proyecto por defecto: `ESFNN - España Resto Finanzas`
- Organización: `havasit-iberia`
- Al crear Tasks, siempre incluir `"Custom.CompetencyCenter": "Develop"` en `additionalFields`
- Vincular Tasks al US con `parentId`
- Numeración de tareas: ver `.github/skills/guidelines/azure-devops/SKILL.md`

---

## Diagnóstico de arranque (normal)

Al iniciar VS Code aparecen estos mensajes en el output (son informativos, no errores):

```
[warning] [server stderr]   AZURE_DEVOPS_ORG_URL: https://dev.azure.com/havasit-iberia
[warning] [server stderr]   AZURE_DEVOPS_AUTH_METHOD: pat
[warning] [server stderr]   AZURE_DEVOPS_PAT: SET (hidden)
[warning] [server stderr]   AZURE_DEVOPS_DEFAULT_PROJECT: ESFNN - España Resto Finanzas
[warning] [server stderr]   AZURE_DEVOPS_API_VERSION: NOT SET   ← opcional, ignorar
[warning] [server stderr]   NODE_ENV: NOT SET                   ← opcional, ignorar
[info] Discovered 44 tools
```

Si `AZURE_DEVOPS_PAT` aparece como `NOT SET` → la variable de entorno no está definida o VS Code no la ve. Redefinirla y reiniciar VS Code.
