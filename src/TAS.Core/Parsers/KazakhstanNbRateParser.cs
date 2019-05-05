using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

using TAS.Core.Models;
using TAS.Core.Exceptions;

namespace TAS.Core.Parsers
{
    /// <summary>
    ///     Rates parser of Republic Kazakhstan National Bank
    /// </summary>
    public class KazakhstanNbRateParser : IRemoteRateParser
    {
        public IEnumerable<Rate> Parse(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                throw new ArgumentException($"The '{nameof(xml)}' cannot be null or empty.");

            var rates = new List<Rate>();
            var ratesElement = XDocument.Parse(xml).Element("rates");

            if (ratesElement == null)
                return rates;

            var date = ParseDate(ratesElement);
            
            foreach(var item in ratesElement.Elements("item"))
            {
                var rate = new Rate
                {
                    FullName = item.Element("fullname").Value,
                    Title = item.Element("title").Value,
                    Description = ParseDescription(item),
                    Date = date
                };

                rates.Add(rate);
            }

            return rates;
        }

        private DateTime ParseDate(XElement rates)
        {
            var rawDate = rates.Element("date").Value;

            if (DateTime.TryParseExact(rawDate, Constants.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                return date;

            throw new RemoteRateSourceException($"Cannot parse date from rates: '{rates.Value}'");
        }

        private decimal ParseDescription(XElement item)
        {
            if (decimal.TryParse(item.Element("description").Value, out decimal description))
                return description;

            throw new RemoteRateSourceException($"Cannot parse description from item: '{item.Value}'");
        }
    }
}
