using System;
using System.Threading.Tasks;

namespace VegaAsis.Windows.Robot
{
    /// <summary>
    /// Şirket portalına giriş ve teklif akışı için robot arayüzü. Her TRF_* sınıfı bu arayüzü uygular.
    /// </summary>
    public interface ICompanyRobot
    {
        /// <summary>Şirket kodu (örn. "ak", "anadolu").</summary>
        string CompanyId { get; }

        /// <summary>Şirket görünen adı.</summary>
        string CompanyName { get; }

        /// <summary>Giriş sayfası URL'si.</summary>
        string LoginUrl { get; }

        /// <summary>Tarayıcı ile giriş sayfasına gidip (gerekirse) giriş yapar. Başarılı ise true.</summary>
        Task<bool> LoginAsync(IBrowserDriver driver);

        /// <summary>Teklif parametreleri ile sorgu yapar; sonuç metni veya hata mesajı döner. Stub için null/boş.</summary>
        Task<string> GetOfferAsync(IBrowserDriver driver, object offerParams);
    }
}
