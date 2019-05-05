using System;
using System.Threading.Tasks;

using TAS.Core.Models;

namespace TAS.Core.Repositories
{
    public interface IRemoteRateSource
    {
        Task<Rate[]> GetRatesAsync(DateTime date);
    }
}
