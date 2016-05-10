namespace RollingRoad.WinApplication.Dialogs
{
    public interface ISaveFileDialog
    {
        string DefaultExt { get; set; }
        string Filter { get; set; }
        string FileName { get; }

        bool ShowDialog();
    }
}
