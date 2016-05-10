using System;

namespace RollingRoad.Core.ApplicationServices
{
    /// <summary>
    /// Invokes OnLog every time there's a log message, does not store log.
    /// </summary>
    public class Logger : ILogger
    {
        /// <summary>
        /// Write to log
        /// </summary>
        /// <param name="line">Line to write</param>
        public virtual void WriteLine(string line)
        {
            OnLog?.Invoke(this, line);
        }

        /// <summary>
        /// Called when there's a new line written, without a newline at the end
        /// </summary>
        public event EventHandler<string> OnLog;
    }
}
