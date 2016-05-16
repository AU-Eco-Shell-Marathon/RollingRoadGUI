using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Management;
using System.Windows;
using RollingRoad.Core.ApplicationServices;
using RollingRoad.Core.DomainServices;
using RollingRoad.Infrastructure.DataAccess;

namespace RollingRoad.WinApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class App : Application
    {
        public ILogger Logger { get; }
        public ApplicationContext Context { get; }
        public IUnitOfWork UnitOfWork { get; }

        public App()
        {
            if (!Directory.Exists("Logs"))
                Directory.CreateDirectory("Logs");

            Logger = new FileLogger("Logs/log" + DateTime.Now.ToString("yyyyddMMHHmmss", CultureInfo.InvariantCulture) + ".log");

            Logger.WriteLine("RR GUI Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
            Logger.WriteLine("Directory: " + Directory.GetCurrentDirectory());

            Logger.WriteLine(GetOSFriendlyName() + " " + GetOSBit());
            Logger.WriteLine(Environment.OSVersion.ToString());
            Logger.WriteLine("Proccessor Count: " + Environment.ProcessorCount);

            Logger.WriteLine(".Net Version: " + Environment.Version);
            
            AppDomain.CurrentDomain.UnhandledException += CatchException;

            Context = ApplicationContext.Create();
            UnitOfWork = new UnitOfWork(Context);
        }

        public static string GetOSBit()
        {
            if (Environment.Is64BitOperatingSystem)
                return "x64";

            return "x86";
        }

        public static string GetOSFriendlyName()
        {
            string result = string.Empty;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
            foreach (ManagementObject os in searcher.Get())
            {
                result = os["Caption"].ToString();
                break;
            }
            return result;
        }

        void CatchException(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception) args.ExceptionObject;
            
            Logger.WriteLine(e.Message + ": " + e.StackTrace);

            AppDomain.CurrentDomain.UnhandledException -= CatchException;
            throw e;
        }


        void App_Startup(object sender, StartupEventArgs args)
        {
            try
            {
                MainWindow mainWin = new MainWindow();
                mainWin.Show();
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message + ": " + e.StackTrace);
                throw;
            }
        }
    }
}
