using System.Collections.Generic;
using System.Threading.Tasks;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Core.Contracts
{
    public interface IDaskService
    {
        Task<IReadOnlyList<DaskTeminatDto>> GetTeminatlarAsync(string il = null, string ilce = null);
    }
}
