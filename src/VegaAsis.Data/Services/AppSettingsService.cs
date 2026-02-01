using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using VegaAsis.Core.Contracts;
using VegaAsis.Core.DTOs;
using VegaAsis.Data.Entities;

namespace VegaAsis.Data.Services
{
    public class AppSettingsService : IAppSettingsService
    {
        private readonly VegaAsisDbContext _db;

        public AppSettingsService(VegaAsisDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<AppSettingsDto> GetSettingsAsync()
        {
            var entity = await _db.AppSettings.FirstOrDefaultAsync().ConfigureAwait(false);
            return entity == null ? null : ToDto(entity);
        }

        public async Task<AppSettingsDto> UpdateSettingsAsync(AppSettingsDto dto)
        {
            AppSettings entity;
            if (dto.Id != Guid.Empty)
            {
                entity = await _db.AppSettings.FindAsync(dto.Id).ConfigureAwait(false);
                if (entity == null)
                {
                    return null;
                }
            }
            else
            {
                entity = await _db.AppSettings.FirstOrDefaultAsync().ConfigureAwait(false);
                if (entity == null)
                {
                    entity = new AppSettings
                    {
                        Id = Guid.NewGuid(),
                        CreatedAt = DateTime.UtcNow
                    };
                    _db.AppSettings.Add(entity);
                }
            }

            entity.UserId = dto.UserId;
            entity.LogoUrl = dto.LogoUrl;
            entity.WelcomeImageUrl = dto.WelcomeImageUrl;
            entity.FacebookUrl = dto.FacebookUrl;
            entity.InstagramUrl = dto.InstagramUrl;
            entity.LinkedinUrl = dto.LinkedinUrl;
            entity.TwitterUrl = dto.TwitterUrl;
            entity.WebsiteUrl = dto.WebsiteUrl;
            entity.Whatsapp = dto.Whatsapp;
            entity.HomeScreenText = dto.HomeScreenText;
            entity.PdfHeader = dto.PdfHeader;
            entity.PdfFooter = dto.PdfFooter;
            entity.PhoneRequiredOnLogin = dto.PhoneRequiredOnLogin;
            entity.ShowSocialMediaLogos = dto.ShowSocialMediaLogos;
            entity.DetailedPolicySaveScreen = dto.DetailedPolicySaveScreen;
            entity.HideCommissionFromHome = dto.HideCommissionFromHome;
            entity.UserSeeOwnPrices = dto.UserSeeOwnPrices;
            entity.MobileAssistanceActive = dto.MobileAssistanceActive;
            entity.OfficeWorkersSeeOwnOffers = dto.OfficeWorkersSeeOwnOffers;
            entity.UsersHidePoliciesScreen = dto.UsersHidePoliciesScreen;
            entity.ChatPermissionAdmins = dto.ChatPermissionAdmins;
            entity.ChatPermissionOfficeWorkers = dto.ChatPermissionOfficeWorkers;
            entity.ChatPermissionUsers = dto.ChatPermissionUsers;
            entity.CaptchaApiKey = dto.CaptchaApiKey;
            entity.IntegrationUrl = dto.IntegrationUrl;
            entity.SmsLoginActive = dto.SmsLoginActive;
            entity.SmsMessageHeader = dto.SmsMessageHeader;
            entity.SmsUserNo = dto.SmsUserNo;
            entity.SmsUsername = dto.SmsUsername;
            entity.SmsPassword = dto.SmsPassword;
            entity.SmsApiType = dto.SmsApiType;
            entity.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync().ConfigureAwait(false);
            return ToDto(entity);
        }

        private static AppSettingsDto ToDto(AppSettings entity)
        {
            return new AppSettingsDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                LogoUrl = entity.LogoUrl,
                WelcomeImageUrl = entity.WelcomeImageUrl,
                FacebookUrl = entity.FacebookUrl,
                InstagramUrl = entity.InstagramUrl,
                LinkedinUrl = entity.LinkedinUrl,
                TwitterUrl = entity.TwitterUrl,
                WebsiteUrl = entity.WebsiteUrl,
                Whatsapp = entity.Whatsapp,
                HomeScreenText = entity.HomeScreenText,
                PdfHeader = entity.PdfHeader,
                PdfFooter = entity.PdfFooter,
                PhoneRequiredOnLogin = entity.PhoneRequiredOnLogin,
                ShowSocialMediaLogos = entity.ShowSocialMediaLogos,
                DetailedPolicySaveScreen = entity.DetailedPolicySaveScreen,
                HideCommissionFromHome = entity.HideCommissionFromHome,
                UserSeeOwnPrices = entity.UserSeeOwnPrices,
                MobileAssistanceActive = entity.MobileAssistanceActive,
                OfficeWorkersSeeOwnOffers = entity.OfficeWorkersSeeOwnOffers,
                UsersHidePoliciesScreen = entity.UsersHidePoliciesScreen,
                ChatPermissionAdmins = entity.ChatPermissionAdmins,
                ChatPermissionOfficeWorkers = entity.ChatPermissionOfficeWorkers,
                ChatPermissionUsers = entity.ChatPermissionUsers,
                CaptchaApiKey = entity.CaptchaApiKey,
                IntegrationUrl = entity.IntegrationUrl,
                SmsLoginActive = entity.SmsLoginActive,
                SmsMessageHeader = entity.SmsMessageHeader,
                SmsUserNo = entity.SmsUserNo,
                SmsUsername = entity.SmsUsername,
                SmsPassword = entity.SmsPassword,
                SmsApiType = entity.SmsApiType,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }
    }
}
