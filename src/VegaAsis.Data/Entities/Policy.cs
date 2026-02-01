using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VegaAsis.Data.Entities
{
    [Table("policies", Schema = "public")]
    public class Policy
    {
        [Column("id")]
        public Guid Id { get; set; }
        [Column("user_id")]
        public Guid UserId { get; set; }
        [Column("police_turu")]
        public string PoliceTuru { get; set; }
        [Column("police_no")]
        public string PoliceNo { get; set; }
        [Column("yenileme_no")]
        public string YenilemeNo { get; set; }
        [Column("zeyil_no")]
        public string ZeyilNo { get; set; }
        [Column("sirket")]
        public string Sirket { get; set; }
        [Column("musteri")]
        public string Musteri { get; set; }
        [Column("tc_vergi")]
        public string TcVergi { get; set; }
        [Column("plaka")]
        public string Plaka { get; set; }
        [Column("prim")]
        public decimal? Prim { get; set; }
        [Column("doviz_tipi")]
        public string DovizTipi { get; set; }
        [Column("baslangic_tarihi")]
        public DateTime? BaslangicTarihi { get; set; }
        [Column("bitis_tarihi")]
        public DateTime? BitisTarihi { get; set; }
        [Column("kayit_tarihi")]
        public DateTime KayitTarihi { get; set; }
        [Column("kayit_tipi")]
        public string KayitTipi { get; set; }
        [Column("personel")]
        public string Personel { get; set; }
        [Column("kullanici_adi")]
        public string KullaniciAdi { get; set; }
        [Column("ana_acente")]
        public string AnaAcente { get; set; }
        [Column("kesen_acente")]
        public string KesenAcente { get; set; }
        [Column("acentem_alinma_durumu")]
        public string AcentemAlinmaDurumu { get; set; }
        [Column("aciklama")]
        public string Aciklama { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
