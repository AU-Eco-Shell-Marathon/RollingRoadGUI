using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;

namespace RollingRoad.Core.ApplicationServices
{
    [ExcludeFromCodeCoverage]
    public class FileLogger : Logger, IDisposable
    {
        public string FilePath { get; }
        private readonly StreamWriter _writer;

        public FileLogger(string path)
        {
            FilePath = path;
            _writer = new StreamWriter(File.Open(FilePath, FileMode.Create));
        }

        public override void WriteLine(string line)
        {
            _writer.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture) + ": " + line);
            _writer.Flush();
            base.WriteLine(line);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _writer?.Dispose();
            }
        }
    }
}
