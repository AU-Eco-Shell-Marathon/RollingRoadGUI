using System;
using System.Collections.Generic;
using RollingRoad.Core.DomainModel;

namespace RollingRoad.Core.ApplicationServices
{
    public class LiveDataPointsEventArgs : EventArgs
    {
        public LiveDataPointsEventArgs(ICollection<Tuple<DataPoint, DataType>> data)
        {
            Data = data;
        }

        public ICollection<Tuple<DataPoint, DataType>> Data { get; }
    }
    
    public delegate void OnStatusUpdate(object sender, LiveDataSourceStatus status);

    public enum LiveDataSourceStatus
    {
        Stopped,
        Running,
        Disconnected
    }

    public interface ILiveDataSource
    {

        /// <summary>
        /// Updated each time a fullset om data has been recived.
        /// </summary>
        event EventHandler<LiveDataPointsEventArgs> OnNextReadValue;

        /// <summary>
        /// Starts the transmission/recieving of data
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the transmission/recieving of data
        /// </summary>
        void Stop();

        LiveDataSourceStatus Status { get; }

        event OnStatusUpdate OnStatusChange;

        /// <summary>
        /// Logger used
        /// </summary>
        ILogger Logger { set; }
    }
}
