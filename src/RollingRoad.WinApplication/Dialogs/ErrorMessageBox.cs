using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace RollingRoad.WinApplication.Dialogs
{
    [ExcludeFromCodeCoverage]
    public class ErrorMessageBox : IErrorMessageBox
    {
        public void Show(string title, string message)
        {
            System.Windows.MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
