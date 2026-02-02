using System;

namespace VegaAsis.Core.DTOs
{
    /// <summary>
    /// Kasko teklif/sorgu parametreleri (WS entegrasyonu için temel alanlar).
    /// </summary>
    public class KaskoParametreDto
    {
        public string Plaka { get; set; }
        public string TcVergi { get; set; }
        public string KaskoSigortaSirketi { get; set; }
        public string KaskoAcenteKodu { get; set; }
        public string KaskoPoliceNo { get; set; }
        public DateTime? KaskoBaslangicTarihi { get; set; }
        public DateTime? KaskoBitisTarihi { get; set; }
        public string KullaniciAdi { get; set; }
        public string MusteriAdi { get; set; }
    }
}
