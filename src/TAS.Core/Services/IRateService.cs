using System;
using System.Threading.Tasks;

using TAS.Core.Models;

namespace TAS.Core.Services
{
    public interface IRateService
    {
        Task<Rate[]> GetRatesAsync(DateTime date);
    }
}
