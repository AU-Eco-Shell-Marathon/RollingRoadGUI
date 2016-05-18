using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RollingRoad.WinApplication.Dialogs
{
    public class MessageBox : IMessageBox
    {
        public MessageBoxResult Show(string title, string message, MessageBoxButton button, MessageBoxImage image)
        {
            return System.Windows.MessageBox.Show(message, title, button, image);
        }
    }
}
