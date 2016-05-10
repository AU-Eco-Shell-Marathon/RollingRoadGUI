using Microsoft.Practices.Prism.Mvvm;
using RollingRoad.Core.ApplicationServices;

namespace RollingRoad.WinApplication.ViewModels
{
    public class MotorControlViewModel : BindableBase
    {
        private readonly IMotorControl _control;

        public MotorControlViewModel(IMotorControl control)
        {
            _control = control;
        }

        public int CruiseSpeed
        {
            get { return _control.CruiseSpeed; }
            set
            {
                _control.CruiseSpeed = value;
                OnPropertyChanged(nameof(CruiseSpeed));
            }
        }

        public int MaxSpeed
        {
            get { return _control.MaxSpeed; }
            set
            {
                _control.MaxSpeed = value;
                OnPropertyChanged(nameof(MaxSpeed));
            }
        }
    }
}
