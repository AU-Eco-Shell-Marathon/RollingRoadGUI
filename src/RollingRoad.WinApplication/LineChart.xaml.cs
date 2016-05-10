using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using RollingRoad.Core.DomainModel;
using RollingRoad.WinApplication.Annotations;
using RollingRoad.WinApplication.ViewModels;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace RollingRoad.WinApplication
{
    /// <summary>
    /// Interaction logic for LineChart.xaml
    /// </summary>
    public partial class LineChart : UserControl, INotifyPropertyChanged
    {
        public string XAxisName { get; } = "Time";

        public ICollection<DataListViewModel> ItemsSource
        {
            get { return (ICollection<DataListViewModel>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
                                                                            "ItemsSource",
                                                                            typeof(ICollection<DataListViewModel>),
                                                                            typeof(LineChart),
                                                                            new FrameworkPropertyMetadata(ItemsSourceChange, null));

        public ICollection<string> RefreshRateOptions { get; } = new List<string>() {"Off", "500 ms", "1000 ms", "5000 ms", "10000 ms"};
        public ICollection<int> BufferSizeOptions { get; } = new List<int>() {100, 250, 500, 750, 1000, 10000};

        public int SelectedRefreshRate
        {
            get { return _selectedRefreshRate; }
            set
            {
                if (_selectedRefreshRate == value)
                    return;

                if (RefreshRateOptions.ElementAt(value) == "Off")
                {
                    if (_timer.IsEnabled)
                        _timer.Stop();
                }
                else
                {
                    int timeInMs = int.Parse(RefreshRateOptions.ElementAt(value).Split(' ')[0]);

                    _timer.Interval = TimeSpan.FromMilliseconds(timeInMs);

                    if(!_timer.IsEnabled)
                        _timer.Start();
                }
                _selectedRefreshRate = value;
                _settings.SetIntStat(nameof(SelectedRefreshRate), value);
                OnPropertyChanged();

            }
        }
        public int SelectedBufferSize
        {
            get { return _selectedBufferSize; }
            set
            {
                if (_selectedBufferSize == value)
                    return;

                _selectedBufferSize = value;
                _settings.SetIntStat(nameof(_selectedBufferSize), value);
                OnPropertyChanged();
            }
        }

        public string BufferSizeUsage
        {
            get { return _bufferSizeUsage; }
            private set
            {
                _bufferSizeUsage = value;
                OnPropertyChanged();
            }
        }

        private static void ItemsSourceChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineChart control = d as LineChart;

            control?.Refresh();
        }

        private readonly Dictionary<string, Color> _colorDictionary = new Dictionary<string, Color>();
        private ISettingsProvider _settings => Settings.DefaultSettings;

        public LineChart()
        {
            InitializeComponent();
            _timer = new DispatcherTimer();
            _timer.Tick += (sender, e) => Refresh();

            SelectedRefreshRate = _settings.GetIntStat(nameof(SelectedRefreshRate));
            SelectedBufferSize = _settings.GetIntStat(nameof(SelectedBufferSize));

            Chart.LegendVisible = false;
        }

        private readonly DispatcherTimer _timer;
        private int _selectedRefreshRate;
        private int _selectedBufferType;
        private int _selectedBufferSize;
        private bool _bufferSizeEnabled;

        private bool ShouldUpdate()
        {
            return true;
        }

        public void Refresh()
        {
            if (!ShouldUpdate())
                return;

            Clear();

            if (ItemsSource == null || ItemsSource.Count == 0)
                return;

            int backbufferSize = 0;

            DataListViewModel xAxis = null;
            EnumerableDataSource<DataPoint> xData = null;
            
            foreach (DataListViewModel dataList in ItemsSource)
            {
                backbufferSize += dataList.Data.Count * sizeof(double);
                if (dataList.Name == XAxisName)
                {
                    xAxis = dataList;
                    xData = new EnumerableDataSource<DataPoint>(xAxis.Data.Reverse().Take(BufferSizeOptions.ElementAt(SelectedBufferSize)));
                    xData.SetXMapping(x => x.Value);
                    HorizontalAxisTitle.Content = $"{xAxis.Name} ({xAxis.Unit})";
                    continue;
                }

                if (xAxis == null)
                    continue;

                if(!dataList.Selected)
                    continue;

                EnumerableDataSource<DataPoint> yData = new EnumerableDataSource<DataPoint>(dataList.Data.Reverse().Take(BufferSizeOptions.ElementAt(SelectedBufferSize)));
                yData.SetYMapping(x => x.Value);

                CompositeDataSource source = new CompositeDataSource(xData, yData);
                Color color = GetLineColor(dataList.Name + dataList.DataSetIndex + "LineColor");
                dataList.Fill = new SolidColorBrush(color);

                LineGraph test = Chart.AddLineGraph(source, color, 2, dataList.ToString());
            }

            BufferSizeUsage = (backbufferSize/1024) + " KB";
        }

        private Color GetLineColor(string key)
        {
            Color color;

            if (!_colorDictionary.TryGetValue(key, out color))
            {
                string colorStr = _settings.GetStat(key);

                if (string.IsNullOrEmpty(colorStr))
                {
                    color = GenerateRandom();
                    _settings.SetStat(key, $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}");
                    _colorDictionary.Add(key, color);
                }
                else
                {
                    object colorObj = ColorConverter.ConvertFromString(colorStr);
                    color = (Color) colorObj;
                    _colorDictionary.Add(key, color);
                }
            }

            return color;
        }

        private Random _rand = new Random();
        private string _bufferSizeUsage = "0 KB";

        private Color GenerateRandom()
        {
            byte[] b = new byte[3];
            _rand.NextBytes(b);
            return Color.FromRgb(b[0], b[1], b[2]);
        }

        public void Clear()
        {
            try
            {
                Chart.Children.RemoveAll(typeof(LineGraph));
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
