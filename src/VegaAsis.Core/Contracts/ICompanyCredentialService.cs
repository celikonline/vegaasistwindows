using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VegaAsis.Core.Contracts
{
    /// <summary>
    /// Şirket portal giriş bilgileri servisi.
    /// </summary>
    public interface ICompanyCredentialService
    {
        /// <summary>
        /// Belirtilen şirket ve kullanıcı için credential bilgisini döndürür.
        /// </summary>
        Task<CompanyCredentialDto> GetCredentialAsync(string companyId, Guid? userId = null);

        /// <summary>
        /// Belirtilen kullanıcının tüm şirket credential'larını döndürür.
        /// </summary>
        Task<IReadOnlyList<CompanyCredentialDto>> GetAllCredentialsAsync(Guid? userId = null);

        /// <summary>
        /// Credential bilgisini kaydeder (varsa günceller, yoksa oluşturur).
        /// </summary>
        Task<CompanyCredentialDto> SaveCredentialAsync(string companyId, string username, string password, Guid? userId = null);

        /// <summary>
        /// Credential bilgisini siler.
        /// </summary>
        Task<bool> DeleteCredentialAsync(string companyId, Guid? userId = null);

        /// <summary>
        /// Credential'ın aktif/pasif durumunu değiştirir.
        /// </summary>
        Task<bool> SetActiveAsync(string companyId, bool isActive, Guid? userId = null);
    }

    /// <summary>
    /// Şirket credential DTO.
    /// </summary>
    public class CompanyCredentialDto
    {
        public Guid Id { get; set; }
        public string CompanyId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Guid? UserId { get; set; }
        public bool IsActive { get; set; }
    }
}
