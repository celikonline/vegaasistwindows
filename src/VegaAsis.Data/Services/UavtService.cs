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
    /// UAVT sorgu servisi. App.config UavtApiBaseUrl doluysa HTTP GET ile API'ye istek atar; yanıtı UavtSonucDto listesine dönüştürür.
    /// </summary>
    public class UavtService : IUavtService
    {
        private static string GetBaseUrl()
        {
            var url = ConfigurationManager.AppSettings["UavtApiBaseUrl"];
            return string.IsNullOrWhiteSpace(url) ? null : url.Trim();
        }

        public async Task<IReadOnlyList<UavtSonucDto>> SorgulaTcVergiAsync(string tcVergi)
        {
            if (string.IsNullOrWhiteSpace(tcVergi))
                return new List<UavtSonucDto>();

            var baseUrl = GetBaseUrl();
            if (string.IsNullOrEmpty(baseUrl))
                return new List<UavtSonucDto>();

            var url = baseUrl.TrimEnd('/') + "?tcVergi=" + Uri.EscapeDataString(tcVergi.Trim());
            return await CagriYapVeDonusturAsync(url).ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<UavtSonucDto>> SorgulaPlakaAsync(string plaka)
        {
            if (string.IsNullOrWhiteSpace(plaka))
                return new List<UavtSonucDto>();

            var baseUrl = GetBaseUrl();
            if (string.IsNullOrEmpty(baseUrl))
                return new List<UavtSonucDto>();

            var url = baseUrl.TrimEnd('/') + "?plaka=" + Uri.EscapeDataString(plaka.Trim());
            return await CagriYapVeDonusturAsync(url).ConfigureAwait(false);
        }

        private static async Task<IReadOnlyList<UavtSonucDto>> CagriYapVeDonusturAsync(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(30);
                    var response = await client.GetAsync(url).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return ParseUavtJson(json);
                }
            }
            catch
            {
                return new List<UavtSonucDto>();
            }
        }

        private static List<UavtSonucDto> ParseUavtJson(string json)
        {
            var list = new List<UavtSonucDto>();
            if (string.IsNullOrWhiteSpace(json)) return list;
            try
            {
                var serializer = new JavaScriptSerializer();
                var arr = serializer.Deserialize<List<Dictionary<string, object>>>(json);
                if (arr == null) return list;
                foreach (var item in arr)
                {
                    var dto = new UavtSonucDto
                    {
                        Kaynak = GetString(item, "Kaynak", "kaynak"),
                        Aciklama = GetString(item, "Aciklama", "aciklama"),
                        Durum = GetString(item, "Durum", "durum")
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
