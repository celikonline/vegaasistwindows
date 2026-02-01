using System;

namespace VegaAsis.Core.DTOs
{
    /// <summary>
    /// WS teklif sorgu sonucu – tarih, plaka, şirket, branş, fiyat.
    /// </summary>
    public class WSTeklifSonucDto
    {
        public DateTime? Tarih { get; set; }
        public string Plaka { get; set; }
        public string Sirket { get; set; }
        public string Brans { get; set; }
        public string Fiyat { get; set; }
    }
}
