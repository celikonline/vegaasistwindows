using System;

namespace VegaAsis.Core.DTOs
{
    /// <summary>
    /// Trafik teklif/sorgu parametreleri (WS entegrasyonu için temel alanlar).
    /// </summary>
    public class TrafikParametreDto
    {
        public string Plaka { get; set; }
        public string TcVergi { get; set; }
        public string BelgeSeri { get; set; }
        public string BelgeNo { get; set; }
        public DateTime? DogumTarihi { get; set; }
        public string TrafikSigortaSirketi { get; set; }
        public string TrafikAcenteKodu { get; set; }
        public string TrafikPoliceNo { get; set; }
        public DateTime? TrafikBaslangicTarihi { get; set; }
        public DateTime? TrafikBitisTarihi { get; set; }
        public bool KisaVadeliPolice { get; set; }
        public string MusteriAdi { get; set; }
        public string Meslek { get; set; }
    }
}
