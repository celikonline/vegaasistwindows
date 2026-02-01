using System;

namespace VegaAsis.Core.DTOs
{
    public class UserGroupDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
