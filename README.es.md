# Port Killer

[English](README.md) · [Türkçe](README.tr.md) · [Deutsch](README.de.md) · [Français](README.fr.md) · **Español** · [Português](README.pt-BR.md) · [Italiano](README.it.md) · [Русский](README.ru.md) · [简体中文](README.zh-CN.md) · [日本語](README.ja.md) · [한국어](README.ko.md)

Una pequeña aplicación de bandeja para Windows que muestra qué programa escucha en qué puerto de este equipo y finaliza con un clic los servidores de desarrollo bloqueados.

La próxima vez que veas `EADDRINUSE: address already in use :::3000`, en lugar de escribir `Get-NetTCPConnection -LocalPort 3000 | ... | Stop-Process` en PowerShell, haz clic en el icono de la bandeja y pulsa el botón ✕ junto a la fila.

Creado por [Softviser Yazılım](https://softviser.com.tr). Código abierto bajo la licencia MIT.

| Tema claro | Tema oscuro |
|---|---|
| ![Port Killer, tema claro](docs/main-light.png) | ![Port Killer, tema oscuro](docs/main-dark.png) |

## Descarga

Descarga `PortKiller-1.0.0-win-x64.exe` desde la página de [Releases](https://github.com/softviser/port-killer/releases). Es un único archivo: sin instalador y sin necesidad de tener .NET instalado. Requiere Windows 10 u 11 (64 bits).

El ejecutable no está firmado, por lo que Windows SmartScreen puede mostrar una advertencia la primera vez que lo ejecutes. Elige **Más información > Ejecutar de todas formas** para continuar. Puedes verificar la descarga con `SHA256SUMS.txt`:

```powershell
Get-FileHash .\PortKiller-1.0.0-win-x64.exe -Algorithm SHA256
```

## Qué hace

- Agrupa por programa los puertos TCP en escucha (IPv4 e IPv6). Las herramientas de desarrollo como Node.js, Python, .NET, Docker y PostgreSQL tienen sus propios grupos arriba; todo lo demás va a "Otras aplicaciones" al final.
- El botón ✕ al final de cada fila finaliza el proceso, igual que `Stop-Process -Force` en PowerShell. Pide confirmación antes de finalizar cualquier cosa que no sea una herramienta de desarrollo.
- La lista se actualiza cada 3 segundos. Cuando un puerto nuevo empieza a escuchar, recibes una notificación de Windows; al hacer clic en ella se abre la lista.
- El cuadro de búsqueda filtra por puerto, nombre del programa, PID y dirección mientras escribes. Escribir `:3000` o `3000` da el mismo resultado.
- La interfaz sigue el idioma de visualización de Windows (34 idiomas). El tema claro/oscuro y el color de énfasis también provienen de la configuración de Windows, y los cambios se aplican sin reiniciar la aplicación.
- No necesita derechos de administrador. Puede finalizar procesos iniciados con tu propia cuenta de usuario.

![Búsqueda](docs/search-light.png)

### Qué oculta

El objetivo de la aplicación es gestionar los puertos abiertos durante el desarrollo, no apagar lo que mantiene Windows en marcha. Por eso:

- No se muestran los puertos por debajo del 1024 (servicios de Windows como SMB, RPC y HTTP.sys).
- Los procesos del sistema como `System`, `svchost`, `lsass`, `services`, `wininit`, `winlogon`, `csrss`, `smss`, `spoolsv`, Windows Defender y el indexador de búsqueda nunca aparecen en la lista.

La lista de programas que se consideran herramientas de desarrollo es el diccionario `DevTools` de [`Ports.cs`](src/PortKiller/Ports.cs). Si falta una herramienta que usas, añádela ahí.

## Uso

Al iniciar la aplicación, aparece un icono rojo "PK" en la bandeja del sistema.

- **Clic izquierdo:** abre u oculta la lista de puertos.
- **Clic derecho:** menú con Mostrar puertos, Actualizar, Acerca de y Salir.

Cerrar la ventana no cierra la aplicación; solo oculta la lista. Para salir por completo, elige **Salir** en el menú de la bandeja.

| Atajo | Acción |
|---|---|
| F5 | Actualizar la lista |
| Ctrl+F | Ir al cuadro de búsqueda |
| Delete | Finalizar el proceso de la fila seleccionada |
| Esc | Borrar la búsqueda; ocultar la ventana si la búsqueda está vacía |

![Ventana Acerca de](docs/about-light.png)

## Limitaciones conocidas

- Para finalizar un proceso que se ejecuta como administrador (por ejemplo, PostgreSQL instalado como servicio de Windows), Port Killer también debe ejecutarse como administrador. De lo contrario, muestra un mensaje de error.
- Con herramientas que se reinician solas, como `nodemon`, puedes recibir otra notificación de "puerto nuevo" para el mismo puerto si un reinicio coincide con la comprobación de 3 segundos.
- En el tema oscuro, la barra de desplazamiento que aparece cuando la lista no cabe en la ventana sigue siendo clara.
- Solo se muestran los puertos TCP en escucha; los puertos UDP no.

## Compilar desde el código fuente

Requisitos: Windows 10 u 11 y el [SDK de .NET 8](https://dotnet.microsoft.com/download).

```powershell
git clone https://github.com/softviser/port-killer.git
cd port-killer

# Run during development
dotnet run --project src\PortKiller

# Single-file exe that also runs on machines without .NET (artifacts\PortKiller.exe)
dotnet publish src\PortKiller -c Release -r win-x64 --self-contained `
    -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:EnableCompressionInSingleFile=true -p:DebugType=none -o artifacts
```

### Estructura del proyecto

| Archivo | Contenido |
|---|---|
| `Ports.cs` | Lectura de los puertos en escucha, listas de herramientas de desarrollo y procesos del sistema, finalización de procesos |
| `TrayApp.cs` | Icono de la bandeja, menú, comprobaciones periódicas y notificaciones |
| `PortsForm.cs` | Tabla de puertos agrupada, búsqueda y barra superior |
| `AboutForm.cs` | Ventana Acerca de |
| `Strings.cs` | Textos de la interfaz en 34 idiomas |
| `Theme.cs` | Colores basados en el tema claro/oscuro de Windows y el color de énfasis |

### Traducciones

Los textos de la interfaz están en la tabla de [`Strings.cs`](src/PortKiller/Strings.cs), una línea por idioma. Para añadir un idioma, agrega su código (como `"sr"` o `"pt-BR"`) y las 13 cadenas en el mismo orden. Si detectas un error de traducción, no dudes en abrir un issue o un pull request.

## Licencia

[MIT](LICENSE) © 2026 Softviser Yazılım
