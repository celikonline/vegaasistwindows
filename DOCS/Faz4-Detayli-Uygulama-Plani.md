# Faz 4 – Detaylı Uygulama Planı

**Süre:** 4+ hafta (büyük iş)  
**Kaynak:** Open-Hizli-Teklif-VegaAsis-Karsilastirma-Plani.md, Bölüm 5–6 – Robot / Şirket Entegrasyonu

---

## Genel Bakış

Faz 4’ün amacı: Open Hızlı Teklif’teki **CompanysBot** (90+ TRF_*.cs şirket robotu), **ChromiumDriver** benzeri tarayıcı altyapısı, **AllLogins / AllOffers** (toplu giriş ve teklif çekme), **reCaptcha / CapthaResolver** ve **Tramer** entegrasyonunu VegaAsis’e taşımak veya eşdeğer bir mimari kurmak. Bu faz teknik olarak en ağır fazdır; Selenium/Playwright veya Chromium tabanlı otomasyon, captcha çözümü ve şirket portallarına özel akışlar içerir.

---

## Mevcut Durum

| Bileşen | Durum | Açıklama |
|---------|--------|----------|
| SirketlerRobotForm | ✅ Mevcut (placeholder) | Şirket bar (ANADOLU, ANA, ALLIANZ, AK), sekmeler, toolbar (Geri/İleri/Yenile/Home, URL, Giriş, Başlat/Duraklat/Durdur, + Yeni), **WebBrowser** kontrolü (IE/Edge legacy), durum çubuğu. Gerçek robot mantığı yok. |
| ICompanySettingsService | ✅ Mevcut | GetSelectedCompaniesAsync, GetCompanySetting, SaveCompanySettingAsync, DeleteCompanySettingAsync, LoadSettingsAsync. Şirket seçimleri ve ayarları DB’de. |
| SirketSecimForm | ✅ Mevcut | Şirket seçimi; IndexViewControl / MainForm ile entegre. |
| CompanysBot / TRF_* | ❌ Eksik | Open Hızlı Teklif’te 90+ şirket robotu (Selenium/Chromium). VegaAsis’te yok. |
| ChromiumDriver / Selenium / Playwright | ❌ Eksik | Tarayıcı otomasyon altyapısı. SirketlerRobotForm şu an WebBrowser (IE) kullanıyor. |
| AllLogins / AllOffers | ❌ Eksik | Toplu giriş ve toplu teklif çekme akışı. |
| reCaptcha / CapthaResolver | ❌ Eksik | Captcha çözüm servisi. |
| Tramer (TramerCevap, TramerIstek) | ❌ Eksik | Tramer sorgu/cevap; frmTramerSorgu / SbmSorgusuForm ile ilişkili olabilir. |

---

## Görev 4.1: CompanysBot Altyapısı (Tarayıcı Otomasyon)

### Amaç

Şirket portallarına otomatik giriş ve teklif çekmek için tarayıcı otomasyon altyapısı kurmak. Open Hızlı Teklif’te **ChromiumDriver** / Selenium kullanılıyor; VegaAsis’te **Selenium WebDriver** (Chrome/Edge) veya **Playwright for .NET** ile benzeri sağlanabilir.

### Yapılacaklar

| Adım | Açıklama | Dosya / Konum | Tahmini |
|------|----------|----------------|---------|
| 4.1.1 | **Kütüphane seçimi**: Selenium WebDriver (Chrome/Edge) veya Playwright for .NET. .NET Framework 4.8 uyumluluğu (Selenium 3.x/4.x) veya .NET Standard 2.0 (Playwright) | Proje referansları | 0,5 gün |
| 4.1.2 | **IBrowserDriver / IWebDriverFactory**: Tarayıcı örneği oluşturma, kapatma, headless/headed ayarı. Ortak arayüz ile Selenium veya Playwright implementasyonu değiştirilebilir | VegaAsis.Windows/Robot/ veya VegaAsis.Core/Contracts/ | 1 gün |
| 4.1.3 | **Driver yaşam döngüsü**: SirketlerRobotForm veya ana robot koordinatörü tarafından tek/çoklu sekme yönetimi; timeout, sayfa yükleme bekleme | Robot/ | 1 gün |
| 4.1.4 | **SirketlerRobotForm entegrasyonu**: WebBrowser yerine Selenium/Playwright ile açılan tarayıcıyı göstermek veya “harici tarayıcı” modunda çalıştırmak (form sadece Başlat/Durdur ve log göstersin) | SirketlerRobotForm.cs | 1–2 gün |
| 4.1.5 | **Chrome/Edge binary yolu ve sürücü**: ChromeDriver/msedgedriver PATH veya NuGet ile yönetim; gerekirse Tools klasöründe lokal driver | App.config, paketler | 0,5 gün |

### Teknik Notlar

- **Selenium**: `WebDriver.ChromeDriver` / `WebDriver.EdgeDriver` NuGet; .NET Framework 4.8 ile uyumlu.
- **Playwright**: `Microsoft.Playwright`; .NET Framework için resmi destek sınırlı olabilir; .NET Core/5+ tercih edilir. VegaAsis .NET 4.8 ise Selenium daha uygun.
- **Headless**: Sunucu/arka planda çalışma için headless mod; UI test için headed.

### Bağımlılıklar

- Chrome veya Edge yüklü (veya headless için ilgili binary).
- NuGet: Selenium.WebDriver, Selenium.WebDriver.ChromeDriver (veya EdgeDriver).

---

## Görev 4.2: Şirket Robotları (TRF_* Taşınması)

### Amaç

Open Hızlı Teklif’teki şirket bazlı robot sınıflarını (TRF_Anadolu, TRF_AkSigorta vb.) VegaAsis’e taşımak veya aynı işi yapan yeni sınıflar yazmak. Her şirket için: giriş sayfası, kullanıcı/şifre alanları, captcha (varsa), teklif formu doldurma, sonuç okuma.

### Yapılacaklar

| Adım | Açıklama | Dosya / Konum | Tahmini |
|------|----------|----------------|---------|
| 4.2.1 | **Ortak arayüz**: ICompanyRobot veya IŞirketRobot (LoginAsync, FillOfferAsync, GetResultAsync vb.); her TRF_* bu arayüzü uygular | VegaAsis.Core/Contracts/ veya VegaAsis.Windows/Robot/ | 0,5 gün |
| 4.2.2 | **Robot kayıt / bulma**: Şirket adı veya kodu → ICompanyRobot implementasyonu. Fabrika veya DI ile eşleme | Robot/CompanyRobotRegistry.cs benzeri | 0,5 gün |
| 4.2.3 | **Öncelikli şirketlerle başlama**: 2–3 şirket (örn. AK, ANADOLU, ANA) için stub veya gerçek akış; giriş URL’i, selector’lar config veya sabit | Robot/TRF_AkSigorta.cs vb. | 2–5 gün / şirket |
| 4.2.4 | **Selector / URL yönetimi**: Her şirket için URL ve CSS/XPath selector’ları config (JSON/DB) veya sabit; değişiklikde kod değiştirmemek | App.config, ayar tablosu veya JSON | 1 gün |
| 4.2.5 | **Hata ve log**: Timeout, element bulunamadı, captcha bekleniyor; loglama ve kullanıcıya mesaj | Tüm robot sınıfları | Sürekli |

### Öncelik Sırası (Örnek)

1. AK Sigorta (mevcut formda örnek URL var)  
2. ANA Sigorta  
3. Anadolu Sigorta  
4. Diğer şirketler (liste Open Hızlı Teklif’e göre genişletilir)

### Bağımlılıklar

- 4.1 tamamlanmış olmalı (tarayıcı driver).
- ICompanySettingsService: şirket adı, giriş bilgisi (hassas veri için güvenli saklama ayrı konu).

---

## Görev 4.3: AllLogins / AllOffers

### Amaç

Seçili şirketlere **toplu giriş** (AllLogins) ve **toplu teklif çekme** (AllOffers) akışı. IndexViewControl’de “Sorguyu Başlat” veya SirketlerRobotForm’da “Tümüne Giriş” / “Tümünden Teklif Al” benzeri.

### Yapılacaklar

| Adım | Açıklama | Dosya / Konum | Tahmini |
|------|----------|----------------|---------|
| 4.3.1 | **AllLogins**: Seçili şirket listesi (CompanySettingsService’ten); her biri için ICompanyRobot.LoginAsync; paralel veya sıralı; başarı/hata özeti | Robot/AllLoginsRunner.cs veya servis | 1–2 gün |
| 4.3.2 | **AllOffers**: Sorgu parametreleri (SorguSession benzeri); her şirket için teklif formu doldurma ve sonuç toplama; IndexViewControl grid’e veya geçici yapıya yazma | Robot/AllOffersRunner.cs | 2–3 gün |
| 4.3.3 | **İptal / Duraklat**: Uzun süren toplu işlemde iptal ve duraklat; thread/task iptali | Runner sınıfları | 0,5 gün |
| 4.3.4 | **UI bağlantısı**: SirketlerRobotForm’da “Tümüne Giriş”, “Tümünden Teklif Al” butonları; ProgressBar veya durum metni; MainForm/IndexView ile veri paylaşımı (opsiyonel) | SirketlerRobotForm.cs | 1 gün |

### Bağımlılıklar

- 4.1 (driver), 4.2 (en az bir şirket robotu).
- SorguSession veya ortak DTO: teklif parametreleri.

---

## Görev 4.4: reCaptcha / CapthaResolver

### Amaç

Şirket portallarında çıkan captcha’ları otomatik veya yarı otomatik çözmek. Open Hızlı Teklif’te **CapthaResolver** veya 3. parti servis kullanılıyor olabilir.

### Yapılacaklar

| Adım | Açıklama | Dosya / Konum | Tahmini |
|------|----------|----------------|---------|
| 4.4.1 | **ICaptchaResolver arayüzü**: SolveAsync(stream/image veya sayfa URL’i) → string (cevap metni). Mock implementasyon: manuel giriş dialog | VegaAsis.Core/Contracts/ veya Robot/ | 0,5 gün |
| 4.4.2 | **Manuel çözüm**: Captcha görüntüsü gösterilir, kullanıcı metin girer; robot bu metni ilgili alana yazar. En az bu olmalı | Robot/ManuelCaptchaResolver.cs, form/dialog | 1 gün |
| 4.4.3 | **(Opsiyonel) 3. parti API**: 2Captcha, Anti-Captcha vb. ücretli servis; API key config’de; rate limit ve hata yönetimi | Robot/ExternalCaptchaResolver.cs | 1–2 gün |
| 4.4.4 | **Robot entegrasyonu**: Giriş/teklif akışında captcha tespiti (element veya sayfa içeriği); resolver çağrısı ve sonucu forma yazma | TRF_* sınıfları | 0,5 gün / akış |

### Bağımlılıklar

- 4.1, 4.2 (robot akışında captcha adımı).

---

## Görev 4.5: Tramer Entegrasyonu

### Amaç

**Tramer** sorgu/cevap akışı: araç hasar geçmişi veya benzeri bilginin Tramer sisteminden alınması. Open Hızlı Teklif’te TramerIstek, TramerCevap vb. sınıflar var; VegaAsis’te TramerSorguForm / SbmSorgusuForm ile ilişkilendirilebilir.

### Yapılacaklar

| Adım | Açıklama | Dosya / Konum | Tahmini |
|------|----------|----------------|---------|
| 4.5.1 | **Tramer API / protokol**: Mevcut Tramer entegrasyon dokümanı veya Open Hızlı Teklif’teki kullanım (HTTP, SOAP, vb.); VegaAsis’te hangi endpoint ve format kullanılacak | Doküman / mevcut kod incelemesi | 0,5–1 gün |
| 4.5.2 | **ITramerService veya TramerClient**: Sorgu gönder, cevap oku; DTO’lar (TramerSorguDto, TramerCevapDto) | VegaAsis.Core/Contracts/, VegaAsis.Data/Services/ veya ayrı client projesi | 1–2 gün |
| 4.5.3 | **TramerSorguForm / SbmSorgusuForm**: Form alanlarından parametre al, servisi çağır, sonucu göster | Forms/TramerSorguForm.cs, SbmSorgusuForm.cs | 1 gün |
| 4.5.4 | **Hata ve timeout**: Ağ hataları, zaman aşımı, yetki hataları | TramerClient | 0,5 gün |

### Not

- Tramer’in resmi API’si veya kullanım koşulları proje sahibince netleştirilmeli; sadece teknik altyapı planı burada.

### Bağımlılıklar

- Tramer erişim bilgileri (URL, kullanıcı/şifre veya token).

---

## Uygulama Sırası (Önerilen)

```
Hafta 1:  4.1 (CompanysBot altyapısı)     → Driver, arayüz, SirketlerRobotForm’a bağlama
Hafta 2:  4.2.1–4.2.3 (Şirket robotu)    → ICompanyRobot, 1–2 şirket stub/gerçek
Hafta 3:  4.3 (AllLogins / AllOffers)    → Toplu giriş ve teklif akışı
          4.4.1–4.4.2 (Captcha manuel)   → Arayüz + manuel çözüm
Hafta 4:  4.2.4–4.2.5 (Selector, log)    → Config ve hata yönetimi
          4.4.3–4.4.4 (Captcha opsiyonel)
          4.5 (Tramer)                   → API netleşirse
```

---

## Bağımlılık Grafiği

```
4.1 (Driver / Altyapı)
  │
  ├──► 4.2 (Şirket Robotları TRF_*)
  │         │
  │         ├──► 4.3 (AllLogins / AllOffers)
  │         │
  │         └──► 4.4 (Captcha)
  │
  └──► 4.5 (Tramer)  [bağımsız; sadece API bilgisi]
```

- 4.1 olmadan 4.2 ve 4.3 yapılamaz.
- 4.2’de en az bir robot olmadan 4.3 anlamlı değil.
- 4.4, 4.2 ile birlikte (captcha gerektiren şirketlerde) kullanılır.
- 4.5, diğerlerinden bağımsız; Tramer API’ye bağlı.

---

## Test Kontrol Listesi

- [ ] Tarayıcı (Chrome/Edge) Selenium/Playwright ile açılıyor ve kapatılıyor  
- [ ] SirketlerRobotForm’da “Başlat” ile otomasyon tarayıcısı veya sekme açılıyor  
- [ ] En az bir şirket için giriş sayfasına gidiliyor (manuel giriş yeterli)  
- [ ] AllLogins: Seçili 2+ şirket için giriş denemesi; başarı/hata listesi  
- [ ] AllOffers: Tek şirket için teklif parametreleri gönderiliyor; sonuç (mock bile) alınıyor  
- [ ] Captcha çıktığında manuel çözüm dialog’u açılıyor ve cevap forma yazılıyor  
- [ ] Tramer: Sorgu gönderildiğinde cevap veya hata mesajı alınıyor (API erişimi varsa)  

---

## Dosya / Klasör Özeti

| Konum | İşlem | Açıklama |
|-------|--------|----------|
| VegaAsis.Core/Contracts/ | Yeni / Genişletme | ICompanyRobot, ICaptchaResolver, (ITramerService) |
| VegaAsis.Windows/Robot/ veya VegaAsis.Data/Robot/ | Yeni | BrowserDriverFactory, AllLoginsRunner, AllOffersRunner, CompanyRobotRegistry |
| VegaAsis.Windows/Robot/TRF_*.cs | Yeni | TRF_AkSigorta, TRF_Anadolu vb. (şirket bazlı) |
| VegaAsis.Windows/Forms/SirketlerRobotForm.cs | Güncelleme | WebBrowser yerine driver tabanlı akış veya harici tarayıcı + kontrol |
| VegaAsis.Windows/Forms/ManuelCaptchaForm.cs | Yeni (opsiyonel) | Captcha görüntüsü + TextBox + OK |
| App.config / ayar tablosu | Yeni | Chrome/Edge path, driver timeout, Tramer URL/API key (gizli) |

---

## Risk ve Notlar

- **Lisans / yasal**: Otomasyon ve captcha çözümü şirket kullanım koşullarına aykırı olabilir; proje sahibi kontrol etmeli.  
- **Bakım**: Şirket siteleri sık değişir; selector ve akış güncellemesi gerekir. Config/veritabanı ile yönetim önerilir.  
- **Güvenlik**: Giriş bilgileri ve API anahtarları şifreli veya güvenli depolamada tutulmalı.  
- **Open Hızlı Teklif kodu**: Decompile edilmiş ve obfuscated olabilir; TRF_* mantığı elle yeniden yazılabilir veya sadece akış dokümante edilip VegaAsis’te sıfırdan implemente edilir.

---

*Bu plan Faz 4 için detaylı rehber niteliğindedir. Süre ve öncelik proje kısıtlarına göre güncellenebilir.*
