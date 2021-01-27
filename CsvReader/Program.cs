using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using CsvReader.Services;
using CsvReader.ViewModels;

namespace CsvReader
{
    public class Program
    {
        private static IContainer Container { get; set; }
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<CsvReaderService>().As<ICsvReaderService>();
            Container = builder.Build();
            
            RunApplication();
        }

        private static void RunApplication()
        {
            using var scope = Container.BeginLifetimeScope();
            var readerService = scope.Resolve<ICsvReaderService>();
            
            Console.WriteLine("Hello! Nice to see you in CSV reader");
            Console.WriteLine("Your current directory: " + Environment.CurrentDirectory);
            Console.WriteLine("Please put you CSV file in this directory and click Enter.");
            Console.ReadKey();

            var availableFileNames = readerService.GetCsvFileNamesFromCurrentFolder();

            while (!availableFileNames.Any())
            {
                Console.WriteLine("Program can't find CSV files in {0} directory. Please check your file location and try again!", Environment.CurrentDirectory);
                Console.ReadKey();
                availableFileNames = readerService.GetCsvFileNamesFromCurrentFolder();
            }
            
            Console.WriteLine("Finding files:");
            var num = 0;
            foreach (var name in availableFileNames)
            {
                Console.Write(num + ". " + name + "\n");
                num++;
            }
            
            var value = GetInputFileNumber(availableFileNames);

            try
            {
                var list = readerService.ReadCsvFile(availableFileNames.ElementAtOrDefault(value));
                var result = readerService.ConvertToOutputModel(list);

                WriteOutput(result);
                
                Console.WriteLine("\nComplete!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Please restart application.");
            }

            Console.ReadKey();
        }

        private static void WriteOutput(IList<QuoteViewModel> result)
        {
            var shorthands = new List<string>();

            foreach (var item in result)
            {
                foreach (var quote in item.Quotes)
                {
                    shorthands.Add(quote.Shorthand);
                }
            }

            var shorthandsQuotes = shorthands.GroupBy(x => x).Select(g => new {g.Key, Value = string.Empty }).OrderBy(x => x.Key).ToArray();
            
            Console.Write(",");

            for (int i = 0; i < shorthandsQuotes.Length; i++)
            {
                Console.Write(shorthandsQuotes[i].Key);
                if(i != shorthandsQuotes.Length-1)
                    Console.Write(",");
            }
            Console.WriteLine();

            int unparsedValuesRowNum = 0;
            foreach (var item in result)
            {
                if(item.ObservationDate.HasValue)
                    Console.Write(item.ObservationDate.Value.ToShortDateString() + ",");
                else
                {
                    if(unparsedValuesRowNum < 1)
                        Console.WriteLine("Unparsed dates:");
                    
                    Console.Write(item.ObservationDateString + ",");
                    unparsedValuesRowNum++;
                }
                
                for (int i = 0; i < shorthandsQuotes.Length; i++)
                {
                    Console.Write(item.Quotes.FirstOrDefault(x => x.Shorthand == shorthandsQuotes[i].Key)?.Price ?? string.Empty);
                    if(i != shorthandsQuotes.Length-1)
                        Console.Write(",");
                }
                Console.WriteLine();

            }
        }

        private static int GetInputFileNumber(IList<string> availableFileNames)
        {
            var value = 0;
            while (true)
            {
                Console.Write("\nWrite the file number which you want to read: "); 
                
                if (!int.TryParse(Console.ReadLine(), out value))
                {
                    Console.Write("\nInvalid input");
                    continue;
                }
                
                if (availableFileNames.ElementAtOrDefault(value) != null)
                    break;
                else
                {
                    Console.Write("\nInvalid number");
                }
            }

            return value;
        }
    }
}