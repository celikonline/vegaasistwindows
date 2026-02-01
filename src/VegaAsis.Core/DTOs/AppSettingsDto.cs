using System;

namespace VegaAsis.Core.DTOs
{
    public class AppSettingsDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string LogoUrl { get; set; }
        public string WelcomeImageUrl { get; set; }
        public string FacebookUrl { get; set; }
        public string InstagramUrl { get; set; }
        public string LinkedinUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string WebsiteUrl { get; set; }
        public string Whatsapp { get; set; }
        public string HomeScreenText { get; set; }
        public string PdfHeader { get; set; }
        public string PdfFooter { get; set; }
        public bool PhoneRequiredOnLogin { get; set; }
        public bool ShowSocialMediaLogos { get; set; }
        public bool DetailedPolicySaveScreen { get; set; }
        public bool HideCommissionFromHome { get; set; }
        public bool UserSeeOwnPrices { get; set; }
        public bool MobileAssistanceActive { get; set; }
        public bool OfficeWorkersSeeOwnOffers { get; set; }
        public bool UsersHidePoliciesScreen { get; set; }
        public bool ChatPermissionAdmins { get; set; }
        public bool ChatPermissionOfficeWorkers { get; set; }
        public bool ChatPermissionUsers { get; set; }
        public string CaptchaApiKey { get; set; }
        public string IntegrationUrl { get; set; }
        public bool SmsLoginActive { get; set; }
        public string SmsMessageHeader { get; set; }
        public string SmsUserNo { get; set; }
        public string SmsUsername { get; set; }
        public string SmsPassword { get; set; }
        public string SmsApiType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
