# Changelog

Notable changes between releases are recorded in this file. The format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and version numbers follow [Semantic Versioning](https://semver.org/).

## [Unreleased]

## [1.0.2] - 2026-10-26

### Fixed

- The executable now has its own icon in File Explorer and the taskbar instead of the generic Windows one.

### Changed

- English is the default everywhere a language cannot be picked per user: the changelog, release notes, repository description and the file's version details. The README is available in 11 languages and the app follows the Windows display language, as before.
- The README explains that Windows 11 Smart App Control blocks the app because it is not code-signed yet.

## [1.0.1] - 2026-10-26

### Added

- A second, roughly 0.2 MB executable for machines that already have the .NET 8 Desktop Runtime: `PortKiller-1.0.1-win-x64.exe` keeps .NET bundled (about 66 MB), `PortKiller-1.0.1-win-x64-requires-net8.exe` does not.

### Changed

- The download section of the READMEs now describes both files and their requirements.
- Removed an unused theme hook that existed only for testing. No change in behavior.

## [1.0.0] - 2026-10-26

First release.

### Added

- Runs in the system tray. Left click opens the list; the right-click menu has Show, Refresh, About and Exit.
- Lists processes listening on TCP ports on this computer (IPv4 and IPv6). Data comes from the Windows `GetExtendedTcpTable` API, so it does not depend on `netstat` output, which changes with the Windows display language.
- Groups ports by program: Node.js, Bun, Deno, Python, Java, .NET, PHP, Ruby, Go, Docker, WSL, PostgreSQL, MySQL, MariaDB, Redis, MongoDB, nginx, Apache, Caddy, ngrok, VS Code, Visual Studio and JetBrains IDEs get their own groups at the top; other applications go at the bottom.
- A button on each row kills the process. Programs that are not development tools ask for confirmation first.
- Ports below 1024 and system processes Windows needs (`System`, `svchost`, `lsass`, `services` and others) are not listed, so they cannot be killed by accident.
- The list refreshes every 3 seconds, and a Windows notification appears when a new port starts listening.
- Instant search across port, program, PID and address.
- Refresh button in the window and keyboard shortcuts (F5, Ctrl+F, Delete, Esc).
- Interface in 34 languages based on the Windows display language, with right-to-left layout for Arabic, Hebrew and Persian.
- Theme that follows the Windows light/dark app mode and accent color, updating without a restart when the settings change.
- About window with version, license, website and source code links.

[Unreleased]: https://github.com/softviser/port-killer/compare/v1.0.2...HEAD
[1.0.2]: https://github.com/softviser/port-killer/compare/v1.0.1...v1.0.2
[1.0.1]: https://github.com/softviser/port-killer/compare/v1.0.0...v1.0.1
[1.0.0]: https://github.com/softviser/port-killer/releases/tag/v1.0.0
