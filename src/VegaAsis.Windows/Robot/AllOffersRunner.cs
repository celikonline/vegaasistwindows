using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VegaAsis.Windows.Robot
{
    /// <summary>
    /// Seçili şirket listesi için toplu teklif (AllOffers). Her şirket için önce LoginAsync, sonra GetOfferAsync çağırır.
    /// </summary>
    public static class AllOffersRunner
    {
        /// <summary>
        /// Verilen şirket kodları için sırayla giriş yapıp teklif alır; sonuç listesini döndürür.
        /// </summary>
        /// <param name="driver">Aktif tarayıcı sürücüsü.</param>
        /// <param name="companyIds">Şirket kodları.</param>
        /// <param name="offerParams">Teklif parametreleri (Plaka, Tckn vb.).</param>
        /// <param name="userId">Credential için kullanıcı ID (TRF_Base robotlarına SetUserId ile geçirilir).</param>
        /// <returns>Her şirket için AllOfferResult.</returns>
        public static async Task<List<AllOfferResult>> RunAsync(IBrowserDriver driver, IEnumerable<string> companyIds, object offerParams, Guid? userId = null)
        {
            var results = new List<AllOfferResult>();
            if (driver == null || !driver.IsActive) return results;
            var ids = (companyIds ?? Enumerable.Empty<string>()).Where(id => !string.IsNullOrWhiteSpace(id)).Distinct().ToList();
            foreach (var companyId in ids)
            {
                var robot = CompanyRobotRegistry.GetRobot(companyId);
                if (robot == null)
                {
                    results.Add(new AllOfferResult { CompanyId = companyId, CompanyName = companyId, LoginSuccess = false, OfferResult = "Robot bulunamadı." });
                    continue;
                }
                if (robot is TRF_Base trf)
                    trf.SetUserId(userId);
                try
                {
                    var loginOk = await robot.LoginAsync(driver).ConfigureAwait(false);
                    if (!loginOk)
                    {
                        results.Add(new AllOfferResult { CompanyId = robot.CompanyId, CompanyName = robot.CompanyName, LoginSuccess = false, OfferResult = "Giriş başarısız." });
                        continue;
                    }
                    var teklif = await robot.GetOfferAsync(driver, offerParams).ConfigureAwait(false);
                    results.Add(new AllOfferResult
                    {
                        CompanyId = robot.CompanyId,
                        CompanyName = robot.CompanyName,
                        LoginSuccess = true,
                        OfferResult = teklif ?? "(sonuç yok)"
                    });
                }
                catch (Exception ex)
                {
                    results.Add(new AllOfferResult { CompanyId = robot.CompanyId, CompanyName = robot.CompanyName, LoginSuccess = false, OfferResult = "Hata: " + ex.Message });
                }
            }
            return results;
        }
    }
}
