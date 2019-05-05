using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using TAS.Core.Models;
using TAS.Core.Repositories;

namespace TAS.Core.Services
{
    public class RateService : IRateService
    {
        private readonly IRemoteRateSource remoteSource;
        private readonly RateDbContext dbContext;

        public RateService(RateDbContext dbContext, IRemoteRateSource remoteSource)
        {
            this.dbContext = dbContext;
            this.remoteSource = remoteSource;
        }

        public async Task<Rate[]> GetRatesAsync(DateTime date)
        {
            var rates = await dbContext
                .Rates
                .Where(r => r.Date == date)
                .ToArrayAsync();

            if (rates.Length == 0)
            {
                rates = await remoteSource.GetRatesAsync(date);

                await dbContext.AddRangeAsync(rates);
                await dbContext.SaveChangesAsync();
            }

            return rates;
        }
    }
}
