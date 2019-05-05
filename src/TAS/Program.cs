using System;
using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;

using TAS.Core;
using TAS.Core.Exceptions;
using TAS.Core.Models;
using TAS.Core.Parsers;
using TAS.Core.Repositories;
using TAS.Core.Services;

namespace TAS
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = CreateService();
            var input = string.Empty;

            Console.OutputEncoding = Encoding.Unicode;

            while(input != "exit")
            {
                try
                {
                    Console.WriteLine("Please, enter date. For example, 01.05.2019");
                    Console.WriteLine("Enter 'exit' to close app.");

                    input = Console.ReadLine();
                    var date = DateTime.ParseExact(input, Constants.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None);

                    Console.WriteLine();

                    DisplayRates(service.GetRatesAsync(date).Result);

                    Console.WriteLine();

                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid date format. Please, enter valid date.");
                }
                catch (RemoteRateSourceException error)
                {
                    Console.WriteLine("Oops, error ocurs when downloading rates from remote source. " +
                        $"See error: {error}");
                }
                catch (Exception error)
                {
                    Console.WriteLine(error);
                }
            }
        }

        private static void DisplayRates(Rate[] rates)
        {
            if (rates.Length == 0)
            {
                Console.WriteLine("Sorry, we have no data for this date.");
            }
            else
            {
                foreach (var rate in rates)
                    Console.WriteLine($"{rate.FullName} ({rate.Title}) - {rate.Description}");
            }
        }

        private static IRateService CreateService()
        {
            return new RateService(CreateDbContext(connection), CreateRemoteSource());
        }

        private static string connection = @"Server=(localdb)\mssqllocaldb;Database=tasdb;Trusted_Connection=True;";
        private static RateDbContext CreateDbContext(string connection)
        {
            var options = SqlServerDbContextOptionsExtensions
                .UseSqlServer(new DbContextOptionsBuilder<RateDbContext>(), connection)
                .Options;

            return new RateDbContext(options);
        }

        private static IRemoteRateSource CreateRemoteSource()
        {
            return new KazakhstanNbRateSource(new KazakhstanNbRateParser());
        }
    }
}
