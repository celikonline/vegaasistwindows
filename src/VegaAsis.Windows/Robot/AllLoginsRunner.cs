using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VegaAsis.Core.Contracts;

namespace VegaAsis.Windows.Robot
{
    /// <summary>
    /// Seçili şirket listesi için toplu giriş (AllLogins). Her şirket için ICompanyRobot.LoginAsync çağırır; başarı/hata listesi döndürür.
    /// </summary>
    public static class AllLoginsRunner
    {
        /// <summary>
        /// Seçili şirket listesini ayarlardan alır; yoksa verilen fallback listeyi kullanır.
        /// </summary>
        /// <param name="driver">Aktif tarayıcı sürücüsü (null ise oluşturulmaz, her şirket için aynı driver kullanılır).</param>
        /// <param name="settingsService">Şirket ayar servisi (seçili şirketler).</param>
        /// <param name="fallbackCompanyIds">Fallback şirket kodları (örn. "ak", "ana", "anadolu").</param>
        /// <param name="userId">Credential için kullanıcı ID (TRF_Base robotlarına SetUserId ile geçirilir).</param>
        /// <returns>Her şirket için AllLoginResult.</returns>
        public static async Task<List<AllLoginResult>> RunAsync(IBrowserDriver driver, ICompanySettingsService settingsService, IEnumerable<string> fallbackCompanyIds, Guid? userId = null)
        {
            var results = new List<AllLoginResult>();
            if (driver == null || !driver.IsActive) return results;
            List<string> ids = null;
            if (settingsService != null)
            {
                try
                {
                    ids = await settingsService.GetSelectedCompaniesAsync().ConfigureAwait(false);
                }
                catch
                {
                    ids = null;
                }
            }
            if (ids == null || ids.Count == 0)
            {
                ids = (fallbackCompanyIds ?? Enumerable.Empty<string>()).ToList();
            }
            ids = ids.Where(id => !string.IsNullOrWhiteSpace(id)).Distinct().ToList();
            foreach (var companyId in ids)
            {
                var robot = CompanyRobotRegistry.GetRobot(companyId);
                if (robot == null)
                {
                    results.Add(new AllLoginResult { CompanyId = companyId, CompanyName = companyId, Success = false, Message = "Robot bulunamadı." });
                    continue;
                }
                if (robot is TRF_Base trf)
                    trf.SetUserId(userId);
                try
                {
                    var ok = await robot.LoginAsync(driver).ConfigureAwait(false);
                    results.Add(new AllLoginResult
                    {
                        CompanyId = robot.CompanyId,
                        CompanyName = robot.CompanyName,
                        Success = ok,
                        Message = ok ? "OK" : "Başarısız"
                    });
                }
                catch (Exception ex)
                {
                    results.Add(new AllLoginResult { CompanyId = robot.CompanyId, CompanyName = robot.CompanyName, Success = false, Message = "Hata: " + ex.Message });
                }
            }
            return results;
        }

        /// <summary>
        /// Verilen şirket kodları için sırayla giriş dener; sonuç listesini döndürür.
        /// </summary>
        /// <param name="driver">Aktif tarayıcı sürücüsü (null ise oluşturulmaz, her şirket için aynı driver kullanılır).</param>
        /// <param name="companyIds">Şirket kodları (örn. "ak", "ana", "anadolu").</param>
        /// <param name="userId">Credential için kullanıcı ID (opsiyonel).</param>
        /// <returns>Her şirket için AllLoginResult.</returns>
        public static Task<List<AllLoginResult>> RunAsync(IBrowserDriver driver, IEnumerable<string> companyIds, Guid? userId = null)
        {
            return RunAsync(driver, null, companyIds, userId);
        }
    }
}
