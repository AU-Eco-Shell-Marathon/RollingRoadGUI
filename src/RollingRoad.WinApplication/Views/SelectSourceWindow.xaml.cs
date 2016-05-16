using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using RollingRoad.WinApplication.Dialogs;

namespace RollingRoad.WinApplication.Views
{
    /// <summary>
    /// Interaction logic for SelectSourceDialog.xaml
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class SelectSourceWindow : Window
    {
        private readonly ISelectSourceDialog _vm;

        public SelectSourceWindow(ISelectSourceDialog vm)
        {
            _vm = vm;
            _vm.OnClose += OnClose;

            PreviewKeyDown += (sender, args) => { if (args.Key == Key.Escape) Close(); };

            DataContext = _vm;
            InitializeComponent();
        }

        private void OnClose(object sender, bool args)
        {
            _vm.OnClose -= OnClose;
            DialogResult = args;
            Close();
        }
    }
}
