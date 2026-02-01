using System;

namespace VegaAsis.Core.DTOs
{
    public class QuotaSettingsDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int DailyQueryLimit { get; set; }
        public int MonthlyQueryLimit { get; set; }
        public int DailyOfferLimit { get; set; }
        public int MonthlyOfferLimit { get; set; }
        public int DailyPolicyLimit { get; set; }
        public int MonthlyPolicyLimit { get; set; }
        public int CurrentDailyQueries { get; set; }
        public int CurrentMonthlyQueries { get; set; }
        public int CurrentDailyOffers { get; set; }
        public int CurrentMonthlyOffers { get; set; }
        public int CurrentDailyPolicies { get; set; }
        public int CurrentMonthlyPolicies { get; set; }
        public DateTime? LastDailyReset { get; set; }
        public DateTime? LastMonthlyReset { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
