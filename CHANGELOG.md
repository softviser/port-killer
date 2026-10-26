# Değişiklik günlüğü

Bu dosyada sürümler arasındaki önemli değişiklikler tutulur. Biçim [Keep a Changelog](https://keepachangelog.com/tr-TR/1.1.0/) önerilerine, sürüm numaraları [Semantic Versioning](https://semver.org/lang/tr/) kurallarına uyar.

## [Yayınlanmadı]

## [1.0.0] - 2026-10-26

İlk sürüm.

### Eklendi

- Sistem tepsisinde çalışan uygulama. Sol tıklayınca liste açılır, sağ tık menüsünde göster, yenile, hakkında ve çıkış var.
- Bu bilgisayarda TCP portlarını dinleyen süreçlerin listesi (IPv4 ve IPv6). Veriler Windows'un `GetExtendedTcpTable` API'sinden okunur, bu yüzden `netstat` çıktısının Windows diline göre değişmesinden etkilenmez.
- Programa göre gruplama: Node.js, Bun, Deno, Python, Java, .NET, PHP, Ruby, Go, Docker, WSL, PostgreSQL, MySQL, MariaDB, Redis, MongoDB, nginx, Apache, Caddy, ngrok, VS Code, Visual Studio ve JetBrains IDE'leri ayrı gruplarda üstte, geri kalan uygulamalar en altta.
- Her satırda süreci sonlandıran düğme. Geliştirme aracı olmayan bir program için önce onay sorulur.
- 1024 altındaki portlar ve Windows'un çalışması için gerekli sistem süreçleri (`System`, `svchost`, `lsass`, `services` vb.) listelenmez, yanlışlıkla kapatılamaz.
- Liste 3 saniyede bir yenilenir; yeni bir port açıldığında Windows bildirimi gösterilir.
- Port, program, PID ve adres üzerinde anında arama.
- Pencere içinde yenile düğmesi ve klavye kısayolları (F5, Ctrl+F, Delete, Esc).
- Windows görüntü diline göre 34 dilde arayüz; Arapça, İbranice ve Farsça için sağdan sola yerleşim.
- Windows'un açık/koyu uygulama modunu ve vurgu rengini izleyen tema. Ayarlar değiştiğinde uygulama yeniden başlatılmadan güncellenir.
- Sürüm, lisans, web sitesi ve kaynak kod bağlantılarını gösteren Hakkında penceresi.

[Yayınlanmadı]: https://github.com/softviser/port-killer/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/softviser/port-killer/releases/tag/v1.0.0
