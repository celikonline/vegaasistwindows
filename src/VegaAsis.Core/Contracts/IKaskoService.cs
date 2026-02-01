using System.Collections.Generic;
using System.Threading.Tasks;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Core.Contracts
{
    public interface IKaskoService
    {
        Task<IReadOnlyList<KaskoTeminatDto>> GetTeminatlarAsync();
    }
}
