using System.Collections.Generic;
using CsvReader.Models;
using CsvReader.ViewModels;

namespace CsvReader.Services
{
    public interface ICsvReaderService
    {
        IList<Quote> ReadCsvFile(string path);
        IList<string> GetCsvFileNamesFromCurrentFolder();
        IList<QuoteViewModel> ConvertToOutputModel(IList<Quote> quotes);
    }
}