using System.Collections.Generic;
using System.Threading.Tasks;
using VegaAsis.Core.Contracts;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Data.Services
{
    public class KonutService : IKonutService
    {
        public Task<IReadOnlyList<KonutTeminatDto>> GetTeminatlarAsync()
        {
            var list = new List<KonutTeminatDto>();
            return Task.FromResult<IReadOnlyList<KonutTeminatDto>>(list);
        }
    }
}
