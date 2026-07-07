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
                ExceptionHandler.Log("Register exception handler");
                AppDomain.CurrentDomain.UnhandledException += ExceptionHandler.UnhandledExceptionHandler;
                Application.ThreadException += ExceptionHandler.ThreadExceptionHandler;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length == 0)
            {
                ExceptionHandler.Log("Start menu");
                SettingsProvider.Load();
                Application.Run(new Menu());
            }
            else
            {
                ExceptionHandler.Log("Processing CLI args");
                ArgProcessor.ProcessArgs(args);

                // Initialize settings
                ExceptionHandler.Log("Load settings");
                if (ArgProcessor.argNoSettings)
                { SettingsProvider.TemporaryMode = true; }
                SettingsProvider.Load();
                ExceptionHandler.Log("Settings loaded");

                switch (ArgProcessor.argMode)
                {
                    case "Prefill":
                    case "Lock":
                        ExceptionHandler.Log("Run menu with args");
                        Menu menu = new Menu
                        {
                            OverrideSettings = true,
                            ArgTimeH = ArgProcessor.argTimeH,
                            ArgTimeM = ArgProcessor.argTimeM,
                            ArgTimeS = ArgProcessor.argTimeS,
                            ArgAction = ArgProcessor.argAction,
                            ArgMode = ArgProcessor.argMode,
                            ArgPassword = ArgProcessor.argPassword,
                            ArgGraceful = ArgProcessor.argGraceful,
                            ArgPreventSleep = ArgProcessor.argPreventSleep,
                            ArgBackground = ArgProcessor.argBackground,
                            ArgUseTimeOfDay = ArgProcessor.argUseTimeOfDay
                        };
                        Application.Run(menu);
                        break;

                    case "Launch":
                    case "ForcedLaunch":
                        bool forced = new bool();
                        if (ArgProcessor.argMode.Equals("Launch")) { forced = false; }
                        else { forced = true; }

                        ExceptionHandler.Log("Run Countdown with args");
                        Timer.CountdownTimeSpan = ArgProcessor.argTimeTS;
                        Timer.Action = ArgProcessor.argAction;
                        Timer.Graceful = ArgProcessor.argGraceful;
                        Timer.PreventSystemSleep = ArgProcessor.argPreventSleep;
                        Timer.Start(ArgProcessor.argPassword, !ArgProcessor.argBackground, forced, false);
                        Application.Run();
                        break;
                }
            }
            ExceptionHandler.CreateAutoLogIfApplicable();
        }
    }
}
