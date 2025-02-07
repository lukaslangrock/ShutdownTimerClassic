using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ShutdownTimer.Helpers
{
    public static class ExceptionHandler
    {
        private static StringBuilder eventLog = new StringBuilder(); // storage for event logs

        public static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;

            string filepath = ProduceLogfile(e, "UnhandledException", true, false);

            string message = "An unhandled exception occurred and the application needs to be terminated!\n\n" +
                "A log file containing information about the process and the error has been saved to your desktop.\n" +
                "Please create an issue on GitHub and include the contents of this log file to help identify and fix the issue.\n\n" +
                "GitHub: github.com/lukaslangrock/ShutdownTimerClassic/issues\n" +
                "Email: lukas.langrock@outlook.de";
            MessageBox.Show(message, "Shutdown Timer Classic crashed and needs to be terminated!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Process.Start(filepath); // Show log to user
        }

        public static void ThreadExceptionHandler(object sender, ThreadExceptionEventArgs args)
        {
            Exception e = args.Exception;

            string filepath = ProduceLogfile(e, "ThreadException", true, false);

            string message = "A thread exception occurred!\n\n" +
                "A log file containing information about the process and the error has been saved to your desktop.\n" +
                "Please create an issue on GitHub and include the contents of this log file to help identify and fix the issue.\n\n" +
                $"Log file location: {filepath}\n" +
                "GitHub: github.com/lukaslangrock/ShutdownTimerClassic/issues\n" +
                "Email: lukas.langrock@outlook.de\n\n" +
                "The application experienced a critical error and may very well be broken. It is not recommended to keep using this instance of the application!\n" +
                "Would you like to terminate the application?";
            DialogResult dialogResult = MessageBox.Show(message, "Shutdown Timer Classic crashed!", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

            Process.Start(filepath); // Show log to user

            if (dialogResult == DialogResult.Yes)
            {
                Process.GetCurrentProcess().Kill();
            }
        }

        public static void CreateManualLog()
        {
            string filepath = ProduceLogfile(null, "NotAnException", false, false);
            Process.Start(filepath); // Show log to user
        }

        public static void CreateAutoLogIfApplicable()
        {
            try
            {
                if (SettingsProvider.SettingsLoaded && SettingsProvider.Settings.SaveEventLogOnExit)
                {
                    Log("Autosaving log as defined per developer option. This feature can be turned off in Settings > Advanced > Developer options");
                    ProduceLogfile(null, "NotAnException", false, true);
                }
            }
            catch
            { }
        }

        // Add a new log to the event log stack
        public static void Log(string data)
        {
            MethodBase method = new StackTrace().GetFrame(1).GetMethod();
            eventLog.AppendLine($"{DateTime.Now.ToString("[HH:mm:ss.ffff]")}[{method.ReflectedType}:{method.Name}] {data}");
        }


        private static string ProduceLogfile(Exception e, String type, bool crash, bool appdata)
        {
            Process process = Process.GetCurrentProcess();
            StringBuilder log = new StringBuilder();

            if (crash)
            {
                log.AppendLine($"{Application.ProductName}@{Application.ProductVersion.Remove(Application.ProductVersion.LastIndexOf("."))} experienced a critical exception.");
                log.AppendLine("The following data includes information about your system, the exception and the internal state of the application at the time of the exception. You may remove certain information (like your username which may be included in the log) to protect your privacy.");
                log.AppendLine("Please open an issue on https://github.com/lukaslangrock/ShutdownTimerClassic and include the contents of this log file to help identify and fix the issue.");
            }
            else
            {
                log.AppendLine($"Log created by {Application.ProductName}@{Application.ProductVersion.Remove(Application.ProductVersion.LastIndexOf("."))}");
                log.AppendLine("The following data includes information about your system and the internal state of the application at the time of log creation. You may remove certain information (like your username which may be included in the log) to protect your privacy.");
                log.AppendLine("This is NOT a crash! This log was created upon user request.");
            }

            // Logs application, process, exception and environment details and returns log filepath
            log.AppendLine("\n\n---- Process Info ----");
            log.AppendLine($"ProcessName: {process.ProcessName}");
            log.AppendLine($"Arguments: {process.StartInfo.Arguments}");
            log.AppendLine($"Threads: {process.Threads.Count}");
            log.AppendLine($"Responding: {process.Responding}");
            log.AppendLine($"StartTime: {process.StartTime}");
            log.AppendLine($"PeakWorkingSet64: {Format.BytesToString(process.PeakWorkingSet64)}");
            log.AppendLine($"WorkingSet64: {Format.BytesToString(process.WorkingSet64)}");
            log.AppendLine($"PeakWorkingSet64: {Format.BytesToString(process.PeakWorkingSet64)}");
            log.AppendLine($"PrivateMemorySize64: {Format.BytesToString(process.PrivateMemorySize64)}"); // The number of bytes that the associated process has allocated that cannot be shared with other processes.
            log.AppendLine($"VirtualMemorySize64: {Format.BytesToString(process.VirtualMemorySize64)}");
            log.AppendLine($"PeakVirtualMemorySize64: {Format.BytesToString(process.PeakVirtualMemorySize64)}"); // The maximum amount of virtual memory that the process has requested.
            log.AppendLine($"PagedSystemMemorySize64: {Format.BytesToString(process.PagedSystemMemorySize64)}");
            log.AppendLine($"NonpagedSystemMemorySize64: {Format.BytesToString(process.NonpagedSystemMemorySize64)}"); // The amount of memory that the system has allocated on behalf of the associated process that cannot be written to the virtual memory paging file.
            log.AppendLine($"PagedMemorySize64: {Format.BytesToString(process.PagedMemorySize64)}"); // The amount of memory that the associated process has allocated that can be written to the virtual memory paging file.
            log.AppendLine($"PeakPagedMemorySize64: {Format.BytesToString(process.PeakPagedMemorySize64)}"); // The amount of memory that the system has allocated on behalf of the associated process that can be written to the virtual memory paging file.
            log.AppendLine($"UserProcessorTime: {process.UserProcessorTime}");
            log.AppendLine($"TotalProcessorTime: {process.TotalProcessorTime}");
            log.AppendLine($"PrivilegedProcessorTime: {process.PrivilegedProcessorTime}");

            log.AppendLine("\n\n---- Application Info ----");
            log.AppendLine($"Product Name: {Application.ProductName}");
            log.AppendLine($"Product Version: {Application.ProductVersion}");
            log.AppendLine($"Current Culture: {Application.CurrentCulture}");
            log.AppendLine($"Executable Path: {Application.ExecutablePath}");
            try
            {
                using (MD5 md5 = MD5.Create())
                {
                    using (FileStream stream = File.OpenRead(Application.ExecutablePath))
                    {
                        string hash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", String.Empty).ToLowerInvariant();
                        log.AppendLine($"MD5 Checksum: {hash}");
                    }
                }
            }
            catch (Exception ex) { log.AppendLine($"MD5 Checksum: FAIL! {ex.Message}"); }

            log.AppendLine("\n\n---- Environment Info ----");
            log.AppendLine($"64-bit OS: {Environment.Is64BitOperatingSystem}");
            log.AppendLine($"64-bit Process: {Environment.Is64BitProcess}");
            log.AppendLine($"OS Version: {Environment.OSVersion}");
            log.AppendLine($"Runtime Version: {Environment.Version}");
            log.AppendLine($"System Uptime: {Environment.TickCount}");
            log.AppendLine($"Shutdown Started: {Environment.HasShutdownStarted}");

            if (crash)
            {
                log.AppendLine("\n\n---- Exception ----");
                log.AppendLine($"Type: {type}");
                log.AppendLine($"Message: {e.Message}");
                log.AppendLine($"Stack Trace:\n {e.StackTrace}");
            }

            log.AppendLine("\n\n---- Internal Event Log ----");
            log.Append(eventLog);

            log.Append("\n\n---- End of Log ----");

            string filepath;
            if (crash)
            {
                filepath = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\ShutdownTimerClassic Exception-Log [{DateTime.Now.Ticks}].txt";
            }
            else if (appdata)
            {
                string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Shutdown Timer Classic\\logs";
                filepath = $"{folder}\\{DateTime.Now.ToString("yyyy-MM-dd_HH-mm")} [{DateTime.Now.Ticks}].txt";
                System.IO.Directory.CreateDirectory(folder);
            }
            else
            {
                filepath = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\ShutdownTimerClassic Manual-Log [{DateTime.Now.Ticks}].txt";
            }

            System.IO.File.WriteAllText(filepath, log.ToString());

            return filepath;
        }
    }
}
