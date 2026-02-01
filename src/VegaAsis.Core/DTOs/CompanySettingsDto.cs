using System;
using System.Collections.Generic;

namespace VegaAsis.Core.DTOs
{
    public class CompanySettingsDto
    {
        public Guid? Id { get; set; }
        public string CompanyName { get; set; }
        public string ProxyAddress { get; set; }
        public string ProxyUser { get; set; }
        public string ProxyPassword { get; set; }
        public string CompanyUsername { get; set; }
        public string CompanyPassword { get; set; }
        public string GoogleSecretKey { get; set; }
        public decimal KaskoSpecialDiscount { get; set; }
        public string IpAddresses { get; set; }
        public bool TrafikTeklifiKaydet { get; set; }
        public bool KaskoTeklifiKaydet { get; set; }
        public bool OtoSessionKaydet { get; set; }
        public bool IkameAracHizmeti { get; set; }
        public bool KesilenPaketGetir { get; set; }
        public Dictionary<string, bool> CompanyBans { get; set; }
    }
}
