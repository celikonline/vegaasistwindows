using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Core.Contracts
{
    public interface ICompanySettingsService
    {
        Task<List<string>> GetSelectedCompaniesAsync();
        CompanySettingsDto GetCompanySetting(string companyName);
        Task<bool> SaveCompanySettingAsync(CompanySettingsDto data);
        Task<bool> DeleteCompanySettingAsync(string companyName);
        Task LoadSettingsAsync();
    }
}
