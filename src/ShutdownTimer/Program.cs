using ShutdownTimer.Helpers;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ShutdownTimer
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (!Debugger.IsAttached)
            {
                ExceptionHandler.LogEvent("[Program] Attaching ExceptionHandler.cs");
                AppDomain.CurrentDomain.UnhandledException += ExceptionHandler.UnhandledExceptionHandler;
                Application.ThreadException += ExceptionHandler.ThreadExceptionHandler;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length == 0)
            {
                ExceptionHandler.LogEvent("[Program] Running Menu");
                SettingsProvider.Load();
                Application.Run(new Menu());
            }
            else
            {
                ExceptionHandler.LogEvent("[Program] Processing args...");
                ArgProcessor.ProcessArgs(args);

                // Initialize settings
                ExceptionHandler.LogEvent("[Program] Loading settings...");
                if (ArgProcessor.argNoSettings)
                { SettingsProvider.TemporaryMode = true; }
                SettingsProvider.Load();
                ExceptionHandler.LogEvent("[Program] Settings loaded");

                switch (ArgProcessor.argMode)
                {
                    case "Prefill":
                    case "Lock":
                        ExceptionHandler.LogEvent("[Program] Running Menu with args");
                        Menu menu = new Menu
                        {
                            OverrideSettings = true,
                            ArgTimeH = ArgProcessor.argTimeH,
                            ArgTimeM = ArgProcessor.argTimeM,
                            ArgTimeS = ArgProcessor.argTimeS,
                            ArgAction = ArgProcessor.argAction,
                            ArgMode = ArgProcessor.argMode,
                            ArgGraceful = ArgProcessor.argGraceful,
                            ArgPreventSleep = ArgProcessor.argPreventSleep,
                            ArgBackground = ArgProcessor.argBackground
                        };
                        Application.Run(menu);
                        break;

                    case "Launch":
                    case "ForcedLaunch":
                        bool forced = new bool();
                        if (ArgProcessor.argMode.Equals("Launch")) { forced = false; }
                        else { forced = true; }

                        ExceptionHandler.LogEvent("[Program] Running Countdown with args");
                        Countdown countdown = new Countdown
                        {
                            CountdownTimeSpan = ArgProcessor.argTimeTS,
                            Action = ArgProcessor.argAction,
                            Graceful = ArgProcessor.argGraceful,
                            PreventSystemSleep = ArgProcessor.argPreventSleep,
                            UI = !ArgProcessor.argBackground,
                            Forced = forced,
                            UserLaunch = false
                        };
                        Application.Run(countdown);
                        break;
                }
            }
        }
    }
}
