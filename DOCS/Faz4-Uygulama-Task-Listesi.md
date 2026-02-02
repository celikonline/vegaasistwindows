# Faz 4 – Uygulama Task Listesi (Adım Adım)

Bu doküman Faz 4’ü uygularken takip edilecek sıralı task listesidir. Tamamlanan maddeler `[x]` ile işaretlenir.

---

## Faz 4.1: CompanysBot Altyapısı

- [x] **4.1.1** NuGet: `Selenium.WebDriver` ve `Selenium.WebDriver.ChromeDriver` paketlerini VegaAsis.Windows projesine ekle.
- [x] **4.1.2** `VegaAsis.Windows/Robot/` klasörünü oluştur; `IBrowserDriver.cs` arayüzünü ekle (Navigate, GetCurrentUrl, Close, Dispose vb.).
- [x] **4.1.3** `ChromeBrowserDriver.cs`: IBrowserDriver implementasyonu; ChromeOptions (headed/headless), ChromeDriverService, WebDriver örneği oluştur ve yönet.
- [x] **4.1.4** SirketlerRobotForm: "Başlat" tıklanınca (veya Başlat menüsünden "Chrome ile Aç") ChromeBrowserDriver oluştur, aktif şirket URL’ine git; form kapatılırken driver’ı kapat.
- [x] **4.1.5** (Opsiyonel) App.config’e `ChromeDriverPath`, `Headless` gibi ayarlar ekle; driver timeout ayarla.

---

## Faz 4.2: Şirket Robotları (İlk Adım)

- [x] **4.2.1** `ICompanyRobot.cs` arayüzü: LoginAsync, GetOfferAsync, CompanyId/CompanyName, LoginUrl.
- [x] **4.2.2** `CompanyRobotRegistry.cs`: Şirket kodu/adı → ICompanyRobot eşlemesi; ak, ana, anadolu kayıtlı.
- [x] **4.2.3** `TRF_AkSigorta.cs`, `TRF_AnaSigortaStub.cs`, `TRF_AnadoluStub.cs`: ICompanyRobot stub; LoginAsync ile Navigate(loginUrl); SirketlerRobotForm "Chrome ile Aç" robot üzerinden çalışıyor.

---

## Faz 4.3: AllLogins / AllOffers (Stub)

- [x] **4.3.1** `AllLoginsRunner.cs`: Seçili şirket listesini al (ICompanySettingsService); her biri için ICompanyRobot.LoginAsync çağır (stub: sadece log); başarı/hata listesi döndür.
- [x] **4.3.2** SirketlerRobotForm’a "Tümüne Giriş" butonu ekle; tıklanınca AllLoginsRunner çalıştır, sonucu MessageBox veya basit log alanında göster.

---

## Faz 4.4: Captcha (Manuel)

- [x] **4.4.1** `ICaptchaResolver.cs` arayüzü: SolveAsync(byte[]) ve SolveAsync(imageBase64) → string (mevcut).
- [x] **4.4.2** `ManuelCaptchaResolver.cs`: Kullanıcıya ManuelCaptchaForm gösterir; girilen metni döndürür. ManuelCaptchaForm: resim + TextBox + OK/İptal.

---

## Faz 4.5: Tramer (Stub / API hazırlığı)

- [x] **4.5.1** Tramer API dokümanı veya endpoint bilgisi topla. (Varsayılan REST/GET + opsiyonel API key header tanımlandı)
- [x] **4.5.2** `ITramerService.cs` ve stub implementasyon; TramerSorguForm’dan çağrılacak şekilde bağla.

---

## Sıra Özeti

1. 4.1.1 → 4.1.2 → 4.1.3 → 4.1.4 → (4.1.5)  
2. 4.2.1 → 4.2.2 → 4.2.3  
3. 4.3.1 → 4.3.2  
4. 4.4.1 → 4.4.2  
5. 4.5.1 → 4.5.2  

İlk uygulama: **4.1.1** (NuGet) ile başlanır.
