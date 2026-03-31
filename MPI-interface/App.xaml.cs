using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MPI_interface
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            InstallCrashHandlers();
            base.OnStartup(e);
        }

        private void InstallCrashHandlers()
        {
            DispatcherUnhandledException += (_, args) =>
            {
                try { LogException("DispatcherUnhandledException", args.Exception); } catch { }
                // No UI popups in production; keep log only.
            };

            AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            {
                try { LogException("AppDomain.UnhandledException", args.ExceptionObject as Exception); } catch { }
            };

            TaskScheduler.UnobservedTaskException += (_, args) =>
            {
                try { LogException("TaskScheduler.UnobservedTaskException", args.Exception); } catch { }
            };
        }

        private static void LogException(string source, Exception? ex)
        {
            string dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MPI-interface");
            Directory.CreateDirectory(dir);

            string path = Path.Combine(dir, "crash.log");
            var sb = new StringBuilder();
            sb.AppendLine("========================================");
            sb.AppendLine(DateTime.Now.ToString("O"));
            sb.AppendLine(source);
            sb.AppendLine(ex?.ToString() ?? "(non-Exception crash or null)");
            File.AppendAllText(path, sb.ToString(), Encoding.UTF8);
        }
    }

}
