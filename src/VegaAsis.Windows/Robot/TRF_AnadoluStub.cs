using System;
using System.Threading.Tasks;

namespace VegaAsis.Windows.Robot
{
    /// <summary>
    /// Anadolu Sigorta şirket robotu – geçici (Stub). Selector ve URL'ler gerçek portala göre güncellenecek; birebir dönüşüm sonrası TRF_Anadolu olarak yeniden adlandırılabilir.
    /// </summary>
    public class TRF_AnadoluStub : ICompanyRobot
    {
        public string CompanyId => "anadolu";
        public string CompanyName => "Anadolu Sigorta";
        public string LoginUrl => "https://www.anadolusigorta.com.tr";

        private const string SelectorUserName = "input[name=\"username\"], input[type=\"text\"], #username";
        private const string SelectorPassword = "input[name=\"password\"], input[type=\"password\"], #password";
        private const string SelectorLoginButton = "button[type=\"submit\"], input[type=\"submit\"], .btn-login";

        public async Task<bool> LoginAsync(IBrowserDriver driver)
        {
            if (driver == null || !driver.IsActive) return false;
            try
            {
                driver.Navigate(LoginUrl);
                await Task.Delay(2000).ConfigureAwait(false);
                var formReady = driver.WaitForElement(SelectorUserName, 10000) ?? driver.WaitForElement(SelectorPassword, 3000);
                if (formReady == null) return true;
                string user = null, pass = null; // TODO: AppSettings/CompanySettings'ten al
                try
                {
                    if (!string.IsNullOrEmpty(user)) driver.SendKeys("#username", user);
                    if (!string.IsNullOrEmpty(pass)) driver.SendKeys("#password", pass);
                    driver.Click(SelectorLoginButton);
                }
                catch { /* alanlar farklı olabilir */ }
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
                var offerUrl = "https://www.anadolusigorta.com.tr/teklif"; // placeholder
                driver.Navigate(offerUrl);
                await Task.Delay(2000).ConfigureAwait(false);
                var plaka = ""; var tckn = "";
                if (offerParams != null)
                {
                    var type = offerParams.GetType();
                    var plakaProp = type.GetProperty("Plaka") ?? type.GetProperty("plaka");
                    var tcknProp = type.GetProperty("Tckn") ?? type.GetProperty("TCKN") ?? type.GetProperty("tckn");
                    if (plakaProp != null) plaka = Convert.ToString(plakaProp.GetValue(offerParams));
                    if (tcknProp != null) tckn = Convert.ToString(tcknProp.GetValue(offerParams));
                }
                try
                {
                    driver.SendKeys("input[name=\"plaka\"], #plaka", plaka ?? "");
                    driver.SendKeys("input[name=\"tckn\"], #tckn", tckn ?? "");
                    driver.Click("button[type=\"submit\"], .btn-sorgula");
                }
                catch { return "Teklif formu alanları bulunamadı."; }
                await Task.Delay(3000).ConfigureAwait(false);
                return "Sorgu gönderildi. URL: " + (driver.GetCurrentUrl() ?? "");
            }
            catch (Exception ex) { return "Hata: " + ex.Message; }
        }
    }
}
