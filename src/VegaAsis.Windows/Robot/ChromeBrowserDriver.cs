using System;
using System.Configuration;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace VegaAsis.Windows.Robot
{
    /// <summary>
    /// Selenium WebDriver ile Chrome tarayıcı yönetimi. Faz 4.1 altyapısı. App.config: ChromeDriverPath, ChromeHeadless, ChromePageLoadTimeoutSeconds, ChromeImplicitWaitSeconds.
    /// </summary>
    public class ChromeBrowserDriver : IBrowserDriver
    {
        private IWebDriver _driver;
        private bool _disposed;

        public bool IsActive => _driver != null;

        private static bool GetConfigBool(string key, bool defaultValue)
        {
            var v = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(v)) return defaultValue;
            return string.Equals(v.Trim(), "true", StringComparison.OrdinalIgnoreCase) || v.Trim() == "1";
        }

        private static int GetConfigInt(string key, int defaultValue)
        {
            var v = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(v)) return defaultValue;
            int i;
            return int.TryParse(v.Trim(), out i) && i > 0 ? i : defaultValue;
        }

        /// <summary>Yeni Chrome oturumu açar. Headless ve timeout App.config'ten okunur; parametre ile override edilebilir.</summary>
        public ChromeBrowserDriver(bool? headless = null)
        {
            var options = new ChromeOptions();
            var useHeadless = headless ?? GetConfigBool("ChromeHeadless", false);
            if (useHeadless)
                options.AddArgument("--headless");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-gpu");
            try
            {
                var driverPath = ConfigurationManager.AppSettings["ChromeDriverPath"];
                if (!string.IsNullOrWhiteSpace(driverPath))
                {
                    var service = ChromeDriverService.CreateDefaultService(driverPath.Trim());
                    _driver = new ChromeDriver(service, options);
                }
                else
                    _driver = new ChromeDriver(options);

                var pageLoad = GetConfigInt("ChromePageLoadTimeoutSeconds", 60);
                var implicitWait = GetConfigInt("ChromeImplicitWaitSeconds", 10);
                _driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(pageLoad);
                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(implicitWait);
            }
            catch (DriverServiceNotFoundException ex)
            {
                throw new InvalidOperationException("ChromeDriver bulunamadı. Chrome yüklü olduğundan ve Selenium.WebDriver.ChromeDriver paketinin yüklü olduğundan emin olun.", ex);
            }
        }

        public void Navigate(string url)
        {
            if (_driver == null) return;
            if (string.IsNullOrWhiteSpace(url)) return;
            _driver.Navigate().GoToUrl(url);
        }

        public string GetCurrentUrl()
        {
            return _driver?.Url ?? string.Empty;
        }

        public void Close()
        {
            if (_driver == null) return;
            try
            {
                _driver.Quit();
            }
            catch { /* kapatma sırasında hata yoksay */ }
            _driver = null;
        }

        public object FindElementById(string id)
        {
            if (_driver == null || string.IsNullOrWhiteSpace(id)) return null;
            try
            {
                return _driver.FindElement(By.Id(id));
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        }

        public object FindElementByCss(string cssSelector)
        {
            if (_driver == null || string.IsNullOrWhiteSpace(cssSelector)) return null;
            try
            {
                return _driver.FindElement(By.CssSelector(cssSelector));
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        }

        public void SendKeys(string elementSelector, string text)
        {
            if (_driver == null || string.IsNullOrWhiteSpace(elementSelector)) return;
            var element = _driver.FindElement(By.CssSelector(elementSelector));
            if (element != null)
                element.SendKeys(text ?? string.Empty);
        }

        public void Click(string elementSelector)
        {
            if (_driver == null || string.IsNullOrWhiteSpace(elementSelector)) return;
            var element = _driver.FindElement(By.CssSelector(elementSelector));
            if (element != null)
                element.Click();
        }

        public object WaitForElement(string selector, int timeoutMs)
        {
            if (_driver == null || string.IsNullOrWhiteSpace(selector)) return null;
            try
            {
                var wait = new WebDriverWait(_driver, TimeSpan.FromMilliseconds(timeoutMs > 0 ? timeoutMs : 5000));
                var element = wait.Until(d => d.FindElement(By.CssSelector(selector)));
                return element;
            }
            catch (WebDriverTimeoutException)
            {
                return null;
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            Close();
            if (_driver != null)
            {
                try { _driver.Dispose(); } catch { }
                _driver = null;
            }
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
