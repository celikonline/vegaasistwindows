using System;

namespace VegaAsis.Core.DTOs
{
    public class PolicyDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string PoliceTuru { get; set; }
        public string PoliceNo { get; set; }
        public string YenilemeNo { get; set; }
        public string ZeyilNo { get; set; }
        public string Sirket { get; set; }
        public string Musteri { get; set; }
        public string TcVergi { get; set; }
        public string Plaka { get; set; }
        public decimal? Prim { get; set; }
        public string DovizTipi { get; set; }
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public DateTime KayitTarihi { get; set; }
        public string KayitTipi { get; set; }
        public string Personel { get; set; }
        public string KullaniciAdi { get; set; }
        public string AnaAcente { get; set; }
        public string KesenAcente { get; set; }
        public string AcentemAlinmaDurumu { get; set; }
        public string Aciklama { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
