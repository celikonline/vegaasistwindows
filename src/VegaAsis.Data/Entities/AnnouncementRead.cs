using System;

namespace VegaAsis.Data.Entities
{
    /// <summary>
    /// Duyuru okundu bilgisi (kullanıcı bazlı).
    /// </summary>
    public class AnnouncementRead
    {
        public Guid Id { get; set; }
        public Guid AnnouncementId { get; set; }
        public Guid UserId { get; set; }
        public DateTime ReadAt { get; set; }
    }
}
