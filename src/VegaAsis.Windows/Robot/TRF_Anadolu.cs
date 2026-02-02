using System;
using System.Threading.Tasks;

namespace VegaAsis.Windows.Robot
{
    /// <summary>
    /// Anadolu Sigorta şirket robotu.
    /// Portal: https://acentem.anadolusigorta.com.tr
    /// </summary>
    public class TRF_Anadolu : TRF_Base
    {
        public override string CompanyId => "anadolu";
        public override string CompanyName => "Anadolu Sigorta";
        public override string LoginUrl => "https://acentem.anadolusigorta.com.tr";

        // Rehber: DOCS/TRF-Selector-Guncelleme.md — gerçek portala göre doğrulanmalı
        // Login form selector'ları
        private const string SelectorUserName = "input[name=\"username\"], input[name=\"kullaniciAdi\"], input[type=\"text\"][id*=\"user\"], #username, #txtKullaniciAdi";
        private const string SelectorPassword = "input[name=\"password\"], input[name=\"sifre\"], input[type=\"password\"], #password, #txtSifre";
        private const string SelectorLoginButton = "button[type=\"submit\"], input[type=\"submit\"], .btn-login, #btnGiris, button.giris-btn";
        private const string SelectorCaptcha = "input[name=\"captcha\"], #captcha, .captcha-input";

        // Teklif form selector'ları
        private const string SelectorPlaka = "input[name=\"plaka\"], #plaka, input[placeholder*=\"laka\"], #txtPlaka";
        private const string SelectorTckn = "input[name=\"tckn\"], input[name=\"tcKimlikNo\"], #tckn, #txtTcKimlikNo, input[placeholder*=\"TC\"]";
        private const string SelectorSorgulaButton = "button[type=\"submit\"], .btn-sorgula, #btnSorgula, input[value*=\"Sorgula\"]";

        public override async Task<bool> LoginAsync(IBrowserDriver driver)
        {
            if (driver == null || !driver.IsActive) return false;

            try
            {
                driver.Navigate(LoginUrl);
                await Task.Delay(2500).ConfigureAwait(false);

                var formReady = driver.WaitForElement(SelectorUserName, 10000) 
                    ?? driver.WaitForElement(SelectorPassword, 3000);

                if (formReady == null)
                    return true;

                // Credential bilgilerini al
                string user = null, pass = null;
                var cred = await GetCredentialAsync().ConfigureAwait(false);
                if (cred != null)
                {
                    user = cred.Username;
                    pass = cred.Password;
                }

                // Kullanıcı adı ve şifre gir
                TrySendKeys(driver, SelectorUserName, user);
                TrySendKeys(driver, SelectorPassword, pass);

                // Captcha kontrolü
                var captchaElement = driver.WaitForElement(SelectorCaptcha, 1000);
                if (captchaElement != null)
                {
                    await Task.Delay(5000).ConfigureAwait(false);
                }

                // Giriş butonuna tıkla
                if (!TryClick(driver, SelectorLoginButton))
                    TryClick(driver, "button[type=\"submit\"]");

                await Task.Delay(3000).ConfigureAwait(false);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override async Task<string> GetOfferAsync(IBrowserDriver driver, object offerParams)
        {
            if (driver == null || !driver.IsActive) return null;

            try
            {
                var offerUrl = LoginUrl + "/trafik";
                driver.Navigate(offerUrl);
                await Task.Delay(2500).ConfigureAwait(false);

                var plaka = GetPlaka(offerParams);
                var tckn = GetTckn(offerParams);

                try
                {
                    TrySendKeys(driver, SelectorPlaka, plaka);
                    TrySendKeys(driver, SelectorTckn, tckn);
                    TryClick(driver, SelectorSorgulaButton);
                }
                catch
                {
                    return "Teklif formu alanları bulunamadı.";
                }

                await Task.Delay(5000).ConfigureAwait(false);
                return "Sorgu gönderildi. URL: " + (driver.GetCurrentUrl() ?? "");
            }
            catch (Exception ex)
            {
                return "Hata: " + ex.Message;
            }
        }
    }
}
