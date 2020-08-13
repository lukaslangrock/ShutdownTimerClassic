using System;
using System.Windows.Forms;

namespace ShutdownTimer
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += Helpers.ExceptionHandler.UnhandledExceptionHandler;
            Application.ThreadException += Helpers.ExceptionHandler.ThreadExceptionHandler;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Menu(args));
        }
    }
}
