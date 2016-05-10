using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Mvvm;
using RollingRoad.Core.ApplicationServices;

namespace RollingRoad.WinApplication.ViewModels
{
    public class LoggerViewModel : BindableBase
    {
        public ObservableCollection<Tuple<string, string>> Log { get; } = new ObservableCollection<Tuple<string, string>>();

        public IDispatcher Dispatcher { get; set; }
        
        public ILogger Logger { get; }

        public LoggerViewModel(IDispatcher dispatcher = null, ILogger logger = null)
        {
            Dispatcher = dispatcher;
            Logger = logger;

            if (Dispatcher == null)
                Dispatcher = new SystemDispatcher(Application.Current.Dispatcher);

            if(Logger == null)
                Logger = new Logger();

            Logger.OnLog += (sender, args) => WriteLine(args);

            Logger.WriteLine("Logger started");
        }

        public void WriteLine(string line)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => Log.Add(new Tuple<string, string>(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture), line))));
        }

        public override string ToString()
        {
            return "Log";
        }
    }
}
