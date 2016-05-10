using System;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Mvvm;
using RollingRoad.Core.ApplicationServices;

namespace RollingRoad.WinApplication.ViewModels
{
    public class PidControlViewModel : BindableBase
    {
        private readonly IPidControl _control;

        public IDispatcher Dispatcher { get; set; }

        public PidControlViewModel(IPidControl control)
        {
            _control = control;
            Dispatcher = new SystemDispatcher(System.Windows.Threading.Dispatcher.CurrentDispatcher);
        }

        public double Kp
        {
            get { return _control.Kp; }
            set
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
                {
                    _control.Kp = value;
                    OnPropertyChanged(nameof(Kp));
                }));
            }
        }
        public double Ki
        {
            get { return _control.Ki; }
            set
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
                {
                    _control.Ki = value;
                    OnPropertyChanged(nameof(Ki));
                }));
            }
        }
        public double Kd
        {
            get { return _control.Kd; }
            set
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
                {
                    _control.Kd = value;
                    OnPropertyChanged(nameof(Kd));
                }));
            }
        }
    }
}
