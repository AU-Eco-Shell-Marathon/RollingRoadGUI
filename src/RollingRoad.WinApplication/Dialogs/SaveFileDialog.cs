namespace RollingRoad.WinApplication.Dialogs
{
    public class SaveFileDialog : ISaveFileDialog
    {

        public string DefaultExt { get; set; }
        public string Filter { get; set; }
        public string FileName { get; private set; }

        public bool ShowDialog()
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog
            {
                DefaultExt = DefaultExt,
                Filter = Filter
            };

            bool? success = dialog.ShowDialog();

            if (success == null)
                return false;

            if (success.Value)
            {
                FileName = dialog.FileName;
                return true;
            }

            return false;
        }
    }
}
