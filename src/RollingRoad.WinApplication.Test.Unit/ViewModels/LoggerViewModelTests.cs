using System;
using System.Linq;
using System.Windows.Threading;
using NSubstitute;
using NUnit.Framework;
using RollingRoad.WinApplication.ViewModels;

namespace RollingRoad.WinApplication.Test.Unit.ViewModels
{
    [TestFixture]
    public class LoggerViewModelTests
    {
        private LoggerViewModel _vm;
        private IDispatcher _dispatcher;

        [SetUp]
        public void SetUp()
        {
            _dispatcher = Substitute.For<IDispatcher>();
            _dispatcher.BeginInvoke(Arg.Any<DispatcherPriority>(), Arg.Do<Delegate>(x => x.Method.Invoke(x.Target, null)));

            _vm = new LoggerViewModel(_dispatcher);
        }

        [Test]
        public void Log_Nothing_HasOneEntry() //"Logger Started"
        {
            Assert.That(_vm.Log.Count, Is.EqualTo(1));
        }

        [Test]
        public void Log_Nothing_LoggerStartedWrittenToLog()
        {
            Assert.That(_vm.Log.First().Item2, Is.EqualTo("Logger started"));
        }

        [Test]
        public void Logger_WriteToLog_LogCountOne()
        {
            _vm.Logger.WriteLine("Test");

            Assert.That(_vm.Log.Count, Is.EqualTo(2));
        }

        [TestCase("Test")]
        [TestCase("TestTwo")]
        public void Logger_WriteToLog_CorrectLogAdded(string value)
        {
            _vm.Logger.WriteLine(value);
            
            Assert.That(_vm.Log[1].Item2, Is.EqualTo(value));
        }

        [Test]
        public void ToString_Nothing_ReturnLog()
        {
            Assert.That(_vm.ToString(), Is.EqualTo("Log"));
        }
    }
}
