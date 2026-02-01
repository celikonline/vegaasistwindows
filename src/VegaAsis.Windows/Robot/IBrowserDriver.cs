using System;

namespace VegaAsis.Windows.Robot
{
    /// <summary>
    /// Tarayıcı otomasyonu için ortak arayüz. Selenium veya Playwright implementasyonu kullanılabilir.
    /// </summary>
    public interface IBrowserDriver : IDisposable
    {
        /// <summary>Belirtilen URL'ye gider.</summary>
        void Navigate(string url);

        /// <summary>Mevcut sayfa URL'sini döndürür.</summary>
        string GetCurrentUrl();

        /// <summary>Tarayıcıyı kapatır ve kaynakları serbest bırakır.</summary>
        void Close();

        /// <summary>Tarayıcı açık mı?</summary>
        bool IsActive { get; }

        /// <summary>ID ile element bulur. Implementasyon Selenium IWebElement döndürür (object olarak).</summary>
        object FindElementById(string id);

        /// <summary>CSS seçici ile element bulur. Implementasyon Selenium IWebElement döndürür (object olarak).</summary>
        object FindElementByCss(string cssSelector);

        /// <summary>elementSelector CSS seçicidir (örn. "#username", ".btn-login"). Elementi bulup metin yazar.</summary>
        void SendKeys(string elementSelector, string text);

        /// <summary>elementSelector CSS seçicidir. Elementi bulup tıklar.</summary>
        void Click(string elementSelector);

        /// <summary>Belirtilen süre (ms) boyunca selector ile elementin görünmesini bekler; bulunursa element (object) döner, yoksa null.</summary>
        object WaitForElement(string selector, int timeoutMs);
    }
}
