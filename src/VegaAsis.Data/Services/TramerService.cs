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
    /// Tramer sorgu servisi. App.config TramerApiBaseUrl doluysa HTTP GET ile API'ye istek atar; yanıtı TramerSonucDto listesine dönüştürür.
    /// </summary>
    public class TramerService : ITramerService
    {
        private static string GetBaseUrl()
        {
            var url = ConfigurationManager.AppSettings["TramerApiBaseUrl"];
            return string.IsNullOrWhiteSpace(url) ? null : url.Trim();
        }

        public async Task<IReadOnlyList<TramerSonucDto>> SorgulaPlakaAsync(string plaka)
        {
            if (string.IsNullOrWhiteSpace(plaka))
                return new List<TramerSonucDto>();

            var baseUrl = GetBaseUrl();
            if (string.IsNullOrEmpty(baseUrl))
                return new List<TramerSonucDto>();

            var url = baseUrl.TrimEnd('/') + "?plaka=" + Uri.EscapeDataString(plaka.Trim());
            return await CagriYapVeDonusturAsync(url).ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<TramerSonucDto>> SorgulaSasiNoAsync(string sasiNo)
        {
            if (string.IsNullOrWhiteSpace(sasiNo))
                return new List<TramerSonucDto>();

            var baseUrl = GetBaseUrl();
            if (string.IsNullOrEmpty(baseUrl))
                return new List<TramerSonucDto>();

            var url = baseUrl.TrimEnd('/') + "?sasiNo=" + Uri.EscapeDataString(sasiNo.Trim());
            return await CagriYapVeDonusturAsync(url).ConfigureAwait(false);
        }

        private static async Task<IReadOnlyList<TramerSonucDto>> CagriYapVeDonusturAsync(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(30);
                    var response = await client.GetAsync(url).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return ParseTramerJson(json);
                }
            }
            catch
            {
                return new List<TramerSonucDto>();
            }
        }

        private static List<TramerSonucDto> ParseTramerJson(string json)
        {
            var list = new List<TramerSonucDto>();
            if (string.IsNullOrWhiteSpace(json)) return list;
            try
            {
                var serializer = new JavaScriptSerializer();
                var arr = serializer.Deserialize<List<Dictionary<string, object>>>(json);
                if (arr == null) return list;
                foreach (var item in arr)
                {
                    var dto = new TramerSonucDto
                    {
                        Plaka = GetString(item, "Plaka", "plaka"),
                        Marka = GetString(item, "Marka", "marka"),
                        Model = GetString(item, "Model", "model"),
                        Sirket = GetString(item, "Sirket", "sirket", "SirketAdi"),
                        Aciklama = GetString(item, "Aciklama", "aciklama")
                    };
                    var hasarTarihi = GetString(item, "HasarTarihi", "hasarTarihi");
                    if (!string.IsNullOrEmpty(hasarTarihi))
                    {
                        DateTime dt;
                        if (DateTime.TryParse(hasarTarihi, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                            dto.HasarTarihi = dt;
                    }
                    list.Add(dto);
                }
            }
            catch { /* JSON format farklıysa boş liste */ }
            return list;
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
    }
}
