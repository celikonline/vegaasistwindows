using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VegaAsis.Core.Contracts;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Data.Services
{
    /// <summary>
    /// Bildirim listesi – şu an stub (örnek veri); ileride duyurular/DB ile beslenebilir.
    /// </summary>
    public class BildirimService : IBildirimService
    {
        public async Task<IReadOnlyList<BildirimDto>> GetAllAsync()
        {
            await Task.Delay(0).ConfigureAwait(false);

            var list = new List<BildirimDto>();
            list.Add(new BildirimDto
            {
                Tarih = DateTime.Now.AddHours(-1),
                Tip = "Bilgi",
                Baslik = "Hoş geldiniz",
                Icerik = "VegaAsis uygulamasına giriş yaptınız."
            });
            list.Add(new BildirimDto
            {
                Tarih = DateTime.Now.AddHours(-2),
                Tip = "Uyarı",
                Baslik = "Güncelleme",
                Icerik = "Yeni sürüm mevcut."
            });
            list.Add(new BildirimDto
            {
                Tarih = DateTime.Now.AddDays(-1),
                Tip = "Duyuru",
                Baslik = "Bakım",
                Icerik = "Pazar gecesi bakım planlandı."
            });
            return list;
        }
    }
}
