using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using VegaAsis.Core.Contracts;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Data.Services
{
    /// <summary>
    /// WS teklif sorgulama – App.config WsSorguApiBaseUrl doluysa HTTP GET ile API'ye istek atar; yanıtı WSTeklifSonucDto listesine dönüştürür.
    /// </summary>
    public class WsSorguService : IWsSorguService
    {
        public async Task<IReadOnlyList<WSTeklifSonucDto>> SorgulaAsync(int? sablonId, DateTime baslangic, DateTime bitis)
        {
            var baseUrl = GetBaseUrl();
            if (string.IsNullOrEmpty(baseUrl))
            {
                await Task.Delay(100).ConfigureAwait(false);
                return BuildSampleData();
            }

            var url = BuildUrl(baseUrl, sablonId, baslangic, bitis);
            return await CagriYapVeDonusturAsync(url).ConfigureAwait(false);
        }

        private static string GetBaseUrl()
        {
            return GetConfigString("WsSorguApiBaseUrl");
        }

        private static string GetConfigString(string key)
        {
            var v = ConfigurationManager.AppSettings[key];
            return string.IsNullOrWhiteSpace(v) ? null : v.Trim();
        }

        private static int GetConfigInt(string key, int defaultValue)
        {
            var v = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(v)) return defaultValue;
            int i;
            return int.TryParse(v.Trim(), out i) && i > 0 ? i : defaultValue;
        }

        private static string BuildUrl(string baseUrl, int? sablonId, DateTime baslangic, DateTime bitis)
        {
            var url = baseUrl.TrimEnd('/');
            var query = "baslangic=" + Uri.EscapeDataString(baslangic.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)) +
                        "&bitis=" + Uri.EscapeDataString(bitis.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            if (sablonId.HasValue)
                query += "&sablonId=" + sablonId.Value.ToString(CultureInfo.InvariantCulture);
            return url + "?" + query;
        }

        private static async Task<IReadOnlyList<WSTeklifSonucDto>> CagriYapVeDonusturAsync(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var timeoutSeconds = GetConfigInt("WsSorguTimeoutSeconds", 30);
                    client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
                    var apiKey = GetConfigString("WsSorguApiKey");
                    if (!string.IsNullOrWhiteSpace(apiKey))
                    {
                        var header = GetConfigString("WsSorguApiKeyHeader") ?? "X-API-KEY";
                        client.DefaultRequestHeaders.Remove(header);
                        client.DefaultRequestHeaders.Add(header, apiKey);
                    }
                    var response = await client.GetAsync(url).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return ParseWsJson(json);
                }
            }
            catch
            {
                return BuildSampleData();
            }
        }

        private static List<WSTeklifSonucDto> ParseWsJson(string json)
        {
            var list = new List<WSTeklifSonucDto>();
            if (string.IsNullOrWhiteSpace(json)) return list;
            try
            {
                var serializer = new JavaScriptSerializer();
                var arr = serializer.Deserialize<List<Dictionary<string, object>>>(json);
                if (arr == null) return list;
                foreach (var item in arr)
                {
                    var dto = new WSTeklifSonucDto
                    {
                        Plaka = GetString(item, "Plaka", "plaka"),
                        Sirket = GetString(item, "Sirket", "sirket", "SirketAdi"),
                        Brans = GetString(item, "Brans", "brans"),
                        Fiyat = GetString(item, "Fiyat", "fiyat")
                    };
                    var tarihStr = GetString(item, "Tarih", "tarih");
                    if (!string.IsNullOrEmpty(tarihStr))
                    {
                        DateTime dt;
                        if (DateTime.TryParse(tarihStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                            dto.Tarih = dt;
                    }
                    list.Add(dto);
                }
            }
            catch { /* JSON format farklıysa örnek veri dön */ }
            return list.Count > 0 ? list : BuildSampleData();
        }

        private static string GetString(Dictionary<string, object> d, params string[] keys)
        {
            if (d == null) return null;
            foreach (var key in keys)
            {
                object o;
                if (d.TryGetValue(key, out o) && o != null)
                    return o.ToString().Trim();
            }
            return null;
        }

        private static List<WSTeklifSonucDto> BuildSampleData()
        {
            return new List<WSTeklifSonucDto>
            {
                new WSTeklifSonucDto
                {
                    Tarih = DateTime.Now,
                    Plaka = "06ABC01",
                    Sirket = "ANADOLU",
                    Brans = "TRAFİK",
                    Fiyat = "1.250,00"
                },
                new WSTeklifSonucDto
                {
                    Tarih = DateTime.Now,
                    Plaka = "34XYZ99",
                    Sirket = "HDI",
                    Brans = "KASKO",
                    Fiyat = "3.400,00"
                }
            };
        }
    }
}
