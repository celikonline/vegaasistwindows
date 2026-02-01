using System;

namespace VegaAsis.Data.Entities
{
    public class Announcement
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TargetRole { get; set; }
    }
}
