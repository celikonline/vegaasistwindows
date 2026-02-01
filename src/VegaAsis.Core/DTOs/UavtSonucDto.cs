using System;

namespace VegaAsis.Core.DTOs
{
    /// <summary>
    /// UAVT sorgu sonucu – kaynak, tarih, açıklama, durum.
    /// </summary>
    public class UavtSonucDto
    {
        public string Kaynak { get; set; }
        public DateTime? Tarih { get; set; }
        public string Aciklama { get; set; }
        public string Durum { get; set; }
    }
}
