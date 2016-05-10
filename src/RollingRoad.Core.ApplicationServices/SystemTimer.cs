using System;
using System.Diagnostics.CodeAnalysis;
using System.Timers;

namespace RollingRoad.Core.ApplicationServices
{
    [ExcludeFromCodeCoverage]
    public class SystemTimer : ITimer
    {
        public event Action Elapsed;

        private Timer _timer;

        public void Start(int ms)
        {
            if (_timer != null)
                _timer.Elapsed -= TimerCallback;

            _timer = new Timer
            {
                Interval = ms
            };

            _timer.Elapsed += TimerCallback;
            _timer.Start();
        }

        public void Stop()
        {
            _timer?.Stop();
        }

        private void TimerCallback(object o, ElapsedEventArgs e)
        {
            Elapsed?.Invoke();
        }
    }
}
