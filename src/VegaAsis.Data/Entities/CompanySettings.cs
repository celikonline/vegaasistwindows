using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VegaAsis.Data.Entities
{
    [Table("company_settings", Schema = "public")]
    public class CompanySettings
    {
        [Column("id")]
        public Guid Id { get; set; }
        [Column("user_id")]
        public Guid UserId { get; set; }
        [Column("company_name")]
        public string CompanyName { get; set; }
        [Column("company_username")]
        public string CompanyUsername { get; set; }
        [Column("company_password")]
        public string CompanyPassword { get; set; }
        [Column("proxy_address")]
        public string ProxyAddress { get; set; }
        [Column("proxy_user")]
        public string ProxyUser { get; set; }
        [Column("proxy_password")]
        public string ProxyPassword { get; set; }
        [Column("google_secret_key")]
        public string GoogleSecretKey { get; set; }
        [Column("ip_addresses")]
        public string IpAddresses { get; set; }
        [Column("kasko_special_discount")]
        public decimal KaskoSpecialDiscount { get; set; }
        [Column("kasko_teklifi_kaydet")]
        public bool KaskoTeklifiKaydet { get; set; }
        [Column("trafik_teklifi_kaydet")]
        public bool TrafikTeklifiKaydet { get; set; }
        [Column("oto_session_kaydet")]
        public bool OtoSessionKaydet { get; set; }
        [Column("ikame_arac_hizmeti")]
        public bool IkameAracHizmeti { get; set; }
        [Column("kesilen_paket_getir")]
        public bool KesilenPaketGetir { get; set; }
        [Column("company_bans")]
        public string CompanyBans { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
