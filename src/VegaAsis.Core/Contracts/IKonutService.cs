using System.Collections.Generic;
using System.Threading.Tasks;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Core.Contracts
{
    public interface IKonutService
    {
        Task<IReadOnlyList<KonutTeminatDto>> GetTeminatlarAsync();
    }
}
