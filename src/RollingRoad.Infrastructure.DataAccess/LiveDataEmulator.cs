using System;
using System.Collections.Generic;
using System.Linq;
using RollingRoad.Core.ApplicationServices;
using RollingRoad.Core.DomainModel;

namespace RollingRoad.Infrastructure.DataAccess
{
    public class LiveDataEmulator : ILiveDataSource
    {
        private readonly DataSet _source;
        private readonly DataList _xAxis;
        private int _index;
        private double _lastMs;

        /// <summary>
        /// Event when at new value is sent
        /// </summary>
        public event EventHandler<LiveDataPointsEventArgs> OnNextReadValue;
        /// <summary>
        /// Logger used for debug messages
        /// </summary>
        public ILogger Logger { get; set; }

        private ITimer _timer;
        private LiveDataSourceStatus _status;

        /// <summary>
        /// Timer used for timing when to send new values
        /// </summary>
        public ITimer Timer
        {
            private get { return _timer; }
            set
            {
                //If a timer is allready connected, disconnect the event
                if (_timer != null)
                    _timer.Elapsed -= SendNextValue;

                _timer = value;

                //If the new timer is not not, connect the event
                if (_timer != null)
                    _timer.Elapsed += SendNextValue;
            }
        }

        /// <summary>
        /// Ctor, before the timer starts sending values, start must be called
        /// </summary>
        /// <param name="source">Source to emulate</param>
        public LiveDataEmulator(DataSet source)
        {
            _source = source;
            
            _xAxis = source.DataLists.FirstOrDefault(x => x.Name == "Time");

            Timer = new SystemTimer();
            
            Reset();
        }

        //Stop the transmission of values
        public void Stop()
        {
            Status = LiveDataSourceStatus.Stopped;
            Timer.Stop();
            Logger?.WriteLine("Emulator: stopping");
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

        /// <summary>
        /// Start the timer
        /// </summary>
        public void Start()
        {
            if (Status == LiveDataSourceStatus.Disconnected)
                return;

            Status = LiveDataSourceStatus.Running;
            Logger?.WriteLine("Emulator: starting");
            SendNextValue();
        }

        /// <summary>
        /// Resets the timer, making it possible to re-transmit all values
        /// </summary>
        private void Reset()
        {
            Stop();
            _index = 0;

            if(_xAxis != null && _xAxis.Data.Count > 0)
                _lastMs = GetMs(_xAxis.Data.First().Value, _xAxis.Unit);
        }

        private double GetMs(double value, string unit)
        {
            switch (unit)
            {
                case "Seconds":
                    return value*1000;
                case "ms":
                    return value;
                default:
                    Logger?.WriteLine("Unknown time unit: " + unit);
                    return 0;
            }
        }
        
        /// <summary>
        /// Transmits next value and restarts the timer if more data i available
        /// </summary>
        private void SendNextValue()
        {
            if(_xAxis == null)
                return;

            //If there's not more available data, stop. 
            if (_index >= _xAxis.Data.Count)
            {
                Logger?.WriteLine("Emulator: done");
                Timer.Stop();
                return;
            }

            //Setup data to transmit
            IList<Tuple<DataPoint, DataType>> entry = _source.DataLists.Select(
                dataList =>
                    new Tuple<DataPoint, DataType>(
                        dataList.Data.ElementAt(_index),
                        new DataType(dataList.Name, dataList.Unit))).ToList();

            OnNextReadValue?.Invoke(this, new LiveDataPointsEventArgs(entry)); //Send value

            //Calculate time to wait
            double time = GetMs(_xAxis.Data.ElementAt(_index).Value, _xAxis.Unit);
            double delta = time - _lastMs;
            _lastMs = time;

            //Move data index
            _index++;

            if (Math.Abs(delta) < double.Epsilon)
                delta = 1;

            if(Status == LiveDataSourceStatus.Running)
                Timer.Start((int) delta);
        }

        public override string ToString()
        {
            return "Live data emulator (" + _source + ")";
        }
    }
}
