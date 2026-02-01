using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VegaAsis.Core.Contracts;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Data.Services
{
    /// <summary>
    /// WS teklif sorgulama – şu an stub; gerçek WS/API entegrasyonu eklendiğinde doldurulacak.
    /// </summary>
    public class WsSorguService : IWsSorguService
    {
        public async Task<IReadOnlyList<WSTeklifSonucDto>> SorgulaAsync(int? sablonId, DateTime baslangic, DateTime bitis)
        {
            await Task.Delay(100).ConfigureAwait(false);

            var list = new List<WSTeklifSonucDto>();
            list.Add(new WSTeklifSonucDto
            {
                Tarih = DateTime.Now,
                Plaka = "06ABC01",
                Sirket = "ANADOLU",
                Brans = "TRAFİK",
                Fiyat = "1.250,00"
            });
            list.Add(new WSTeklifSonucDto
            {
                Tarih = DateTime.Now,
                Plaka = "34XYZ99",
                Sirket = "HDI",
                Brans = "KASKO",
                Fiyat = "3.400,00"
            });
            return list;
        }
    }
}
