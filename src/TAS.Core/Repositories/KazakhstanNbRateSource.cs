using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

using TAS.Core.Models;
using TAS.Core.Parsers;
using TAS.Core.Exceptions;

namespace TAS.Core.Repositories
{
    public class KazakhstanNbRateSource : IRemoteRateSource
    {
        private static readonly string url = "https://nationalbank.kz/rss/get_rates.cfm";
        private readonly IRemoteRateParser parser;

        public KazakhstanNbRateSource(IRemoteRateParser parser)
        {
            this.parser = parser;
        }

        public async Task<Rate[]> GetRatesAsync(DateTime date)
        {
            try
            {
                return await TryGetRatesAsync(date);
            }
            catch (HttpRequestException error)
            {
                //Todo: Have to write to log
                throw new RemoteRateSourceException(error.Message);
            }

        }

        private async Task<Rate[]> TryGetRatesAsync(DateTime date)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(GetUrl(date));
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                return parser.Parse(content).ToArray();
            }
        }

        private string GetUrl(DateTime date)
        {
            return $"{url}?fdate={date.ToString(Constants.DateFormat)}";
        }
    }
}
