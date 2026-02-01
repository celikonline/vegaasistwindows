using System;
using System.Threading.Tasks;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Core.Contracts
{
    public interface IAuthService
    {
        Guid? GetCurrentUserId { get; }
        ProfileDto GetCurrentProfile();
        Task<bool> LoginAsync(string email);
        void Logout();
    }
}
