using System;
using Microsoft.Practices.Prism.Mvvm;
using RollingRoad.Core.ApplicationServices;

namespace RollingRoad.WinApplication.ViewModels
{
    /// <summary>
    /// Torque view model, uses "TorqueMin" and "TorqueMax" from DefaultSettings file to enable limits
    /// </summary>
    public class TorqueControlViewModel : BindableBase
    {
        private readonly ITorqueControl _control;
        private double _torque;

        public double Min { get; }
        public double Max { get; }
        
        public TorqueControlViewModel(ITorqueControl control, ISettingsProvider settings = null)
        {
            _control = control;

            if(settings == null)
                settings = Settings.DefaultSettings;


            Min = settings.GetDoubleStat("TorqueMin");
            Max = settings.GetDoubleStat("TorqueMax", 5);
        }

        public double Torque
        {
            get { return _torque; }
            set
            {
                _torque = value.Clamp(Min, Max);
                _control.SetTorque(_torque);
                OnPropertyChanged(nameof(Torque));
            }
        }
    }
}
