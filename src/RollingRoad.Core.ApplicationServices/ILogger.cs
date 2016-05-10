using System;

namespace RollingRoad.Core.ApplicationServices
{
    public interface ILogger
    {
        /// <summary>
        /// Write a line to the log. (Appends newline)
        /// </summary>
        /// <param name="line"></param>
        void WriteLine(string line);

        /// <summary>
        /// Event that may be called on line recieved
        /// </summary>
        event EventHandler<string> OnLog;
    }
}
