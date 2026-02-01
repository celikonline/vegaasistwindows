using System;

namespace VegaAsis.Core.DTOs
{
    /// <summary>
    /// Tramer sorgu sonucu – plaka, marka, model, hasar tarihi, şirket vb.
    /// </summary>
    public class TramerSonucDto
    {
        public string Plaka { get; set; }
        public string Marka { get; set; }
        public string Model { get; set; }
        public DateTime? HasarTarihi { get; set; }
        public string Sirket { get; set; }
        public string Aciklama { get; set; }
    }
}
