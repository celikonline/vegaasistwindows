using System;
using System.Threading.Tasks;
using VegaAsis.Core.Contracts;

namespace VegaAsis.Windows.Robot
{
    /// <summary>
    /// Tüm TRF şirket robotları için ortak base class.
    /// Credential yönetimi ve ortak helper metodları içerir.
    /// </summary>
    public abstract class TRF_Base : ICompanyRobot
    {
        /// <summary>Şirket kodu (örn. "ak", "anadolu").</summary>
        public abstract string CompanyId { get; }

        /// <summary>Şirket görünen adı.</summary>
        public abstract string CompanyName { get; }

        /// <summary>Giriş sayfası URL'si.</summary>
        public abstract string LoginUrl { get; }

        /// <summary>Credential için kullanıcı ID (opsiyonel, null ise global credential kullanılır).</summary>
        protected Guid? UserId { get; private set; }

        /// <summary>Giriş yapılacak kullanıcı ID'sini ayarlar.</summary>
        public void SetUserId(Guid? userId) => UserId = userId;

        /// <summary>Tarayıcı ile giriş sayfasına gidip (gerekirse) giriş yapar. Başarılı ise true.</summary>
        public abstract Task<bool> LoginAsync(IBrowserDriver driver);

        /// <summary>Teklif parametreleri ile sorgu yapar; sonuç metni veya hata mesajı döner.</summary>
        public abstract Task<string> GetOfferAsync(IBrowserDriver driver, object offerParams);

        /// <summary>Şirket için credential bilgilerini getirir.</summary>
        protected async Task<CompanyCredentialDto> GetCredentialAsync()
        {
            try
            {
                if (!ServiceLocator.IsInitialized) return null;
                var service = ServiceLocator.Resolve<ICompanyCredentialService>();
                if (service == null) return null;
                return await service.GetCredentialAsync(CompanyId, UserId).ConfigureAwait(false);
            }
            catch { return null; }
        }

        /// <summary>offerParams nesnesinden Plaka değerini çıkarır.</summary>
        protected static string GetPlaka(object offerParams)
        {
            if (offerParams == null) return "";
            var type = offerParams.GetType();
            var prop = type.GetProperty("Plaka") ?? type.GetProperty("plaka");
            return prop != null ? Convert.ToString(prop.GetValue(offerParams)) ?? "" : "";
        }

        /// <summary>offerParams nesnesinden TCKN değerini çıkarır.</summary>
        protected static string GetTckn(object offerParams)
        {
            if (offerParams == null) return "";
            var type = offerParams.GetType();
            var prop = type.GetProperty("Tckn") ?? type.GetProperty("TCKN") ?? type.GetProperty("tckn") ?? type.GetProperty("TcKimlikNo");
            return prop != null ? Convert.ToString(prop.GetValue(offerParams)) ?? "" : "";
        }

        /// <summary>Güvenli element tıklama.</summary>
        protected static bool TryClick(IBrowserDriver driver, string selector)
        {
            try { driver.Click(selector); return true; }
            catch { return false; }
        }

        /// <summary>Güvenli text girişi.</summary>
        protected static bool TrySendKeys(IBrowserDriver driver, string selector, string text)
        {
            if (string.IsNullOrEmpty(text)) return true;
            try { driver.SendKeys(selector, text); return true; }
            catch { return false; }
        }
    }
}
