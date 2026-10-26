# Port Killer

[English](README.md) · [Türkçe](README.tr.md) · **Deutsch** · [Français](README.fr.md) · [Español](README.es.md) · [Português](README.pt-BR.md) · [Italiano](README.it.md) · [Русский](README.ru.md) · [简体中文](README.zh-CN.md) · [日本語](README.ja.md) · [한국어](README.ko.md)

Eine kleine Windows-Tray-App, die zeigt, welches Programm auf diesem Computer auf welchem Port lauscht, und hängengebliebene Entwicklungsserver mit einem Klick beendet.

Wenn Sie das nächste Mal `EADDRINUSE: address already in use :::3000` sehen, müssen Sie nicht mehr `Get-NetTCPConnection -LocalPort 3000 | ... | Stop-Process` in PowerShell tippen. Klicken Sie stattdessen auf das Tray-Symbol und dann auf die Schaltfläche ✕ neben der Zeile.

Entwickelt von [Softviser Yazılım](https://softviser.com.tr). Open Source unter der MIT-Lizenz.

| Helles Design | Dunkles Design |
|---|---|
| ![Port Killer, helles Design](docs/main-light.png) | ![Port Killer, dunkles Design](docs/main-dark.png) |

## Download

Jede Version auf der Seite [Releases](https://github.com/softviser/port-killer/releases) enthält zwei Builds. Beide bestehen aus einer einzigen Datei ohne Installer und benötigen Windows 10 oder 11 (64 Bit).

| Datei | Größe | Voraussetzung |
|---|---|---|
| `PortKiller-<version>-win-x64.exe` | ~66 MB | Keine; .NET ist enthalten |
| `PortKiller-<version>-win-x64-requires-net8.exe` | ~0.2 MB | [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) (x64) |

Wenn .NET 8 bereits installiert ist, wie auf den meisten Rechnern von .NET-Entwicklern, reicht die kleine Datei. Fehlt die Runtime, zeigt Windows einen Hinweis mit einem Download-Link an.

Die ausführbare Datei ist nicht codesigniert, daher warnt Windows SmartScreen beim ersten Start möglicherweise. Wählen Sie **Weitere Informationen > Trotzdem ausführen**, um fortzufahren. Den Download können Sie mit `SHA256SUMS.txt` überprüfen:

```powershell
Get-FileHash .\PortKiller-*.exe -Algorithm SHA256
```

Unter Windows 11 mit aktivierter **Intelligenter App-Steuerung** werden unsignierte Apps direkt blockiert, und die Option „Trotzdem ausführen“ gibt es nicht. Port Killer ist noch nicht codesigniert und startet auf solchen Rechnern daher nicht.

## Funktionen

- Gruppiert lauschende TCP-Ports (IPv4 und IPv6) nach Programm. Entwicklungswerkzeuge wie Node.js, Python, .NET, Docker und PostgreSQL erhalten eigene Gruppen am Anfang der Liste; alles andere landet unten unter "Andere Anwendungen".
- Die Schaltfläche ✕ am Ende jeder Zeile beendet den Prozess, genau wie `Stop-Process -Force` in PowerShell. Bei allem, was kein Entwicklungswerkzeug ist, wird vorher um Bestätigung gebeten.
- Die Liste wird alle 3 Sekunden aktualisiert. Sobald ein neuer Port zu lauschen beginnt, erscheint eine Windows-Benachrichtigung; ein Klick darauf öffnet die Liste.
- Das Suchfeld filtert schon während der Eingabe nach Port, Programmname, PID und Adresse. `:3000` und `3000` liefern dasselbe Ergebnis.
- Die Oberfläche folgt der Anzeigesprache von Windows (34 Sprachen). Helles/dunkles Design und Akzentfarbe stammen ebenfalls aus den Windows-Einstellungen, und Änderungen werden ohne Neustart der App übernommen.
- Benötigt keine Administratorrechte. Es lassen sich Prozesse beenden, die unter Ihrem eigenen Benutzerkonto gestartet wurden.

![Suche](docs/search-light.png)

### Was ausgeblendet wird

Die App soll Ports verwalten, die bei der Entwicklung geöffnet werden, und nicht Dinge beenden, die Windows am Laufen halten. Deshalb gilt:

- Ports unter 1024 (Windows-Dienste wie SMB, RPC und HTTP.sys) werden nicht aufgelistet.
- Systemprozesse wie `System`, `svchost`, `lsass`, `services`, `wininit`, `winlogon`, `csrss`, `smss`, `spoolsv`, Windows Defender und der Suchindexer erscheinen nie in der Liste.

Welche Programme als Entwicklungswerkzeuge gelten, steht im Dictionary `DevTools` in [`Ports.cs`](src/PortKiller/Ports.cs). Fehlt ein Werkzeug, das Sie verwenden, tragen Sie es dort ein.

## Verwendung

Nach dem Start erscheint ein rotes "PK"-Symbol im Infobereich der Taskleiste.

- **Linksklick:** öffnet oder versteckt die Portliste.
- **Rechtsklick:** Menü mit Ports anzeigen, Aktualisieren, Info und Beenden.

Das Schließen des Fensters beendet die App nicht, sondern versteckt nur die Liste. Um die App ganz zu beenden, wählen Sie im Tray-Menü **Beenden**.

| Tastenkürzel | Aktion |
|---|---|
| F5 | Liste aktualisieren |
| Ctrl+F | Zum Suchfeld springen |
| Delete | Prozess der ausgewählten Zeile beenden |
| Esc | Suche leeren; Fenster verstecken, wenn die Suche leer ist |

![Info-Fenster](docs/about-light.png)

## Bekannte Einschränkungen

- Um einen Prozess zu beenden, der als Administrator läuft (zum Beispiel PostgreSQL als Windows-Dienst), muss auch Port Killer als Administrator laufen. Andernfalls erscheint eine Fehlermeldung.
- Bei Werkzeugen, die sich selbst neu starten, etwa `nodemon`, kann für denselben Port erneut eine Benachrichtigung über einen neuen Port erscheinen, wenn ein Neustart zufällig mit der 3-Sekunden-Prüfung zusammenfällt.
- Im dunklen Design bleibt die Bildlaufleiste, die erscheint, wenn die Liste nicht ins Fenster passt, hell.
- Es werden nur TCP-Listener angezeigt, keine UDP-Ports.

## Aus dem Quellcode bauen

Voraussetzungen: Windows 10 oder 11 und das [.NET 8 SDK](https://dotnet.microsoft.com/download).

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

### Projektstruktur

| Datei | Inhalt |
|---|---|
| `Ports.cs` | Lesen der lauschenden Ports, Listen der Entwicklungswerkzeuge und Systemprozesse, Beenden von Prozessen |
| `TrayApp.cs` | Tray-Symbol, Menü, regelmäßige Prüfungen und Benachrichtigungen |
| `PortsForm.cs` | Gruppierte Porttabelle, Suche und obere Leiste |
| `AboutForm.cs` | Info-Fenster |
| `Strings.cs` | Oberflächentexte in 34 Sprachen |
| `Theme.cs` | Farben auf Basis des hellen/dunklen Windows-Designs und der Akzentfarbe |

### Übersetzungen

Die Oberflächentexte stehen in der Tabelle in [`Strings.cs`](src/PortKiller/Strings.cs), eine Zeile pro Sprache. Um eine Sprache hinzuzufügen, tragen Sie ihren Code (etwa `"sr"` oder `"pt-BR"`) und die 13 Texte in derselben Reihenfolge ein. Wenn Ihnen ein Übersetzungsfehler auffällt, eröffnen Sie gern ein Issue oder einen Pull Request.

## Lizenz

[MIT](LICENSE) © 2026 Softviser Yazılım
