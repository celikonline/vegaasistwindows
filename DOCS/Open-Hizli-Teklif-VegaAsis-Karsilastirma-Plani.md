# Open Hızlı Teklif vs VegaAsis Windows – Karşılaştırma ve Yapılacaklar Planı

**Orijinal Proje:** `C:\Users\celik\Desktop\Open Hızlı Teklif Proj\Open Hizli Teklif-cleaned\Open_Hizli_Teklif`  
**Hedef Proje:** `vegaasistwindows` (.NET 4.7 WinForms + EF6 + PostgreSQL)

**Tamamlanan fazlar:** Faz 1 (Kritik Eksikler), Faz 2 (Form Tamamlama), Faz 3 (Raporlar ve Grafikler – grafik entegrasyonu, gerçek veri: Şirket/Meslek/Ürün/Doğum Tarihi/Kesilen Poliçeler + Kullanım Tarzı/Marka, tarih filtresi, export/yazdır), Faz 4 (Robot altyapı: IBrowserDriver, TRF_* stub, ICaptchaResolver, ITramerService stub, Tümüne Giriş/Tümünden Teklif Al), Faz 5, Faz 6, Faz 7. Ek: Offer Kullanım Tarzı/Araç Markası (entity, DTO, IndexView + TeklifDetayForm girişi, RaporGrafikForm gerçek veri). Servis-form entegrasyonu: DASK/Kasko/İMM/Konut/TSS servisleri DaskDetaylariForm, KaskoTeminatlariForm, ImmTeminatlariForm, KonutTeminatlariForm, TssDetaylariForm ile bağlandı; DASK’ta İl/İlçe değişince teminat yenileme + Yenile butonu. Multi-Agent ayrıntı: `DOCS/Multi-Agent-Task-Listesi.md`. Son güncelleme: Şubat 2026.

---

## 1. Genel Mimari Karşılaştırma

| Özellik | Open Hızlı Teklif (Orijinal) | VegaAsis Windows (Hedef) |
|---------|------------------------------|---------------------------|
| **UI Framework** | DevExpress (GridControl, XtraEditors, XtraPrinting) | Standart WinForms (DataGridView) |
| **Veri Kaynağı** | MySQL + WCF Servis (HizliTeklifMySqlServis) | PostgreSQL + EF6 |
| **Ana Form Yapısı** | Form1 + tab/panel ile UserControl geçişi | MainForm + _contentPanel ile Form/UserControl |
| **Merkezi Bağlantı** | ConnectSorgu (ekCls) – sorgu/session nesnesi | ServiceLocator + DI (AuthService, OfferService vb.) |
| **Şirket Robotları** | CompanysBot (90+ TRF_*.cs sınıfı – Selenium/Chromium) | IBrowserDriver, ICompanyRobot, TRF_* stub’lar; altyapı taşındı |
| **Kod Durumu** | Decompile/obfuscate (Delegate*.smethod_0) | Temiz C# |

---

## 2. Form / UserControl Eşleme Tablosu

### 2.1 Ana Form ve Gezinme

| Open Hızlı Teklif | VegaAsis Windows | Durum |
|-------------------|------------------|-------|
| Form1 (ana form) | MainForm | ✅ Mevcut |
| Tab/panel ile sayfa geçişi | Menü + OpenForm / ShowIndexView | ✅ Farklı mimari |
| ConnectSorgu → UserControl’lere geçiş | Service inject edilerek Form’lara | ✅ Daha modern |

### 2.2 Ana Ekran ve Teklif Sorgusu

| Open Hızlı Teklif | VegaAsis Windows | Durum |
|-------------------|------------------|-------|
| UserControlTeklifler (teklif listesi) | TekliflerForm + IndexViewControl | ✅ Teklif listesi ayrı form |
| - | TekliflerForm | ✅ Teklif listesi ayrı form |
| frmSirketSec | SirketSecimForm | ✅ Mevcut |
| frmKarekodTara | WebcamQRForm | ✅ Mevcut |
| frmHizliDaskSorgu | DaskDetaylariForm | ✅ Mevcut (İl/İlçe/Mahalle/İnşaat/Yapı) |
| frmDaskTeminatlari | DaskDetaylariForm içinde | ✅ Birleştirildi |
| frmKonutTeminatlari | KonutTeminatlariForm (IKonutService) | ✅ Mevcut |
| frmImmTeminatlari | ImmTeminatlariForm | ✅ Mevcut |
| frmTssDetay | TssDetaylariForm | ✅ Mevcut |
| frmKaskoEski / frmTeminatEkran / frmTeminatSec | KaskoTeminatlariForm | ✅ Birleştirildi |

### 2.3 Poliçe ve Doküman

| Open Hızlı Teklif | VegaAsis Windows | Durum |
|-------------------|------------------|-------|
| UserControlPolicelerim | PolicelerimForm | ✅ Mevcut |
| UserControlPolicelerYeni | PolicelerimForm + PoliceDetayForm (Yeni Poliçe) | ✅ Mevcut |
| UserControlPoliceKaydet | PoliceDetayForm / TeklifDetayForm | ✅ Karşılıklandı |
| UserControlAracOncekiPol | AracOncekiPolControl | ✅ Mevcut |
| UserControlDaskOncekiPol | DaskOncekiPolControl | ✅ Mevcut |
| frmOncekiPolice | OncekiPoliceForm | ✅ Mevcut |
| frmPdf / frmPDFDuzen | PDFExportForm | ✅ Temel export mevcut |
| frmPdfUpload | TeklifDosyalariForm | ✅ Mevcut (dosya ekle/indir/sil) |
| UserControlDocManager | TeklifDosyalariForm | ✅ Mevcut |

### 2.4 Raporlar

| Open Hızlı Teklif | VegaAsis Windows | Durum |
|-------------------|------------------|-------|
| UserControlRaporlar | RaporlarForm | ✅ Mevcut |
| UserControlCityGrafik | RaporGrafikForm (Şirket/İl bazlı) | ✅ RaporGrafikForm’da |
| UserControlSirketBazliGrafik | RaporGrafikForm | ✅ Mevcut |
| UserControlMesleklereGoreTeklifGrafigi | RaporGrafikForm | ✅ Mevcut |
| UserControlTeklifKullanimTarziGrafigi | RaporGrafikForm (Offer.KullanimTarzi) | ✅ Mevcut |
| UserControlTeklifMarkaGrafigi | RaporGrafikForm (Offer.AracMarkasi) | ✅ Mevcut |
| UserControlTeklifOtorizasyonOranlari | RaporGrafikForm | ✅ Mevcut |
| UserControlTeklifKomisyonKazanci | RaporGrafikForm | ✅ Mevcut |
| UserControlUrunGrafigi | RaporGrafikForm | ✅ Mevcut |
| UserControlDogumTarihiPortfoy | RaporGrafikForm | ✅ Mevcut |
| UserControlKesilenPoliceler | RaporGrafikForm | ✅ Mevcut |

### 2.5 Destek, Ajanda, Duyuru

| Open Hızlı Teklif | VegaAsis Windows | Durum |
|-------------------|------------------|-------|
| UserControlDestekTaleplerim | DestekTalepleriForm | ✅ Mevcut |
| UserControlCalendar | AjandaYenilemeForm | ✅ Mevcut |
| UserControlDuyurular | DuyurularForm | ✅ Mevcut |
| UserControlCanliDestek | CanliDestekForm | ✅ Mevcut |
| UserControlCanliUretim | CanliUretimForm | ✅ Mevcut |
| frmDuyuruGonder | DuyuruGonderForm | ✅ Mevcut |
| frmDuyuruKullanici | DuyurularForm içinde | ✅ Birleştirildi |

### 2.6 Diğer Formlar (Open Hızlı Teklif’te Var)

| Open Hızlı Teklif | VegaAsis Windows | Durum |
|-------------------|------------------|-------|
| frmConfig | AdminPanelForm / DigerAyarlarControl / ConfigForm | ✅ Mevcut |
| frmExcelOku | ExcelOkuForm | ✅ Mevcut |
| frmCokluFiyat | CokluFiyatForm | ✅ Mevcut |
| frmManuelUavtSorgu | ManuelUavtSorguForm | ✅ Mevcut |
| frmTramerSorgu | TramerSorguForm (ITramerService stub) | ✅ Mevcut |
| frmPoliceNoGit | PoliceNoGitForm → PolicelerimForm arama | ✅ Mevcut |
| frmTeklifNoGit | TeklifNoGitForm → TekliflerForm arama | ✅ Mevcut |
| frm loading / frmBildirimEkrani | LoadingForm, BildirimEkraniForm | ✅ Mevcut |
| frmSablonDuzenle (Otomatik_Sorgu) | SablonDuzenleForm | ✅ Mevcut |
| frmWSTeklifleriniSorgula | WSTeklifleriniSorgulaForm | ✅ Mevcut |
| UserControlRobotState (Loader) | SirketlerRobotForm (Giriş, Durdur, Tümüne Giriş, Tümünden Teklif Al) | ✅ Mevcut |
| YetkilendirmeHT | YetkilendirmeForm | ✅ Mevcut |

---

## 3. Servis / Veri Katmanı Karşılaştırma

| Open Hızlı Teklif | VegaAsis Windows | Durum |
|-------------------|------------------|-------|
| HizliTeklifMySqlServis (WCF) | VegaAsis.Data Services | ✅ EF6 + PostgreSQL |
| clsAuth | AuthService | ✅ Mevcut |
| clsPolice | PolicyService | ✅ Mevcut |
| clsServisTeklif / clsTeklifDetayBilgileri | OfferService | ✅ Mevcut |
| clsDaskFiyat / clsDaskTeminatlar | IDaskService, DaskServiceStub (GetTeminatlarAsync) | ✅ Stub mevcut |
| clsKaskoDetayServis / KaskoTeminat | IKaskoService, KaskoServiceStub (GetTeminatlarAsync) | ✅ Stub mevcut |
| clsImmFiyat | IImmService, ImmServiceStub (GetTeminatlarAsync) | ✅ Stub mevcut |
| clsKonutFiyat / clsKonutTeminatlar | IKonutService, KonutServiceStub (GetTeminatlarAsync) | ✅ Stub mevcut |
| clsTssFiyat / clsTssAileBireyleri | ITssService, TssServiceStub (GetAileBireyleriAsync) | ✅ Stub mevcut |
| clsSirketlerPaylasim | SharedCompanyService | ✅ Mevcut |
| clsSirketlerSecili / InsCompanys | CompanySettingsService | ✅ Mevcut |
| clsKota | QuotaSettingsService | ✅ Mevcut |
| clsKullanicilar / clsKullaniciServis | UserManagementService | ✅ Mevcut |
| clsDuyuru | IAnnouncementService, AnnouncementService | ✅ Mevcut |
| clsAjanda | AppointmentService | ✅ Mevcut |
| MongoTeklif / MongoClass | - | ✅ N/A (kullanılmıyor) |

---

## 4. Branş ve Web Servisi Modelleri

| Open Hızlı Teklif | VegaAsis Windows | Durum |
|-------------------|------------------|-------|
| WebServisClass (40+ sınıf) | VegaAsis.Core DTOs | ✅ Temel parametre DTO'ları eklendi |
| TrafikParametre, KonutParametre | TrafikParametreDto, KonutParametreDto | ✅ Mevcut |
| KullanimTarzlari, KasaTipleri | KullanimTarziOptions, KasaTipleri | ✅ Mevcut |
| Meslek, Cinsiyet, FuelsType | Professions, GenderOptions, FuelTypes | ✅ Mevcut |
| Citys (İl/İlçe) | TurkeyLocations | ✅ Mevcut |

---

## 5. Robot / Tarayıcı Altyapısı

| Open Hızlı Teklif | VegaAsis Windows | Durum |
|-------------------|------------------|-------|
| CompanysBot (90+ TRF_*.cs) | IBrowserDriver, ChromeBrowserDriver, ICompanyRobot, CompanyRobotRegistry, TRF_* stub’lar | ✅ Altyapı/stub mevcut |
| Tools/ChromiumDriver, CreateChromium | ChromeBrowserDriver (Selenium) | ✅ Mevcut |
| Tools/clsSelectedCompanys, InsCompanys | CompanySettingsService, SirketlerRobotForm | ✅ Mevcut |
| reCaptha / CapthaResolver | ICaptchaResolver, ManuelCaptchaResolver, ManualCaptchaForm | ✅ Mevcut (manuel; 3. parti API isteğe bağlı) |
| AllLogins, AllOffers | SirketlerRobotForm: Tümüne Giriş, Tümünden Teklif Al (sıralı; GetOfferAsync stub) | ✅ Altyapı mevcut |
| Tramer, TramerCevap, TramerIstek | ITramerService, TramerServiceStub, TramerSorguForm | ✅ Stub + UI mevcut |

---

## 6. Yapılacaklar Planı (Öncelik Sırasına Göre)

### Faz 1: Kritik Eksikler (1–2 Hafta) — Tamamlandı

| # | Görev | Açıklama | Durum |
|---|-------|----------|--------|
| 1.1 | **IndexViewControl → Sorgu Akışı** | Sorguyu Başlat, Duraklat, Yeni Sorgu Kaydet butonlarına iş mantığı bağla (MainForm + OfferService, SorguSession) | ✅ |
| 1.2 | **PolicyTypeSelect + Branş Formları** | TRAFİK/KASKO/TSS/DASK/KONUT/İMM seçimine göre ilgili formu aç; grid’de branş kolonlarına tıklamada form tetikle | ✅ Mevcut |
| 1.3 | **Veri Kaynakları** | İl/İlçe, Meslek, Kullanım Tarzı listelerini doldur (TurkeyLocations, Professions, KullanimTarziOptions) | ✅ Mevcut |
| 1.4 | **Trafik/Kasko Pol. Bilgisi Tab** | Kasko tab'ına Başlangıç/Bitiş T., fieldKey ve SorguSession senkronu eklendi | ✅ |
| 1.5 | **frmHizliDaskSorgu Benzeri DASK** | DaskDetaylariForm’u zenginleştir: İl/İlçe/Mahalle cascade, Yapı Tarzı, İnşa Yılı, Kat Sayısı | ✅ Mevcut |

### Faz 2: Form Tamamlama (2–3 Hafta) — Tamamlandı

| # | Görev | Açıklama | Durum |
|---|-------|----------|--------|
| 2.1 | **frmOncekiPolice** | Önceki poliçe bilgisi girişi (Araç/DASK) | ✅ OncekiPoliceForm |
| 2.2 | **UserControlAracOncekiPol / UserControlDaskOncekiPol** | Önceki poliçe UserControl’leri veya form içine entegre | ✅ AracOncekiPolControl, DaskOncekiPolControl |
| 2.3 | **frmExcelOku** | Excel’den toplu veri okuma | ✅ ExcelOkuForm (ClosedXML) |
| 2.4 | **frmCokluFiyat** | Çoklu fiyat karşılaştırma dialog | ✅ CokluFiyatForm |
| 2.5 | **frmPoliceNoGit / frmTeklifNoGit** | Poliçe/teklif numarasına git; filtre entegrasyonu | ✅ PoliceNoGitForm, TeklifNoGitForm → PolicelerimForm/TekliflerForm arama ile açılıyor |
| 2.6 | **frmDuyuruGonder** | Admin duyuru gönderme | ✅ DuyuruGonderForm (UI; Duyuru servisi ayrı) |
| 2.7 | **PDF İyileştirme** | frmPdf/frmPDFDuzen benzeri düzenleme, frmPdfUpload | ✅ PDFExportForm + TeklifDosyalariForm |

### Faz 3: Raporlar ve Grafikler (2 Hafta) — Tamamlandı

| # | Görev | Açıklama | Durum |
|---|-------|----------|--------|
| 3.1 | **RaporlarForm Grafik Entegrasyonu** | RaporGrafikForm (Chart), İl/Şirket bazlı ve diğer grafikler; IOfferService/IPolicyService ile gerçek veri | ✅ |
| 3.2 | **Diğer Grafikler** | Mesleklere Göre, Ürün Grafiği, Doğum Tarihi Portföy, Kesilen Poliçeler; tarih filtresi ile veri | ✅ |
| 3.3 | **Rapor Export** | Grafiği Kaydet (PNG/JPEG), Yazdır (PrintDocument + PrintPreviewDialog) | ✅ |

### Faz 4: Robot / Şirket Entegrasyonu (4+ Hafta – Büyük İş) — Altyapı tamamlandı

| # | Görev | Açıklama | Durum |
|---|-------|----------|--------|
| 4.1 | **CompanysBot Altyapısı** | Selenium/Playwright veya mevcut ChromiumDriver benzeri altyapı | ✅ IBrowserDriver, ChromeBrowserDriver, ICompanyRobot, TRF_* stub |
| 4.2 | **Şirket Robotları** | TRF_* sınıflarının taşınması (öncelikli şirketlerle başla) | ✅ Stub tamam; gerçek akış opsiyonel |
| 4.3 | **AllLogins / AllOffers** | Toplu giriş ve teklif çekme akışı | ✅ SirketlerRobotForm: Tümüne Giriş, Tümünden Teklif Al (stub) |
| 4.4 | **reCaptcha / CapthaResolver** | Captcha çözümü | ✅ ICaptchaResolver, ManuelCaptchaResolver, ManualCaptchaForm |
| 4.5 | **Tramer Entegrasyonu** | Tramer sorgu/cevap akışı | ✅ ITramerService, TramerServiceStub, TramerSorguForm; gerçek API isteğe bağlı |

### Faz 5: Destek ve Admin (1 Hafta) — Multi-Agent

**Kural:** Her agent aynı anda yalnızca **1 dosya** üzerinde çalışır. Paralel çalışma için farklı agent'lar farklı dosyalara atanır.

| Agent | Tek dosya (çalışma hedefi) | Görev |
|-------|----------------------------|--------|
| **5.A** | `src/VegaAsis.Windows/UserControls/PaylasilanSirketlerControl.cs` | Paylaşım formu: "Benimle Paylaşılanlar" grid, yeni paylaşım ekleme/silme, WinForms-Paralel-Agent-Aktarim-Plani.md maddeleri |
| **5.B** | `src/VegaAsis.Windows/UserControls/WebEkranlariControl.cs` | WEB Ekranları paneli: WebUser listesi, unlicensed_agent_only filtresi, CRUD arayüzü (servis 5.C'de) |
| **5.C** | `src/VegaAsis.Data/Services/WebUserService.cs` | WebUser CRUD, unlicensed_agent_only filtreleme, gerekli DTO/entity kullanımı |
| **5.D** | `src/VegaAsis.Windows/Forms/ConfigForm.cs` | Genel ayarlar: proxy (aktif/host/port/user/pass), benchmark ayarları, ConfigStorage ile kaydet/yükle |
| **5.E** | `src/VegaAsis.Windows/Forms/AdminPanelForm.cs` | Yönetim paneli: Tab host (Paylaşılan Şirketler / WEB Ekranları / Diğer Ayarlar), DI ile control'leri besleme |
| **5.F** | `src/VegaAsis.Windows/UserControls/DigerAyarlarControl.cs` | Diğer ayarlar paneli: AppSettings binding, admin-only alanlar |

**Sıra önerisi:** 5.C → 5.B (servis hazır olsun); 5.A, 5.D, 5.F paralel; son olarak 5.E (tab'ları bir araya getirir).

---

### Faz 6: Kalan Formlar ve İyileştirmeler — Multi-Agent

**Kural:** Her agent aynı anda yalnızca **1 dosya** üzerinde çalışır.

| Agent | Tek dosya (çalışma hedefi) | Görev |
|-------|----------------------------|--------|
| **6.A** | `src/VegaAsis.Windows/Forms/ManuelUavtSorguForm.cs` | Manuel UAVT sorgu ekranı: alanlar, sorgula butonu, sonuç gösterimi |
| **6.B** | `src/VegaAsis.Windows/Forms/TramerSorguForm.cs` | Tramer sorgu ekranı (veya SbmSorgusuForm ile ayrım netleştirilmiş Tramer ekranı) |
| **6.C** | `src/VegaAsis.Windows/Forms/LoadingForm.cs` | frmLoading: bekleme göstergesi, progress mesajı, modal kullanım |
| **6.D** | `src/VegaAsis.Windows/Forms/BildirimEkraniForm.cs` | frmBildirimEkrani: bildirim listesi / tek bildirim gösterimi |
| **6.E** | `src/VegaAsis.Windows/Forms/SablonDuzenleForm.cs` | Otomatik sorgu şablonu düzenleme (frmSablonDuzenle) |
| **6.F** | `src/VegaAsis.Windows/Forms/WSTeklifleriniSorgulaForm.cs` | WS tekliflerini toplu sorgulama ekranı (frmWSTeklifleriniSorgula) |
| **6.G** | `src/VegaAsis.Windows/UserControls/IndexViewControl.cs` | Layout: kaymalar, responsive (WinForms-Paralel-Agent-Aktarim-Plani.md Bölüm 1.1), anchor/dock düzenleri |

**Sıra önerisi:** 6.A, 6.B, 6.C, 6.D paralel; 6.E, 6.F paralel; 6.G en son (tüm formlar sabitlendikten sonra layout).

---

### Faz 7: Test, Dokümantasyon ve Son Rötuşlar — Multi-Agent

**Kural:** Her agent aynı anda yalnızca **1 dosya** üzerinde çalışır. Faz 7, kritik akışların testi, dokümantasyon ve tek-dosya rötuşlarıyla sınırlıdır.

| Agent | Tek dosya (çalışma hedefi) | Görev |
|-------|----------------------------|--------|
| **7.A** | `src/VegaAsis.Windows/Forms/MainForm.cs` | Menü/ toolbar tutarlılığı, form açma kapama, hata mesajları; son rötuşlar |
| **7.B** | `src/VegaAsis.Data/VegaAsisDbContext.cs` veya `src/VegaAsis.Data/Services/AuthService.cs` | Veri/oturum: migration uyumu, connection handling, gerekirse AuthService davranışı |
| **7.C** | `DOCS/Open-Hizli-Teklif-VegaAsis-Karsilastirma-Plani.md` | Bu planın güncellenmesi: tamamlanan maddeler, durum sütunu, tarih |
| **7.D** | `README.md` | Kurulum, çalıştırma, ortam (PostgreSQL, .NET 4.7), opsiyonel robot/Config bilgisi |
| **7.E** | `src/VegaAsis.Windows/Program.cs` veya `src/VegaAsis.Windows/ServiceLocator.cs` | Başlangıç: DI kayıtları, exception handler, tek dosyada toplanan bootstrap rötuşları |

**Sıra önerisi:** 7.B, 7.E paralel (altyapı); 7.A (ana form); 7.C ve 7.D paralel (doküman).

---

## 7. Özet Eksik Listesi (Hızlı Referans)

| Kategori | Durum | Not |
|----------|--------|-----|
| **Formlar** | ✅ Taşındı | OncekiPolice, ExcelOku, CokluFiyat, PoliceNoGit, TeklifNoGit, DuyuruGonder, ManuelUavtSorgu, TramerSorgu, Loading, BildirimEkrani, SablonDuzenle, WSTeklifleriniSorgula karşılıkları mevcut |
| **Grafik / Rapor** | ✅ RaporGrafikForm | Şirket/Meslek/Ürün/Doğum Tarihi/Kesilen Poliçeler + Kullanım Tarzı/Marka grafikleri gerçek veri ile |
| **Robot Altyapısı** | ✅ Altyapı/stub | IBrowserDriver, ICompanyRobot, TRF_* stub, ICaptchaResolver, ITramerService stub; gerçek teklif/API entegrasyonu isteğe bağlı |
| **Servisler** | ✅ Stub mevcut | DASK/Kasko/İMM/Konut/TSS: IDaskService, IKaskoService, IImmService, IKonutService, ITssService + stub’lar; Duyuru (IAnnouncementService) mevcut; gerçek API/DB isteğe bağlı |
| **WebServis DTO’ları** | ✅ Temel | Trafik/Kasko/Dask/Konut parametre DTO'ları eklendi; detaylar opsiyonel |

---

## 8. Teknik Notlar

### 8.1 Open Hızlı Teklif Kısıtları

- Kod decompile edilmiş; `Delegate*.smethod_0` gibi obfuscated çağrılar var.
- DevExpress lisansı gerekebilir; VegaAsis standart WinForms kullanıyor.
- MySQL/WCF yerine PostgreSQL/EF6 kullanılıyor – API farkları dikkate alınmalı.

### 8.2 Taşırken Dikkat Edilecekler

1. **ConnectSorgu benzeri session:** Sorgu sırasında seçili şirketler, müşteri/araç bilgileri, branch bilgisi tutulmalı.
2. **Branş bazlı grid:** Trafik, Kasko, TSS, DASK, Konut, İMM kolonlarına tıklanınca ilgili form açılmalı.
3. **subPrices / Diğer Fiyatlar:** Grid’de satır genişletme ile alt fiyatlar gösterilmeli.
4. **Şirket seçimi:** SirketSecimForm zaten var; IndexViewControl ile entegrasyon güçlendirilmeli.

### 8.3 Önerilen İlk Adım

**Faz 1.1–1.3** ile başlamak mantıklı:

1. IndexViewControl’deki Sorguyu Başlat/Yeni Sorgu Kaydet butonlarına mock veya gerçek iş mantığı ekle.
2. PolicyTypeSelect ComboBox ekle; seçime göre TrafikTeklifiForm, KaskoTeminatlariForm vb. aç.
3. İl, İlçe, Meslek, Kullanım Tarzı combo’larını TurkeyLocations, Professions, KullanimTarziOptions ile doldur.

---

*Bu plan, Open Hızlı Teklif (orijinal) ile VegaAsis Windows karşılaştırmasına dayanmaktadır. WinForms-Paralel-Agent-Aktarim-Plani.md ile birlikte kullanılmalıdır.*
