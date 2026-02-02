using System;

namespace VegaAsis.Core.DTOs
{
    /// <summary>
    /// DASK teklif/sorgu parametreleri (WS entegrasyonu için temel alanlar).
    /// </summary>
    public class DaskParametreDto
    {
        public string Il { get; set; }
        public string Ilce { get; set; }
        public string Mahalle { get; set; }
        public string YapiTarzi { get; set; }
        public int? KatSayisi { get; set; }
        public int? BinaYapimYili { get; set; }
        public string DaskPoliceNo { get; set; }
        public DateTime? DaskBaslangicTarihi { get; set; }
        public DateTime? DaskBitisTarihi { get; set; }
        public string TcVergi { get; set; }
        public string MusteriAdi { get; set; }
    }
}
