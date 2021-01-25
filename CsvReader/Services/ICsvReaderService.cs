using System.Collections.Generic;
using CsvReader.Models;

namespace CsvReader.Services
{
    public interface ICsvReaderService
    {
        IList<Quote> ReadCsvFile(string path);
        IList<string> GetCsvFileNamesFromCurrentFolder();
    }
}