# Prompt: Añadir entrada de traducción en los ficheros de recursos

## Descripción
Añade una nueva entrada de traducción en los tres ficheros de recursos JSON de la aplicación:
- `HM.Presupuestos.Server\wwwroot\data\app.es.json`
- `HM.Presupuestos.Server\wwwroot\data\app.en.json`
- `HM.Presupuestos.Server\wwwroot\data\app.pt.json`

## Estructura de los ficheros
Los ficheros JSON tienen secciones principales como:
- `Menu` — entradas del menú de navegación
- `Pages` — textos por página (ej: `Pages.Versiones.Titulo`, `Pages.Index.Hola`)
- `Common` — textos reutilizables en toda la app

Cada entrada sigue este formato:
```json
"ClaveEntrada": {
  "label": "Texto traducido"
}
```

Para entradas de menú el formato es:
```json
"Menu_N": {
  "label": "Texto del menú",
  "url": "/ruta",
  "icono": "fa-solid fa-icono",
  "visible": true,
  "code": N
}
```

## Instrucciones para Copilot

El usuario **siempre** indicará el texto en español. Las traducciones a inglés y portugués se generan **automáticamente**.

Cuando el usuario indique:
- **La clave** de la nueva entrada (ej: `NuevaPagina.Titulo`)
- **La sección** donde va (ej: `Pages.NuevaPagina` o `Common`)
- **El texto en español**

Debes:
1. Leer los tres ficheros JSON para entender la estructura actual de la sección indicada
2. Traducir automáticamente el texto español al **inglés** para `app.en.json`
3. Traducir automáticamente el texto español al **portugués** para `app.pt.json`
4. Añadir la entrada en los tres ficheros manteniendo el formato y la indentación existente
5. Nunca romper la sintaxis JSON (comas, llaves, corchetes). Al usar `replace_string_in_file` para insertar una nueva entrada:
   - El `oldString` debe incluir únicamente el texto anterior al punto de inserción (por ejemplo, la línea justo antes de donde se insertará la nueva clave), **sin incluir** la llave de apertura `{` de la entrada siguiente.
   - El `newString` debe contener el texto original del `oldString` más la nueva entrada completa con todas sus llaves `{` y `}`.
   - **Nunca** omitir ni "consumir" la `{` de apertura del objeto que viene a continuación.
   - Ejemplo correcto de `newString` cuando se inserta antes de `"UnauthorizedUser"`:
     ```
         },
         "NuevaEntrada": {
           "label": "Texto"
         },
         "UnauthorizedUser": {
     ```
6. Respetar las siguientes reglas de posición:
   - Si la sección es **`Common`**: insertar la entrada en **orden alfabético** por clave
   - Para **cualquier otra sección**: insertar la entrada al **final de la sección**, antes del cierre `}`
7. Tras modificar los JSON, actualizar la clase `HM.Presupuestos.Server\Helper\TextosApp.cs`:
   - Leer el fichero para localizar la clase estática correspondiente a la sección (ej: `Common`, `Pages`, `Mensajes`)
   - Añadir una nueva constante `public const string` siguiendo el patrón existente:
     ```csharp
     /// <summary>Texto en español</summary>
     public const string Clave = "Seccion:Clave:label";
     ```
   - Respetar el **orden alfabético** si la sección es `Common`, o añadir al final de la clase si es otra sección
   - No modificar ninguna otra parte de la clase

## Ejemplo de uso

**Usuario:** Añade la clave `Titulo` en la sección `Pages.NuevaPagina` con el texto "Nueva Página".

**Resultado esperado en app.es.json:**
```json
"NuevaPagina": {
  "Titulo": {
    "label": "Nueva Página"
  }
}
```

**Resultado esperado en app.en.json** (traducción automática al inglés):
```json
"NuevaPagina": {
  "Titulo": {
    "label": "New Page"
  }
}
```

**Resultado esperado en app.pt.json** (traducción automática al portugués):
```json
"NuevaPagina": {
  "Titulo": {
    "label": "Nova Página"
  }
}
```

## Ejemplo con sección Common (orden alfabético)

**Usuario:** Añade la clave `Guardar` en la sección `Common` con el texto "Guardar".

Las entradas en `Common` se insertan respetando el orden alfabético de la clave.
Si existen `Cancelar` y `Loading`, la nueva clave `Guardar` se insertará entre ambas.
