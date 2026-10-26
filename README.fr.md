# Port Killer

[English](README.md) · [Türkçe](README.tr.md) · [Deutsch](README.de.md) · **Français** · [Español](README.es.md) · [Português](README.pt-BR.md) · [Italiano](README.it.md) · [Русский](README.ru.md) · [简体中文](README.zh-CN.md) · [日本語](README.ja.md) · [한국어](README.ko.md)

Une petite application Windows de la zone de notification qui montre quel programme écoute sur quel port de cet ordinateur, et termine les serveurs de développement bloqués en un clic.

La prochaine fois que vous verrez `EADDRINUSE: address already in use :::3000`, au lieu de taper `Get-NetTCPConnection -LocalPort 3000 | ... | Stop-Process` dans PowerShell, cliquez sur l'icône de la zone de notification, puis sur le bouton ✕ à côté de la ligne.

Développé par [Softviser Yazılım](https://softviser.com.tr). Open source, sous licence MIT.

| Thème clair | Thème sombre |
|---|---|
| ![Port Killer, thème clair](docs/main-light.png) | ![Port Killer, thème sombre](docs/main-dark.png) |

## Téléchargement

Chaque version publiée sur la page [Releases](https://github.com/softviser/port-killer/releases) contient deux builds. Les deux tiennent en un seul fichier, sans installateur, et nécessitent Windows 10 ou 11 (64 bits).

| Fichier | Taille | Prérequis |
|---|---|---|
| `PortKiller-<version>-win-x64.exe` | ~66 MB | Aucun ; .NET est inclus |
| `PortKiller-<version>-win-x64-requires-net8.exe` | ~0.2 MB | [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) (x64) |

Si .NET 8 est déjà installé, comme sur la plupart des machines de développeurs .NET, le petit fichier suffit. Si le runtime est absent, Windows affiche un message avec un lien pour le télécharger.

L'exécutable n'est pas signé, donc Windows SmartScreen peut afficher un avertissement au premier lancement. Choisissez **Informations complémentaires > Exécuter quand même** pour continuer. Vous pouvez vérifier le téléchargement avec `SHA256SUMS.txt` :

```powershell
Get-FileHash .\PortKiller-*.exe -Algorithm SHA256
```

## Fonctionnalités

- Regroupe par programme les ports TCP en écoute (IPv4 et IPv6). Les outils de développement comme Node.js, Python, .NET, Docker et PostgreSQL ont leurs propres groupes en haut ; tout le reste va dans « Autres applications » en bas.
- Le bouton ✕ au bout de chaque ligne termine le processus, comme `Stop-Process -Force` dans PowerShell. Une confirmation est demandée avant de terminer tout ce qui n'est pas un outil de développement.
- La liste s'actualise toutes les 3 secondes. Quand un nouveau port se met à écouter, vous recevez une notification Windows ; un clic dessus ouvre la liste.
- La zone de recherche filtre par port, nom de programme, PID et adresse pendant la saisie. Taper `:3000` ou `3000` donne le même résultat.
- L'interface suit la langue d'affichage de Windows (34 langues). Le thème clair/sombre et la couleur d'accentuation viennent aussi des paramètres Windows, et les changements s'appliquent sans redémarrer l'application.
- Aucun droit d'administrateur n'est nécessaire. Elle peut terminer les processus lancés sous votre propre compte utilisateur.

![Recherche](docs/search-light.png)

### Ce qui est masqué

Le but de l'application est de gérer les ports ouverts pendant le développement, pas d'arrêter ce qui fait tourner Windows. Donc :

- Les ports inférieurs à 1024 (services Windows comme SMB, RPC et HTTP.sys) ne sont pas listés.
- Les processus système comme `System`, `svchost`, `lsass`, `services`, `wininit`, `winlogon`, `csrss`, `smss`, `spoolsv`, Windows Defender et l'indexeur de recherche n'apparaissent jamais dans la liste.

La liste des programmes considérés comme outils de développement est le dictionnaire `DevTools` dans [`Ports.cs`](src/PortKiller/Ports.cs). Si un outil que vous utilisez manque, ajoutez-le là.

## Utilisation

Au démarrage de l'application, une icône rouge « PK » apparaît dans la zone de notification.

- **Clic gauche :** ouvre ou masque la liste des ports.
- **Clic droit :** menu avec Afficher les ports, Actualiser, À propos et Quitter.

Fermer la fenêtre ne quitte pas l'application ; cela masque seulement la liste. Pour quitter complètement, choisissez **Quitter** dans le menu de la zone de notification.

| Raccourci | Action |
|---|---|
| F5 | Actualiser la liste |
| Ctrl+F | Aller à la zone de recherche |
| Delete | Terminer le processus de la ligne sélectionnée |
| Esc | Effacer la recherche ; masquer la fenêtre si la recherche est vide |

![Fenêtre À propos](docs/about-light.png)

## Limitations connues

- Pour terminer un processus exécuté en tant qu'administrateur (par exemple PostgreSQL installé comme service Windows), Port Killer doit lui aussi s'exécuter en tant qu'administrateur. Sinon, un message d'erreur s'affiche.
- Avec les outils qui se relancent eux-mêmes, comme `nodemon`, vous pouvez recevoir une nouvelle notification de « nouveau port » pour le même port si un redémarrage tombe au moment de la vérification toutes les 3 secondes.
- Dans le thème sombre, la barre de défilement qui apparaît quand la liste ne tient pas dans la fenêtre reste claire.
- Seuls les ports TCP en écoute sont affichés ; les ports UDP ne le sont pas.

## Compiler depuis les sources

Prérequis : Windows 10 ou 11 et le [SDK .NET 8](https://dotnet.microsoft.com/download).

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

### Structure du projet

| Fichier | Contenu |
|---|---|
| `Ports.cs` | Lecture des ports en écoute, listes des outils de développement et des processus système, fin de processus |
| `TrayApp.cs` | Icône de la zone de notification, menu, vérifications périodiques et notifications |
| `PortsForm.cs` | Tableau de ports groupé, recherche et barre supérieure |
| `AboutForm.cs` | Fenêtre À propos |
| `Strings.cs` | Textes de l'interface en 34 langues |
| `Theme.cs` | Couleurs basées sur le thème clair/sombre de Windows et la couleur d'accentuation |

### Traductions

Les textes de l'interface se trouvent dans le tableau de [`Strings.cs`](src/PortKiller/Strings.cs), une ligne par langue. Pour ajouter une langue, ajoutez son code (comme `"sr"` ou `"pt-BR"`) et les 13 chaînes dans le même ordre. Si vous repérez une erreur de traduction, n'hésitez pas à ouvrir une issue ou une pull request.

## Licence

[MIT](LICENSE) © 2026 Softviser Yazılım
