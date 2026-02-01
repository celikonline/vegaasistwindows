using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VegaAsis.Data.Entities
{
    [Table("received_company_shares", Schema = "public")]
    public class ReceivedCompanyShare
    {
        [Column("id")]
        public Guid Id { get; set; }
        [Column("receiver_user_id")]
        public Guid ReceiverUserId { get; set; }
        [Column("share_id")]
        public Guid? ShareId { get; set; }
        [Column("company_name")]
        public string CompanyName { get; set; }
        [Column("from_agent_code")]
        public string FromAgentCode { get; set; }
        [Column("received_at")]
        public DateTime ReceivedAt { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
