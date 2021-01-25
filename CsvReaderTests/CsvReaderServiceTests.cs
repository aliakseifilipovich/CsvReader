using System;
using CsvReader.Services;
using NUnit.Framework;

namespace CsvReaderTests
{
    public class CsvReaderServiceTests
    {
        private CsvReaderService serviceUnderTest;

        [SetUp]
        public void Setup()
        {
            serviceUnderTest = new CsvReaderService();
        }

        [Test]
        public void Test_Path_Parameter_Cannot_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => serviceUnderTest.ReadCsvFile(null));
        }
    }
}