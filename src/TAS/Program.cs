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
        private static string exit = "exit";
        static void Main(string[] args)
        {
            var service = CreateService();

            Console.OutputEncoding = Encoding.Unicode;
            var stop = false;

            while(!stop)
            {
                try
                {
                    stop = Run(service);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid date format. Please, enter valid date. \n");
                }
                catch (RemoteRateSourceException error)
                {
                    Console.WriteLine("Oops, error occurred when downloading rates from remote source. " +
                        $"See error: {error}. \n");
                }
                catch (Exception error)
                {
                    Console.WriteLine($"{error}. \n");
                }
            }
        }

        private static bool Run(IRateService service)
        {
            Console.WriteLine(
                "Please, enter date. For example, 01.05.2019. \n" +
                "Enter 'exit' to close app.");

            var input = Console.ReadLine();

            if (input == exit)
                return true;

            var date = DateTime.ParseExact(input, Constants.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None);

            Console.WriteLine();

            try
            {
                DisplayRates(service.GetRatesAsync(date).Result);
            }
            catch(AggregateException error)
            {
                if (error.InnerException is RemoteRateSourceException)
                    throw error.InnerException;

                throw error;
            }

            Console.WriteLine();

            return false;
        }

        private static void DisplayRates(Rate[] rates)
        {
            if (rates.Length == 0)
            {
                Console.WriteLine("Sorry, we have no data for this date. \n");
            }
            else
            {
                foreach (var rate in rates)
                    Console.WriteLine($"{rate.FullName} ({rate.Title}) - {rate.Description}");
            }
        }

        // You can use DI container instead below methods
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
