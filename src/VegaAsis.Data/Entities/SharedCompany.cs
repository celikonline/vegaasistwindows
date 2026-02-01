using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VegaAsis.Data.Entities
{
    [Table("shared_companies", Schema = "public")]
    public class SharedCompany
    {
        [Column("id")]
        public Guid Id { get; set; }
        [Column("owner_user_id")]
        public Guid OwnerUserId { get; set; }
        [Column("company_name")]
        public string CompanyName { get; set; }
        [Column("shared_with_agent_code")]
        public string SharedWithAgentCode { get; set; }
        [Column("restrictions")]
        public string Restrictions { get; set; }
        [Column("status")]
        public string Status { get; set; }
        [Column("shared_at")]
        public DateTime SharedAt { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
