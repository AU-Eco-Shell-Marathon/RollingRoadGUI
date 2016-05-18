using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RollingRoad.WinApplication.Dialogs
{
    public interface IMessageBox
    {
        MessageBoxResult Show(string title, string message, MessageBoxButton button, MessageBoxImage image);
    }
}
