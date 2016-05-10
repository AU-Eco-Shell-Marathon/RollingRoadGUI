using NSubstitute;
using NUnit.Framework;
using RollingRoad.Core.ApplicationServices;
using RollingRoad.WinApplication.ViewModels;

namespace RollingRoad.WinApplication.Test.Unit.ViewModels
{
    [TestFixture]
    public class TorqueControlViewModelTests
    {
        private TorqueControlViewModel _vm;
        private ITorqueControl _control;
        private ISettingsProvider _settings;

        private double _minTorque = 0;
        private double _maxTorque = 5;

        [SetUp]
        public void SetUp()
        {
            _control = Substitute.For<ITorqueControl>();
            _settings = Substitute.For<ISettingsProvider>();

            _settings.GetDoubleStat("TorqueMin", Arg.Any<double>()).Returns(_minTorque);
            _settings.GetDoubleStat("TorqueMax", Arg.Any<double>()).Returns(_maxTorque);

            _vm = new TorqueControlViewModel(_control, _settings);
        }

        [Test]
        public void Torque_SetMaxTorque_MaxSet()
        {
            _vm.Torque = _maxTorque;

            Assert.That(_vm.Torque, Is.EqualTo(_maxTorque).Within(double.Epsilon));
        }

        [Test]
        public void Torque_SetLargerThanMax_ValueClampedToMax()
        {
            _vm.Torque = _maxTorque + 1;

            Assert.That(_vm.Torque, Is.EqualTo(_maxTorque).Within(double.Epsilon));
        }

        [Test]
        public void Torque_SetValue_InterfaceSetTorqueCalled()
        {
            _vm.Torque = 10.5;

            _control.Received(1).SetTorque(Arg.Any<double>());
        }

        [Test]
        public void Torque_SetValue_InterfaceSetTorqueCalledWithCorrectValue()
        {
            _vm.Torque = _maxTorque;

            _control.Received(1).SetTorque(_maxTorque);
        }
    }
}
