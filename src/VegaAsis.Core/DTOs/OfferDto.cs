using System;

namespace VegaAsis.Core.DTOs
{
    public class OfferDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Sirket { get; set; }
        public string Personel { get; set; }
        public string PoliceTipi { get; set; }
        public bool Trf { get; set; }
        public bool Ksk { get; set; }
        public bool Tss { get; set; }
        public bool Dsk { get; set; }
        public bool Knt { get; set; }
        public bool Imm { get; set; }
        public string Musteri { get; set; }
        public string TcVergi { get; set; }
        public DateTime? DogumTarihi { get; set; }
        public string Plaka { get; set; }
        public string BelgeSeri { get; set; }
        public DateTime? SonIslem { get; set; }
        public string KayitSekli { get; set; }
        public string TaliDisAcente { get; set; }
        public string Telefon { get; set; }
        public bool Calisildi { get; set; }
        public string Aciklama { get; set; }
        public string Policelestirme { get; set; }
        public bool AcentemGonderildi { get; set; }
        public bool AcentemWebeGonderildi { get; set; }
        public string Meslek { get; set; }
        public string ArsivDonemi { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
