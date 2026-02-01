using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VegaAsis.Data.Entities
{
    [Table("web_users", Schema = "public")]
    public class WebUser
    {
        [Column("id")]
        public Guid Id { get; set; }
        [Column("user_id")]
        public Guid UserId { get; set; }
        [Column("web_username")]
        public string WebUsername { get; set; }
        [Column("username")]
        public string Username { get; set; }
        [Column("full_name")]
        public string FullName { get; set; }
        [Column("password")]
        public string Password { get; set; }
        [Column("gsm")]
        public string Gsm { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [Column("is_licensed")]
        public bool IsLicensed { get; set; }
        [Column("license_offer_only")]
        public bool LicenseOfferOnly { get; set; }
        [Column("license_online_policy")]
        public bool LicenseOnlinePolicy { get; set; }
        [Column("license_company_screen")]
        public bool LicenseCompanyScreen { get; set; }
        [Column("unlicensed_agent_only")]
        public bool UnlicensedAgentOnly { get; set; }
        [Column("responsible_staff")]
        public string ResponsibleStaff { get; set; }
        [Column("banned_companies")]
        public string BannedCompanies { get; set; }
        [Column("internal_bans")]
        public string InternalBans { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
