
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Threading;
using NSubstitute;
using NUnit.Framework;
using RollingRoad.Core.ApplicationServices;
using RollingRoad.Core.DomainModel;
using RollingRoad.WinApplication.ViewModels;

namespace RollingRoad.WinApplication.Test.Unit.ViewModels
{
    [TestFixture]
    public class LiveDataSourceViewModelTests
    {
        private LiveDataSourceViewModel _vm;
        private IDispatcher _dispatcher;
        private ILiveDataSource _source;

        [SetUp]
        public void SetUp()
        {
            _dispatcher = Substitute.For<IDispatcher>();
            _dispatcher.BeginInvoke(Arg.Any<DispatcherPriority>(), Arg.Do<Delegate>(x => x.Method.Invoke(x.Target, null)));

            _source = Substitute.For<ILiveDataSource>();

            _vm = new LiveDataSourceViewModel(_source);
            _vm.Dispatcher = _dispatcher;
        }

        [Test]
        public void Source_SetSource_StartCalledOnSource()
        {
            _source.Received(1).Start();
        }

        [Test]
        public void StartStopCommand_SourceSet_CanExecuteTrue()
        {
            Assert.That(_vm.StartStopCommand.CanExecute, Is.True);
        }

        [Test]
        public async Task StartStopCommand_SourceSetAndCommandExecuted_StopCalledOnceOnSource()
        {
            await _vm.StartStopCommand.Execute();

            _source.Received(1).Stop();
        }

        [Test]
        public async Task StartStopCommand_SourceSetAndCommandExecutedTwice_StopCalledOnceOnSource()
        {
            await _vm.StartStopCommand.Execute();
            await _vm.StartStopCommand.Execute();

            _source.Received(1).Stop();
        }

        [Test]
        public async Task StartStopCommand_SourceSetAndCommandExecutedTwice_StartCalledTwiceOnSource()
        {
            await _vm.StartStopCommand.Execute();
            await _vm.StartStopCommand.Execute();

            _source.Received(2).Start();
        }

        [Test]
        public void Collection_Nothing_IsEmpty()
        {
            Assert.That(_vm.DataSet.Collection, Is.Empty);
        }
        
        /*public void Collection_SourceSend1Datapoint_NewListAdded()
        {
            _source.OnNextReadValue +=
                Raise.Event(new LiveDataPointsEventArgs(new List<Tuple<DataPoint, DataType>>() {new Tuple<DataPoint, DataType>(new DataPoint(1), new DataType())}));

            Assert.That(_vm.DataSet.Count, Is.EqualTo(1));
        }*/
    }
}
