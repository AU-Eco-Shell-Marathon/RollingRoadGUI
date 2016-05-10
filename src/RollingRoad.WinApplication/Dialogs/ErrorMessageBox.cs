using System.Windows;

namespace RollingRoad.WinApplication.Dialogs
{
    public class ErrorMessageBox : IErrorMessageBox
    {
        public void Show(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
