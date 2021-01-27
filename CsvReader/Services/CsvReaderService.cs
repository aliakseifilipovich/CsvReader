using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvReader.Models;
using CsvReader.ViewModels;

namespace CsvReader.Services
{
    public class CsvReaderService : ICsvReaderService
    {
        public IList<Quote> ReadCsvFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException($"Path cannot be null");
            
            using (var reader = new StreamReader(path))
            using (var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var dynamicData =  csv.GetRecords<dynamic>().ToArray();

                var parsedData = ParseDataFromDynamicType(dynamicData).OrderBy(x => x.ObservationDate).ToList();
                
                return parsedData;
            }
        }

        public IList<string> GetCsvFileNamesFromCurrentFolder()
        {
            return Directory.GetFiles(Environment.CurrentDirectory, "*.csv").ToList();
        }

        public IList<QuoteViewModel> ConvertToOutputModel(IList<Quote> quotes)
        {
            var groupedQuotesList = quotes
                .GroupBy(u => u.ObservationDate)
                .Select(group => new QuoteViewModel {ObservationDate = group.Key, Quotes = group.ToArray()})
                .ToList();

            var unparsedDateQuotes = groupedQuotesList.Where(x => x.ObservationDate == null).Select(x => x.Quotes).ToList();

            if (!unparsedDateQuotes.Any()) return groupedQuotesList;
            
            var groupedQuotesUnparsedDateList = unparsedDateQuotes.First()
                .GroupBy(u => u.ObservationDateString)
                .Select(group => new QuoteViewModel {ObservationDateString = group.Key, Quotes = group.ToArray()})
                .ToList();

            var result = groupedQuotesList.Where(x => x.ObservationDate != null).ToList();

            return  result.Union(groupedQuotesUnparsedDateList).ToList();
        }

        private IList<Quote> ParseDataFromDynamicType(dynamic[] fileData)
        {
            var result = new List<Quote>();
            
            var row = new List<Tuple<string, object>>();
            
            foreach (var dataRow in fileData)
            {
                var col = (IDictionary<string, object>)dataRow;
                foreach (var item in col)
                {
                    row.Add(new Tuple<string, object>(item.Key, item.Value));
                }
                
                result.Add(CreateQuoteRow(row));    
            }
            return result;
        }

        private Quote CreateQuoteRow(IEnumerable<Tuple<string, object>> parsedRow)
        {
            var quote = new Quote();

            foreach (var col in parsedRow)
            {
                if (col.Item1.ToLower() == nameof(quote.ObservationDate).ToLower())
                {
                    quote.ObservationDate = ConvertToDate(col.Item2?.ToString());
                    quote.ObservationDateString = quote.ObservationDate == null ? col.Item2?.ToString() : string.Empty;
                }
                else if (col.Item1.ToLower() == nameof(quote.Shorthand).ToLower())
                    quote.Shorthand = col.Item2?.ToString() ?? string.Empty;
                else if (col.Item1.ToLower() == nameof(quote.From).ToLower())
                    quote.From = col.Item2?.ToString() ?? string.Empty;
                else if (col.Item1.ToLower() == nameof(quote.To).ToLower())
                    quote.To = col.Item2?.ToString() ?? string.Empty;
                else if (col.Item1.ToLower() == nameof(quote.Price).ToLower())
                    quote.Price = col.Item2?.ToString() ?? string.Empty;
            }

            return quote;
        }

        private DateTime? ConvertToDate(string date)
        { 
            return DateTime.TryParse(date, out _) ? DateTime.Parse(date).Date : null;
        }
    }
}