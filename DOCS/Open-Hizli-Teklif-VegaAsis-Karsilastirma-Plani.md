# Open Hızlı Teklif vs VegaAsis Windows – Karşılaştırma ve Yapılacaklar Planı

**Orijinal Proje:** `C:\Users\celik\Desktop\Open Hızlı Teklif Proj\Open Hizli Teklif-cleaned\Open_Hizli_Teklif`  
**Hedef Proje:** `vegaasistwindows` (.NET 4.7 WinForms + EF6 + PostgreSQL)

---

## 1. Genel Mimari Karşılaştırma

| Özellik | Open Hızlı Teklif (Orijinal) | VegaAsis Windows (Hedef) |
|---------|------------------------------|---------------------------|
| **UI Framework** | DevExpress (GridControl, XtraEditors, XtraPrinting) | Standart WinForms (DataGridView) |
| **Veri Kaynağı** | MySQL + WCF Servis (HizliTeklifMySqlServis) | PostgreSQL + EF6 |
| **Ana Form Yapısı** | Form1 + tab/panel ile UserControl geçişi | MainForm + _contentPanel ile Form/UserControl |
| **Merkezi Bağlantı** | ConnectSorgu (ekCls) – sorgu/session nesnesi | ServiceLocator + DI (AuthService, OfferService vb.) |
| **Şirket Robotları** | CompanysBot (90+ TRF_*.cs sınıfı – Selenium/Chromium) | Yok – henüz taşınmadı |
| **Kod Durumu** | Decompile/obfuscate (Delegate*.smethod_0) | Temiz C# |

---

## 2. Form / UserControl Eşleme Tablosu

### 2.1 Ana Form ve Gezinme

| Open Hızlı Teklif | VegaAsis Windows | Durum |
|-------------------|------------------|-------|
| Form1 (ana form) | MainForm | ✅ Mevcut |
| Tab/panel ile sayfa geçişi | Menü + OpenForm / ShowIndexView | ⚠️ Yapı farklı |
| ConnectSorgu → UserControl’lere geçiş | Service inject edilerek Form’lara | ✅ Daha modern |

### 2.2 Ana Ekran ve Teklif Sorgusu

| Open Hızlı Teklif | VegaAsis Windows | Durum |
|-------------------|------------------|-------|
| UserControlTeklifler (teklif listesi) | IndexViewControl (ana grid + sağ panel) | ⚠️ IndexViewControl teklif listesi değil, sorgu sonucu grid |
| - | TekliflerForm | ✅ Teklif listesi ayrı form |
| frmSirketSec | SirketSecimForm | ✅ Mevcut |
| frmKarekodTara | WebcamQRForm | ✅ Mevcut |
| frmHizliDaskSorgu | DaskDetaylariForm | ⚠️ Orijinal daha kapsamlı (İl/İlçe/Mahalle API, payload) |
| frmDaskTeminatlari | DaskDetaylariForm içinde | ⚠️ Orijinal userControlDaskTeminatlari ayrı |
| frmKonutTeminatlari | DaskDetaylariForm (DASK/KONUT birleşik) | ⚠️ Konut ayrı form olabilir |
| frmImmTeminatlari | ImmTeminatlariForm | ✅ Mevcut |
| frmTssDetay | TssDetaylariForm | ✅ Mevcut |
| frmKaskoEski / frmTeminatEkran / frmTeminatSec | KaskoTeminatlariForm | ⚠️ Orijinal 3 form, hedef tek form |

### 2.3 Poliçe ve Doküman

| Open Hızlı Teklif | VegaAsis Windows | Durum |
|-------------------|------------------|-------|
| UserControlPolicelerim | PolicelerimForm | ✅ Mevcut |
| UserControlPolicelerYeni | - | ❌ Eksik (yeni poliçe ekranı) |
| UserControlPoliceKaydet | PoliceDetayForm / TeklifDetayForm | ⚠️ Karşılığı belirsiz |
| UserControlAracOncekiPol | - | ❌ Eksik |
| UserControlDaskOncekiPol | - | ❌ Eksik |
| frmOncekiPolice | - | ❌ Eksik |
| frmPdf / frmPDFDuzen | PDFExportForm | ⚠️ Orijinal daha zengin |
| frmPdfUpload | TeklifDosyalariForm? | ⚠️ Karşılığı net değil |
| UserControlDocManager | TeklifDosyalariForm | ⚠️ Kısmi |

### 2.4 Raporlar

| Open Hızlı Teklif | VegaAsis Windows | Durum |
|-------------------|------------------|-------|
| UserControlRaporlar | RaporlarForm | ✅ Mevcut |
| UserControlCityGrafik | - | ❌ Eksik |
| UserControlSirketBazliGrafik | - | ❌ Eksik |
| UserControlMesleklereGoreTeklifGrafigi | - | ❌ Eksik |
| UserControlTeklifKullanimTarziGrafigi | - | ❌ Eksik |
| UserControlTeklifMarkaGrafigi | - | ❌ Eksik |
| UserControlTeklifOtorizasyonOranlari | - | ❌ Eksik |
| UserControlTeklifKomisyonKazanci | - | ❌ Eksik |
| UserControlUrunGrafigi | - | ❌ Eksik |
| UserControlDogumTarihiPortfoy | - | ❌ Eksik |
| UserControlKesilenPoliceler | - | ❌ Eksik |

### 2.5 Destek, Ajanda, Duyuru

| Open Hızlı Teklif | VegaAsis Windows | Durum |
|-------------------|------------------|-------|
| UserControlDestekTaleplerim | DestekTalepleriForm | ✅ Mevcut |
| UserControlCalendar | AjandaYenilemeForm | ✅ Mevcut |
| UserControlDuyurular | DuyurularForm | ✅ Mevcut |
| UserControlCanliDestek | CanliDestekForm | ✅ Mevcut |
| UserControlCanliUretim | CanliUretimForm | ✅ Mevcut |
| frmDuyuruGonder | - | ❌ Eksik (admin duyuru gönder) |
| frmDuyuruKullanici | DuyurularForm içinde | ⚠️ Birleşik olabilir |

### 2.6 Diğer Formlar (Open Hızlı Teklif’te Var)

| Open Hızlı Teklif | VegaAsis Windows | Durum |
|-------------------|------------------|-------|
| frmConfig | AdminPanelForm / DigerAyarlarControl | ⚠️ Kısmi |
| frmExcelOku | - | ❌ Eksik |
| frmCokluFiyat | - | ❌ Eksik |
| frmManuelUavtSorgu | - | ❌ Eksik |
| frmTramerSorgu | SbmSorgusuForm? | ⚠️ Tramer farklı olabilir |
| frmPoliceNoGit | - | ❌ Eksik |
| frmTeklifNoGit | - | ❌ Eksik |
| frm loading / frmBildirimEkrani | - | ❌ Eksik |
| frmSablonDuzenle (Otomatik_Sorgu) | - | ❌ Eksik |
| frmWSTeklifleriniSorgula | - | ❌ Eksik |
| UserControlRobotState (Loader) | SirketlerRobotForm | ⚠️ Robot durumu farklı |
| YetkilendirmeHT | YetkilendirmeForm | ✅ Mevcut |

---

## 3. Servis / Veri Katmanı Karşılaştırma

| Open Hızlı Teklif | VegaAsis Windows | Durum |
|-------------------|------------------|-------|
| HizliTeklifMySqlServis (WCF) | VegaAsis.Data Services | ✅ EF6 + PostgreSQL |
| clsAuth | AuthService | ✅ Mevcut |
| clsPolice | PolicyService | ✅ Mevcut |
| clsServisTeklif / clsTeklifDetayBilgileri | OfferService | ✅ Mevcut |
| clsDaskFiyat / clsDaskTeminatlar | - | ⚠️ DASK servisi eksik |
| clsKaskoDetayServis / KaskoTeminat | - | ⚠️ Kasko servisi eksik |
| clsImmFiyat | - | ⚠️ İMM servisi eksik |
| clsKonutFiyat / clsKonutTeminatlar | - | ⚠️ Konut servisi eksik |
| clsTssFiyat / clsTssAileBireyleri | - | ⚠️ TSS servisi eksik |
| clsSirketlerPaylasim | SharedCompanyService | ✅ Mevcut |
| clsSirketlerSecili / InsCompanys | CompanySettingsService | ✅ Mevcut |
| clsKota | QuotaSettingsService | ✅ Mevcut |
| clsKullanicilar / clsKullaniciServis | UserManagementService | ✅ Mevcut |
| clsDuyuru | - | ⚠️ Duyuru servisi eksik |
| clsAjanda | AppointmentService | ✅ Mevcut |
| MongoTeklif / MongoClass | - | ❌ MongoDB entegrasyonu yok |

---

## 4. Branş ve Web Servisi Modelleri

| Open Hızlı Teklif | VegaAsis Windows | Durum |
|-------------------|------------------|-------|
| WebServisClass (40+ sınıf) | VegaAsis.Core DTOs | ⚠️ TrafikParametre, KaskoParametre, DaskParametre vb. eksik/yarım |
| TrafikParametre, KonutParametre | - | ❌ DTO taşınmalı |
| KullanimTarzlari, KasaTipleri | KullanimTarziOptions | ⚠️ Kısmi |
| Meslek, Cinsiyet, FuelsType | Professions | ⚠️ Kısmi |
| Citys (İl/İlçe) | TurkeyLocations | ✅ Mevcut |

---

## 5. Robot / Tarayıcı Altyapısı

| Open Hızlı Teklif | VegaAsis Windows | Durum |
|-------------------|------------------|-------|
| CompanysBot (90+ TRF_*.cs) | - | ❌ Tamamen eksik |
| Tools/ChromiumDriver, CreateChromium | - | ❌ Eksik |
| Tools/clsSelectedCompanys, InsCompanys | - | ⚠️ CompanySettings ile kısmen |
| reCaptha / CapthaResolver | - | ❌ Eksik |
| AllLogins, AllOffers | - | ❌ Eksik |
| Tramer, TramerCevap, TramerIstek | - | ❌ Eksik |

---

## 6. Yapılacaklar Planı (Öncelik Sırasına Göre)

### Faz 1: Kritik Eksikler (1–2 Hafta)

| # | Görev | Açıklama | Bağımlılık |
|---|-------|----------|------------|
| 1.1 | **IndexViewControl → Sorgu Akışı** | Sorguyu Başlat, Duraklat, Yeni Sorgu Kaydet butonlarına iş mantığı bağla | OfferService, ConnectSorgu benzeri session |
| 1.2 | **PolicyTypeSelect + Branş Formları** | TRAFİK/KASKO/TSS/DASK/KONUT/İMM seçimine göre ilgili formu aç; grid’de branş kolonlarına tıklamada form tetikle | Mevcut formlar |
| 1.3 | **Veri Kaynakları** | İl/İlçe, Meslek, Kullanım Tarzı listelerini doldur (TurkeyLocations, Professions, KullanimTarziOptions) | Data klasörü |
| 1.4 | **Trafik/Kasko Pol. Bilgisi Tab** | IndexViewControl’deki boş tab’lara React’taki alanları ekle | - |
| 1.5 | **frmHizliDaskSorgu Benzeri DASK** | DaskDetaylariForm’u zenginleştir: İl/İlçe/Mahalle cascade, Yapı Tarzı, İnşa Yılı, Kat Sayısı | TurkeyLocations |

### Faz 2: Form Tamamlama (2–3 Hafta)

| # | Görev | Açıklama |
|---|-------|----------|
| 2.1 | **frmOncekiPolice** | Önceki poliçe bilgisi girişi (Araç/DASK) |
| 2.2 | **UserControlAracOncekiPol / UserControlDaskOncekiPol** | Önceki poliçe UserControl’leri veya form içine entegre |
| 2.3 | **frmExcelOku** | Excel’den toplu veri okuma |
| 2.4 | **frmCokluFiyat** | Çoklu fiyat karşılaştırma dialog |
| 2.5 | **frmPoliceNoGit / frmTeklifNoGit** | Poliçe/teklif numarasına git |
| 2.6 | **frmDuyuruGonder** | Admin duyuru gönderme |
| 2.7 | **PDF İyileştirme** | frmPdf/frmPDFDuzen benzeri düzenleme, frmPdfUpload |

### Faz 3: Raporlar ve Grafikler (2 Hafta)

| # | Görev | Açıklama |
|---|-------|----------|
| 3.1 | **RaporlarForm Grafik Entegrasyonu** | UserControlCityGrafik, UserControlSirketBazliGrafik benzeri grafikler |
| 3.2 | **Diğer Grafikler** | MesleklereGoreTeklifGrafigi, TeklifKullanimTarziGrafigi, TeklifMarkaGrafigi vb. |
| 3.3 | **Rapor Export** | DevExpress XtraPrinting yerine standart PrintDocument veya 3. parti kütüphane |

### Faz 4: Robot / Şirket Entegrasyonu (4+ Hafta – Büyük İş)

| # | Görev | Açıklama |
|---|-------|----------|
| 4.1 | **CompanysBot Altyapısı** | Selenium/Playwright veya mevcut ChromiumDriver benzeri altyapı |
| 4.2 | **Şirket Robotları** | TRF_* sınıflarının taşınması (öncelikli şirketlerle başla) |
| 4.3 | **AllLogins / AllOffers** | Toplu giriş ve teklif çekme akışı |
| 4.4 | **reCaptcha / CapthaResolver** | Captcha çözümü |
| 4.5 | **Tramer Entegrasyonu** | Tramer sorgu/cevap akışı |

### Faz 5: Destek ve Admin (1 Hafta)

| # | Görev | Açıklama |
|---|-------|----------|
| 5.1 | **PaylasilanSirketlerControl** | Yeni paylaşım formu, Benimle Paylaşılanlar paneli (WinForms-Paralel-Agent-Aktarim-Plani.md’deki maddeler) |
| 5.2 | **WebEkranlariControl** | WebUserService, unlicensed_agent_only, CRUD |
| 5.3 | **frmConfig Benzeri** | Genel ayarlar, proxy, benchmark vb. |

### Faz 6: Kalan Formlar ve İyileştirmeler

| # | Görev | Açıklama |
|---|-------|----------|
| 6.1 | **frmManuelUavtSorgu** | Manuel UAVT sorgu ekranı |
| 6.2 | **frmTramerSorgu** | Tramer sorgu ekranı |
| 6.3 | **Loading / Bildirim** | frmLoading, frmBildirimEkrani |
| 6.4 | **Otomatik Sorgu** | frmSablonDuzenle, frmWSTeklifleriniSorgula |
| 6.5 | **Layout Düzeltmeleri** | WinForms-Paralel-Agent-Aktarim-Plani.md Bölüm 1.1 (kaymalar, responsive) |

---

## 7. Özet Eksik Listesi (Hızlı Referans)

| Kategori | Eksik Sayısı | Örnekler |
|----------|--------------|----------|
| **Formlar** | ~15 | frmOncekiPolice, frmExcelOku, frmCokluFiyat, frmPoliceNoGit, frmTeklifNoGit, frmDuyuruGonder, frmManuelUavtSorgu, frmTramerSorgu |
| **UserControls** | ~12 | Grafik kontrolleri (9 adet), UserControlAracOncekiPol, UserControlDaskOncekiPol, UserControlPoliceKaydet |
| **Servisler** | ~6 | DASK, Kasko, İMM, Konut, TSS detay servisleri; Duyuru servisi |
| **Robot Altyapısı** | Tam set | CompanysBot, ChromiumDriver, AllLogins, AllOffers, reCaptcha |
| **WebServis DTO’ları** | ~30 | TrafikParametre, KaskoParametre, DaskParametre, KonutParametre vb. |

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
