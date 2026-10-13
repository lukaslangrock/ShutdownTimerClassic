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

            if (args.Length != 0)
            {
                ExceptionHandler.Log("Processing CLI args");
                try
                {
                    ArgProcessor.ProcessArgs(args);
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Log("Failed to process CLI args: " + ex.Message);
                    MessageBox.Show("Failed to process command line arguments:\n\n" + ex.Message, "Invalid arguments", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Initialize settings
                ExceptionHandler.Log("Load settings");
                if (!ArgProcessor.argUseSettings && !String.IsNullOrEmpty(ArgProcessor.argMode))
                {
                    // unless explicitly requested, do not load settings when running in CLI mode
                    // The use /SetMode acts as the central trigger to apply CLI values, thus its presence will block loading/saving settings
                    SettingsProvider.TemporaryMode = true;
                }
                SettingsProvider.Load();
                ExceptionHandler.Log("Settings loaded");

                string[] validActions = { "Shutdown", "Restart", "Hibernate", "Sleep", "Logout", "Lock", "Custom Command" };
                if (Array.IndexOf(validActions, ArgProcessor.argAction) == -1)
                {
                    ExceptionHandler.Log("Launch aborted: invalid /SetAction argument");
                    MessageBox.Show($"'{ArgProcessor.argAction}' is not a valid action.\n\nPlease supply a valid action using the /SetAction argument.", "Invalid action", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                switch (ArgProcessor.argMode)
                {
                    case "Prefill":
                    case "Lock":
                        ExceptionHandler.Log("Run menu with args");

                        Menu menu = new Menu
                        {
                            ApplyArgumentValues = true,
                            ArgTimeH = ArgProcessor.argTimeH,
                            ArgTimeM = ArgProcessor.argTimeM,
                            ArgTimeS = ArgProcessor.argTimeS,
                            ArgAction = ArgProcessor.argAction,
                            ArgMode = ArgProcessor.argMode,
                            ArgPassword = ArgProcessor.argPassword,
                            ArgCommand = ArgProcessor.argCommand,
                            ArgGraceful = ArgProcessor.argGraceful,
                            ArgPreventSleep = ArgProcessor.argPreventSleep,
                            ArgBackground = ArgProcessor.argBackground,
                            ArgUseTimeOfDay = ArgProcessor.argUseTimeOfDay
                        };
                        Application.Run(menu);
                        ExceptionHandler.CreateAutoLogIfApplicable();
                        return;

                    case "Launch":
                    case "ForcedLaunch":
                        bool forced = !ArgProcessor.argMode.Equals("Launch");

                        if (string.IsNullOrWhiteSpace(ArgProcessor.argTime) || ArgProcessor.argTimeTS <= TimeSpan.Zero)
                        {
                            ExceptionHandler.Log("Launch aborted: time is missing, zero, or invalid");
                            MessageBox.Show("The countdown requires a valid time greater than zero.\n\nPlease supply one using the /SetTime argument.", "Invalid time", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // A custom command action requires a command to execute
                        if (ArgProcessor.argAction.Equals("Custom Command") && string.IsNullOrWhiteSpace(ArgProcessor.argCommand))
                        {
                            ExceptionHandler.Log("Launch aborted: action is 'Custom Command' but no /SetCommand argument was supplied");
                            MessageBox.Show("The action 'Custom Command' requires a command to execute.\n\nPlease supply one using the /SetCommand argument.", "Missing custom command", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        ExceptionHandler.Log("Run Countdown with args");
                        Timer.CountdownTimeSpan = ArgProcessor.argTimeTS;
                        Timer.Action = ArgProcessor.argAction;
                        Timer.Command = ArgProcessor.argCommand;
                        Timer.Graceful = ArgProcessor.argGraceful;
                        Timer.PreventSystemSleep = ArgProcessor.argPreventSleep;
                        Timer.Start(ArgProcessor.argPassword, !ArgProcessor.argBackground, forced, false);
                        Application.Run();
                        ExceptionHandler.CreateAutoLogIfApplicable();
                        return;

                    default:
                        if (ArgProcessor.argMode != null)
                        {
                            ExceptionHandler.Log("Launch aborted: invalid /SetMode argument");
                            MessageBox.Show($"'{ArgProcessor.argMode}' is not a valid control mode.\n\nValid modes are: Prefill, Lock, Launch, ForcedLaunch.", "Invalid mode", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        break;
                }
            }

            ExceptionHandler.Log("Start menu");
            if (!SettingsProvider.SettingsLoaded)
            {
                SettingsProvider.Load();
            }
            Application.Run(new Menu());
            ExceptionHandler.CreateAutoLogIfApplicable();
        }
    }
}
