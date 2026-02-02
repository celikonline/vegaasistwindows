using System;

namespace VegaAsis.Core.DTOs
{
    /// <summary>
    /// Konut teklif/sorgu parametreleri (WS entegrasyonu için temel alanlar).
    /// </summary>
    public class KonutParametreDto
    {
        public string Il { get; set; }
        public string Ilce { get; set; }
        public string Mahalle { get; set; }
        public string KonutTipi { get; set; }
        public int? KatSayisi { get; set; }
        public int? BinaYapimYili { get; set; }
        public string TcVergi { get; set; }
        public string MusteriAdi { get; set; }
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
    }
}
