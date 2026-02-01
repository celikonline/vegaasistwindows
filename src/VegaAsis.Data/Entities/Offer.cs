using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VegaAsis.Data.Entities
{
    [Table("offers", Schema = "public")]
    public class Offer
    {
        [Column("id")]
        public Guid Id { get; set; }
        [Column("user_id")]
        public Guid UserId { get; set; }
        [Column("sirket")]
        public string Sirket { get; set; }
        [Column("personel")]
        public string Personel { get; set; }
        [Column("police_tipi")]
        public string PoliceTipi { get; set; }
        [Column("trf")]
        public bool Trf { get; set; }
        [Column("ksk")]
        public bool Ksk { get; set; }
        [Column("tss")]
        public bool Tss { get; set; }
        [Column("dsk")]
        public bool Dsk { get; set; }
        [Column("knt")]
        public bool Knt { get; set; }
        [Column("imm")]
        public bool Imm { get; set; }
        [Column("musteri")]
        public string Musteri { get; set; }
        [Column("tc_vergi")]
        public string TcVergi { get; set; }
        [Column("dogum_tarihi")]
        public DateTime? DogumTarihi { get; set; }
        [Column("plaka")]
        public string Plaka { get; set; }
        [Column("belge_seri")]
        public string BelgeSeri { get; set; }
        [Column("son_islem")]
        public DateTime? SonIslem { get; set; }
        [Column("kayit_sekli")]
        public string KayitSekli { get; set; }
        [Column("tali_dis_acente")]
        public string TaliDisAcente { get; set; }
        [Column("telefon")]
        public string Telefon { get; set; }
        [Column("calisildi")]
        public bool Calisildi { get; set; }
        [Column("aciklama")]
        public string Aciklama { get; set; }
        [Column("policelestirme")]
        public string Policelestirme { get; set; }
        [Column("acentem_gonderildi")]
        public bool AcentemGonderildi { get; set; }
        [Column("acentem_webe_gonderildi")]
        public bool AcentemWebeGonderildi { get; set; }
        [Column("meslek")]
        public string Meslek { get; set; }
        [Column("arsiv_donemi")]
        public string ArsivDonemi { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
