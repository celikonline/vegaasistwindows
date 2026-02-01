using System;

namespace VegaAsis.Core.DTOs
{
    public class GroupMemberDto
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public Guid MemberUserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
