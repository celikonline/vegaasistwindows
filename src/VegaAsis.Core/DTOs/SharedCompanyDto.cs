using System;

namespace VegaAsis.Core.DTOs
{
    public class SharedCompanyDto
    {
        public Guid Id { get; set; }
        public Guid OwnerUserId { get; set; }
        public string CompanyName { get; set; }
        public string SharedWithAgentCode { get; set; }
        public string Restrictions { get; set; }
        public string Status { get; set; }
        public DateTime SharedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
