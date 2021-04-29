using System;
using System.Diagnostics;
using System.Windows.Forms;
using ShutdownTimer.Helpers;

namespace ShutdownTimer
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (!Debugger.IsAttached)
            {
                AppDomain.CurrentDomain.UnhandledException += ExceptionHandler.UnhandledExceptionHandler;
                Application.ThreadException += ExceptionHandler.ThreadExceptionHandler;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length == 0)
            {
                Application.Run(new Menu());
            }
            else
            {
                ArgProcessor.ProcessArgs(args);

                // Initialize settings
                ExceptionHandler.LogEvent("[Program] Loading settings..");
                if (ArgProcessor.argNoSettings)
                { SettingsProvider.TemporaryMode = true; }
                SettingsProvider.Load();
                ExceptionHandler.LogEvent("[Program] Settings loaded");

                switch (ArgProcessor.argMode)
                {
                    case "Prefill":
                    case "Lock":
                        Menu menu = new Menu
                        {
                            overrideSettings = true,
                            ArgTimeH = ArgProcessor.argTimeH,
                            ArgTimeM = ArgProcessor.argTimeM,
                            ArgTimeS = ArgProcessor.argTimeS,
                            ArgAction = ArgProcessor.argAction,
                            ArgMode = ArgProcessor.argMode,
                            ArgGraceful = ArgProcessor.argGraceful,
                            ArgSleep = ArgProcessor.argSleep,
                            ArgBackground = ArgProcessor.argBackground
                        };
                        Application.Run(menu);
                        break;

                    case "Launch":
                    case "ForcedLaunch":
                        bool forced = new bool();
                        if (ArgProcessor.argMode.Equals("Launch")) { forced = false; }
                        else { forced = true; }

                        Countdown countdown = new Countdown
                        {
                            CountdownTimeSpan = ArgProcessor.argTimeTS,
                            Action = ArgProcessor.argAction,
                            Graceful = ArgProcessor.argGraceful,
                            PreventSystemSleep = ArgProcessor.argSleep,
                            UI = ArgProcessor.argBackground,
                            Forced = forced
                        };
                        Application.Run(countdown);
                        break;
                }
            }
        }
    }
}
