# WinForms Paralel Agent Aktarım Planı

Kaynak: `D:\workspace\vega\vegaasis` (React + Supabase)  
Hedef: `vegaasistwindows` (.NET 4.7 WinForms + EF6 + PostgreSQL)

---

## Tamamlanan İşler (Son Oturum)

| Tarih | Öğe | Açıklama |
|-------|-----|----------|
| Güncel | **Duyuru servisi** | `IAnnouncementService` + `AnnouncementService` + `Announcement` entity; `DuyurularForm` servisten veri yüklüyor, "Duyuru Gönder" ile `CreateAsync` çağrılıyor. |
| Güncel | **DASK Yapı Tarzı** | `YapiTarziOptions.cs` eklendi (Betonarme, Yığma, Çelik, Ahşap, Karma, Prefabrik, Briket, Kerpiç, Taş, Diğer); `DaskDetaylariForm` bu listeyi kullanıyor. |
| Güncel | **Kısa Vadeli Poliçe** | `SorguSession.KisaVadeliPolice` eklendi; `IndexViewControl` toolbar’daki "Kısa Vadeli Poliçe Çalış" checkbox’ı `SyncToSession` / `LoadFromSession` ile senkron. |
| Güncel | **Branş formları** | Combo TRAFİK → TrafikTeklifiForm; SBM kolonu çift tık → SbmSorgusuForm. BranchFormRequested / BranchCellClickRequested bağlı. |
| Güncel | **Trafik Pol. Bilgisi** | Başlangıç T. / Bitiş T. alanları; SorguSession senkron. |
| Güncel | **Diğer Fiyatlar (subPrices)** | "Diğer Fiyatları Göster" ile grid alt fiyat satırları; RowTag, RefreshCompanyGrid, çift tıklama güncellendi. |
| Güncel | **Agent Paylaşımı** | PaylasilanSirketlerControl: Şirket dropdown, Paylaş/Kaldır, Benim Paylaştıklarım + Benimle Paylaşılanlar (SplitContainer), GetReceivedSharesAsync, CreateAsync/DeleteAsync bağlı. |
| Güncel | **Web Ekranları** | IWebUserService + WebUserService, WebEkranlariControl CRUD + unlicensed_agent_only; WebUserForm Lisanssız Acente, banned_companies, internal_bans, Description alanları eklendi. |
| Güncel | **Veri kaynakları** | TurkeyLocations (İl/İlçe), Professions (Meslek), KullanimTarziOptions (Kullanım Tarzı), VehicleBrandsAndTypes (Marka/Tip); IndexViewControl AddFormRowIlIlce, Meslek, Kullanım Tarzı, Marka/Tip, Şasi No kullanıyor. |
| Güncel | **Header layout** | rightFlow WrapContents = true (Giriş Yap, CANLI DESTEK, CANLI ÜRETİM dar pencerede wrap); topHeader Height 88, MinimumSize (0,52). |

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
| PaylasilanSirketlerTab | PaylasilanSirketlerControl | ✅ Mevcut (paylaş/kaldır/received shares) |
| WebEkranlariTab | WebEkranlariControl | ✅ Mevcut (CRUD + unlicensed_agent_only) |
| DigerAyarlarTab | DigerAyarlarControl | ✅ Mevcut |
| TekliflerTab (admin) | TekliflerAyarControl | ✅ Mevcut |
| GruplarTab | GruplarControl | ✅ Mevcut |
| BenchmarkTab | BenchmarkControl | ✅ Mevcut |
| KotaAyarlariTab | KotaAyarlariControl | ✅ Mevcut |
| **Hooks / Services** |
| useSharedCompanies | SharedCompanyService | ✅ Mevcut |
| useWebUsers | WebUserService | ✅ Mevcut (WebEkranlariControl bağlı) |
| useCompanySettings | CompanySettingsService | ✅ Mevcut |
| useUserManagement | UserManagementService | ✅ Mevcut |
| useQuotaSettings | QuotaSettingsService | ✅ Mevcut |
| useUserGroups | UserGroupService | ✅ Mevcut |
| useAppSettings | AppSettingsService | ✅ Mevcut |
| useOffers | OfferService | ✅ Mevcut |
| usePolicies | PolicyService | ✅ Mevcut |
| useAppointments | AppointmentService | ✅ Mevcut |
| useAnnouncements / Duyurular | AnnouncementService | ✅ Mevcut (DuyurularForm entegre) |

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
| **"Belge Seri:" etiketi kesiliyor** | Etiket sol kenardan taşıyor veya GroupBox içinde clipping oluyor | `AddFormRow` – Label `Location(8, y)`, input `Left=120`; GroupBox `Size(320,220)` | ✅ Tüm form satır etiketleri `Size(110, 20)` ile sabitlendi (Belge Seri, Marka/Tip, İl/İlçe vb.). |
| **Floating action butonları** | Yeni, SBM Sorgu, Kuyruk butonları sağda panelde | `_rightActionPanel` Width=120, Dock=Right | ✅ Sağ panel 120px; tam etiketli butonlar. Referans app’e benzer şekilde ~120px genişlikte, tam etiketli butonlar kullan (React QueryPanel sağ kolon ~120px) |
| **Sabit boyutlar** | `grpMusteri` 320x220, `_aracBilgileriTabs` 320x200 – dar panelde taşma | `BuildRightPanel` | `Anchor = Right` kullan; `scrollPanel` içindeki kontroller için `Width = rightPanel.ClientSize.Width - 24` gibi dinamik hesapla |
| **TopBar taşması** | lblTeklifId, chkDigerFiyat, chkAltFiyat, btnPdf sabit Location ile 500px+ gerektiriyor | `topBar` controls | FlowLayoutPanel ile wrap veya küçük fontlarla sığdır |
| **"2 Tarayıcıları Kapat" hizası** | Referans app’te "2 Tarayıcıları Kapat" metninde ufak hiza kayması | - | Button.TextAlign, Padding ayarları ile düzelt |

### Sol Panel (Grid) Kaymaları

| Sorun | Açıklama | Kod Konumu | Önerilen Çözüm |
|-------|----------|------------|----------------|
| **Boş beyaz alan** | Grid’in altında geniş boşluk; az satırda panel dolmuyor | `_companyGrid` Dock=Fill, Panel1 | Normal; grid az satırda üstte kalır. Alternatif: grid için `RowFill` veya alt bilgi alanı ekle |
| **Uyarı kolonu truncate** | "Ek Teminatlı Trafik Teklifidir!" gibi metinler "..." ile kesiliyor | `_companyGrid` | ✅ Uyarı MinimumWidth=220; Şirket/Uyarı FillWeight 120/180. |
| **Kolon genişlikleri** | Tüm kolonlar Fill ile eşit; bazı kolonlar (Şirket, Uyarı) daha geniş olmalı | DataGridView columns | ✅ Şirket FillWeight 120, Uyarı FillWeight 180. |

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
| **TRAFİK** | QueryPanel + TrafikTeklifiDialog | TrafikTeklifiForm | ✅ Combo TRAFİK seçilince açılıyor | BranchFormRequested ile bağlı |
| **KASKO** | KaskoTeminatlariDialog + farklı araç alanları | KaskoTeminatlariForm | ✅ Combo KASKO seçilince açılıyor | BranchFormRequested; Kasko Pol. tab içeriği kısmen dolu |
| **TSS** | TssDetaylariDialog | TssDetaylariForm | ✅ Combo TSS seçilince açılıyor | BranchFormRequested |
| **DASK / KONUT** | DaskDetaylariDialog | DaskDetaylariForm | ✅ Combo DASK/KONUT seçilince açılıyor | BranchFormRequested |
| **İMM** | ImmTeminatlariDialog | ImmTeminatlariForm | ✅ Combo İMM seçilince açılıyor | BranchFormRequested |
| **SBM** | SbmSorgusuDialog | SbmSorgusuForm | ✅ SBM butonu + grid SBM kolonu çift tıklama | Bağlı |
| **Kuyruk** | KuyrukSorgusuDialog | KuyrukSorgusuForm | ✅ Kuyruk Sorgusu butonu | Bağlı |
| **Webcam QR** | WebcamQRDialog | WebcamQRForm | ✅ Webcam QR Oku butonu | Bağlı |

### Eksik Parçalar

| # | Eksik | Detay |
|---|-------|-------|
| 1 | **PolicyTypeSelect** | React’ta TRAFİK/KASKO/TSS/DASK/KONUT/İMM seçici var; seçime göre ilgili dialog açılıyor. WinForms’ta yok. |
| 2 | **Seçili şirket özeti** | React QueryPanel’de seçilen şirket (ikon, ad, durum, fiyat) gösteriliyor. IndexViewControl’de yok. |
| 3 | **Trafik Pol. Bilgisi tab içeriği** | Tab mevcut ama boş; React’ta Sigorta Şirketi, Acente Kodu, Poliçe No, Kademe vb. var. |
| 4 | **Kasko Pol. Bilgisi tab içeriği** | ✅ Sigorta Şirketi, Acente Kodu, Poliçe No, Kademe, Hasarsızlık %, Kalan Gün, Yenileme No. React’ta benzer alanlar var. |
| 5 | **İl / İlçe verisi** | ✅ TurkeyLocations.GetCityNames(), GetDistrictsByCity(); AddFormRowIlIlce ile İl/İlçe combo'lar dolu. React `turkeyLocations` kullanıyor; WinForms combo’lar boş "Seçiniz". |
| 6 | **Meslek listesi** | ✅ Professions.List; IndexViewControl Müşteri Bilgileri Meslek combo. |
| 7 | **Kullanım Tarzı listesi** | ✅ KullanimTarziOptions.List (OTOMOBİL, TAKSİ, MİNİBÜS vb.); Araç Bilgileri tab. React’ta OTOMOBİL, TAKSİ, MİNİBÜS vb.; WinForms’ta "KULLANIM TARZI SEÇİNİZ" placeholder. |
| 8 | **Şasi No alanı** | ✅ Araç Bilgileri tab'inde Şasi No satırı var (AddFormRow). React’ta var; WinForms Araç Bilgileri tab’inde yok. |
| 9 | **Kısa Vadeli Poliçe Çalış** | ✅ SorguSession + IndexViewControl checkbox bağlandı. app’te’ta yok. |
| 10 | **Marka / Tip verisi** | ✅ VehicleBrandsAndTypes.GetBrandDisplays(), GetTypesByBrandDisplay(); AddFormRowMarkaTip. React’ta örnek değerler var; WinForms’ta sabit "59 - FORD/OTOSAN" vb. – API/veri kaynağı gerekebilir. |
| 11 | **Branş değişiminde grid filtreleme** | ✅ CompanyRow.UnavailableBranches; Şirket kolonunda (Yok) + üstü çizili; Trafik/Kasko/Sbm/TSS/Konut/Dask/İMM kolonlarında ilgili branş yoksa hücre üstü çizili + gri (GetBranchForColumn, CellFormatting). React’ta `activeBranch` ile `unavailableBranches` (üstü çizili) kullanılıyor; WinForms grid’de yok. |
| 12 | **subPrices / Diğer Fiyatlar expand** | React CompanyTable’da satır genişletme (expand) ve alt fiyatlar var; WinForms grid’de yok. |
| 13 | **Nav butonları → form açma** | ✅ MainForm'da Teklifler, Policelerim, SirketlerRobot, Raporlar, Destek Talepleri, Ajanda, Duyurular event'leri bağlı; tıklanınca ilgili form açılıyor. |
| 14 | **PDF Aktar / Yükle** | ✅ PDF Aktar → PDFExportForm açılıyor. PDF Yükle → OpenPdfUpload (dosya seçimi + bilgi); sunucu yükleme/import ileride eklenebilir. |
| 15 | **Sorguyu Başlat** | ✅ Session'dan OfferDto oluşturulup OfferService.CreateAsync ile teklif kaydı; başarı/hata mesajı. |
| 16 | **Duraklat / Devam Et** | ✅ IndexViewControl'de session Durum (Running/Paused) ve buton metni güncelleniyor; DuraklatRequested tetikleniyor (arka plan iş yok). |
| 17 | **Yeni Sorgu Kaydet** | ✅ Session.Reset + LoadFromSession + mesaj; YeniSorguKaydetRequested tetikleniyor. |

### Veri Kaynakları (Taşınması Gereken)

| Veri | React Dosya | WinForms Karşılığı |
|------|-------------|---------------------|
| İl/İlçe | `turkeyLocations.ts` | ✅ TurkeyLocations.cs (GetCityNames, GetDistrictsByCity) |
| Meslekler | `professions.ts` | ✅ Professions.cs (List) |
| Kullanım tarzları | QueryPanel sabit liste | ✅ KullanimTarziOptions.cs (List) |
| Marka/Tip | - | ✅ VehicleBrandsAndTypes.cs (GetBrandDisplays, GetTypesByBrandDisplay) |

---

## 2. Agent Domain Eksikleri

### 2.1 PaylasilanSirketlerControl (Agent Paylaşımı)

| Özellik | React | WinForms | Durum |
|---------|-------|----------|-------|
| Paylaşım formu (Şirket + Acente Kodu) | Var | Var (_topPanel: _cmbSirket, _txtAcenteKodu, _btnPaylas) | ✅ |
| `selectedCompanies` → Şirket dropdown | Var | Var (GetSelectedCompaniesAsync → _cmbSirket) | ✅ |
| Yeni paylaşım ekleme (shareCompany) | Var | BtnPaylas_Click → CreateAsync | ✅ |
| Benim Paylaştıklarım listesi | Var | Var (_dgvPaylasilanlar, SplitContainer) | ✅ |
| Benimle Paylaşılanlar (receivedShares) | Var | Var (_dgvAlinanlar, GetReceivedSharesAsync) | ✅ |
| Paylaşım silme (deleteShare) | Var | BtnKaldir_Click → DeleteAsync | ✅ |
| Owner user filtresi | user.id | _authService.GetCurrentUserId | ✅ |
| SharedAt tarih formatı | tr-TR | dd.MM.yyyy (CultureInfo tr-TR) | ✅ |

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
| Web kullanıcı CRUD | useWebUsers | WebUserService Create/Update/Delete, WebEkranlariControl Yeni/Düzenle/Sil | ✅ |
| unlicensed_agent_only checkbox | Var | WebUserForm _chkUnlicensedAgentOnly, grid ColUnlicensed | ✅ |
| WebUser entity/service | Supabase web_users | WebUser entity, WebUsers DbSet, WebUserService | ✅ |
| Lisans alanları (is_licensed, license_offer_only, vb.) | Var | WebUserForm + WebUserDto | ✅ |
| banned_companies, internal_bans | Var | WebUserForm _txtBannedCompanies, _txtInternalBans | ✅ |

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
| 1 | PaylasilanSirketlerControl: Yeni paylaşım formu + shareCompany | Agent | ✅ Tamamlandı |
| 2 | PaylasilanSirketlerControl: Benimle Paylaşılanlar paneli | Agent | ✅ Tamamlandı |
| 3 | PaylasilanSirketlerControl: deleteShare handler | Agent | ✅ Tamamlandı |
| 4 | PaylasilanSirketlerControl: selectedCompanies (şirket dropdown) | Agent | ✅ Tamamlandı |
| 5 | WebUserService + IWebUserService | Web Ekranları | ✅ Tamamlandı |
| 6 | WebEkranlariControl: unlicensed_agent_only + CRUD | Web Ekranları | ✅ Tamamlandı |
| 7 | SharedCompanyService.UpdateShare (opsiyonel) | Agent | Düşük |
| 8 | DbContext.WebUsers (web_users tablosu) | Veri | ✅ Mevcut |

---

## 7. Sonraki Adım Önerisi

### Tamamlanan (Layout / Branş / Veri / Agent / Web / Sorgu)
- **Layout:** Belge Seri etiketleri 110px, sağ panel 120px, Uyarı/Şirket FillWeight, header rightFlow WrapContents + Height 88.
- **Branş:** PolicyTypeSelect combo, BranchFormRequested, Trafik/Kasko Pol. tab, seçili şirket paneli.
- **Veri:** TurkeyLocations, Professions, KullanimTarziOptions, VehicleBrandsAndTypes.
- **Agent:** PaylasilanSirketlerControl paylaş/kaldır/received shares. **Web:** WebUserService, WebEkranlariControl CRUD.
- **Sorgu/PDF:** Sorguyu Başlat, Duraklat, Yeni Sorgu Kaydet, PDF Aktar, PDF Yükle bağlandı.

### Kalan / Opsiyonel
1. **Layout ince ayar:** Nav sıkışıklığı, çift menü birleştirme, TopBar taşması (dar pencerede).
2. **SharedCompanyService.UpdateShare** (opsiyonel, düşük öncelik).
3. **PDF Yükle:** Sunucuya yükleme veya PDF import mantığı (şu an dosya seçimi).
4. **Duraklat:** Arka plan sorgu/robot eklendiğinde gerçek duraklatma entegrasyonu.

_(Öncelik 2-5 tamamlandı.)_
2. ~~**Web Ekranları:** `web_users` tablosunu~~ schema’da doğrula, `WebUserService` oluştur, `WebEkranlariControl`’ü gerçek servise bağla.

---

## 8. Özet (Ekran Görüntülerine Göre)

| Kategori | Tespit Edilen | Aksiyon |
|----------|---------------|---------|
| **Kaymalar** | Belge Seri, sağ panel 120px, Uyarı FillWeight, header WrapContents + Height 88 yapıldı; nav/TopBar ince ayar opsiyonel | Bölüm 1.1 (çoğu tamamlandı) |
| **Eksik formlar** | PolicyTypeSelect, Trafik/Kasko Pol. tab, branş dialogları | ✅ Tamamlandı (Bölüm 1.2) |
| **Eksik veri** | İl/İlçe, Meslek, Kullanım Tarzı, Marka/Tip | ✅ TurkeyLocations, Professions, KullanimTarziOptions, VehicleBrandsAndTypes |
| **Sorgu / PDF butonları** | Sorguyu Başlat (teklif oluşturma), Duraklat (session), Yeni Sorgu Kaydet (reset), PDF Aktar (form), PDF Yükle (dosya seçimi) | ✅ Bağlandı (Eksik Parçalar #14-17) |
