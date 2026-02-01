using System;

namespace VegaAsis.Core.DTOs
{
    public class ReceivedCompanyShareDto
    {
        public Guid Id { get; set; }
        public Guid ReceiverUserId { get; set; }
        public Guid? ShareId { get; set; }
        public string CompanyName { get; set; }
        public string FromAgentCode { get; set; }
        public DateTime ReceivedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
