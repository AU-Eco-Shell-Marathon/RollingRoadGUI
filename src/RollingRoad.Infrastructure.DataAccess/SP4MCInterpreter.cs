using System;
using System.IO;
using System.Text;
using RollingRoad.Core.ApplicationServices;

namespace RollingRoad.Infrastructure.DataAccess
{
    // ReSharper disable once InconsistentNaming
    public class SP4MCInterpreter : IMotorControl, IDisposable
    {
        public int CruiseSpeed
        {
            get { return _cruiseSpeed; }
            set
            {
                _cruiseSpeed = value;
                SendCruiseSpeed();
            }
        }

        public int MaxSpeed
        {
            get { return _maxSpeed; }
            set
            {
                _maxSpeed = value; 
                SendMaxSpeed();
            }
        }

        private const string CommandDivider = "\n";
        private readonly StreamWriter _writer;
        private int _cruiseSpeed;
        private int _maxSpeed;

        private enum PacketId
        {
            Handshake = 0,
            MaxSpeed = 1,
            CruiseSpeed = 2
        }
        
        public SP4MCInterpreter(Stream stream)
        {
            _writer = new StreamWriter(stream, Encoding.ASCII);
        }

        private void SendCruiseSpeed()
        {
            SendValue(PacketId.CruiseSpeed, CruiseSpeed);
        }

        private void SendMaxSpeed()
        {
            SendValue(PacketId.MaxSpeed, MaxSpeed);
        }

        private void SendValue(PacketId id, int value)
        {
            SendCommand((int)id + " " + value);
        }

        private void SendCommand(string cmd)
        {
            _writer?.Write(cmd + CommandDivider);
            _writer?.Flush();
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
