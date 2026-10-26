# Port Killer

[English](README.md) · [Türkçe](README.tr.md) · [Deutsch](README.de.md) · [Français](README.fr.md) · [Español](README.es.md) · [Português](README.pt-BR.md) · [Italiano](README.it.md) · [Русский](README.ru.md) · [简体中文](README.zh-CN.md) · [日本語](README.ja.md) · **한국어**

이 컴퓨터에서 어떤 프로그램이 어떤 포트를 수신 대기하고 있는지 보여 주고, 멈춰 버린 개발 서버를 클릭 한 번으로 종료해 주는 작은 Windows 트레이 앱입니다.

다음에 `EADDRINUSE: address already in use :::3000` 오류가 보이면, PowerShell에서 `Get-NetTCPConnection -LocalPort 3000 | ... | Stop-Process`를 입력하는 대신 트레이 아이콘을 클릭하고 해당 행 옆의 ✕ 버튼을 누르면 됩니다.

[Softviser Yazılım](https://softviser.com.tr)에서 만들었습니다. MIT 라이선스의 오픈 소스입니다.

| 라이트 테마 | 다크 테마 |
|---|---|
| ![Port Killer, 라이트 테마](docs/main-light.png) | ![Port Killer, 다크 테마](docs/main-dark.png) |

## 다운로드

[Releases](https://github.com/softviser/port-killer/releases) 페이지의 각 릴리스에는 두 가지 빌드가 있습니다. 둘 다 설치가 필요 없는 단일 파일이며 Windows 10 또는 11(64비트)이 필요합니다.

| 파일 | 크기 | 요구 사항 |
|---|---|---|
| `PortKiller-<version>-win-x64.exe` | ~66 MB | 없음, .NET 포함 |
| `PortKiller-<version>-win-x64-requires-net8.exe` | ~0.2 MB | [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) (x64) |

이미 .NET 8이 설치되어 있다면(대부분의 .NET 개발자 PC가 그렇습니다) 작은 파일로 충분합니다. 런타임이 없으면 Windows가 다운로드 링크가 있는 메시지를 표시합니다.

실행 파일에는 코드 서명이 없으므로 처음 실행할 때 Windows SmartScreen이 경고를 표시할 수 있습니다. 이때 **추가 정보 > 실행**을 선택하면 계속할 수 있습니다. 다운로드한 파일은 `SHA256SUMS.txt`와 대조해 확인할 수 있습니다.

```powershell
Get-FileHash .\PortKiller-*.exe -Algorithm SHA256
```

**스마트 앱 컨트롤**이 켜져 있는 Windows 11에서는 서명되지 않은 앱이 바로 차단되며 '실행' 옵션도 없습니다. Port Killer는 아직 코드 서명이 되어 있지 않아 이런 PC에서는 실행되지 않습니다.

## 기능

- 수신 대기 중인 TCP 포트(IPv4 및 IPv6)를 프로그램별로 묶어서 보여 줍니다. Node.js, Python, .NET, Docker, PostgreSQL 같은 개발 도구는 각각 별도 그룹으로 위쪽에 표시되고, 나머지는 모두 맨 아래의 "기타 앱"에 들어갑니다.
- 각 행 끝의 ✕ 버튼은 프로세스를 종료하며, PowerShell의 `Stop-Process -Force`와 동일하게 동작합니다. 개발 도구가 아닌 프로세스를 종료하기 전에는 먼저 확인을 요청합니다.
- 목록은 3초마다 새로 고쳐집니다. 새 포트가 수신 대기를 시작하면 Windows 알림이 표시되며, 알림을 클릭하면 목록이 열립니다.
- 검색창은 입력하는 대로 포트, 프로그램 이름, PID, 주소를 기준으로 걸러 줍니다. `:3000`을 입력하든 `3000`을 입력하든 결과는 같습니다.
- 인터페이스는 Windows 표시 언어를 따릅니다(34개 언어). 라이트/다크 테마와 강조 색도 Windows 설정을 따르며, 변경 사항은 앱을 다시 시작하지 않아도 적용됩니다.
- 관리자 권한이 필요하지 않습니다. 본인 사용자 계정으로 시작된 프로세스를 종료할 수 있습니다.

![검색](docs/search-light.png)

### 숨기는 항목

이 앱의 목적은 개발 중에 열린 포트를 관리하는 것이지, Windows를 돌아가게 하는 요소를 끄는 것이 아닙니다. 그래서 다음과 같이 처리합니다.

- 1024 미만의 포트(SMB, RPC, HTTP.sys 같은 Windows 서비스)는 표시하지 않습니다.
- `System`, `svchost`, `lsass`, `services`, `wininit`, `winlogon`, `csrss`, `smss`, `spoolsv`, Windows Defender, 검색 인덱서 같은 시스템 프로세스는 목록에 나타나지 않습니다.

개발 도구로 취급하는 프로그램 목록은 [`Ports.cs`](src/PortKiller/Ports.cs)의 `DevTools` 딕셔너리입니다. 사용하는 도구가 빠져 있다면 그곳에 추가하세요.

## 사용법

앱을 시작하면 시스템 트레이에 빨간색 "PK" 아이콘이 나타납니다.

- **왼쪽 클릭:** 포트 목록을 열거나 숨깁니다.
- **오른쪽 클릭:** 포트 보기, 새로 고침, 정보, 종료가 있는 메뉴가 열립니다.

창을 닫아도 앱은 종료되지 않고 목록만 숨겨집니다. 완전히 종료하려면 트레이 메뉴에서 **종료**를 선택하세요.

| 단축키 | 동작 |
|---|---|
| F5 | 목록 새로 고침 |
| Ctrl+F | 검색창으로 이동 |
| Delete | 선택한 행의 프로세스 종료 |
| Esc | 검색어 지우기. 검색창이 비어 있으면 창 숨기기 |

![정보 창](docs/about-light.png)

## 알려진 제한 사항

- 관리자 권한으로 실행 중인 프로세스(예: Windows 서비스로 설치된 PostgreSQL)를 종료하려면 Port Killer도 관리자 권한으로 실행해야 합니다. 그렇지 않으면 오류 메시지가 표시됩니다.
- `nodemon`처럼 스스로 재시작하는 도구는 재시작이 3초 간격 검사와 우연히 겹치면 같은 포트에 대해 "새 포트" 알림이 다시 뜰 수 있습니다.
- 다크 테마에서는 목록이 창에 다 들어가지 않을 때 나타나는 스크롤바가 밝은 색으로 남습니다.
- TCP 수신 대기만 표시하며, UDP 포트는 표시하지 않습니다.

## 소스에서 빌드하기

요구 사항: Windows 10 또는 11, 그리고 [.NET 8 SDK](https://dotnet.microsoft.com/download).

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

### 프로젝트 구성

| 파일 | 내용 |
|---|---|
| `Ports.cs` | 수신 대기 포트 읽기, 개발 도구 및 시스템 프로세스 목록, 프로세스 종료 |
| `TrayApp.cs` | 트레이 아이콘, 메뉴, 주기적 검사 및 알림 |
| `PortsForm.cs` | 그룹별 포트 표, 검색, 상단 바 |
| `AboutForm.cs` | 정보 창 |
| `Strings.cs` | 34개 언어의 인터페이스 텍스트 |
| `Theme.cs` | Windows 라이트/다크 테마와 강조 색을 기반으로 한 색상 |

### 번역

인터페이스 텍스트는 [`Strings.cs`](src/PortKiller/Strings.cs)의 표에 있으며, 언어당 한 줄입니다. 언어를 추가하려면 해당 언어 코드(`"sr"`, `"pt-BR"` 등)와 13개의 문자열을 같은 순서로 추가하세요. 번역 오류를 발견하면 언제든지 issue나 pull request를 열어 주세요.

## 라이선스

[MIT](LICENSE) © 2026 Softviser Yazılım
