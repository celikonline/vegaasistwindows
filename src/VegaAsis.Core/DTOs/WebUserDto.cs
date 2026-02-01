using System;

namespace VegaAsis.Core.DTOs
{
    public class WebUserDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string WebUsername { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string Gsm { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public bool IsLicensed { get; set; }
        public bool LicenseOfferOnly { get; set; }
        public bool LicenseOnlinePolicy { get; set; }
        public bool LicenseCompanyScreen { get; set; }
        public bool UnlicensedAgentOnly { get; set; }
        public string ResponsibleStaff { get; set; }
        public string BannedCompanies { get; set; }
        public string InternalBans { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
