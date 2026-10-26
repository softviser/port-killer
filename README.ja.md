# Port Killer

[English](README.md) · [Türkçe](README.tr.md) · [Deutsch](README.de.md) · [Français](README.fr.md) · [Español](README.es.md) · [Português](README.pt-BR.md) · [Italiano](README.it.md) · [Русский](README.ru.md) · [简体中文](README.zh-CN.md) · **日本語** · [한국어](README.ko.md)

このコンピューターでどのプログラムがどのポートで待ち受けているかを表示し、止まってしまった開発サーバーをワンクリックで終了できる、小さな Windows タスクトレイアプリです。

次に `EADDRINUSE: address already in use :::3000` と表示されたら、PowerShell で `Get-NetTCPConnection -LocalPort 3000 | ... | Stop-Process` と入力する代わりに、トレイアイコンをクリックして、その行の横にある ✕ ボタンを押すだけです。

[Softviser Yazılım](https://softviser.com.tr) が開発しています。MIT ライセンスのオープンソースです。

| ライトテーマ | ダークテーマ |
|---|---|
| ![Port Killer、ライトテーマ](docs/main-light.png) | ![Port Killer、ダークテーマ](docs/main-dark.png) |

## ダウンロード

[Releases](https://github.com/softviser/port-killer/releases) ページの各リリースには 2 種類のビルドがあります。どちらも単一ファイルでインストーラーは不要で、Windows 10 または 11（64 ビット）が必要です。

| ファイル | サイズ | 必要なもの |
|---|---|---|
| `PortKiller-<version>-win-x64.exe` | ~66 MB | なし（.NET を同梱） |
| `PortKiller-<version>-win-x64-requires-net8.exe` | ~0.2 MB | [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) (x64) |

すでに .NET 8 がインストールされている場合（多くの .NET 開発者の環境がそうです）は、小さいほうで十分です。ランタイムがない場合は、Windows がダウンロード リンク付きのメッセージを表示します。

実行ファイルにはコード署名がないため、初回起動時に Windows SmartScreen が警告を表示することがあります。その場合は **詳細情報 > 実行** を選んで続行してください。ダウンロードしたファイルは `SHA256SUMS.txt` と照合して確認できます。

```powershell
Get-FileHash .\PortKiller-*.exe -Algorithm SHA256
```

## できること

- 待ち受け中の TCP ポート（IPv4 と IPv6）をプログラムごとにグループ化します。Node.js、Python、.NET、Docker、PostgreSQL などの開発ツールは専用のグループとして上に表示され、それ以外はすべて下の「その他のアプリ」にまとめられます。
- 各行の右端にある ✕ ボタンでプロセスを終了します。PowerShell の `Stop-Process -Force` と同じ動作です。開発ツール以外を終了する場合は、事前に確認が表示されます。
- 一覧は 3 秒ごとに更新されます。新しいポートが待ち受けを始めると Windows の通知が表示され、クリックすると一覧が開きます。
- 検索ボックスは、入力に合わせてポート、プログラム名、PID、アドレスで絞り込みます。`:3000` と `3000` はどちらも同じ結果になります。
- 表示言語は Windows の表示言語に従います（34 言語）。ライト/ダークテーマとアクセントカラーも Windows の設定から取得し、変更はアプリを再起動しなくても反映されます。
- 管理者権限は不要です。自分のユーザーアカウントで起動したプロセスを終了できます。

![検索](docs/search-light.png)

### 非表示にするもの

このアプリの目的は、開発中に開いたポートを管理することであり、Windows の動作を支えているものを終了させることではありません。そのため、次のようにしています。

- 1024 未満のポート（SMB、RPC、HTTP.sys などの Windows サービス）は表示されません。
- `System`、`svchost`、`lsass`、`services`、`wininit`、`winlogon`、`csrss`、`smss`、`spoolsv`、Windows Defender、検索インデクサーなどのシステムプロセスは一覧に表示されません。

開発ツールとして扱うプログラムの一覧は、[`Ports.cs`](src/PortKiller/Ports.cs) の `DevTools` ディクショナリです。使っているツールが含まれていない場合は、そこに追加してください。

## 使い方

アプリを起動すると、システムトレイに赤い「PK」アイコンが表示されます。

- **左クリック:** ポート一覧を開く、または隠します。
- **右クリック:** ポートを表示、更新、バージョン情報、終了のメニューを表示します。

ウィンドウを閉じてもアプリは終了せず、一覧が隠れるだけです。完全に終了するには、トレイメニューから **終了** を選んでください。

| ショートカット | 動作 |
|---|---|
| F5 | 一覧を更新 |
| Ctrl+F | 検索ボックスに移動 |
| Delete | 選択した行のプロセスを終了 |
| Esc | 検索をクリア。検索が空ならウィンドウを隠す |

![バージョン情報ウィンドウ](docs/about-light.png)

## 既知の制限

- 管理者として実行されているプロセス（たとえば Windows サービスとしてインストールされた PostgreSQL）を終了するには、Port Killer も管理者として実行する必要があります。そうしないと、エラーメッセージが表示されます。
- `nodemon` のように自動で再起動するツールでは、再起動がたまたま 3 秒ごとのチェックと重なると、同じポートに対して再び「新しいポート」の通知が出ることがあります。
- ダークテーマでは、一覧がウィンドウに収まらないときに表示されるスクロールバーが明るい色のままです。
- 表示されるのは TCP の待ち受けのみで、UDP ポートは表示されません。

## ソースからのビルド

必要なもの: Windows 10 または 11 と [.NET 8 SDK](https://dotnet.microsoft.com/download)。

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

### プロジェクト構成

| ファイル | 内容 |
|---|---|
| `Ports.cs` | 待ち受けポートの取得、開発ツールとシステムプロセスの一覧、プロセスの終了 |
| `TrayApp.cs` | トレイアイコン、メニュー、定期チェック、通知 |
| `PortsForm.cs` | グループ化されたポートの表、検索、トップバー |
| `AboutForm.cs` | バージョン情報ウィンドウ |
| `Strings.cs` | 34 言語分の画面テキスト |
| `Theme.cs` | Windows のライト/ダークテーマとアクセントカラーに基づく配色 |

### 翻訳

画面のテキストは [`Strings.cs`](src/PortKiller/Strings.cs) の表にあり、1 言語につき 1 行です。言語を追加するには、その言語コード（`"sr"` や `"pt-BR"` など）と 13 個の文字列を、同じ順序で追加してください。翻訳の間違いを見つけたら、お気軽に issue または pull request を送ってください。

## ライセンス

[MIT](LICENSE) © 2026 Softviser Yazılım
