using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VegaAsis.Windows.Robot
{
    /// <summary>
    /// Birden fazla şirkete paralel teklif sorgusu. Her şirket için ayrı ChromeDriver instance kullanır;
    /// maxConcurrency ile eşzamanlı tarayıcı sayısı sınırlanır.
    /// </summary>
    public static class ParallelOfferRunner
    {
        /// <summary>
        /// Verilen şirket kodları için paralel teklif alır. Her şirket için ayrı driver oluşturulur.
        /// </summary>
        /// <param name="companyIds">Şirket kodları.</param>
        /// <param name="offerParams">Teklif parametreleri (Plaka, Tckn vb.).</param>
        /// <param name="userId">Credential için kullanıcı ID (opsiyonel).</param>
        /// <param name="maxConcurrency">Aynı anda açık tarayıcı sayısı (varsayılan 3).</param>
        /// <param name="headless">Chrome headless modda çalışsın mı.</param>
        /// <returns>Her şirket için AllOfferResult (sıra companyIds ile aynı olmayabilir).</returns>
        public static async Task<List<AllOfferResult>> RunAsync(
            IEnumerable<string> companyIds,
            object offerParams,
            Guid? userId = null,
            int maxConcurrency = 3,
            bool headless = false)
        {
            var ids = (companyIds ?? Enumerable.Empty<string>())
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct()
                .ToList();

            if (ids.Count == 0)
                return new List<AllOfferResult>();

            if (maxConcurrency < 1)
                maxConcurrency = 1;

            var semaphore = new SemaphoreSlim(maxConcurrency);
            var results = new List<AllOfferResult>();
            var lockResults = new object();

            var tasks = ids.Select(async companyId =>
            {
                await semaphore.WaitAsync().ConfigureAwait(false);
                IBrowserDriver driver = null;
                try
                {
                    driver = new ChromeBrowserDriver(headless);
                    var robot = CompanyRobotRegistry.GetRobot(companyId);
                    if (robot == null)
                    {
                        lock (lockResults)
                        {
                            results.Add(new AllOfferResult
                            {
                                CompanyId = companyId,
                                CompanyName = companyId,
                                LoginSuccess = false,
                                OfferResult = "Robot bulunamadı."
                            });
                        }
                        return;
                    }

                    if (robot is TRF_Base trf)
                        trf.SetUserId(userId);

                    var loginOk = await robot.LoginAsync(driver).ConfigureAwait(false);
                    if (!loginOk)
                    {
                        lock (lockResults)
                        {
                            results.Add(new AllOfferResult
                            {
                                CompanyId = robot.CompanyId,
                                CompanyName = robot.CompanyName,
                                LoginSuccess = false,
                                OfferResult = "Giriş başarısız."
                            });
                        }
                        return;
                    }

                    var teklif = await robot.GetOfferAsync(driver, offerParams).ConfigureAwait(false);
                    lock (lockResults)
                    {
                        results.Add(new AllOfferResult
                        {
                            CompanyId = robot.CompanyId,
                            CompanyName = robot.CompanyName,
                            LoginSuccess = true,
                            OfferResult = teklif ?? "(sonuç yok)"
                        });
                    }
                }
                catch (Exception ex)
                {
                    var robot = CompanyRobotRegistry.GetRobot(companyId);
                    lock (lockResults)
                    {
                        results.Add(new AllOfferResult
                        {
                            CompanyId = companyId,
                            CompanyName = robot?.CompanyName ?? companyId,
                            LoginSuccess = false,
                            OfferResult = "Hata: " + ex.Message
                        });
                    }
                }
                finally
                {
                    if (driver != null)
                    {
                        try { driver.Dispose(); }
                        catch { }
                    }
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks).ConfigureAwait(false);
            return results;
        }
    }
}
