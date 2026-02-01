using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VegaAsis.Data.Entities
{
    [Table("appointments", Schema = "public")]
    public class Appointment
    {
        [Column("id")]
        public Guid Id { get; set; }
        [Column("user_id")]
        public Guid UserId { get; set; }
        [Column("title")]
        public string Title { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [Column("location")]
        public string Location { get; set; }
        [Column("label")]
        public string Label { get; set; }
        [Column("time_display")]
        public string TimeDisplay { get; set; }
        [Column("source")]
        public string Source { get; set; }
        [Column("start_date")]
        public DateTime StartDate { get; set; }
        [Column("start_time")]
        public TimeSpan StartTime { get; set; }
        [Column("end_date")]
        public DateTime EndDate { get; set; }
        [Column("end_time")]
        public TimeSpan EndTime { get; set; }
        [Column("all_day")]
        public bool AllDay { get; set; }
        [Column("reminder_minutes")]
        public int? ReminderMinutes { get; set; }
        [Column("reminder_sent")]
        public bool ReminderSent { get; set; }
        [Column("customer_email")]
        public string CustomerEmail { get; set; }
        [Column("customer_phone")]
        public string CustomerPhone { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
