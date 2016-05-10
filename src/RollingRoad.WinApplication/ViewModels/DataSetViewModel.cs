using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Practices.Prism.Mvvm;
using RollingRoad.Core.DomainModel;

namespace RollingRoad.WinApplication.ViewModels
{
    public class DataSetViewModel : BindableBase
    {
        private bool _isSelected;
        private int _datasetIndex = 0;
        private DataSet DataSet { get; }

        public DataSetViewModel(DataSet dataset)
        {
            DataSet = dataset;

            foreach (DataList dataList in dataset.DataLists)
            {
                Collection.Add(new DataListViewModel(dataList));
            }
        }

        public string Name => DataSet.Name;
        public string Description => DataSet.Description;

        public int DatasetIndex
        {
            get { return _datasetIndex; }
            set
            {
                _datasetIndex = value;
                OnPropertyChanged(nameof(DataSet));

            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
                OnPropertyChanged(nameof(ToStringProperty));
            }
        }

        public void Clear()
        {
            DataSet.DataLists.Clear();
        }

        public ObservableCollection<DataListViewModel> Collection { get; } = new ObservableCollection<DataListViewModel>();

        public string ToStringProperty => ToString();

        public override string ToString()
        {
            return $"D{DatasetIndex}:{Name}";
        }
    }
}
