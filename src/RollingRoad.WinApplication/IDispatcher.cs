using System;
using System.Windows.Threading;

namespace RollingRoad.WinApplication
{
    public interface IDispatcher
    {
        void BeginInvoke(DispatcherPriority prio, Delegate method);
    }
}
