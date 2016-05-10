using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using RollingRoad.Core.ApplicationServices;
using RollingRoad.Infrastructure.DataAccess;
using MessageBox = System.Windows.MessageBox;

namespace RollingRoad.WinApplication.Dialogs
{
    public class SelectSourceDialog : ISelectSourceDialog
    {
        public bool ShowDialog()
        {
            Window win = new Views.SelectSourceWindow(this);

            bool? success = win.ShowDialog();

            if (success == null)
                return false;

            return success.Value;
        }

        public event EventHandler<bool> OnClose;

        public ILiveDataSource LiveDataSource { get; private set; }
        public IDisposable DisposableSource { get; private set; }
        public ILogger Logger { get; set; }

        public IOpenFileDialog OpenFileDialog { get; set; } = new OpenFileDialog()
        {
            DefaultExt = ".csv",
            Filter = "CSV Files (*.csv)|*.csv"
        };

        public ICommand LoadFromFileCommand { get; }
        public ICommand OpenComPortCommand { get; }

        public ObservableCollection<string> ComPorts { get; }
        public int ComPortsSelectedIndex { get; set; }

        public SelectSourceDialog()
        {
            ComPorts = new ObservableCollection<string>(SerialPort.GetPortNames());

            LoadFromFileCommand = new DelegateCommand(OpenFromFile);
            OpenComPortCommand = new DelegateCommand(ConnectComPortButton);
        }

        private void OpenFromFile()
        {
            if (OpenFileDialog.ShowDialog())
            {
                // Open document 
                string filename = OpenFileDialog.FileName;

                try
                {
                    LiveDataSource = new LiveDataEmulator(CsvDataFile.LoadFromFile(filename, "shell eco marathon"));
                    OnClose?.Invoke(this, true);
                }
                catch (Exception exception)
                {
                    Logger?.WriteLine($"Error opening file {filename}: " + exception.Message);
                    MessageBox.Show(exception.Message, "Error opening file", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ConnectComPortButton()
        {
            string portname = ComPorts.ElementAtOrDefault(ComPortsSelectedIndex);
            
            if (portname == null)
                return;

            SerialPort port = new SerialPort(portname) { BaudRate = 9600 };
            port.Open();
            
            LiveDataSource = new SerialConnection(port);
            DisposableSource = port;
            OnClose?.Invoke(this, true);
        }
    }
}
