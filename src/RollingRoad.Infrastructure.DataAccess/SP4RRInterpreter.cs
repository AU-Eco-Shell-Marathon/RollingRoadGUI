using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using RollingRoad.Core.ApplicationServices;
using RollingRoad.Core.DomainModel;

namespace RollingRoad.Infrastructure.DataAccess
{
    // ReSharper disable once InconsistentNaming
    public class SP4RRInterpreter : ILiveDataSource, ITorqueControl, IPidControl, ICalibrateControl
    {
        /// <summary>
        /// Updated each time a fullset om data has been recived.
        /// </summary>
        public event EventHandler<LiveDataPointsEventArgs> OnNextReadValue;

        private Thread _listenThread;
        private readonly StreamReader _reader;
        private readonly StreamWriter _writer;
        private readonly Dictionary<int, DataType> _typeDictionary = new Dictionary<int, DataType>();
        private double _kp;
        private double _ki;
        private double _kd;
        private LiveDataSourceStatus _status;

        /// <summary>
        /// Logger used
        /// </summary>
        public ILogger Logger { get; set; }

        public double Kp
        {
            get { return _kp; }
            set
            {
                _kp = value;
                ResendPid();
            }
        }

        public double Ki
        {
            get { return _ki; }
            set
            {
                _ki = value;
                ResendPid();
            }
        }

        public double Kd
        {
            get { return _kd; }
            set
            {
                _kd = value;
                ResendPid();
            }
        }

        private static readonly CultureInfo CultureInfo = new CultureInfo("en-US");
        private const string CommandDivider = "\n";

        /// <summary>
        /// Packet id as specified in the protocol
        /// </summary>
        private enum PacketId
        {
            HandShake = 0,
            UnitDescription = 1,
            Stop = 2,
            Information = 3,
            TorqueCtrl = 4,
            PidCtrl = 5,
            Calibrate = 6
        }

        //Create a new interpreter from stream
        public SP4RRInterpreter(StreamReader reader, StreamWriter writer)
        {
            _reader = reader;
            _writer = writer;
        }

        /// <summary>
        /// Thread function
        /// </summary>
        private void ListenThread()
        {
            while (Status == LiveDataSourceStatus.Running)
            {
                Listen();
            }
        }

        /// <summary>
        /// Listens a single time for incomming messages (Is blocking until the line is recieved)
        /// </summary>
        public void Listen()
        {
            try
            {
                string line = _reader.ReadLine();

                if (string.IsNullOrEmpty(line))
                    return;

                string[] values = line.Split(' ');

                //Read packet id
                PacketId packetId = (PacketId) int.Parse(values[0]);

                if (Status != LiveDataSourceStatus.Running && packetId != PacketId.HandShake)
                    return;

                switch (packetId)
                {
                    case PacketId.UnitDescription:

                        if (values.Length > 4)
                        {
                            throw new Exception("Invalid unit description length: " + line);
                        }

                        int typeId = int.Parse(values[1]);
                        string typeName = values[2];
                        string typeUnit = "Unknown";

                        if (values.Length == 4)
                            typeUnit = values[3];

                        Logger?.WriteLine("New type recieved: " + typeName);

                        if (_typeDictionary.ContainsKey(typeId))
                        {
                            _typeDictionary[typeId] = new DataType(typeName, typeUnit);
                        }
                        else
                        {
                            _typeDictionary.Add(typeId, new DataType(typeName, typeUnit));
                        }
                        break;
                    case PacketId.Information:

                        //<PacketID> <ID 0 Value> <ID 1 Value> .. <ID X Value>
                        int valuesToRead = values.Length - 1;
                        ICollection<Tuple<DataPoint, DataType>> dataRead = new List<Tuple<DataPoint, DataType>>();

                        for (int i = 0; i < valuesToRead; i++)
                        {
                            string inputStr = values[i + 1];
                            double value;

                            if(!double.TryParse(inputStr, NumberStyles.Any, CultureInfo, out value))
                            {
                                value = 0;
                            }

                            if (!_typeDictionary.ContainsKey(i))
                            {
                                Logger?.WriteLine("Unknown datatype recieved, sending handshake");
                                SendHandshake();
                                if (i == 0)
                                    _typeDictionary.Add(i, new DataType("Time", "Unknown"));
                                else
                                    _typeDictionary.Add(i, new DataType("Unknown", "Unknown"));
                            }
                            
                            dataRead.Add(new Tuple<DataPoint, DataType>(new DataPoint(value), _typeDictionary[i]));
                        }

                        OnNextReadValue?.Invoke(this, new LiveDataPointsEventArgs(dataRead));
                        break;
                    case PacketId.PidCtrl:

                        //Start id + 3 doubles
                        if (values.Length != 4)
                        {
                            Logger?.WriteLine("Packet not matching protocol (5): " + line +
                                              ". Should be 5 <double> <double> <double>");
                            return;
                        }

                        _kp = double.Parse(values[1], CultureInfo);
                        _ki = double.Parse(values[2], CultureInfo);
                        _kd = double.Parse(values[3], CultureInfo);

                        Logger?.WriteLine($"PID Values recieved: {_kp} {_ki} {_kd}");

                        break;
                    default:
                        Logger?.WriteLine("Unknown id recieved: " + (int) packetId);
                        break;
                }
            }
            catch (Exception e)
            {
                Status = LiveDataSourceStatus.Disconnected;
                Logger?.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Send a new torque to set to the rollingroad
        /// </summary>
        /// <param name="torque"></param>
        public void SetTorque(double torque)
        {
            string torqueString = torque.ToString(CultureInfo);

            Logger?.WriteLine("Sending torque " + torqueString);
            SendCommand((int)PacketId.TorqueCtrl + " " + torqueString);
        }

        private void ResendPid()
        {
            string pidString = Kp.ToString(CultureInfo) + " " + Ki.ToString(CultureInfo) + " " + Kd.ToString(CultureInfo);
            Logger?.WriteLine("Sending PID " + pidString);

            SendCommand((int)PacketId.PidCtrl + " " + pidString);
        }

        private void SendCommand(string cmd)
        {
            _writer?.Write(cmd + CommandDivider);
            _writer?.Flush();
        }

        public void Stop()
        {
            SendCommand(((int)PacketId.Stop).ToString());
            Status = LiveDataSourceStatus.Stopped;
        }

        public LiveDataSourceStatus Status
        {
            get { return _status; }
            private set
            {
                _status = value; 
                OnStatusChange?.Invoke(this, value);
            }
        }

        public event OnStatusUpdate OnStatusChange;

        public void SendHandshake()
        {
            SendCommand((int)PacketId.HandShake + " RollingRoad");
        }

        public void Start()
        {
            Start(true);
        }

        public void Start(bool startThread)
        {
            if (Status == LiveDataSourceStatus.Disconnected)
                return;

            SendHandshake();
            
            Status = LiveDataSourceStatus.Running;
            if (!startThread)
                return;

            if (_listenThread == null)
                _listenThread = new Thread(ListenThread) { IsBackground = true };

            if (_listenThread.ThreadState != ThreadState.Unstarted && _listenThread.ThreadState != ThreadState.Running)
                _listenThread = new Thread(ListenThread) { IsBackground = true };

            _listenThread.Start();
        }

        ~SP4RRInterpreter()
        {
            Stop();
        }

        public void Calibrate()
        {
            SendCommand(((int)PacketId.Calibrate).ToString());
        }
    }
}
