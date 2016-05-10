using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using RollingRoad.Core.DomainModel;
using RollingRoad.Infrastructure.DataAccess;

namespace RollingRoad.Test.Unit.DataAccess
{
    [TestFixture]
    public class CsvDataInterpeterTests
    {
        /// <summary>
        /// https://stackoverflow.com/questions/8047064/convert-string-to-system-io-stream
        /// </summary>
        /// <param name="str">String to convent into stream reader</param>
        /// <returns></returns>
        private StreamReader CreateStreamReaderFromString(string str)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(str);
            MemoryStream stream = new MemoryStream(byteArray);
            return new StreamReader(stream);
        }

        [Test]
        public void LoadFromStream_EmptyReader_ExceptionThrown()
        {
            StreamReader reader = CreateStreamReaderFromString("");

            Assert.Throws<EndOfStreamException>(() => CsvDataInterpreter.LoadFromStream(reader, "shell eco marathon"));
        }

        [Test]
        public void LoadFromStream_IncorrectHeader_ExceptionThrown()
        {
            StreamReader reader = CreateStreamReaderFromString("testtest;type1;type2");

            Assert.Throws<Exception>(() => CsvDataInterpreter.LoadFromStream(reader, "shell eco marathon"));
        }

        [Test]
        public void LoadFromStream_CorrectUpperCaseHeader_DatasourceCreated()
        {
            StreamReader reader = CreateStreamReaderFromString("SHELL ECO MARATHON;type1;type2\n;unit1;unit2");

            Assert.That(CsvDataInterpreter.LoadFromStream(reader, "shell eco marathon"), Is.Not.Null);
        }

        [Test]
        public void LoadFromStream_NoDescriptionInStream_DescriptionIsEmpty()
        {
            StreamReader reader = CreateStreamReaderFromString("SHELL ECO MARATHON;type1;type2\n;unit1;unit2");
            DataSet source = CsvDataInterpreter.LoadFromStream(reader, "shell eco marathon");

            Assert.That(source.Description, Is.Empty.Or.Null);
        }

        [Test]
        public void LoadFromStream_DescriptionInStream_DescriptionLoaded()
        {
            StreamReader reader = CreateStreamReaderFromString("SHELL ECO MARATHON;type1;type2\nTest Description;unit1;unit2");
            DataSet source = CsvDataInterpreter.LoadFromStream(reader, "shell eco marathon");

            Assert.That(source.Description, Is.EqualTo("Test Description"));
        }

        [Test]
        public void LoadFromStream_DataTypeAdded_DatalistWithCorrectNameReturned()
        {
            StreamReader reader = CreateStreamReaderFromString("SHELL ECO MARATHON;type1;type2\nTest Description;unit1;unit2");
            DataSet source = CsvDataInterpreter.LoadFromStream(reader, "shell eco marathon");

            Assert.That(source.DataLists.FirstOrDefault(x => x.Name == "type1").Name, Is.EqualTo("type1"));
        }

        [Test]
        public void LoadFromStream_DataTypeAdded_DatalistWithCorrectUnitReturned()
        {
            StreamReader reader = CreateStreamReaderFromString("SHELL ECO MARATHON;type1;type2\nTest Description;unit1;unit2");
            DataSet source = CsvDataInterpreter.LoadFromStream(reader, "shell eco marathon");

            Assert.That(source.DataLists.FirstOrDefault(x => x.Name == "type1").Unit, Is.EqualTo("unit1"));
        }

        [Test]
        public void LoadFromStream_DataTypeNotAdded_DatalistNotFound()
        {
            StreamReader reader = CreateStreamReaderFromString("SHELL ECO MARATHON;type1;type2\nTest Description;unit1;unit2");
            DataSet source = CsvDataInterpreter.LoadFromStream(reader, "shell eco marathon");

            Assert.That(source.DataLists.FirstOrDefault(x => x.Name == "type3"), Is.EqualTo(null));
        }
        
        [TestCase(5.0)]
        [TestCase(-5.0)]
        [TestCase(0.0)]
        [TestCase(0.852)]
        [TestCase(-0.852)]
        public void LoadFromStream_DataTypeAddedWithOneDataPoint_FirstDataPointPresent(double data)
        {
            StreamReader reader = CreateStreamReaderFromString($"SHELL ECO MARATHON;type1;type2\nTest Description;unit1;unit2\n;{data.ToString(new CultureInfo("en-US"))};3");
            DataSet source = CsvDataInterpreter.LoadFromStream(reader, "shell eco marathon");
            
            Assert.That(source.DataLists.FirstOrDefault(x => x.Name == "type1").Data.First().Value, Is.EqualTo(data));
        }

        [Test]
        public void LoadFromStream_DataTypeAddedWithOneScientificDatapointLargeE_FirstDataPointPresent()
        {
            StreamReader reader = CreateStreamReaderFromString("SHELL ECO MARATHON;type1;type2\nTest Description;unit1;unit2\n;3.0E2;3");
            DataSet source = CsvDataInterpreter.LoadFromStream(reader, "shell eco marathon");

            Assert.That(source.DataLists.FirstOrDefault(x => x.Name == "type1").Data.First().Value, Is.EqualTo(300));
        }

        [Test]
        public void LoadFromStream_DataTypeAddedWithOneScientificDatapointSmallE_FirstDataPointPresent()
        {
            StreamReader reader = CreateStreamReaderFromString("SHELL ECO MARATHON;type1;type2\nTest Description;unit1;unit2\n;3.0e2;3");
            DataSet source = CsvDataInterpreter.LoadFromStream(reader, "shell eco marathon");

            Assert.That(source.DataLists.FirstOrDefault(x => x.Name == "type1").Data.First().Value, Is.EqualTo(300));
        }

        [TestCase(5.0)]
        [TestCase(-5.0)]
        [TestCase(0.0)]
        [TestCase(0.852)]
        [TestCase(-0.852)]
        public void LoadFromStream_DataTypeAddedWithTWoDataPoints_SecondDataPointPresent(double data)
        {
            StreamReader reader = CreateStreamReaderFromString($"SHELL ECO MARATHON;type1;type2\nTest Description;unit1;unit2\n;1;2\n;{data.ToString(new CultureInfo("en-US"))};3");
            DataSet source = CsvDataInterpreter.LoadFromStream(reader, "shell eco marathon");

            Assert.That(source.DataLists.First(x => x.Name == "type1").Data.ElementAt(1).Value, Is.EqualTo(data));
        }

        [Test]
        public void WriteToStream_TypePresentButNoData_DescriptionPresent()
        {
            StringBuilder builder = new StringBuilder();
            TextWriter writer = new StringWriter(builder);
            DataSet dataset = new DataSet();
            string testDescription = "testdescription1234";

            dataset.Description = testDescription;
            dataset.DataLists.Add(new DataList("Test", "Test2"));

            CsvDataInterpreter.WriteToStream(writer, dataset, "");

            Assert.That(writer.ToString(), Does.Contain(testDescription));
        }


        [TestCase(5.0, "5")]
        [TestCase(-5.0, "-5")]
        [TestCase(0.0, "0")]
        [TestCase(0.852, "0.852")]
        [TestCase(-0.852, "-0.852")]
        public void WriteToStream_TypePresentAndOneDataPoint_DataPresent(double value, string valueString)
        {
            StringBuilder builder = new StringBuilder();
            TextWriter writer = new StringWriter(builder);
            DataSet dataset = new DataSet();
            DataList list = new DataList("Test", "Test2") { Data = new List<DataPoint>() { new DataPoint(value)}};

            dataset.DataLists.Add(list);

            CsvDataInterpreter.WriteToStream(writer, dataset, "");

            Assert.That(writer.ToString(), Does.Contain(valueString));
        }
    }
}
