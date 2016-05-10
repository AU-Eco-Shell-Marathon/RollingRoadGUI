namespace RollingRoad.WinApplication.Views
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow
    {
        public string Version => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public AboutWindow()
        {
            InitializeComponent();
        }
    }
}
