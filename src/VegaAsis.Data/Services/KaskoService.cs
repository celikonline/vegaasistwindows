using System.Collections.Generic;
using System.Threading.Tasks;
using VegaAsis.Core.Contracts;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Data.Services
{
    public class KaskoService : IKaskoService
    {
        public Task<IReadOnlyList<KaskoTeminatDto>> GetTeminatlarAsync()
        {
            var list = new List<KaskoTeminatDto>();
            return Task.FromResult<IReadOnlyList<KaskoTeminatDto>>(list);
        }
    }
}
