using System;

namespace RollingRoad.Core.ApplicationServices
{
    public interface ITimer
    {
        /// <summary>
        /// Initiate timer to make a call to <see cref="Elapsed"/>
        /// </summary>
        /// <param name="ms">Time to delay in milliseconds</param>
        void Start(int ms);

        /// <summary>
        /// Stop the timer if currently running
        /// </summary>
        void Stop();

        /// <summary>
        /// Event called when the timer is done
        /// </summary>
        event Action Elapsed;
    }
}
