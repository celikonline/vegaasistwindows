using System;
using System.Collections.Generic;

namespace VegaAsis.Core.DTOs
{
    public class UserSettingsDto
    {
        public Guid? Id { get; set; }
        public Guid UserId { get; set; }
        public string Gsm { get; set; }
        public string Description { get; set; }
        public string GroupName { get; set; }
        public bool IsOfficeWorker { get; set; }
        public bool CanViewWebService { get; set; }
        public bool CanSaveSession { get; set; }
        public bool HideCompanyDetails { get; set; }
        public List<string> AllowedIps { get; set; }
        public List<string> BannedCompanies { get; set; }
        public Dictionary<string, Dictionary<string, bool>> CompanyRestrictions { get; set; }
    }
}
