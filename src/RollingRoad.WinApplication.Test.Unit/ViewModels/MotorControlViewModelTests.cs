using NSubstitute;
using NUnit.Framework;
using RollingRoad.Core.ApplicationServices;
using RollingRoad.WinApplication.ViewModels;

namespace RollingRoad.WinApplication.Test.Unit.ViewModels
{
    [TestFixture]
    public class MotorControlViewModelTests
    {
        private IMotorControl _control;
        private MotorControlViewModel _vm;

        [SetUp]
        public void SetUp()
        {
            _control = Substitute.For<IMotorControl>();
            _vm = new MotorControlViewModel(_control);
        }

        [TestCase(10)]
        [TestCase(20)]
        public void MaxSpeed_SetValue_ValueSet(int value)
        {
            _vm.MaxSpeed = value;
            
            Assert.That(_vm.MaxSpeed, Is.EqualTo(value));
        }

        [TestCase(10)]
        [TestCase(20)]
        public void CruiseSpeed_SetValue_ValueSet(int value)
        {
            _vm.CruiseSpeed = value;

            Assert.That(_vm.CruiseSpeed, Is.EqualTo(value));
        }

        [TestCase(10)]
        [TestCase(20)]
        public void MaxSpeed_SetValue_ValueSetOnInterface(int value)
        {
            _vm.MaxSpeed = value;

            _control.Received(1).MaxSpeed = value;
        }

        [TestCase(10)]
        [TestCase(20)]
        public void CruiseSpeed_SetValue_ValueSetOnInterface(int value)
        {
            _vm.CruiseSpeed = value;

            _control.Received(1).CruiseSpeed = value;
        }
    }
}
