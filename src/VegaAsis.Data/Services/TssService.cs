using System.Collections.Generic;
using System.Threading.Tasks;
using VegaAsis.Core.Contracts;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Data.Services
{
    public class TssService : ITssService
    {
        public Task<IReadOnlyList<TssBireyDto>> GetAileBireyleriAsync()
        {
            var list = new List<TssBireyDto>();
            return Task.FromResult<IReadOnlyList<TssBireyDto>>(list);
        }
    }
}
