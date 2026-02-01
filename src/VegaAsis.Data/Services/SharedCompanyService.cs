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
    public class SharedCompanyService : ISharedCompanyService
    {
        private readonly VegaAsisDbContext _db;

        public SharedCompanyService(VegaAsisDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<IReadOnlyList<SharedCompanyDto>> GetAllAsync(Guid? ownerUserId = null)
        {
            var query = _db.SharedCompanies.OrderByDescending(s => s.CreatedAt).AsQueryable();
            if (ownerUserId.HasValue)
            {
                query = query.Where(s => s.OwnerUserId == ownerUserId.Value);
            }

            var list = await query.ToListAsync().ConfigureAwait(false);
            return list.Select(ToDto).ToList();
        }

        public async Task<SharedCompanyDto> CreateAsync(SharedCompanyDto dto)
        {
            var entity = new SharedCompany
            {
                Id = Guid.NewGuid(),
                OwnerUserId = dto.OwnerUserId,
                CompanyName = dto.CompanyName,
                SharedWithAgentCode = dto.SharedWithAgentCode,
                Restrictions = dto.Restrictions,
                Status = dto.Status,
                SharedAt = dto.SharedAt != default(DateTime) ? dto.SharedAt : DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.SharedCompanies.Add(entity);
            await _db.SaveChangesAsync().ConfigureAwait(false);
            return ToDto(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _db.SharedCompanies.FindAsync(id).ConfigureAwait(false);
            if (entity == null)
            {
                return false;
            }

            _db.SharedCompanies.Remove(entity);
            await _db.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }

        public async Task<SharedCompanyDto> UpdateAsync(Guid id, SharedCompanyDto dto)
        {
            var entity = await _db.SharedCompanies.FindAsync(id).ConfigureAwait(false);
            if (entity == null)
            {
                return null;
            }

            entity.Status = dto.Status ?? entity.Status;
            entity.Restrictions = dto.Restrictions ?? entity.Restrictions;
            entity.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync().ConfigureAwait(false);
            return ToDto(entity);
        }

        public async Task<IReadOnlyList<ReceivedCompanyShareDto>> GetReceivedSharesAsync(Guid receiverUserId)
        {
            var list = await _db.ReceivedCompanyShares
                .Where(r => r.ReceiverUserId == receiverUserId)
                .OrderByDescending(r => r.ReceivedAt)
                .ToListAsync()
                .ConfigureAwait(false);

            return list.Select(ToReceivedShareDto).ToList();
        }

        private static SharedCompanyDto ToDto(SharedCompany entity)
        {
            return new SharedCompanyDto
            {
                Id = entity.Id,
                OwnerUserId = entity.OwnerUserId,
                CompanyName = entity.CompanyName,
                SharedWithAgentCode = entity.SharedWithAgentCode,
                Restrictions = entity.Restrictions,
                Status = entity.Status,
                SharedAt = entity.SharedAt,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        private static ReceivedCompanyShareDto ToReceivedShareDto(ReceivedCompanyShare entity)
        {
            return new ReceivedCompanyShareDto
            {
                Id = entity.Id,
                ReceiverUserId = entity.ReceiverUserId,
                ShareId = entity.ShareId,
                CompanyName = entity.CompanyName,
                FromAgentCode = entity.FromAgentCode,
                ReceivedAt = entity.ReceivedAt,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
