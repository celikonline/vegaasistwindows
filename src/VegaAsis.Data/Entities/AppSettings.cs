using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VegaAsis.Data.Entities
{
    [Table("app_settings", Schema = "public")]
    public class AppSettings
    {
        [Column("id")]
        public Guid Id { get; set; }
        [Column("user_id")]
        public Guid UserId { get; set; }
        [Column("logo_url")]
        public string LogoUrl { get; set; }
        [Column("welcome_image_url")]
        public string WelcomeImageUrl { get; set; }
        [Column("facebook_url")]
        public string FacebookUrl { get; set; }
        [Column("instagram_url")]
        public string InstagramUrl { get; set; }
        [Column("linkedin_url")]
        public string LinkedinUrl { get; set; }
        [Column("twitter_url")]
        public string TwitterUrl { get; set; }
        [Column("website_url")]
        public string WebsiteUrl { get; set; }
        [Column("whatsapp")]
        public string Whatsapp { get; set; }
        [Column("home_screen_text")]
        public string HomeScreenText { get; set; }
        [Column("pdf_header")]
        public string PdfHeader { get; set; }
        [Column("pdf_footer")]
        public string PdfFooter { get; set; }
        [Column("phone_required_on_login")]
        public bool PhoneRequiredOnLogin { get; set; }
        [Column("show_social_media_logos")]
        public bool ShowSocialMediaLogos { get; set; }
        [Column("detailed_policy_save_screen")]
        public bool DetailedPolicySaveScreen { get; set; }
        [Column("hide_commission_from_home")]
        public bool HideCommissionFromHome { get; set; }
        [Column("user_see_own_prices")]
        public bool UserSeeOwnPrices { get; set; }
        [Column("mobile_assistance_active")]
        public bool MobileAssistanceActive { get; set; }
        [Column("office_workers_see_own_offers")]
        public bool OfficeWorkersSeeOwnOffers { get; set; }
        [Column("users_hide_policies_screen")]
        public bool UsersHidePoliciesScreen { get; set; }
        [Column("chat_permission_admins")]
        public bool ChatPermissionAdmins { get; set; }
        [Column("chat_permission_office_workers")]
        public bool ChatPermissionOfficeWorkers { get; set; }
        [Column("chat_permission_users")]
        public bool ChatPermissionUsers { get; set; }
        [Column("captcha_api_key")]
        public string CaptchaApiKey { get; set; }
        [Column("integration_url")]
        public string IntegrationUrl { get; set; }
        [Column("sms_login_active")]
        public bool SmsLoginActive { get; set; }
        [Column("sms_message_header")]
        public string SmsMessageHeader { get; set; }
        [Column("sms_user_no")]
        public string SmsUserNo { get; set; }
        [Column("sms_username")]
        public string SmsUsername { get; set; }
        [Column("sms_password")]
        public string SmsPassword { get; set; }
        [Column("sms_api_type")]
        public string SmsApiType { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
