# VegaAsis Windows

Vega Hızlı Teklif Sistemi – .NET Framework 4.7 Windows Forms masaüstü uygulaması.  
Veri katmanı: Entity Framework 6 + PostgreSQL (Npgsql).

## Gereksinimler

- **.NET Framework 4.7**
- **PostgreSQL** (veritabanı: `vegaacente`, kullanıcı: `postgres`, şifre: `123456` – geliştirme için; üretimde kendi bağlantı ayarlarınızı kullanın)
- **Visual Studio 2017+** veya **MSBuild** (derleme için)

## Kurulum

1. Depoyu klonlayın.
2. PostgreSQL’de `vegaacente` veritabanını ve şemayı oluşturun (proje kökündeki veya `vegaasis` referans projesindeki şema/tablolar kullanılabilir). Rapor grafikleri (Kullanım Tarzı / Marka) için `offers` tablosuna `kullanim_tarzi` ve `arac_markasi` kolonları ekleyin: `DOCS/migration-offer-kullanim-tarzi-arac-markasi.sql`.
3. `VegaAsis.Windows\App.config` içinde bağlantı dizesini kontrol edin: `connectionStrings` → `VegaAsisDbContext`.
4. Çözümü açıp derleyin:
   ```bat
   dotnet build VegaAsis.sln
   ```
   veya Visual Studio ile **Build → Build Solution**.

## Çalıştırma

- Visual Studio: **F5** (Debug) veya **Ctrl+F5** (Start Without Debugging).
- Komut satırı:
  ```bat
  cd src\VegaAsis.Windows\bin\Debug
  VegaAsis.Windows.exe
  ```

İlk açılışta **Giriş** ekranı gelir; ardından ana sayfa (IndexView), menü ve sayfalar kullanılabilir.

## Ortam / Ayarlar

- **Genel ayarlar:** Menü → Yönetim → Genel Ayarlar (proxy, varsayılan tarayıcı, benchmark).
- **Yönetim paneli:** Ctrl+P veya Yönetim → Yönetim Paneli (şirket ayarları, paylaşılan şirketler, WEB ekranları, diğer ayarlar, gruplar, benchmark).
- Kullanıcı ayarları ve bağlantı bilgisi uygulama veri klasöründe saklanabilir (`%AppData%\VegaAsis`).

## Robot / Tarayıcı (Faz 4)

Şirket robotları (TRF_* sınıfları) ve tarayıcı entegrasyonu ayrı bir fazdadır. Yönetim panelinden “Şirketler / Robot” ve ilgili sayfalar kullanılabilir; tam entegrasyon için Faz 4 planına bakın.

## Proje yapısı

- `src/VegaAsis.Core` – Sözleşmeler (interfaces), DTO’lar
- `src/VegaAsis.Data` – EF6 DbContext, entity’ler, servisler
- `src/VegaAsis.Windows` – WinForms: MainForm, Forms, UserControls, Robot

## Referanslar

- **Open Hızlı Teklif** (orijinal): karşılaştırma ve yapılacaklar için `DOCS/Open-Hizli-Teklif-VegaAsis-Karsilastirma-Plani.md`
- **Multi-Agent görev listesi:** `DOCS/Multi-Agent-Task-Listesi.md` (Faz 5–7)
