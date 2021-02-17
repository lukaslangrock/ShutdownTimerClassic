using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ShutdownTimer.Helpers
{
    public static class ExceptionHandler
    {
        private static Stack<string> eventLog; // storage for important event logs

        public static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;

            string filepath = LogException(e, "UnhandledException");

            string message = "An unhandled exception occurred and the application needs to be terminated!\n\n" +
                "A log file containing information about the process and the error has been saved to your desktop.\n" +
                "Please create an issue on GitHub and include the contents of this log file to help identify and fix the issue.\n\n" +
                "GitHub: github.com/lukaslangrock/ShutdownTimerClassic/issues\n" +
                "Email: lukas.langrock@mailbox.org";
            MessageBox.Show(message, "Shutdown Timer Classic crashed and needs to be terminated!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Process.Start(filepath); // Show log to user
        }

        public static void ThreadExceptionHandler(object sender, ThreadExceptionEventArgs args)
        {
            Exception e = args.Exception;

            string filepath = LogException(e, "ThreadException");

            string message = "A thread exception occurred!\n\n" +
                "A log file containing information about the process and the error has been saved to your desktop.\n" +
                "Please create an issue on GitHub and include the contents of this log file to help identify and fix the issue.\n\n" +
                $"Log file location: {filepath}\n" +
                "GitHub: github.com/lukaslangrock/ShutdownTimerClassic/issues\n" +
                "Email: lukas.langrock@mailbox.org\n\n" +
                "The application experienced a critical error and may very well be broken. It is not recommended to keep using this instance of the application!\n" +
                "Would you like to terminate the application?";
            DialogResult dialogResult = MessageBox.Show(message, "Shutdown Timer Classic crashed!", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

            Process.Start(filepath); // Show log to user

            if (dialogResult == DialogResult.Yes)
            {
                Process.GetCurrentProcess().Kill();
            }
        }

        // Add a new log to the event log stack
        public static void LogEvent(string data)
        {
            if (eventLog is null) { eventLog = new Stack<string>(); }

            eventLog.Push(data);
        }

        // Logs application, process, exception and environment details and returns log filepath
        private static string LogException(Exception e, String type)
        {
            Process process = Process.GetCurrentProcess();
            StringBuilder log = new StringBuilder();

            log.Append($"{Application.ProductName}@{Application.ProductVersion.Remove(Application.ProductVersion.LastIndexOf("."))} experienced a critical exception.\n");
            log.Append("The following data includes information about your system, the exception and the internal state of the application at the time of the exception. You may remove certain information (like your username which may be included in the log) to protect your privacy.\n");
            log.Append("Please open an issue on https://github.com/lukaslangrock/ShutdownTimerClassic and include the contents of this log file to help identify and fix the issue.\n");

            log.Append("\n\n---- Process Info ----\n");
            log.Append($"PID: {process.Id}\n");
            log.Append($"ProcessName: {process.ProcessName}\n");
            log.Append($"Arguments: {process.StartInfo.Arguments}\n");
            log.Append($"PriorityClass: {process.PriorityClass}\n");
            log.Append($"Threads: {process.Threads.Count}\n");
            log.Append($"Responding: {process.Responding}\n");
            log.Append($"HasExited: {process.HasExited}\n");
            log.Append($"StartTime: {process.StartTime}\n");
            log.Append($"PrivateMemorySize64: {process.PrivateMemorySize64}\n");
            log.Append($"VirtualMemorySize64: {process.VirtualMemorySize64}\n");
            log.Append($"PagedMemorySize64: {process.PagedMemorySize64}\n");
            log.Append($"PeakWorkingSet64: {process.PeakWorkingSet64}\n");
            log.Append($"WorkingSet64: {process.WorkingSet64}\n");
            log.Append($"UserProcessorTime: {process.UserProcessorTime}\n");
            log.Append($"TotalProcessorTime: {process.TotalProcessorTime}\n");

            log.Append("\n\n---- Application Info ----\n");
            log.Append($"Product Name: {Application.ProductName}\n");
            log.Append($"Product Version: {Application.ProductVersion}\n");
            log.Append($"Current Culture: {Application.CurrentCulture}\n");
            log.Append($"Executable Path: {Application.ExecutablePath}\n");

            log.Append("\n\n---- Environment Info ----\n");
            log.Append($"64-bit OS: {Environment.Is64BitOperatingSystem}\n");
            log.Append($"64-bit Process: {Environment.Is64BitProcess}\n");
            log.Append($"OS Version: {Environment.OSVersion}\n");
            log.Append($"Runtime Version: {Environment.Version}\n");
            log.Append($"System Uptime: {Environment.TickCount}\n");
            log.Append($"Machine Name: {Environment.MachineName}\n");
            log.Append($"Processor Count: {Environment.ProcessorCount}\n");
            log.Append($"Shutdown Started: {Environment.HasShutdownStarted}\n");


            log.Append("\n\n---- Exception ----\n");
            log.Append($"Type: {type}\n");
            log.Append($"Message: {e.Message}\n");
            log.Append($"Stack Trace:\n {e.StackTrace}\n");

            log.Append("\n\n---- Internal Event Log ----\n");
            foreach (var item in eventLog)
                log.Append(item + "\n");

            log.Append("\n\n---- End of Log ----");

            string filepath = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\ShutdownTimerClassic Exception-Log [{DateTime.Now.Ticks}].txt";
            System.IO.File.WriteAllText(filepath, log.ToString());

            return filepath;
        }
    }
}
