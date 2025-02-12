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
                ExceptionHandler.Log("Attaching ExceptionHandler.cs");
                AppDomain.CurrentDomain.UnhandledException += ExceptionHandler.UnhandledExceptionHandler;
                Application.ThreadException += ExceptionHandler.ThreadExceptionHandler;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length == 0)
            {
                ExceptionHandler.Log("Running Menu");
                SettingsProvider.Load();
                Application.Run(new Menu());
            }
            else
            {
                ExceptionHandler.Log("Processing args...");
                ArgProcessor.ProcessArgs(args);

                // Initialize settings
                ExceptionHandler.Log("Loading settings...");
                if (ArgProcessor.argNoSettings)
                { SettingsProvider.TemporaryMode = true; }
                SettingsProvider.Load();
                ExceptionHandler.Log("Settings loaded");

                switch (ArgProcessor.argMode)
                {
                    case "Prefill":
                    case "Lock":
                        ExceptionHandler.Log("Running Menu with args");
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

                        ExceptionHandler.Log("Running Countdown with args" + forced.ToString());
                        Timer.CountdownTimeSpan = ArgProcessor.argTimeTS;
                        Timer.Action = ArgProcessor.argAction;
                        Timer.Graceful = ArgProcessor.argGraceful;
                        Timer.PreventSystemSleep = ArgProcessor.argPreventSleep;
                        Timer.Start(null, !ArgProcessor.argBackground, forced, false);
                        Application.Run();
                        break;
                }
            }
            ExceptionHandler.CreateAutoLogIfApplicable();
        }
    }
}
