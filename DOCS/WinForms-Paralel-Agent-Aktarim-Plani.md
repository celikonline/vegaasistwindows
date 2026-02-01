# WinForms Paralel Agent Aktarım Planı

Kaynak: `D:\workspace\vega\vegaasis` (React + Supabase)  
Hedef: `vegaasistwindows` (.NET 4.7 WinForms + EF6 + PostgreSQL)

---

## 1. Genel Eşleme Tablosu

| Kaynak (React) | Hedef (WinForms) | Durum |
|----------------|------------------|-------|
| **Sayfalar / Formlar** |
| `/` (Index) | `IndexViewControl` | ✅ Mevcut |
| `/auth` | `AuthForm` | ✅ Mevcut |
| `/teklifler` | `TekliflerForm` | ✅ Mevcut |
| `/sirketler-robot` | `SirketlerRobotForm` | ✅ Mevcut |
| `/policelerim` | `PolicelerimForm` | ✅ Mevcut |
| `/raporlar` | `RaporlarForm` | ✅ Mevcut |
| `/destek-talepleri` | `DestekTalepleriForm` | ✅ Mevcut |
| `/ajanda-yenileme` | `AjandaYenilemeForm` | ✅ Mevcut |
| `/duyurular` | `DuyurularForm` | ✅ Mevcut |
| `/canli-uretim` | `CanliUretimForm` | ✅ Mevcut |
| `/canli-destek` | `CanliDestekForm` | ✅ Mevcut |
| Admin Panel Dialog | `AdminPanelForm` | ✅ Mevcut |
| **Admin Tabs / UserControls** |
| SirketAyarlariTab | SirketAyarlariControl | ✅ Mevcut |
| KullaniciAyarlariTab | KullaniciAyarlariUserControl | ✅ Mevcut |
| PaylasilanSirketlerTab | PaylasilanSirketlerControl | ⚠️ Kısmi |
| WebEkranlariTab | WebEkranlariControl | ⚠️ Kısmi |
| DigerAyarlarTab | DigerAyarlarControl | ✅ Mevcut |
| TekliflerTab (admin) | TekliflerAyarControl | ✅ Mevcut |
| GruplarTab | GruplarControl | ✅ Mevcut |
| BenchmarkTab | BenchmarkControl | ✅ Mevcut |
| KotaAyarlariTab | KotaAyarlariControl | ✅ Mevcut |
| **Hooks / Services** |
| useSharedCompanies | SharedCompanyService | ✅ Mevcut |
| useWebUsers | ❌ WebUserService | ❌ Eksik |
| useCompanySettings | CompanySettingsService | ✅ Mevcut |
| useUserManagement | UserManagementService | ✅ Mevcut |
| useQuotaSettings | QuotaSettingsService | ✅ Mevcut |
| useUserGroups | UserGroupService | ✅ Mevcut |
| useAppSettings | AppSettingsService | ✅ Mevcut |
| useOffers | OfferService | ✅ Mevcut |
| usePolicies | PolicyService | ✅ Mevcut |
| useAppointments | AppointmentService | ✅ Mevcut |

---

## 1.1 Ana Ekran Kaymaları (Layout Sorunları)

Ekran görüntülerine göre tespit edilen düzen problemleri:

### Üst Bölüm (Header) Kaymaları

| Sorun | Açıklama | Kod Konumu | Önerilen Çözüm |
|-------|----------|------------|----------------|
| **Giriş Yap butonu kesiliyor** | Sağ köşede "→ Giriş Yap", "CANLI DESTEK", "CANLI ÜRETİM" butonları dar pencerede truncate oluyor | `IndexViewControl.BuildTopHeader()` – `rightPanel` sabit 340px, `FlowLayoutPanel` RightToLeft | `rightPanel.Width` dinamik yap veya `rightFlow.WrapContents = true` ile sarmala |
| **Nav butonları sıkışıklığı** | ANA EKRAN, ŞİRKETLER, TEKLİFLER vb. butonlar sabit `Location(320,6)` ile konumlanıyor; başlık ile çakışabilir | `navFlow` fixed position | `TableLayoutPanel` veya `FlowLayoutPanel` ile responsive yap; min genişlikte scroll veya wrap |
| **Çift menü karmaşası** | Hem MainForm ToolStrip hem IndexViewControl içinde nav butonları var; tekrarlı ve tutarsız | MainForm + IndexViewControl | Nav’i tek yerde tut (ör. sadece MainForm menü/toolbar) veya IndexViewControl nav’i MainForm ile senkronize et |

### Sağ Panel (Query Panel) Kaymaları

| Sorun | Açıklama | Kod Konumu | Önerilen Çözüm |
|-------|----------|------------|----------------|
| **"Belge Seri:" etiketi kesiliyor** | Etiket sol kenardan taşıyor veya GroupBox içinde clipping oluyor | `AddFormRow` – Label `Location(8, y)`, input `Left=120`; GroupBox `Size(320,220)` | Etiket genişliğini `Label.AutoSize = false` + `Width = 110` yap; veya `MinimumSize` ile güvence al |
| **Floating action butonları** | Yeni, SBM Sorgu, Kuyruk butonları sağda 48px panelde; formla görsel kopukluk | `_rightActionPanel` Width=48, Dock=Right | Referans app’e benzer şekilde ~120px genişlikte, tam etiketli butonlar kullan (React QueryPanel sağ kolon ~120px) |
| **Sabit boyutlar** | `grpMusteri` 320x220, `_aracBilgileriTabs` 320x200 – dar panelde taşma | `BuildRightPanel` | `Anchor = Right` kullan; `scrollPanel` içindeki kontroller için `Width = rightPanel.ClientSize.Width - 24` gibi dinamik hesapla |
| **TopBar taşması** | lblTeklifId, chkDigerFiyat, chkAltFiyat, btnPdf sabit Location ile 500px+ gerektiriyor | `topBar` controls | FlowLayoutPanel ile wrap veya küçük fontlarla sığdır |
| **"2 Tarayıcıları Kapat" hizası** | Referans app’te "2 Tarayıcıları Kapat" metninde ufak hiza kayması | - | Button.TextAlign, Padding ayarları ile düzelt |

### Sol Panel (Grid) Kaymaları

| Sorun | Açıklama | Kod Konumu | Önerilen Çözüm |
|-------|----------|------------|----------------|
| **Boş beyaz alan** | Grid’in altında geniş boşluk; az satırda panel dolmuyor | `_companyGrid` Dock=Fill, Panel1 | Normal; grid az satırda üstte kalır. Alternatif: grid için `RowFill` veya alt bilgi alanı ekle |
| **Uyarı kolonu truncate** | "Ek Teminatlı Trafik Teklifidir!" gibi metinler "..." ile kesiliyor | `_companyGrid` AutoSizeColumnsMode.Fill | Uyarı kolonuna `MinimumWidth` veya `AutoSizeMode = DisplayedCells` ver |
| **Kolon genişlikleri** | Tüm kolonlar Fill ile eşit; bazı kolonlar (Şirket, Uyarı) daha geniş olmalı | DataGridView columns | `FillWeight` veya sabit `Width` ile öncelik ver |

### Genel Layout Önerileri

- **FlowLayoutPanel / TableLayoutPanel**: Sabit `Location` yerine layout paneller kullan.
- **Anchor / Dock**: `grpMusteri` ve tab’ler için `AnchorStyles.Right` eklenmeli.
- **MinimumSize**: IndexViewControl `MinimumSize(1000,600)` – küçük ekranlarda scroll çubuğu gerekebilir.
- **Splitter**: Panel2MinSize 320px – dar pencerede sağ panel oransız büyük kalabilir; 280px’e düşürülebilir.

---

## 1.2 Eksik Formlar ve Branş Bazlı Yapı

Grid’de Trafik, Kasko, TSS, DASK, Konut, IMM kolonları var ancak sağ panel sadece **TRAFİK** için dolu; diğer branşlara ait formlar/panel eksik veya bağlı değil.

### Branş Bazlı Form / Panel Durumu

| Branş | React | WinForms Form | IndexViewControl’e Bağlı | Durum |
|-------|-------|---------------|--------------------------|-------|
| **TRAFİK** | QueryPanel + TrafikTeklifiDialog | TrafikTeklifiForm | ❌ Sağ panel sadece TRAFİK alanları | Form var, ana ekrandan açılmıyor |
| **KASKO** | KaskoTeminatlariDialog + farklı araç alanları | KaskoTeminatlariForm | ❌ Kasko Pol. Bilgisi tab boş | Form var, tab/dialog boş |
| **TSS** | TssDetaylariDialog | TssDetaylariForm | ❌ - | Form var, tetiklenmiyor |
| **DASK / KONUT** | DaskDetaylariDialog | DaskDetaylariForm | ❌ - | Form var, tetiklenmiyor |
| **İMM** | ImmTeminatlariDialog | ImmTeminatlariForm | ❌ - | Form var, tetiklenmiyor |
| **SBM** | SbmSorgusuDialog | SbmSorgusuForm | ✅ SBM Sorgula butonu | Bağlı |
| **Kuyruk** | KuyrukSorgusuDialog | KuyrukSorgusuForm | ✅ Kuyruk Sorgusu butonu | Bağlı |
| **Webcam QR** | WebcamQRDialog | WebcamQRForm | ✅ Webcam QR Oku butonu | Bağlı |

### Eksik Parçalar

| # | Eksik | Detay |
|---|-------|-------|
| 1 | **PolicyTypeSelect** | React’ta TRAFİK/KASKO/TSS/DASK/KONUT/İMM seçici var; seçime göre ilgili dialog açılıyor. WinForms’ta yok. |
| 2 | **Seçili şirket özeti** | React QueryPanel’de seçilen şirket (ikon, ad, durum, fiyat) gösteriliyor. IndexViewControl’de yok. |
| 3 | **Trafik Pol. Bilgisi tab içeriği** | Tab mevcut ama boş; React’ta Sigorta Şirketi, Acente Kodu, Poliçe No, Kademe vb. var. |
| 4 | **Kasko Pol. Bilgisi tab içeriği** | Tab mevcut ama boş; React’ta benzer alanlar var. |
| 5 | **İl / İlçe verisi** | React `turkeyLocations` kullanıyor; WinForms combo’lar boş "Seçiniz". |
| 6 | **Meslek listesi** | React `professionsList` kullanıyor; WinForms boş. |
| 7 | **Kullanım Tarzı listesi** | React’ta OTOMOBİL, TAKSİ, MİNİBÜS vb.; WinForms’ta "KULLANIM TARZI SEÇİNİZ" placeholder. |
| 8 | **Şasi No alanı** | React’ta var; WinForms Araç Bilgileri tab’inde yok. |
| 9 | **Kısa Vadeli Poliçe Çalış** | Referans app’te var; WinForms’ta yok. |
| 10 | **Marka / Tip verisi** | React’ta örnek değerler var; WinForms’ta sabit "59 - FORD/OTOSAN" vb. – API/veri kaynağı gerekebilir. |
| 11 | **Branş değişiminde grid filtreleme** | React’ta `activeBranch` ile `unavailableBranches` (üstü çizili) kullanılıyor; WinForms grid’de yok. |
| 12 | **subPrices / Diğer Fiyatlar expand** | React CompanyTable’da satır genişletme (expand) ve alt fiyatlar var; WinForms grid’de yok. |
| 13 | **Nav butonları → form açma** | IndexViewControl nav butonları (ŞİRKETLER, TEKLİFLER vb.) tıklanınca ilgili form açılmıyor. |
| 14 | **PDF Aktar / Yükle** | Butonlar var; işlev bağlı değil. |
| 15 | **Sorguyu Başlat** | Buton var; gerçek sorgu başlatma mantığı yok. |
| 16 | **Duraklat / Devam Et** | Buton var; işlev yok. |
| 17 | **Yeni Sorgu Kaydet** | Buton var; işlev yok. |

### Veri Kaynakları (Taşınması Gereken)

| Veri | React Dosya | WinForms Karşılığı |
|------|-------------|---------------------|
| İl/İlçe | `turkeyLocations.ts` | C# sınıf veya JSON/resource |
| Meslekler | `professions.ts` | C# sınıf veya JSON/resource |
| Kullanım tarzları | QueryPanel sabit liste | C# enum veya liste |

---

## 2. Agent Domain Eksikleri

### 2.1 PaylasilanSirketlerControl (Agent Paylaşımı)

| Özellik | React | WinForms | Durum |
|---------|-------|----------|-------|
| Paylaşım formu (Şirket + Acente Kodu) | Var | ❌ Yok | **Eksik** |
| `selectedCompanies` → Şirket dropdown | Var | ❌ Yok | **Eksik** |
| Yeni paylaşım ekleme (shareCompany) | Var | `_btnYeniPaylasim` click handler yok | **Eksik** |
| Benim Paylaştıklarım listesi | Var | Var (tek DGV) | ⚠️ Gösterim farklı |
| Benimle Paylaşılanlar (receivedShares) | Var | ❌ Yok | **Eksik** |
| Paylaşım silme (deleteShare) | Var | `_btnKaldir` handler yok | **Eksik** |
| Owner user filtresi | user.id | AuthService.CurrentUser | ⚠️ Kontrol et |
| SharedAt tarih formatı | tr-TR | - | **Eksik** |

**Gerekli İşlemler:**
1. `PaylasilanSirketlerControl` için `ICompanySettingsService` / `ISharedCompanyService` + `IAuthService` enjekte et (şirket listesi için)
2. Yeni paylaşım dialog/form ekle: Şirket dropdown + Acente kodu input
3. İki panel: "Benim Paylaştıklarım" + "Benimle Paylaşılanlar" (SplitContainer)
4. `GetReceivedSharesAsync` çağrısı ekle (AuthService.CurrentUserId ile)
5. Silme ve yeni ekleme event handler'ları bağla

---

### 2.2 WebEkranlariControl (unlicensed_agent_only)

| Özellik | React | WinForms | Durum |
|---------|-------|----------|-------|
| Web kullanıcı CRUD | useWebUsers | Hardcoded mock | **Eksik** |
| unlicensed_agent_only checkbox | Var | ❌ Yok | **Eksik** |
| WebUser entity/service | Supabase web_users | ❌ Entity var, Service yok | **Eksik** |
| Lisans alanları (is_licensed, license_offer_only, vb.) | Var | Sadece mock | **Eksik** |
| banned_companies, internal_bans | Var | ❌ Yok | **Eksik** |

**Gerekli İşlemler:**
1. `WebUserService` + `IWebUserService` oluştur (useWebUsers → C#)
2. `WebUser` entity zaten var; `web_users` tablosu EF'de map edilsin
3. `WebEkranlariControl` form alanları: unlicensed_agent_only dahil tüm alanlar
4. CRUD işlemleri bağla

---

### 2.3 Auth / Profile (agent_code)

| Özellik | React | WinForms | Durum |
|---------|-------|----------|-------|
| profiles.agent_code | Var | Profile entity'de var | ✅ |
| Auth sonrası agent_code kullanımı | Paylaşımda gösterim | - | ⚠️ PaylasilanSirketler'da kullanılıyor mu? |

---

## 3. Paralel Aktarım Grupları

Paralel çalışılabilecek bağımsız iş paketleri:

### Grup A: Agent Paylaşımı (PaylasilanSirketlerControl)
- **Bağımlılık:** ISharedCompanyService, ICompanySettingsService, IAuthService
- **Görevler:**
  1. PaylasilanSirketlerControl UI revizyonu (SplitContainer, form)
  2. Yeni paylaşım dialog
  3. GetReceivedSharesAsync entegrasyonu
  4. shareCompany / deleteShare handler bağlantısı

### Grup B: Web Ekranları (WebEkranlariControl)
- **Bağımlılık:** WebUserService (yeni)
- **Görevler:**
  1. IWebUserService + WebUserService
  2. WebUser entity DbSet
  3. WebEkranlariControl CRUD + unlicensed_agent_only

### Grup C: SharedCompanyService İyileştirmeleri
- **Görevler:**
  1. GetAllAsync'te ownerUserId parametresini Auth ile otomatik doldur
  2. UpdateShare / UpdateShareRestrictions (React'ta var, WinForms'ta yok) – opsiyonel

### Grup D: Diğer Placeholder / Mock Kontrolleri
- IndexViewControl → CompanyTable benzeri gerçek veri
- SirketSecimForm → Gerçek şirket listesi

---

## 4. Öncelik Sırası

| Öncelik | Paket | Tahmini Süre | Bağımlılık |
|---------|-------|--------------|------------|
| 1 | PaylasilanSirketlerControl tamamlama | 1–2 gün | ISharedCompanyService ✅ |
| 2 | WebUserService + WebEkranlariControl | 2–3 gün | web_users tablosu |
| 3 | SharedCompany UpdateShare (opsiyonel) | 0.5 gün | - |
| 4 | Sirket dropdown için CompanySettings entegrasyonu | 0.5 gün | ICompanySettingsService ✅ |

---

## 5. Teknik Detaylar

### 5.1 AdminPanelForm Dependency Injection
- `PaylasilanSirketlerControl` için `ISharedCompanyService` zaten ServiceLocator ile alınıyor.
- `ICompanySettingsService` ve `IAuthService` eklenmeli (şirket listesi + current user için).

### 5.2 PaylasilanSirketlerControl Önerilen Constructor
```csharp
public PaylasilanSirketlerControl(
    ISharedCompanyService sharedCompanyService,
    ICompanySettingsService companySettingsService,
    IAuthService authService)
```

### 5.3 ReceivedCompanyShare Görüntüleme
- `ReceivedCompanyShareDto`: FromAgentCode, CompanyName, ReceivedAt
- İkinci DataGridView veya SplitContainer ile "Benimle Paylaşılanlar" paneli

### 5.4 Web_users Tablosu
- `database-schema.sql` / migration’da `web_users` tanımlı mı kontrol et.
- `VegaAsisDbContext`’e `DbSet<WebUser>` eklenmeli (WebUser entity zaten var).

---

## 6. Özet Eksik Listesi

| # | Eksik | Modül | Öncelik |
|---|-------|-------|---------|
| 1 | PaylasilanSirketlerControl: Yeni paylaşım formu + shareCompany | Agent | Yüksek |
| 2 | PaylasilanSirketlerControl: Benimle Paylaşılanlar paneli | Agent | Yüksek |
| 3 | PaylasilanSirketlerControl: deleteShare handler | Agent | Yüksek |
| 4 | PaylasilanSirketlerControl: selectedCompanies (şirket dropdown) | Agent | Yüksek |
| 5 | WebUserService + IWebUserService | Web Ekranları | Yüksek |
| 6 | WebEkranlariControl: unlicensed_agent_only + CRUD | Web Ekranları | Yüksek |
| 7 | SharedCompanyService.UpdateShare (opsiyonel) | Agent | Düşük |
| 8 | DbContext.WebUsers (web_users tablosu) | Veri | ✅ Mevcut |

---

## 7. Sonraki Adım Önerisi

### Öncelik 1: Ana Ekran Kaymaları (Hızlı Düzeltmeler)
1. **Belge Seri label:** `AddFormRow` içinde label için `MinimumSize` veya sabit genişlik ver.
2. **Right action panel:** 48px yerine ~120px yap; buton etiketlerini tam göster (React QueryPanel benzeri).
3. **Header responsive:** Nav + sağ butonlar için `TableLayoutPanel` veya `FlowLayoutPanel` kullan.
4. **Form sabit boyutları:** `grpMusteri`, `_aracBilgileriTabs` için `Anchor.Right` ve dinamik genişlik.

### Öncelik 2: Branş Bazlı Form Entegrasyonu
1. **PolicyTypeSelect:** ComboBox ile TRAFİK/KASKO/TSS/DASK/KONUT/İMM seçimi ekle.
2. **Seçime göre form:** KASKO seçilince KaskoTeminatlariForm, TSS için TssDetaylariForm vb.
3. **Trafik Pol. / Kasko Pol. tab içerikleri:** React'taki alanları C#'a taşı.
4. **Seçili şirket paneli:** Grid'den seçim al; sağ panel üstünde şirket özeti göster.

### Öncelik 3: Veri Kaynakları
1. turkeyLocations, professions, kullanım tarzları → C# sınıf veya JSON resource.

### Öncelik 4: Agent Paylaşımı
1. **Agent Paylaşımı:** `PaylasilanSirketlerControl` için `ICompanySettingsService` ve `IAuthService` enjekte edip, yeni paylaşım formu + received shares paneli ekle.

### Öncelik 5: Web Ekranları
2. **Web Ekranları:** `web_users` tablosunu schema’da doğrula, `WebUserService` oluştur, `WebEkranlariControl`’ü gerçek servise bağla.

---

## 8. Özet (Ekran Görüntülerine Göre)

| Kategori | Tespit Edilen | Aksiyon |
|----------|---------------|---------|
| **Kaymalar** | Belge Seri kesilmesi, Giriş Yap butonu truncate, floating action butonları, sabit boyutlar | Layout düzeltmeleri (Bölüm 1.1) |
| **Eksik formlar** | PolicyTypeSelect, Trafik/Kasko Pol. tab içerikleri, branş dialogları tetiklenmiyor | Branş entegrasyonu (Bölüm 1.2) |
| **Eksik veri** | İl/İlçe, Meslek, Kullanım Tarzı | Veri kaynakları taşınmalı |
| **Bağlı olmayan butonlar** | Sorguyu Başlat, Duraklat, Yeni Sorgu Kaydet, PDF Aktar/Yükle | İş mantığı eklenmeli |
