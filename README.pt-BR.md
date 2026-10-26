# Port Killer

[English](README.md) · [Türkçe](README.tr.md) · [Deutsch](README.de.md) · [Français](README.fr.md) · [Español](README.es.md) · **Português** · [Italiano](README.it.md) · [Русский](README.ru.md) · [简体中文](README.zh-CN.md) · [日本語](README.ja.md) · [한국어](README.ko.md)

Um pequeno aplicativo de bandeja do Windows que mostra qual programa está escutando em qual porta neste computador e encerra servidores de desenvolvimento travados com um clique.

Da próxima vez que aparecer `EADDRINUSE: address already in use :::3000`, em vez de digitar `Get-NetTCPConnection -LocalPort 3000 | ... | Stop-Process` no PowerShell, clique no ícone da bandeja e pressione o botão ✕ ao lado da linha.

Criado pela [Softviser Yazılım](https://softviser.com.tr). Código aberto sob a licença MIT.

| Tema claro | Tema escuro |
|---|---|
| ![Port Killer, tema claro](docs/main-light.png) | ![Port Killer, tema escuro](docs/main-dark.png) |

## Download

Cada versão na página de [Releases](https://github.com/softviser/port-killer/releases) traz dois builds. Os dois são um único arquivo, sem instalador, e exigem Windows 10 ou 11 (64 bits).

| Arquivo | Tamanho | Requisito |
|---|---|---|
| `PortKiller-<version>-win-x64.exe` | ~66 MB | Nenhum; o .NET vem embutido |
| `PortKiller-<version>-win-x64-requires-net8.exe` | ~0.2 MB | [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) (x64) |

Se o .NET 8 já estiver instalado, como na maioria das máquinas de desenvolvedores .NET, o arquivo pequeno basta. Se o runtime estiver faltando, o Windows mostra uma mensagem com um link para baixá-lo.

O executável não é assinado digitalmente, então o Windows SmartScreen pode exibir um aviso na primeira execução. Escolha **Mais informações > Executar assim mesmo** para continuar. Você pode conferir o download com o `SHA256SUMS.txt`:

```powershell
Get-FileHash .\PortKiller-*.exe -Algorithm SHA256
```

No Windows 11 com o **Controle Inteligente de Aplicativos** ativado, aplicativos sem assinatura são bloqueados diretamente e não há a opção "Executar assim mesmo". O Port Killer ainda não tem assinatura de código, então não vai abrir nessas máquinas.

## O que ele faz

- Agrupa por programa as portas TCP em escuta (IPv4 e IPv6). Ferramentas de desenvolvimento como Node.js, Python, .NET, Docker e PostgreSQL têm grupos próprios no topo; todo o resto fica em "Outros aplicativos", no final.
- O botão ✕ no fim de cada linha encerra o processo, como `Stop-Process -Force` no PowerShell. Antes de encerrar qualquer coisa que não seja uma ferramenta de desenvolvimento, ele pede confirmação.
- A lista é atualizada a cada 3 segundos. Quando uma nova porta começa a escutar, você recebe uma notificação do Windows; ao clicar nela, a lista é aberta.
- A caixa de pesquisa filtra por porta, nome do programa, PID e endereço enquanto você digita. Digitar `:3000` ou `3000` dá o mesmo resultado.
- A interface segue o idioma de exibição do Windows (34 idiomas). O tema claro/escuro e a cor de destaque também vêm das configurações do Windows, e as mudanças são aplicadas sem reiniciar o aplicativo.
- Não precisa de direitos de administrador. Ele consegue encerrar processos iniciados sob a sua própria conta de usuário.

![Pesquisa](docs/search-light.png)

### O que ele oculta

A ideia do aplicativo é gerenciar as portas abertas durante o desenvolvimento, e não desligar o que mantém o Windows funcionando. Por isso:

- Portas abaixo de 1024 (serviços do Windows como SMB, RPC e HTTP.sys) não são listadas.
- Processos do sistema como `System`, `svchost`, `lsass`, `services`, `wininit`, `winlogon`, `csrss`, `smss`, `spoolsv`, o Windows Defender e o indexador de pesquisa nunca aparecem na lista.

A lista de programas tratados como ferramentas de desenvolvimento é o dicionário `DevTools` em [`Ports.cs`](src/PortKiller/Ports.cs). Se faltar uma ferramenta que você usa, adicione-a ali.

## Uso

Ao iniciar o aplicativo, um ícone vermelho "PK" aparece na bandeja do sistema.

- **Clique esquerdo:** abre ou oculta a lista de portas.
- **Clique direito:** menu com Mostrar portas, Atualizar, Sobre e Sair.

Fechar a janela não encerra o aplicativo; ela apenas oculta a lista. Para sair de vez, escolha **Sair** no menu da bandeja.

| Atalho | Ação |
|---|---|
| F5 | Atualizar a lista |
| Ctrl+F | Ir para a caixa de pesquisa |
| Delete | Encerrar o processo da linha selecionada |
| Esc | Limpar a pesquisa; ocultar a janela se a pesquisa estiver vazia |

![Janela Sobre](docs/about-light.png)

## Limitações conhecidas

- Para encerrar um processo que roda como administrador (por exemplo, o PostgreSQL instalado como serviço do Windows), o Port Killer também precisa ser executado como administrador. Caso contrário, ele mostra uma mensagem de erro.
- Com ferramentas que se reiniciam sozinhas, como `nodemon`, você pode receber outra notificação de "nova porta" para a mesma porta se uma reinicialização coincidir com a verificação de 3 segundos.
- No tema escuro, a barra de rolagem que aparece quando a lista não cabe na janela continua clara.
- Apenas listeners TCP são exibidos; portas UDP não.

## Compilando a partir do código-fonte

Requisitos: Windows 10 ou 11 e o [.NET 8 SDK](https://dotnet.microsoft.com/download).

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

### Estrutura do projeto

| Arquivo | Conteúdo |
|---|---|
| `Ports.cs` | Leitura das portas em escuta, listas de ferramentas de desenvolvimento e de processos do sistema, encerramento de processos |
| `TrayApp.cs` | Ícone da bandeja, menu, verificações periódicas e notificações |
| `PortsForm.cs` | Tabela de portas agrupada, pesquisa e barra superior |
| `AboutForm.cs` | Janela Sobre |
| `Strings.cs` | Textos da interface em 34 idiomas |
| `Theme.cs` | Cores baseadas no tema claro/escuro e na cor de destaque do Windows |

### Traduções

Os textos da interface ficam na tabela em [`Strings.cs`](src/PortKiller/Strings.cs), uma linha por idioma. Para adicionar um idioma, inclua o código dele (como `"sr"` ou `"pt-BR"`) e as 13 strings na mesma ordem. Se encontrar algum erro de tradução, fique à vontade para abrir uma issue ou um pull request.

## Licença

[MIT](LICENSE) © 2026 Softviser Yazılım
