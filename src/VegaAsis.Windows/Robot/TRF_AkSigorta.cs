using System;
using System.Threading.Tasks;

namespace VegaAsis.Windows.Robot
{
    /// <summary>
    /// AK Sigorta şirket robotu. Faz 4.2 – giriş ve teklif akışı; selector'lar gerçek portal HTML'ine göre güncellenir.
    /// </summary>
    public class TRF_AkSigorta : ICompanyRobot
    {
        public string CompanyId => "ak";
        public string CompanyName => "AK Sigorta";
        public string LoginUrl => "https://sat2.aksigorta.com.tr/auth/log";

        // Placeholder selectors – gerçek portala göre güncellenecek (örn. Open Hızlı Teklif / sat2 HTML)
        private const string SelectorUserName = "input[name=\"username\"], input[type=\"text\"][name*=\"user\"], #username";
        private const string SelectorPassword = "input[name=\"password\"], input[type=\"password\"], #password";
        private const string SelectorLoginButton = "button[type=\"submit\"], input[type=\"submit\"], .btn-login, button.btn-primary";

        public async Task<bool> LoginAsync(IBrowserDriver driver)
        {
            if (driver == null || !driver.IsActive) return false;
            try
            {
                driver.Navigate(LoginUrl);
                await Task.Delay(2000).ConfigureAwait(false);

                // Giriş formu yüklenene kadar bekle (ilk bulunan form alanı)
                var formReady = driver.WaitForElement(SelectorUserName, 10000) ?? driver.WaitForElement(SelectorPassword, 3000);
                if (formReady == null)
                    return true;

                // Kullanıcı adı / şifre: AppSettings veya Config'ten okunmalı; burada placeholder (boş = sadece tıklama akışı)
                string user = null, pass = null; // TODO: AppSettingsService veya CompanySettings'ten al
                try
                {
                    if (!string.IsNullOrEmpty(user)) driver.SendKeys("#username", user);
                    if (!string.IsNullOrEmpty(pass)) driver.SendKeys("#password", pass);
                }
                catch
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(user)) driver.SendKeys("input[type=\"text\"]", user);
                        if (!string.IsNullOrEmpty(pass)) driver.SendKeys("input[type=\"password\"]", pass);
                    }
                    catch { /* alanlar farklı isimde olabilir */ }
                }

                // Captcha: Bu workspace'te ICaptchaResolver yok; varsa driver üzerinden captcha görseli alınıp
                // resolver'a verilir, sonuç ilgili alana yazılır. Placeholder: captcha alanı varsa manuel girilmeli.
                // if (captchaElement != null && _captchaResolver != null) { var solution = await _captchaResolver.SolveAsync(...); driver.SendKeys("#captcha", solution); }

                try
                {
                    driver.Click(SelectorLoginButton);
                }
                catch
                {
                    driver.Click("button[type=\"submit\"]");
                }

                await Task.Delay(2000).ConfigureAwait(false);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<string> GetOfferAsync(IBrowserDriver driver, object offerParams)
        {
            if (driver == null || !driver.IsActive) return null;
            try
            {
                // Teklif formu sayfasına geçiş – gerçek URL/route portala göre güncellenir
                var offerUrl = "https://sat2.aksigorta.com.tr/offer"; // placeholder
                driver.Navigate(offerUrl);
                await Task.Delay(2000).ConfigureAwait(false);

                // offerParams: plaka, TC vb. – gerçek portala göre property/alan eşlemesi yapılır
                var plaka = "";
                var tckn = "";
                if (offerParams != null)
                {
                    var type = offerParams.GetType();
                    var plakaProp = type.GetProperty("Plaka") ?? type.GetProperty("plaka");
                    var tcknProp = type.GetProperty("Tckn") ?? type.GetProperty("TCKN") ?? type.GetProperty("tckn");
                    if (plakaProp != null) plaka = Convert.ToString(plakaProp.GetValue(offerParams));
                    if (tcknProp != null) tckn = Convert.ToString(tcknProp.GetValue(offerParams));
                }

                // Placeholder selectors – gerçek form alanlarına göre güncellenecek
                try
                {
                    driver.SendKeys("input[name=\"plaka\"], #plaka, input[placeholder*=\"laka\"]", plaka ?? "");
                    driver.SendKeys("input[name=\"tckn\"], #tckn, input[placeholder*=\"TC\"]", tckn ?? "");
                    driver.Click("button[type=\"submit\"], .btn-sorgula, input[type=\"submit\"]");
                }
                catch
                {
                    return "Teklif formu alanları bulunamadı; selector'lar portala göre güncellenmeli.";
                }

                await Task.Delay(3000).ConfigureAwait(false);
                var currentUrl = driver.GetCurrentUrl();
                return "Sorgu gönderildi. URL: " + (currentUrl ?? "");
            }
            catch (Exception ex)
            {
                return "Hata: " + ex.Message;
            }
        }
    }
}
