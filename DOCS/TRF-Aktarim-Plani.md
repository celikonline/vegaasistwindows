# TRF Şirket Robotları Aktarım Planı

**Kaynak:** Open Hızlı Teklif (90+ TRF_*.cs dosyası)  
**Hedef:** VegaAsis Windows (`src/VegaAsis.Windows/Robot/`)  
**Tarih:** Şubat 2026  
**Son Güncelleme:** 02 Şubat 2026

---

## 📊 Genel İlerleme Durumu

| Faz | Durum | İlerleme |
|-----|-------|----------|
| Faz 1: Altyapı | ✅ Tamamlandı | 100% |
| Faz 2: Öncelik 1 (10 şirket) | ✅ Tamamlandı | 100% |
| Faz 3: Öncelik 2 (20 şirket) | ✅ Tamamlandı | 100% |
| Faz 4: Test & İyileştirme | 🔄 Plan hazır | Plan + Login Test formu |

**Toplam TRF Dosyası:** 31 (stub yok, tümü TRF_Base) ✅

---

## 1. Mevcut Altyapı Durumu

### 1.1 Hazır Bileşenler

| Bileşen | Dosya | Durum |
|---------|-------|-------|
| IBrowserDriver | `Robot/IBrowserDriver.cs` | ✅ Hazır |
| ChromeBrowserDriver | `Robot/ChromeBrowserDriver.cs` | ✅ Hazır |
| ICompanyRobot | `Robot/ICompanyRobot.cs` | ✅ Hazır |
| CompanyRobotRegistry | `Robot/CompanyRobotRegistry.cs` | ✅ Hazır |
| ICaptchaResolver | `Robot/ICaptchaResolver.cs` | ✅ Hazır |
| ManuelCaptchaResolver | `Robot/ManuelCaptchaResolver.cs` | ✅ Hazır |
| **TRF_Base** | `Robot/TRF_Base.cs` | ✅ **YENİ** |
| **ICompanyCredentialService** | `Core/Contracts/ICompanyCredentialService.cs` | ✅ **YENİ** |
| **CompanyCredentialService** | `Data/Services/CompanyCredentialService.cs` | ✅ **YENİ** |
| **CompanyCredential Entity** | `Data/Entities/CompanyCredential.cs` | ✅ **YENİ** |

### 1.2 TRF Dosyaları (30 adet)

| # | Dosya | Şirket | Durum | Base |
|---|-------|--------|-------|------|
| 1 | TRF_AkSigorta.cs | AK Sigorta | ✅ | TRF_Base |
| 2 | TRF_Anadolu.cs | Anadolu Sigorta | ✅ | TRF_Base |
| 2a | TRF_AnaSigorta.cs | ANA Sigorta | ✅ | TRF_Base |
| 3 | TRF_Allianz.cs | Allianz | ✅ | TRF_Base |
| 4 | TRF_Sompo.cs | Sompo Japan | ✅ | TRF_Base |
| 5 | TRF_HDI.cs | HDI | ✅ | TRF_Base |
| 6 | TRF_Mapfre.cs | Mapfre | ✅ | TRF_Base |
| 7 | TRF_Gunes.cs | Güneş Sigorta | ✅ | TRF_Base |
| 8 | TRF_Groupama.cs | Groupama | ✅ | TRF_Base |
| 9 | TRF_Zurich.cs | Zurich | ✅ | TRF_Base |
| 10 | TRF_Neova.cs | Neova | ✅ | TRF_Base |
| 11 | TRF_Eureko.cs | Eureko | ✅ | TRF_Base |
| 12 | TRF_Ergo.cs | Ergo | ✅ | TRF_Base |
| 13 | TRF_Generali.cs | Generali | ✅ | TRF_Base |
| 14 | TRF_TurkNippon.cs | Türk Nippon | ✅ | TRF_Base |
| 15 | TRF_Ray.cs | Ray Sigorta | ✅ | TRF_Base |
| 16 | TRF_Doga.cs | Doğa Sigorta | ✅ | TRF_Base |
| 17 | TRF_Ankara.cs | Ankara Sigorta | ✅ | TRF_Base |
| 18 | TRF_Halk.cs | Halk Sigorta | ✅ | TRF_Base |
| 19 | TRF_Koru.cs | Koru Sigorta | ✅ | TRF_Base |
| 20 | TRF_Orient.cs | Orient | ✅ | TRF_Base |
| 21 | TRF_Quick.cs | Quick Sigorta | ✅ | TRF_Base |
| 22 | TRF_DemirHayat.cs | Demir Hayat | ✅ | TRF_Base |
| 23 | TRF_Gulf.cs | Gulf Sigorta | ✅ | TRF_Base |
| 24 | TRF_Magdeburger.cs | Magdeburger | ✅ | TRF_Base |
| 25 | TRF_Bereket.cs | Bereket | ✅ | TRF_Base |
| 26 | TRF_Corpus.cs | Corpus | ✅ | TRF_Base |
| 27 | TRF_Hepiyi.cs | Hepiyi | ✅ | TRF_Base |
| 28 | TRF_Seker.cs | Şeker Sigorta | ✅ | TRF_Base |
| 29 | TRF_Turkiye.cs | Türkiye Sigorta | ✅ | TRF_Base |
| 30 | TRF_Unico.cs | Unico | ✅ | TRF_Base |

### 1.3 ICompanyRobot Arayüzü

```csharp
public interface ICompanyRobot
{
    string CompanyId { get; }
    string CompanyName { get; }
    string LoginUrl { get; }
    Task<bool> LoginAsync(IBrowserDriver driver);
    Task<string> GetOfferAsync(IBrowserDriver driver, object offerParams);
}
```

### 1.4 TRF_Base Abstract Class (YENİ)

```csharp
public abstract class TRF_Base : ICompanyRobot
{
    // Abstract properties
    public abstract string CompanyId { get; }
    public abstract string CompanyName { get; }
    public abstract string LoginUrl { get; }
    
    // Credential yönetimi
    protected Guid? UserId { get; private set; }
    public void SetUserId(Guid? userId);
    protected Task<CompanyCredentialDto> GetCredentialAsync();
    
    // Helper metodlar
    protected static string GetPlaka(object offerParams);
    protected static string GetTckn(object offerParams);
    protected static bool TryClick(IBrowserDriver driver, string selector);
    protected static bool TrySendKeys(IBrowserDriver driver, string selector, string text);
    
    // Abstract metodlar
    public abstract Task<bool> LoginAsync(IBrowserDriver driver);
    public abstract Task<string> GetOfferAsync(IBrowserDriver driver, object offerParams);
}
```

---

## 2. Credential Yönetimi (Tamamlandı)

### 2.1 Veritabanı Tablosu

```sql
CREATE TABLE company_credentials (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    company_id VARCHAR(50) NOT NULL,
    username VARCHAR(100),
    password_encrypted VARCHAR(256),
    user_id UUID,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW(),
    UNIQUE(company_id, user_id)
);
```

**SQL Dosyası:** `DOCS/company_credentials_table.sql`

### 2.2 Servis Arayüzü

```csharp
public interface ICompanyCredentialService
{
    Task<CompanyCredentialDto> GetCredentialAsync(string companyId, Guid? userId = null);
    Task<IReadOnlyList<CompanyCredentialDto>> GetAllCredentialsAsync(Guid? userId = null);
    Task<CompanyCredentialDto> SaveCredentialAsync(string companyId, string username, string password, Guid? userId = null);
    Task<bool> DeleteCredentialAsync(string companyId, Guid? userId = null);
    Task<bool> SetActiveAsync(string companyId, bool isActive, Guid? userId = null);
}
```

### 2.3 DI Kaydı (Program.cs)

```csharp
builder.RegisterType<CompanyCredentialService>()
    .As<ICompanyCredentialService>()
    .InstancePerLifetimeScope();
```

---

## 3. Kalan İşler (Faz 4)

### 3.1 Veritabanı Tablosu

**Otomatik:** Uygulama ilk açılışta `company_credentials` tablosu yoksa oluşturur (`Program.cs` → `CompanyCredentialService.EnsureCompanyCredentialsTable`).

**Manuel (isteğe bağlı):**
```bash
psql -h localhost -U postgres -d vegaacente -f DOCS/company_credentials_table.sql
```

### 3.2 Portal Selector Güncellemeleri

**Rehber:** `DOCS/TRF-Selector-Guncelleme.md` — selector nasıl bulunur, hangi dosyada neler güncellenecek, adım adım örnek.

| Durum | Açıklama |
|-------|----------|
| ✅ Çoklu selector uygulandı | 31 TRF'te rehberdeki çoklu selector (kullaniciAdi, sifre, btnGiris, txtPlaka, txtTcKimlikNo, btnSorgula vb.) eklendi. |
| ⏳ Portal doğrulama | Gerçek portallarda test edilip id/name/class rehbere göre doğrulanmalı. |

### 3.3 Paralel Teklif Altyapısı ✅

| Bileşen | Dosya | Durum |
|---------|-------|-------|
| ParallelOfferRunner | `Robot/ParallelOfferRunner.cs` | ✅ Tamamlandı |
| AllLoginsRunner / AllOffersRunner UserId | `Robot/AllLoginsRunner.cs`, `AllOffersRunner.cs` | ✅ SetUserId desteği eklendi |
| Şirketler Robot menü | "Tümünden Teklif Al (Paralel)" | ✅ Eklendi |

**ParallelOfferRunner:** `maxConcurrency` (varsayılan 3) ile eşzamanlı Chrome penceresi; her şirket için ayrı driver, SemaphoreSlim ile sınır.

### 3.4 Test Matrisi (Faz 4)

**Test planı:** `DOCS/TRF-Faz4-Test-Plani.md` — adımlar, şirket bazlı checklist, rapor.

**Login test yardımcısı:** Şirketler / Robot → ▶ Başlat → **Login Testi (Faz 4)** — tüm şirketler için giriş testi, sonuç grid + "Raporu Kopyala".

| Test | Durum |
|------|-------|
| Login testi (31 şirket) | ⏳ Plan hazır, test yardımcısı eklendi |
| Teklif sorgu testi | ⏳ Tümünden Teklif Al ile yapılır |
| Captcha akışı testi | ⏳ Manuel test |
| Paralel çalıştırma testi | ⏳ Tümünden Teklif Al (Paralel) ile yapılır |

### 3.5 UI Bileşenleri

| Bileşen | Dosya | Durum |
|---------|-------|-------|
| SirketKimlikBilgileriForm | `Forms/SirketKimlikBilgileriForm.cs` | ✅ Tamamlandı |
| SirketlerRobotForm entegrasyonu | `Forms/SirketlerRobotForm.cs` | ✅ "Kimlik Bilgileri" butonu eklendi |

---

## 4. Mimari Özet

```
VegaAsis.Windows
├── Robot/
│   ├── ICompanyRobot.cs          # Arayüz
│   ├── TRF_Base.cs               # Abstract base class
│   ├── TRF_AkSigorta.cs          # ✅ 
│   ├── TRF_Anadolu.cs            # ✅
│   ├── TRF_Allianz.cs            # ✅
│   ├── ... (27 diğer TRF)        # ✅
│   ├── CompanyRobotRegistry.cs   # Şirket kayıt merkezi
│   ├── IBrowserDriver.cs         # Browser arayüzü
│   ├── ChromeBrowserDriver.cs    # Chrome implementasyonu
│   └── ICaptchaResolver.cs       # Captcha arayüzü
│
VegaAsis.Core
├── Contracts/
│   └── ICompanyCredentialService.cs  # Credential arayüzü
│
VegaAsis.Data
├── Entities/
│   └── CompanyCredential.cs      # DB entity
├── Services/
│   └── CompanyCredentialService.cs   # Implementasyon
└── VegaAsisDbContext.cs          # DbSet eklendi
```

---

## 5. Sonraki Adımlar

1. **Veritabanı:** ✅ Tablo uygulama açılışında otomatik oluşturuluyor.
2. **Selector:** ✅ Çoklu selector 31 TRF'te uygulandı; gerçek portallarda test/doğrulama yapılacak.
3. **Test (Faz 4):** Login ve teklif sorgu testleri — bekliyor.
4. **UI:** ✅ Credential yönetim ekranı (SirketKimlikBilgileriForm) tamamlandı.

---

*Son güncelleme: 02 Şubat 2026 — Selector rehberi uygulandı (31 TRF çoklu selector)*
