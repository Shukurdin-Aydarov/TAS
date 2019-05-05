using System.Collections.Generic;

using TAS.Core.Models;

namespace TAS.Core.Parsers
{
    public interface IRemoteRateParser
    {
        IEnumerable<Rate> Parse(string xml);
    }
}
