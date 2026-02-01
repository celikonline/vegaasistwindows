using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Core.Contracts
{
    public interface IUserGroupService
    {
        Task<IReadOnlyList<UserGroupDto>> GetAllAsync();
        Task<UserGroupDto> CreateAsync(UserGroupDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<IReadOnlyList<GroupMemberDto>> GetMembersAsync(Guid groupId);
        Task<bool> AddMemberAsync(Guid groupId, Guid memberUserId);
        Task<bool> RemoveMemberAsync(Guid groupId, Guid memberUserId);
    }
}
