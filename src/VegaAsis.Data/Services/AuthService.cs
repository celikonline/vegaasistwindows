using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using VegaAsis.Core.Contracts;
using VegaAsis.Core.DTOs;
using VegaAsis.Data.Entities;

namespace VegaAsis.Data.Services
{
    public class AuthService : IAuthService
    {
        private readonly VegaAsisDbContext _db;
        private Guid? _currentUserId;

        public AuthService(VegaAsisDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public Guid? GetCurrentUserId
        {
            get { return _currentUserId; }
        }

        public ProfileDto GetCurrentProfile()
        {
            if (!_currentUserId.HasValue)
            {
                return null;
            }

            var profile = _db.Profiles.FirstOrDefault(p => p.UserId == _currentUserId.Value);
            return profile == null ? null : ToDto(profile);
        }

        public async Task<bool> LoginAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var emailNorm = email.Trim();
            var profiles = await _db.Profiles.ToListAsync().ConfigureAwait(false);
            var profile = profiles.FirstOrDefault(p =>
                p.Email != null &&
                string.Equals(p.Email.Trim(), emailNorm, StringComparison.OrdinalIgnoreCase));

            if (profile == null)
            {
                return false;
            }

            _currentUserId = profile.UserId;
            return true;
        }

        public void Logout()
        {
            _currentUserId = null;
        }

        private static ProfileDto ToDto(Profile p)
        {
            return new ProfileDto
            {
                Id = p.Id,
                UserId = p.UserId,
                Email = p.Email,
                FullName = p.FullName,
                Phone = p.Phone,
                CompanyName = p.CompanyName,
                Role = p.Role,
                AgentCode = p.AgentCode,
                AvatarUrl = p.AvatarUrl,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            };
        }
    }
}
