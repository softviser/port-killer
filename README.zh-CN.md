# Port Killer

[English](README.md) · [Türkçe](README.tr.md) · [Deutsch](README.de.md) · [Français](README.fr.md) · [Español](README.es.md) · [Português](README.pt-BR.md) · [Italiano](README.it.md) · [Русский](README.ru.md) · **简体中文** · [日本語](README.ja.md) · [한국어](README.ko.md)

一个小巧的 Windows 托盘应用，显示这台电脑上哪个程序在监听哪个端口，并且一键结束卡住的开发服务器。

下次再看到 `EADDRINUSE: address already in use :::3000` 时，不用再到 PowerShell 里输入 `Get-NetTCPConnection -LocalPort 3000 | ... | Stop-Process`，点一下托盘图标，再按该行旁边的 ✕ 按钮就行了。

由 [Softviser Yazılım](https://softviser.com.tr) 开发。基于 MIT 许可证开源。

| 浅色主题 | 深色主题 |
|---|---|
| ![Port Killer，浅色主题](docs/main-light.png) | ![Port Killer，深色主题](docs/main-dark.png) |

## 下载

从 [Releases](https://github.com/softviser/port-killer/releases) 页面获取 `PortKiller-1.0.0-win-x64.exe`。它是单个文件：无需安装程序，也不需要安装 .NET。需要 Windows 10 或 11（64 位）。

该可执行文件没有代码签名，所以第一次运行时 Windows SmartScreen 可能会弹出警告。选择 **更多信息 > 仍要运行** 即可继续。你可以对照 `SHA256SUMS.txt` 校验下载的文件：

```powershell
Get-FileHash .\PortKiller-1.0.0-win-x64.exe -Algorithm SHA256
```

## 功能

- 按程序对监听中的 TCP 端口（IPv4 和 IPv6）分组。Node.js、Python、.NET、Docker 和 PostgreSQL 等开发工具各自单独成组，排在最上面；其余的都归入最下面的“其他应用”。
- 每行末尾的 ✕ 按钮会结束对应进程，效果与 PowerShell 中的 `Stop-Process -Force` 相同。对于不是开发工具的进程，结束前会先请你确认。
- 列表每 3 秒刷新一次。有新端口开始监听时，你会收到 Windows 通知；点击通知即可打开列表。
- 搜索框会在你输入时按端口、程序名、PID 和地址进行过滤。输入 `:3000` 或 `3000` 结果相同。
- 界面语言跟随 Windows 显示语言（共 34 种语言）。浅色/深色主题和强调色同样取自 Windows 设置，更改后无需重启应用即可生效。
- 不需要管理员权限。它可以结束在你自己的用户账户下启动的进程。

![搜索](docs/search-light.png)

### 隐藏的内容

这个应用的目的是管理开发期间打开的端口，而不是关掉维持 Windows 运行的东西。因此：

- 1024 以下的端口（SMB、RPC、HTTP.sys 等 Windows 服务）不会列出。
- `System`、`svchost`、`lsass`、`services`、`wininit`、`winlogon`、`csrss`、`smss`、`spoolsv`、Windows Defender 和搜索索引器等系统进程不会出现在列表中。

被当作开发工具的程序列表是 [`Ports.cs`](src/PortKiller/Ports.cs) 中的 `DevTools` 字典。如果你用的工具不在其中，请在那里添加。

## 使用方法

启动应用后，系统托盘中会出现一个红色的“PK”图标。

- **左键单击：** 打开或隐藏端口列表。
- **右键单击：** 弹出菜单，包含显示端口、刷新、关于和退出。

关闭窗口并不会退出应用，只是隐藏列表。要完全退出，请在托盘菜单中选择 **退出**。

| 快捷键 | 操作 |
|---|---|
| F5 | 刷新列表 |
| Ctrl+F | 跳到搜索框 |
| Delete | 结束所选行中的进程 |
| Esc | 清除搜索；搜索框为空时隐藏窗口 |

![关于窗口](docs/about-light.png)

## 已知限制

- 要结束以管理员身份运行的进程（例如作为 Windows 服务安装的 PostgreSQL），Port Killer 也必须以管理员身份运行，否则会显示错误信息。
- 对于 `nodemon` 这类会自动重启的工具，如果重启恰好与 3 秒一次的检查同时发生，同一个端口可能会再次触发“新端口”通知。
- 在深色主题下，列表超出窗口高度时出现的滚动条仍然是浅色的。
- 只显示 TCP 监听端口，不显示 UDP 端口。

## 从源码构建

要求：Windows 10 或 11，以及 [.NET 8 SDK](https://dotnet.microsoft.com/download)。

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

### 项目结构

| 文件 | 内容 |
|---|---|
| `Ports.cs` | 读取监听端口、开发工具和系统进程列表、结束进程 |
| `TrayApp.cs` | 托盘图标、菜单、定期检查和通知 |
| `PortsForm.cs` | 分组的端口表格、搜索和顶部栏 |
| `AboutForm.cs` | 关于窗口 |
| `Strings.cs` | 34 种语言的界面文本 |
| `Theme.cs` | 基于 Windows 浅色/深色主题和强调色的配色 |

### 翻译

界面文本位于 [`Strings.cs`](src/PortKiller/Strings.cs) 的表格中，每种语言一行。要添加语言，请按相同顺序加入它的语言代码（如 `"sr"` 或 `"pt-BR"`）和 13 条字符串。如果你发现翻译有误，欢迎提交 issue 或 pull request。

## 许可证

[MIT](LICENSE) © 2026 Softviser Yazılım
