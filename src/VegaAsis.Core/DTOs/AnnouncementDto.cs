using System;

namespace VegaAsis.Core.DTOs
{
    public class AnnouncementDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TargetRole { get; set; }
    }
}
