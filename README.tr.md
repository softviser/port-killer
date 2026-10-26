# Port Killer

[English](README.md) · **Türkçe** · [Deutsch](README.de.md) · [Français](README.fr.md) · [Español](README.es.md) · [Português](README.pt-BR.md) · [Italiano](README.it.md) · [Русский](README.ru.md) · [简体中文](README.zh-CN.md) · [日本語](README.ja.md) · [한국어](README.ko.md)

Bu bilgisayarda hangi programın hangi portu dinlediğini gösteren ve takılı kalan geliştirme sunucularını tek tıkla kapatan küçük bir Windows sistem tepsisi uygulaması.

"`EADDRINUSE: address already in use :::3000`" hatasını gördüğünüzde PowerShell'de `Get-NetTCPConnection -LocalPort 3000 | ... | Stop-Process` yazmak yerine tepsi simgesine tıklayıp satırın yanındaki ✕ düğmesine basmanız yeterli.

Geliştirici: [Softviser Yazılım](https://softviser.com.tr) — MIT lisansı ile açık kaynak.

| Açık tema | Koyu tema |
|---|---|
| ![Port Killer açık tema](docs/main-light.png) | ![Port Killer koyu tema](docs/main-dark.png) |

## İndirme

[Releases](https://github.com/softviser/port-killer/releases) sayfasındaki her sürümde iki dosya bulunur. İkisi de tek dosyadır, kurulum gerektirmez ve Windows 10 veya 11 (64 bit) ister.

| Dosya | Boyut | Gereksinim |
|---|---|---|
| `PortKiller-<version>-win-x64.exe` | ~66 MB | Yok; .NET içinde gelir |
| `PortKiller-<version>-win-x64-requires-net8.exe` | ~0.2 MB | [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) (x64) |

Bilgisayarınızda .NET 8 zaten kuruluysa (çoğu .NET geliştiricisinde olduğu gibi) küçük dosya yeterlidir. Çalışma zamanı yoksa Windows, indirme bağlantısı içeren bir uyarı gösterir.

Dosya dijital olarak imzalı olmadığı için Windows ilk çalıştırmada SmartScreen uyarısı gösterebilir. **Ek bilgi > Yine de çalıştır** ile devam edebilirsiniz. İndirdiğiniz dosyayı `SHA256SUMS.txt` ile doğrulayabilirsiniz:

```powershell
Get-FileHash .\PortKiller-*.exe -Algorithm SHA256
```

## Neler yapıyor

- Dinlenen TCP portlarını (IPv4 ve IPv6) programa göre gruplar. Node.js, Python, .NET, Docker, PostgreSQL gibi geliştirme araçları kendi gruplarında üstte, geri kalan uygulamalar en altta "Diğer uygulamalar" grubunda durur.
- Her satırın sonundaki ✕ düğmesi süreci sonlandırır. Bu, PowerShell'deki `Stop-Process -Force` ile aynı işi yapar. Geliştirme aracı olmayan bir programı kapatmadan önce onay ister.
- Liste 3 saniyede bir yenilenir. Yeni bir port açıldığında Windows bildirimi gelir; bildirime tıklayınca liste açılır.
- Üstteki arama kutusu port, program adı, PID ve adres üzerinde yazdıkça süzer. `:3000` ya da `3000` yazmak aynı sonucu verir.
- Arayüz Windows'un görüntü dilinde açılır (34 dil). Açık/koyu tema ve vurgu rengi de Windows ayarlarından gelir; ayar değiştiğinde uygulamayı yeniden başlatmak gerekmez.
- Yönetici yetkisi istemez. Kendi kullanıcınızla başlattığınız süreçleri kapatabilir.

![Arama](docs/search-light.png)

### Neleri göstermez

Uygulamanın amacı geliştirme sırasında açılan portları yönetmek; sistemin çalışmasını bozabilecek şeyleri kapatmak değil. Bu yüzden:

- 1024'ün altındaki portlar (SMB, RPC, HTTP.sys gibi Windows servisleri) listelenmez.
- `System`, `svchost`, `lsass`, `services`, `wininit`, `winlogon`, `csrss`, `smss`, `spoolsv`, Windows Defender ve arama dizinleyicisi gibi sistem süreçleri listede hiç görünmez.

Geliştirme aracı sayılan programların listesi [`Ports.cs`](src/PortKiller/Ports.cs) içindeki `DevTools` sözlüğündedir. Kullandığınız bir araç listede yoksa oraya eklemeniz yeterli.

## Kullanım

Uygulamayı çalıştırdığınızda kırmızı "PK" simgesi sistem tepsisine yerleşir.

- **Sol tık:** Port listesini açar ya da kapatır.
- **Sağ tık:** Göster, yenile, hakkında ve çıkış menüsü.

Pencereyi kapatmak uygulamayı sonlandırmaz, yalnızca listeyi gizler. Tamamen kapatmak için tepsi menüsünden **Çıkış**'ı seçin.

| Kısayol | İşlem |
|---|---|
| F5 | Listeyi yenile |
| Ctrl+F | Arama kutusuna geç |
| Delete | Seçili satırdaki süreci sonlandır |
| Esc | Aramayı temizle; arama boşsa pencereyi gizle |

![Hakkında penceresi](docs/about-light.png)

## Bilinen kısıtlar

- Yönetici olarak çalışan bir süreci (örneğin bir Windows servisi olarak kurulmuş PostgreSQL) kapatmak için Port Killer'ı da yönetici olarak çalıştırmanız gerekir. Aksi halde uygulama bunu bir hata mesajıyla bildirir.
- `nodemon` gibi kendini sürekli yeniden başlatan araçlarda, yeniden başlatma 3 saniyelik kontrol anına denk gelirse aynı port için tekrar "yeni port" bildirimi gelebilir.
- Koyu temada, liste pencereye sığmadığında görünen kaydırma çubuğu açık renkte kalır.
- Yalnızca TCP dinleyicileri listelenir; UDP portları gösterilmez.

## Kaynaktan derleme

Gerekenler: Windows 10 veya 11 ve [.NET 8 SDK](https://dotnet.microsoft.com/download).

```powershell
git clone https://github.com/softviser/port-killer.git
cd port-killer

# Geliştirme sırasında çalıştırmak için
dotnet run --project src\PortKiller

# .NET kurulu olmayan bilgisayarlarda da çalışan tek dosyalık exe (artifacts\PortKiller.exe)
dotnet publish src\PortKiller -c Release -r win-x64 --self-contained `
    -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:EnableCompressionInSingleFile=true -p:DebugType=none -o artifacts
```

### Proje yapısı

| Dosya | İçerik |
|---|---|
| `Ports.cs` | Dinlenen portların okunması, geliştirme aracı ve sistem süreci listeleri, süreç sonlandırma |
| `TrayApp.cs` | Tepsi simgesi, menü, periyodik kontrol ve bildirimler |
| `PortsForm.cs` | Gruplu port tablosu, arama ve üst çubuk |
| `AboutForm.cs` | Hakkında penceresi |
| `Strings.cs` | 34 dilde arayüz metinleri |
| `Theme.cs` | Windows açık/koyu tema ve vurgu rengine göre renkler |

### Çeviriler

Metinler [`Strings.cs`](src/PortKiller/Strings.cs) içindeki tabloda, her dil için tek satırda tutulur. Yeni bir dil eklemek için o dilin kodunu (`"sr"`, `"pt-BR"` gibi) ve 13 metni aynı sırayla ekleyin. Bir çeviride hata görürseniz issue ya da pull request açabilirsiniz.

## Lisans

[MIT](LICENSE) © 2026 Softviser Yazılım
