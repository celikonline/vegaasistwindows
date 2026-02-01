using System.Collections.Generic;
using System.Threading.Tasks;
using VegaAsis.Core.Contracts;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Data.Services
{
    public class ImmService : IImmService
    {
        public Task<IReadOnlyList<ImmTeminatDto>> GetTeminatlarAsync()
        {
            var list = new List<ImmTeminatDto>();
            return Task.FromResult<IReadOnlyList<ImmTeminatDto>>(list);
        }
    }
}
