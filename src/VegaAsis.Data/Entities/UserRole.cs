using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VegaAsis.Data.Entities
{
    [Table("user_roles", Schema = "public")]
    public class UserRole
    {
        [Column("id")]
        public Guid Id { get; set; }
        [Column("user_id")]
        public Guid UserId { get; set; }
        [Column("role")]
        public string Role { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
