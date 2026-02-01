using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VegaAsis.Windows.Robot
{
    /// <summary>
    /// Seçili şirket listesi için toplu giriş (AllLogins). Her şirket için ICompanyRobot.LoginAsync çağırır; başarı/hata listesi döndürür.
    /// </summary>
    public static class AllLoginsRunner
    {
        /// <summary>
        /// Verilen şirket kodları için sırayla giriş dener; sonuç listesini döndürür.
        /// </summary>
        /// <param name="driver">Aktif tarayıcı sürücüsü (null ise oluşturulmaz, her şirket için aynı driver kullanılır).</param>
        /// <param name="companyIds">Şirket kodları (örn. "ak", "ana", "anadolu").</param>
        /// <returns>Her şirket için AllLoginResult.</returns>
        public static async Task<List<AllLoginResult>> RunAsync(IBrowserDriver driver, IEnumerable<string> companyIds)
        {
            var results = new List<AllLoginResult>();
            if (driver == null || !driver.IsActive) return results;
            var ids = (companyIds ?? Enumerable.Empty<string>()).Where(id => !string.IsNullOrWhiteSpace(id)).Distinct().ToList();
            foreach (var companyId in ids)
            {
                var robot = CompanyRobotRegistry.GetRobot(companyId);
                if (robot == null)
                {
                    results.Add(new AllLoginResult { CompanyId = companyId, CompanyName = companyId, Success = false, Message = "Robot bulunamadı." });
                    continue;
                }
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
    }
}
