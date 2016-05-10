using System;
using System.Globalization;
using System.IO;
using System.Linq;
using RollingRoad.Core.DomainModel;

namespace RollingRoad.Infrastructure.DataAccess
{
    /// <summary>
    /// 
    /// </summary>
    public static class CsvDataInterpreter
    {
        //Seperator to use to split cells
        private const char Seperator = ';';

        //Culture used for parsing and writing doubles
        private static readonly CultureInfo CultureInfo = new CultureInfo("en-US");

        /// <summary>
        /// Write datasource to textwriter streams
        /// </summary>
        /// <param name="writer">Stream to write to</param>
        /// <param name="source">Source to write to stream</param>
        /// <param name="headername">Source to write to stream</param>
        public static void WriteToStream(TextWriter writer, DataSet source, string headername)
        {
            if (source.DataLists.Count == 0)
                return;

            int dataLength = source.DataLists.First().Data.Count;

            //Write header and all type names
            writer.WriteLine(headername + Seperator + string.Join(Seperator.ToString(), source.DataLists.Select(x => x.Name)));

            //Writer description and all type units
            writer.WriteLine(source.Description + Seperator + string.Join(Seperator.ToString(), source.DataLists.Select(x => x.Unit)));

            //Data
            for (int i = 0; i < dataLength; i++)
            {
                //LINQ Magic
                string stringout = source.DataLists.Aggregate("", (current, dataList) => current + (Seperator + dataList.Data.ElementAt(i).Value.ToString(CultureInfo)));

                writer.WriteLine(stringout);
            }
            
            //Force changed to be written
            writer.Flush();
        }

        /// <summary>
        /// Load data source from stream
        /// </summary>
        /// <param name="reader">Stream reader pointing to a CSV-source</param>
        /// <returns>The MemoryDataset loaded with the data</returns>
        public static DataSet LoadFromStream(StreamReader reader, string expectedHeader)
        {
            //Used to keep track of the current line. Used for exceptions to display at what line something went wrong
            int currentLine = 1;
            DataSet data = new DataSet();//Datasource we will load into

            //Make sure stream is not empty
            if (reader.EndOfStream)
                throw new EndOfStreamException("File is empty");

            //Read Names/types
            string line = reader.ReadLine();
            currentLine++;

            if (line == null)
                throw new Exception("First line is empty");

            string[] names = line.Split(Seperator);

            //We need at least to data lists to make a graph 
            if (names.Length < 3)
                throw new Exception("Header is to short");

            //Make sure header name matches (basic check to make not all files are loaded)
            if (names[0].ToLower() != expectedHeader)
                throw new Exception($"Invalid header, should be {expectedHeader} (Is: " + names[0].ToLower() + ")");

            //Units
            line = reader.ReadLine();
            currentLine++;

            if (line == null)
                throw new Exception("Second line is empty");

            string[] units = line.Split(Seperator);

            //Make sure our unit length matches our amount of names
            if (units.Length != names.Length)
                throw new Exception("Unit length does not match names length");

            //Create a new list for each of the units
            for (int i = 1; i < names.Length; i++)
            {
                data.DataLists.Add(new DataList(names[i], units[i]));
            }

            int dataIndex = 0;
            //Read all data
            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                
                if (line == null)
                    throw new Exception($"Line {currentLine} is empty");

                string[] readData = line.Split(';');

                //No data cell is allowed to be null, so our data must be at least as long as our amount of datatypes
                if (readData.Length < names.Length)
                    throw new Exception("Error at line " + currentLine + " (Number of cells does not match number of names)");

                //Parse values
                for (int i = 1; i < readData.Length; i++)
                {
                    double value;

                    if (!double.TryParse(readData[i], NumberStyles.Any, CultureInfo, out value))
                    {
                        throw new Exception("Error at line " + currentLine + " could not parse number");
                    }

                    data.DataLists.ElementAt(i - 1).Data.Add(new DataPoint(value) {DataList = data.DataLists.ElementAt(i - 1), Index = dataIndex});
                }

                //Progress one line
                dataIndex++;
                currentLine++;
            }

            //Setup meta info
            data.Description = units[0];

            return data;
        }
    }
}
