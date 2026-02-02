using System;
using System.Threading.Tasks;

namespace VegaAsis.Windows.Robot
{
    public class TRF_Orient : TRF_Base
    {
        public override string CompanyId => "orient";
        public override string CompanyName => "Orient Sigorta";
        public override string LoginUrl => "https://acenteportal.orientsigorta.com.tr";

        // Rehber: DOCS/TRF-Selector-Guncelleme.md — gerçek portala göre doğrulanmalı
        private const string SelectorUserName = "input[name=\"username\"], input[name=\"kullaniciAdi\"], input[type=\"text\"][id*=\"user\"], #username, #txtKullaniciAdi";
        private const string SelectorPassword = "input[name=\"password\"], input[name=\"sifre\"], input[type=\"password\"], #password, #txtSifre";
        private const string SelectorLoginButton = "button[type=\"submit\"], input[type=\"submit\"], .btn-login, #btnGiris, button.giris-btn";
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
                var formReady = driver.WaitForElement(SelectorUserName, 10000);
                if (formReady == null) return true;
                var cred = await GetCredentialAsync().ConfigureAwait(false);
                TrySendKeys(driver, SelectorUserName, cred?.Username);
                TrySendKeys(driver, SelectorPassword, cred?.Password);
                TryClick(driver, SelectorLoginButton);
                await Task.Delay(3000).ConfigureAwait(false);
                return true;
            }
            catch { return false; }
        }

        public override async Task<string> GetOfferAsync(IBrowserDriver driver, object offerParams)
        {
            if (driver == null || !driver.IsActive) return null;
            try
            {
                driver.Navigate(LoginUrl + "/trafik");
                await Task.Delay(2500).ConfigureAwait(false);
                TrySendKeys(driver, SelectorPlaka, GetPlaka(offerParams));
                TrySendKeys(driver, SelectorTckn, GetTckn(offerParams));
                TryClick(driver, SelectorSorgulaButton);
                await Task.Delay(5000).ConfigureAwait(false);
                return "Sorgu gönderildi. URL: " + (driver.GetCurrentUrl() ?? "");
            }
            catch (Exception ex) { return "Hata: " + ex.Message; }
        }
    }
}
