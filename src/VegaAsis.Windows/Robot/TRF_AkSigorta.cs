using System;
using System.Threading.Tasks;

namespace VegaAsis.Windows.Robot
{
    /// <summary>
    /// AK Sigorta şirket robotu. Selector'lar gerçek portal HTML'ine göre güncellenir.
    /// </summary>
    public class TRF_AkSigorta : TRF_Base
    {
        public override string CompanyId => "ak";
        public override string CompanyName => "AK Sigorta";
        public override string LoginUrl => "https://sat2.aksigorta.com.tr/auth/log";

        // Rehber: DOCS/TRF-Selector-Guncelleme.md — gerçek portala göre doğrulanmalı
        private const string SelectorUserName = "input[name=\"username\"], input[name=\"kullaniciAdi\"], input[type=\"text\"][id*=\"user\"], #username, #txtKullaniciAdi";
        private const string SelectorPassword = "input[name=\"password\"], input[name=\"sifre\"], input[type=\"password\"], #password, #txtSifre";
        private const string SelectorLoginButton = "button[type=\"submit\"], input[type=\"submit\"], .btn-login, #btnGiris, button.btn-primary, button.giris-btn";
        private const string SelectorPlaka = "input[name=\"plaka\"], #plaka, input[placeholder*=\"laka\"], #txtPlaka";
        private const string SelectorTckn = "input[name=\"tckn\"], input[name=\"tcKimlikNo\"], #tckn, #txtTcKimlikNo, input[placeholder*=\"TC\"]";
        private const string SelectorSorgulaButton = "button[type=\"submit\"], .btn-sorgula, #btnSorgula, input[value*=\"Sorgula\"], input[type=\"submit\"]";

        public override async Task<bool> LoginAsync(IBrowserDriver driver)
        {
            if (driver == null || !driver.IsActive) return false;
            try
            {
                driver.Navigate(LoginUrl);
                await Task.Delay(2000).ConfigureAwait(false);

                var formReady = driver.WaitForElement(SelectorUserName, 10000);
                if (formReady == null) return true;

                var cred = await GetCredentialAsync().ConfigureAwait(false);
                TrySendKeys(driver, SelectorUserName, cred?.Username);
                TrySendKeys(driver, SelectorPassword, cred?.Password);
                TryClick(driver, SelectorLoginButton);

                await Task.Delay(2000).ConfigureAwait(false);
                return true;
            }
            catch { return false; }
        }

        public override async Task<string> GetOfferAsync(IBrowserDriver driver, object offerParams)
        {
            if (driver == null || !driver.IsActive) return null;
            try
            {
                driver.Navigate("https://sat2.aksigorta.com.tr/offer");
                await Task.Delay(2000).ConfigureAwait(false);

                TrySendKeys(driver, SelectorPlaka, GetPlaka(offerParams));
                TrySendKeys(driver, SelectorTckn, GetTckn(offerParams));
                TryClick(driver, SelectorSorgulaButton);

                await Task.Delay(3000).ConfigureAwait(false);
                return "Sorgu gönderildi. URL: " + (driver.GetCurrentUrl() ?? "");
            }
            catch (Exception ex) { return "Hata: " + ex.Message; }
        }
    }
}
