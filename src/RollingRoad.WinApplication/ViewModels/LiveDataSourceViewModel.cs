using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using RollingRoad.Core.ApplicationServices;
using RollingRoad.Core.DomainModel;
using RollingRoad.Infrastructure.DataAccess;
using RollingRoad.WinApplication.Dialogs;
using MessageBox = System.Windows.MessageBox;

namespace RollingRoad.WinApplication.ViewModels
{
    public class LiveDataSourceViewModel : BindableBase
    {
        public DelegateCommand ClearCommand { get; }
        public DelegateCommand StartStopCommand { get; }
        public DelegateCommand SelectSourceCommand { get; }
        public DelegateCommand SaveCommand { get; }

        public DataSetViewModel DataSet { get; } = new DataSetViewModel(new DataSet());
        public ICollection<DataListViewModel> DataLists => DataSet.Collection; 
        public IDispatcher Dispatcher { get; set; }
        public ILogger Logger
        {
            get { return _logger; }
            set
            {
                _logger = value;
                SelectSourceDialog.Logger = value;
            }
        }
        public ISelectSourceDialog SelectSourceDialog { get; set; } = new SelectSourceDialog();
        public ISaveFileDialog SaveFileDialog { get; set; } = new SaveFileDialog()
        {
            DefaultExt = ".csv",
            Filter = "CSV Files (*.csv)|*.csv"
        };
        public IErrorMessageBox ErrorMessageBox { get; set; } = new ErrorMessageBox();
        public IMessageBox AskAboutChangesMessageBox { get; set; } = new Dialogs.MessageBox();
        public IDataSetDataFile DataFileSaver { get; set; } = new CsvDataFile();

        public TestSessionViewModel TestSession
        {
            get { return _testSession; }
            private set
            {
                _testSession = value;
                OnPropertyChanged(nameof(TestSession));
                OnPropertyChanged(nameof(TestSessionEnabled));
            }
        }
        public PidControlViewModel PidControl
        {
            get { return _pidControl; }
            private set
            {
                _pidControl = value;
                OnPropertyChanged(nameof(PidControl));
                OnPropertyChanged(nameof(PidControlEnabled));
            }
        }
        public CalibrateControlViewModel CalibrateControl
        {
            get { return _calibrateControl; }
            private set
            {
                _calibrateControl = value;
                OnPropertyChanged(nameof(CalibrateControl));
                OnPropertyChanged(nameof(CalibrateControlEnabled));
            }
        }
        public TorqueControlViewModel TorqueControl
        {
            get { return _torqueControl; }
            private set
            {
                _torqueControl = value;
                OnPropertyChanged(nameof(TorqueControl));
                OnPropertyChanged(nameof(TorqueControlEnabled));
            }
        }

        public bool TestSessionEnabled => TestSession != null;
        public bool PidControlEnabled => PidControl != null;
        public bool CalibrateControlEnabled => CalibrateControl != null;
        public bool TorqueControlEnabled => TorqueControl != null;
        public bool HasBeenSaved { get; private set; } = true;
        public string StartStopButtonText => IsStarted ? "Stop" : "Start";
        public string SelectedSourceText => "Source: " + Source;
        
        private bool _isStarted;
        private TestSessionViewModel _testSession;
        private PidControlViewModel _pidControl;
        private CalibrateControlViewModel _calibrateControl;
        private TorqueControlViewModel _torqueControl;
        private ILiveDataSource _source;
        private ILogger _logger;

        public bool IsStarted
        {
            get { return _isStarted; }
            private set
            {
                _isStarted = value;
                OnPropertyChanged("StartStopButtonText");
            }
        }

        public LiveDataSourceViewModel()
        {
            StartStopCommand        = new DelegateCommand(StartStop     , CanStartStop);
            ClearCommand            = new DelegateCommand(Clear         , CanClear);
            SaveCommand             = new DelegateCommand(() => Save()  , CanSave);
            SelectSourceCommand     = new DelegateCommand(SelectSource);

            Dispatcher = new SystemDispatcher(System.Windows.Threading.Dispatcher.CurrentDispatcher);
            Source = null;
        }

        public LiveDataSourceViewModel(ILiveDataSource initialSource) : this()
        {
            Source = initialSource;
        }

        public ILiveDataSource Source
        {
            get
            {
                return _source;
            }
            private set
            {
                if (_source != null)
                    _source.OnNextReadValue -= ThreadMover;

                _source = value;

                ICalibrateControl cctrl = _source as ICalibrateControl;
                CalibrateControl = cctrl != null ? new CalibrateControlViewModel(cctrl) : null;

                ITorqueControl tctrl = _source as ITorqueControl;
                if (tctrl != null)
                {
                    TorqueControl = new TorqueControlViewModel(tctrl);
                    TestSession = new TestSessionViewModel(TorqueControl);
                }
                else
                {
                    TestSession = null;
                    TorqueControl = null;
                }

                IPidControl pctrl = _source as IPidControl;
                PidControl = pctrl != null ? new PidControlViewModel(pctrl) : null;

                if (_source != null)
                {
                    _source.OnNextReadValue += ThreadMover;
                    _source.Logger = Logger;
                }

                Start();
                StartStopCommand.RaiseCanExecuteChanged();
                OnPropertyChanged(nameof(SelectedSourceText));
            }
        }

        private void Clear()
        {
            if(CheckAndAskAboutChanges())
            {
                DataSet.Clear();
                HasBeenSaved = true;
            }
        }

        private bool CanClear()
        {
            return DataSet.Collection.Count > 0;
        }

        private void Start()
        {
            Source?.Start();
            IsStarted = true;
        }

        private void Stop()
        {
            Source?.Stop();
            IsStarted = false;
        }

        private void StartStop()
        {
            if(IsStarted)
                Stop();
            else
                Start();
        }

        private bool CanStartStop()
        {
            return Source != null;
        }

        private void IncommingData(LiveDataPointsEventArgs datapoints)
        {
            HasBeenSaved = false;
            foreach (Tuple<DataPoint, DataType> datapoint in datapoints.Data)
            {
                DataListViewModel list = DataSet.Collection.FirstOrDefault(x => x.Name == datapoint.Item2.Name);

                if (list == null)
                {
                    list = new DataListViewModel(new DataList(datapoint.Item2.Name, datapoint.Item2.Unit));
                    DataSet.Collection.Add(list);
                    ClearCommand.RaiseCanExecuteChanged();
                    SaveCommand.RaiseCanExecuteChanged();
                }
                double value = datapoint.Item1.Value;

                if (list.Name == "Distance" && TestSession != null)
                {
                    TestSession.LastestDistance = value;
                }

                list.Add(value);
            }
        }


        private void ThreadMover(object sender, LiveDataPointsEventArgs liveDataPointsEventArgs)
        {
            try
            {
                Dispatcher?.BeginInvoke(DispatcherPriority.Input, new Action(() => IncommingData(liveDataPointsEventArgs)));
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private bool CheckAndAskAboutChanges()
        {
            if (HasBeenSaved)
                return true;
            
            MessageBoxResult result = AskAboutChangesMessageBox.Show("Do you want to save changes?", "Unsaved changes",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    return Save();
                case MessageBoxResult.No:
                    return true;
                default:
                    return false;
            }
        }

        private void SelectSource()
        {
            if (!CheckAndAskAboutChanges())
                return;

            Source?.Stop();
            DataSet.Collection.Clear();

            try
            {
                if (SelectSourceDialog.ShowDialog())
                {
                    Source = SelectSourceDialog.LiveDataSource;
                }
            }
            catch (Exception exception)
            {
                ErrorMessageBox.Show("Error opening source!", "Error: " + exception.Message);
            }
        }

        private bool Save()
        {
            if (SaveFileDialog.ShowDialog() != true)
                return false;

            try
            {
                DataSet source = new DataSet()
                {
                    Description = DateTime.Now.ToLongDateString(),
                    DataLists = new List<DataList>(DataSet.Collection.Select(x => x.List))
                };

                //Offset time to start from 0
                DataList list = source.DataLists.FirstOrDefault(x => x.Name == "Time");

                if (list != null)
                {
                    double offset = list.Data.First().Value;

                    foreach (DataPoint dataPoint in list.Data)
                    {
                        dataPoint.Value -= offset;
                    }
                }


                //Save file
                DataFileSaver.WriteFile(SaveFileDialog.FileName, source, "shell eco marathon");
                return true;
            }
            catch (Exception e)
            {
                ErrorMessageBox.Show("Error saving data!", "Error: " + e.Message);
                Logger?.WriteLine("Error saving data: " + e.Message);
            }

            return false;
        }

        private bool CanSave()
        {
            return DataSet.Collection.Count > 0;
        }

        public override string ToString()
        {
            return "Live data";
        }
    }
}
