using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Research.DynamicDataDisplay.Navigation;
using RollingRoad.Core.DomainModel;
using RollingRoad.Infrastructure.DataAccess;

namespace RollingRoad.WinApplication.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public ICommand QuitCommand { get; }
        public ICommand OpenAboutWindowCommand { get; }
        public ObservableCollection<object> Tabs { get; } = new ObjectCollection(); 

        public MainWindowViewModel()
        {
            QuitCommand = new DelegateCommand(Quit);
            OpenAboutWindowCommand = new DelegateCommand(OpenAboutWindow);

            LoggerViewModel loggerViewModel;

            App app = Application.Current as App;
            if (app != null)
            {
                loggerViewModel = new LoggerViewModel(null, app.Logger);

                Tabs.Add(new LiveDataSourceViewModel() { Logger = loggerViewModel.Logger });
                Tabs.Add(new DataSetsViewModel(new Repository<DataSet>(app.Context.DataSets), app.UnitOfWork));
                Tabs.Add(loggerViewModel);
            }
            else
            {
                loggerViewModel = new LoggerViewModel();

                Tabs.Add(new LiveDataSourceViewModel() { Logger = loggerViewModel.Logger });
                Tabs.Add(new DataSetsViewModel());
                Tabs.Add(loggerViewModel);
            }

        }

        private void Quit()
        {
            Application.Current.Shutdown();
        }

        private void OpenAboutWindow()
        {
            Views.AboutWindow window = new Views.AboutWindow();

            window.ShowDialog();
        }
    }
}