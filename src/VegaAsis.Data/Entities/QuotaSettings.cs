using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VegaAsis.Data.Entities
{
    [Table("quota_settings", Schema = "public")]
    public class QuotaSettings
    {
        [Column("id")]
        public Guid Id { get; set; }
        [Column("user_id")]
        public Guid UserId { get; set; }
        [Column("daily_query_limit")]
        public int DailyQueryLimit { get; set; }
        [Column("monthly_query_limit")]
        public int MonthlyQueryLimit { get; set; }
        [Column("daily_offer_limit")]
        public int DailyOfferLimit { get; set; }
        [Column("monthly_offer_limit")]
        public int MonthlyOfferLimit { get; set; }
        [Column("daily_policy_limit")]
        public int DailyPolicyLimit { get; set; }
        [Column("monthly_policy_limit")]
        public int MonthlyPolicyLimit { get; set; }
        [Column("current_daily_queries")]
        public int CurrentDailyQueries { get; set; }
        [Column("current_monthly_queries")]
        public int CurrentMonthlyQueries { get; set; }
        [Column("current_daily_offers")]
        public int CurrentDailyOffers { get; set; }
        [Column("current_monthly_offers")]
        public int CurrentMonthlyOffers { get; set; }
        [Column("current_daily_policies")]
        public int CurrentDailyPolicies { get; set; }
        [Column("current_monthly_policies")]
        public int CurrentMonthlyPolicies { get; set; }
        [Column("last_daily_reset")]
        public DateTime? LastDailyReset { get; set; }
        [Column("last_monthly_reset")]
        public DateTime? LastMonthlyReset { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
