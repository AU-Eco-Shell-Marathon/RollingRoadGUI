using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Threading;

namespace RollingRoad.WinApplication
{
    [ExcludeFromCodeCoverage]
    public class SystemDispatcher : IDispatcher
    {
        private Dispatcher _dispatcher;

        public SystemDispatcher(Dispatcher disp)
        {
            _dispatcher = disp;
        }

        public void BeginInvoke(DispatcherPriority prio, Delegate method)
        {
            _dispatcher.BeginInvoke(prio, method);
        }
    }
}
