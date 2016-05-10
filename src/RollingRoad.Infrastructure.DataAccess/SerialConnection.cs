using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Ports;

namespace RollingRoad.Infrastructure.DataAccess
{
    [ExcludeFromCodeCoverage]
    public class SerialConnection : SP4RRInterpreter, IDisposable
    {
        public SerialPort Port { get; }

        public SerialConnection(SerialPort port) : base(new StreamReader(port.BaseStream), new StreamWriter(port.BaseStream))
        {
            Port = port;
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
                Port?.Dispose();
            }
        }

        public override string ToString()
        {
            return Port.PortName;
        }
    }
}
