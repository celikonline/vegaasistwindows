using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VegaAsis.Data.Entities
{
    [Table("user_settings", Schema = "public")]
    public class UserSettings
    {
        [Column("id")]
        public Guid Id { get; set; }
        [Column("user_id")]
        public Guid UserId { get; set; }
        [Column("gsm")]
        public string Gsm { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [Column("group_name")]
        public string GroupName { get; set; }
        [Column("is_office_worker")]
        public bool IsOfficeWorker { get; set; }
        [Column("can_view_web_service")]
        public bool CanViewWebService { get; set; }
        [Column("can_save_session")]
        public bool CanSaveSession { get; set; }
        [Column("hide_company_details")]
        public bool HideCompanyDetails { get; set; }
        [Column("allowed_ips")]
        public string AllowedIps { get; set; }
        [Column("banned_companies")]
        public string BannedCompanies { get; set; }
        [Column("company_restrictions")]
        public string CompanyRestrictions { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
