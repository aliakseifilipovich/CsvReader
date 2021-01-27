using System;
using CsvReader.Models;

namespace CsvReader.ViewModels
{
    public class QuoteViewModel
    {
        public DateTime? ObservationDate { get; set; }
        
        public string ObservationDateString { get; set; }
        public Quote[] Quotes { get; set; }
    }
}