using System;

namespace CsvReader.Models
{
    public class Quote
    {
        public string ObservationDate { get; set; }
        public string Shorthand { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Price { get; set; }
    }
}