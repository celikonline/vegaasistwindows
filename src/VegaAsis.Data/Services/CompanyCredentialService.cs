using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VegaAsis.Core.Contracts;
using VegaAsis.Data.Entities;

namespace VegaAsis.Data.Services
{
    /// <summary>
    /// Şirket portal giriş bilgileri servisi implementasyonu.
    /// </summary>
    public class CompanyCredentialService : ICompanyCredentialService
    {
        private readonly VegaAsisDbContext _db;

        public CompanyCredentialService(VegaAsisDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        /// <summary>
        /// company_credentials tablosu yoksa oluşturur. Uygulama başlangıcında çağrılabilir.
        /// </summary>
        public static void EnsureCompanyCredentialsTable(VegaAsisDbContext db)
        {
            if (db == null) return;
            try
            {
                db.Database.ExecuteSqlCommand(@"
CREATE TABLE IF NOT EXISTS company_credentials (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    company_id VARCHAR(50) NOT NULL,
    username VARCHAR(100),
    password_encrypted VARCHAR(256),
    user_id UUID,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW(),
    UNIQUE(company_id, user_id)
)");
                db.Database.ExecuteSqlCommand("CREATE INDEX IF NOT EXISTS idx_company_credentials_company_id ON company_credentials(company_id)");
                db.Database.ExecuteSqlCommand("CREATE INDEX IF NOT EXISTS idx_company_credentials_user_id ON company_credentials(user_id)");
                db.Database.ExecuteSqlCommand("CREATE INDEX IF NOT EXISTS idx_company_credentials_active ON company_credentials(is_active) WHERE is_active = true");
            }
            catch
            {
                // Tablo zaten var veya yetki hatası - sessizce geç
            }
        }

        public async Task<CompanyCredentialDto> GetCredentialAsync(string companyId, Guid? userId = null)
        {
            if (string.IsNullOrWhiteSpace(companyId))
                return null;

            var query = _db.CompanyCredentials
                .Where(c => c.CompanyId == companyId && c.IsActive);

            if (userId.HasValue)
                query = query.Where(c => c.UserId == userId.Value);

            var entity = await query.FirstOrDefaultAsync().ConfigureAwait(false);
            return entity != null ? ToDto(entity) : null;
        }

        public async Task<IReadOnlyList<CompanyCredentialDto>> GetAllCredentialsAsync(Guid? userId = null)
        {
            var query = _db.CompanyCredentials.Where(c => c.IsActive);

            if (userId.HasValue)
                query = query.Where(c => c.UserId == userId.Value);

            var list = await query
                .OrderBy(c => c.CompanyId)
                .ToListAsync()
                .ConfigureAwait(false);

            return list.Select(ToDto).ToList();
        }

        public async Task<CompanyCredentialDto> SaveCredentialAsync(string companyId, string username, string password, Guid? userId = null)
        {
            if (string.IsNullOrWhiteSpace(companyId))
                throw new ArgumentException("CompanyId boş olamaz", nameof(companyId));

            var existing = await _db.CompanyCredentials
                .FirstOrDefaultAsync(c => c.CompanyId == companyId && c.UserId == userId)
                .ConfigureAwait(false);

            if (existing != null)
            {
                // Güncelle
                existing.Username = username;
                existing.PasswordEncrypted = EncryptPassword(password);
                existing.UpdatedAt = DateTime.UtcNow;
                existing.IsActive = true;
            }
            else
            {
                // Yeni oluştur
                existing = new CompanyCredential
                {
                    Id = Guid.NewGuid(),
                    CompanyId = companyId,
                    Username = username,
                    PasswordEncrypted = EncryptPassword(password),
                    UserId = userId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _db.CompanyCredentials.Add(existing);
            }

            await _db.SaveChangesAsync().ConfigureAwait(false);
            return ToDto(existing);
        }

        public async Task<bool> DeleteCredentialAsync(string companyId, Guid? userId = null)
        {
            var entity = await _db.CompanyCredentials
                .FirstOrDefaultAsync(c => c.CompanyId == companyId && c.UserId == userId)
                .ConfigureAwait(false);

            if (entity == null)
                return false;

            _db.CompanyCredentials.Remove(entity);
            await _db.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }

        public async Task<bool> SetActiveAsync(string companyId, bool isActive, Guid? userId = null)
        {
            var entity = await _db.CompanyCredentials
                .FirstOrDefaultAsync(c => c.CompanyId == companyId && c.UserId == userId)
                .ConfigureAwait(false);

            if (entity == null)
                return false;

            entity.IsActive = isActive;
            entity.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }

        private static CompanyCredentialDto ToDto(CompanyCredential entity)
        {
            return new CompanyCredentialDto
            {
                Id = entity.Id,
                CompanyId = entity.CompanyId,
                Username = entity.Username,
                Password = DecryptPassword(entity.PasswordEncrypted),
                UserId = entity.UserId,
                IsActive = entity.IsActive
            };
        }

        /// <summary>
        /// Şifreyi basit Base64 encode ile saklar.
        /// Üretim ortamında daha güvenli bir şifreleme kullanılmalıdır.
        /// </summary>
        private static string EncryptPassword(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            // Basit Base64 encode - üretimde AES/RSA kullanılmalı
            var bytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Şifreyi decode eder.
        /// </summary>
        private static string DecryptPassword(string encrypted)
        {
            if (string.IsNullOrEmpty(encrypted))
                return string.Empty;

            try
            {
                var bytes = Convert.FromBase64String(encrypted);
                return Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return encrypted; // Decode edilemezse olduğu gibi döndür
            }
        }
    }
}
