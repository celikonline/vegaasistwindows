using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VegaAsis.Data.Entities
{
    [Table("profiles", Schema = "public")]
    public class Profile
    {
        [Column("id")]
        public Guid Id { get; set; }
        [Column("user_id")]
        public Guid UserId { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("full_name")]
        public string FullName { get; set; }
        [Column("phone")]
        public string Phone { get; set; }
        [Column("company_name")]
        public string CompanyName { get; set; }
        [Column("role")]
        public string Role { get; set; }
        [Column("agent_code")]
        public string AgentCode { get; set; }
        [Column("avatar_url")]
        public string AvatarUrl { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
