using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using VegaAsis.Core.Contracts;
using VegaAsis.Core.DTOs;
using VegaAsis.Data.Entities;

namespace VegaAsis.Data.Services
{
    public class UserGroupService : IUserGroupService
    {
        private readonly VegaAsisDbContext _db;

        public UserGroupService(VegaAsisDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<IReadOnlyList<UserGroupDto>> GetAllAsync()
        {
            var entities = await _db.UserGroups
                .OrderBy(g => g.GroupName)
                .ToListAsync()
                .ConfigureAwait(false);

            return entities.Select(ToDto).ToList();
        }

        public async Task<UserGroupDto> CreateAsync(UserGroupDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            var entity = new UserGroup
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                GroupName = dto.GroupName ?? string.Empty,
                Description = dto.Description ?? string.Empty,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.UserGroups.Add(entity);
            await _db.SaveChangesAsync().ConfigureAwait(false);
            return ToDto(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _db.UserGroups.FindAsync(id).ConfigureAwait(false);
            if (entity == null)
            {
                return false;
            }

            // Önce grup üyelerini sil
            var members = await _db.GroupMembers
                .Where(m => m.GroupId == id)
                .ToListAsync()
                .ConfigureAwait(false);

            foreach (var member in members)
            {
                _db.GroupMembers.Remove(member);
            }

            _db.UserGroups.Remove(entity);
            await _db.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }

        public async Task<IReadOnlyList<GroupMemberDto>> GetMembersAsync(Guid groupId)
        {
            var entities = await _db.GroupMembers
                .Where(m => m.GroupId == groupId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync()
                .ConfigureAwait(false);

            return entities.Select(ToGroupMemberDto).ToList();
        }

        public async Task<bool> AddMemberAsync(Guid groupId, Guid memberUserId)
        {
            // Grup var mı kontrol et
            var groupExists = await _db.UserGroups
                .AnyAsync(g => g.Id == groupId)
                .ConfigureAwait(false);

            if (!groupExists)
            {
                return false;
            }

            // Zaten üye mi kontrol et
            var existingMember = await _db.GroupMembers
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.MemberUserId == memberUserId)
                .ConfigureAwait(false);

            if (existingMember != null)
            {
                return false;
            }

            var entity = new GroupMember
            {
                Id = Guid.NewGuid(),
                GroupId = groupId,
                MemberUserId = memberUserId,
                CreatedAt = DateTime.UtcNow
            };

            _db.GroupMembers.Add(entity);
            await _db.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }

        public async Task<bool> RemoveMemberAsync(Guid groupId, Guid memberUserId)
        {
            var entity = await _db.GroupMembers
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.MemberUserId == memberUserId)
                .ConfigureAwait(false);

            if (entity == null)
            {
                return false;
            }

            _db.GroupMembers.Remove(entity);
            await _db.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }

        private static UserGroupDto ToDto(UserGroup entity)
        {
            return new UserGroupDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                GroupName = entity.GroupName,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        private static GroupMemberDto ToGroupMemberDto(GroupMember entity)
        {
            return new GroupMemberDto
            {
                Id = entity.Id,
                GroupId = entity.GroupId,
                MemberUserId = entity.MemberUserId,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
