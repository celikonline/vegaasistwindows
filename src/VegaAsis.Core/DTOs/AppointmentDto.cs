using System;

namespace VegaAsis.Core.DTOs
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Label { get; set; }
        public string TimeDisplay { get; set; }
        public string Source { get; set; }
        public DateTime StartDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool AllDay { get; set; }
        public int? ReminderMinutes { get; set; }
        public bool ReminderSent { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
