using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VegaAsis.Data.Entities
{
    [Table("group_members", Schema = "public")]
    public class GroupMember
    {
        [Column("id")]
        public Guid Id { get; set; }
        [Column("group_id")]
        public Guid GroupId { get; set; }
        [Column("member_user_id")]
        public Guid MemberUserId { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
