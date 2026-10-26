# Port Killer

**English** · [Türkçe](README.tr.md) · [Deutsch](README.de.md) · [Français](README.fr.md) · [Español](README.es.md) · [Português](README.pt-BR.md) · [Italiano](README.it.md) · [Русский](README.ru.md) · [简体中文](README.zh-CN.md) · [日本語](README.ja.md) · [한국어](README.ko.md)

A small Windows tray app that shows which program is listening on which port on this computer, and kills stuck development servers with one click.

Next time you see `EADDRINUSE: address already in use :::3000`, instead of typing `Get-NetTCPConnection -LocalPort 3000 | ... | Stop-Process` in PowerShell, click the tray icon and press the ✕ button next to the row.

Made by [Softviser Yazılım](https://softviser.com.tr). Open source under the MIT license.

| Light theme | Dark theme |
|---|---|
| ![Port Killer, light theme](docs/main-light.png) | ![Port Killer, dark theme](docs/main-dark.png) |

## Download

Each release on the [Releases](https://github.com/softviser/port-killer/releases) page has two builds. Both are a single file with no installer, and both need Windows 10 or 11 (64-bit).

| File | Size | Requirement |
|---|---|---|
| `PortKiller-<version>-win-x64.exe` | ~66 MB | Nothing else; .NET is bundled |
| `PortKiller-<version>-win-x64-requires-net8.exe` | ~0.2 MB | [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) (x64) |

If .NET 8 is already installed, as on most .NET developer machines, the small one is enough. If the runtime is missing, Windows shows a message with a link to download it.

The executable is not code-signed, so Windows SmartScreen may warn you the first time you run it. Choose **More info > Run anyway** to continue. You can verify the download against `SHA256SUMS.txt`:

```powershell
Get-FileHash .\PortKiller-*.exe -Algorithm SHA256
```

## What it does

- Groups listening TCP ports (IPv4 and IPv6) by program. Development tools such as Node.js, Python, .NET, Docker and PostgreSQL get their own groups at the top; everything else goes into "Other applications" at the bottom.
- The ✕ button at the end of each row kills the process, the same as `Stop-Process -Force` in PowerShell. It asks for confirmation before killing anything that is not a development tool.
- The list refreshes every 3 seconds. When a new port starts listening, you get a Windows notification; clicking it opens the list.
- The search box filters by port, program name, PID and address as you type. Typing `:3000` or `3000` gives the same result.
- The interface follows the Windows display language (34 languages). Light/dark theme and accent color also come from your Windows settings, and changes apply without restarting the app.
- Needs no administrator rights. It can kill processes started under your own user account.

![Search](docs/search-light.png)

### What it hides

The point of the app is to manage ports opened during development, not to shut down things that keep Windows running. So:

- Ports below 1024 (Windows services such as SMB, RPC and HTTP.sys) are not listed.
- System processes such as `System`, `svchost`, `lsass`, `services`, `wininit`, `winlogon`, `csrss`, `smss`, `spoolsv`, Windows Defender and the search indexer never appear in the list.

The list of programs treated as development tools is the `DevTools` dictionary in [`Ports.cs`](src/PortKiller/Ports.cs). If a tool you use is missing, add it there.

## Usage

When you start the app, a red "PK" icon appears in the system tray.

- **Left click:** opens or hides the port list.
- **Right click:** menu with Show, Refresh, About and Exit.

Closing the window does not quit the app; it only hides the list. To quit completely, choose **Exit** from the tray menu.

| Shortcut | Action |
|---|---|
| F5 | Refresh the list |
| Ctrl+F | Jump to the search box |
| Delete | Kill the process in the selected row |
| Esc | Clear the search; hide the window if the search is empty |

![About window](docs/about-light.png)

## Known limitations

- To kill a process running as administrator (for example PostgreSQL installed as a Windows service), Port Killer must also run as administrator. Otherwise it shows an error message.
- With tools that restart themselves, such as `nodemon`, you may get another "new port" notification for the same port if a restart happens to coincide with the 3-second check.
- In the dark theme, the scrollbar that appears when the list does not fit the window stays light.
- Only TCP listeners are shown; UDP ports are not.

## Building from source

Requirements: Windows 10 or 11 and the [.NET 8 SDK](https://dotnet.microsoft.com/download).

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

### Project layout

| File | Contents |
|---|---|
| `Ports.cs` | Reading listening ports, development tool and system process lists, killing processes |
| `TrayApp.cs` | Tray icon, menu, periodic checks and notifications |
| `PortsForm.cs` | Grouped port table, search and top bar |
| `AboutForm.cs` | About window |
| `Strings.cs` | Interface text in 34 languages |
| `Theme.cs` | Colors based on the Windows light/dark theme and accent color |

### Translations

Interface text lives in the table in [`Strings.cs`](src/PortKiller/Strings.cs), one line per language. To add a language, add its code (such as `"sr"` or `"pt-BR"`) and the 13 strings in the same order. If you spot a translation mistake, feel free to open an issue or a pull request.

## License

[MIT](LICENSE) © 2026 Softviser Yazılım
