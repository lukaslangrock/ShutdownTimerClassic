using ShutdownTimer.Helpers;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Application = System.Windows.Forms.Application;

namespace ShutdownTimer
{
    static class Timer
    {
        public static TimeSpan CountdownTimeSpan { get; set; } // timespan after which the power action gets executed
        public static string Action { get; set; } // defines what power action to execute (fallback to shutdown if not changed)
        public static bool Graceful { get; set; } // uses a graceful shutdown which allows apps to save their work or interrupt the shutdown
        public static bool PreventSystemSleep { get; set; } // tells Windows that the system should stay awake during countdown
        public static string Command { get; set; } // for executing a custom command instead of a power action

        private static bool isActive = false; // allows only one activation
        private static Stopwatch clock = new Stopwatch(); // measures exact timespan of passed time
        private static Countdown countdownForm; // countdown window (also serves tray icon)

        public static void Start(string formPassword = null, bool formInForeground = true, bool argForced = false, bool userLaunch = true)
        {
            // make sure Start() can only be called once
            if (isActive)
            {
                ExceptionHandler.Log("ERROR: Timer has already been started!");
                return;
            }

            ExceptionHandler.Log("Starting timer clock");
            isActive = true;
            clock.Start();

            countdownForm = new Countdown
            {
                IsForegroundUI = formInForeground,
                Password = formPassword,
                IsReadOnly = argForced,
                IsUserLaunched = userLaunch
            };

            ExceptionHandler.Log("Starting Countdown form...");
            countdownForm.Show();

            if (PreventSystemSleep)
            {
                // give the system some coffee so it stays awake when tired using some fancy EXECUTION_STATE flags
                ExceptionHandler.Log("Setting EXECUTION_STATE flags to prevent system from going to sleep");
                ExecutionState.SetThreadExecutionState(ExecutionState.EXECUTION_STATE.ES_CONTINUOUS | ExecutionState.EXECUTION_STATE.ES_SYSTEM_REQUIRED);
            }

            // setup automatic evaluation of passed time
            Task.Run(async () =>
            {
                ExceptionHandler.Log("Starting looping timer evaluation task");
                while (isActive)
                {
                    try
                    {
                        countdownForm.UpdateExternal(GetTimeRemaining());
                        EvaluateTimerLoop();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        Console.WriteLine(ex.StackTrace.ToString());
                    }

                    // repeat loop each 150ms
                    await Task.Delay(150);
                }
            });
        }

        public static void Pause()
        {
            if (isActive)
            {
                ExceptionHandler.Log("Pausing clock");
                clock.Stop();
            }
        }

        public static void Resume()
        {
            if (isActive)
            {
                ExceptionHandler.Log("Resuming clock");
                clock.Start();
            }
        }

        public static void Reset()
        {
            if (isActive)
            {
                bool wasRunning = clock.IsRunning;
                ExceptionHandler.Log("Resetting clock");
                clock.Reset();
                if (wasRunning)
                {
                    ExceptionHandler.Log("Starting clock to keep pre-reset state consistent.");
                    clock.Start();
                }
            }
        }

        public static void Restart()
        {
            if (isActive)
            {
                ExceptionHandler.Log("Restarting clock");
                clock.Restart();
            }
        }

        public static bool IsRunning()
        {
            return clock.IsRunning;
        }

        public static TimeSpan GetTimeRemaining()
        {
            return CountdownTimeSpan - clock.Elapsed;
        }

        // is called by a looping task from Timer.Start()
        private static void EvaluateTimerLoop()
        {
            if (GetTimeRemaining().TotalSeconds <= 0)
            {
                ExceptionHandler.Log("Countdown reached 0, getting ready for action");
                isActive = false; // end looped thread
                clock.Stop();
                countdownForm.ExitExternal(); // close countdown window
                ExecutePowerAction(Action);

                // Clear EXECUTION_STATE flags to allow the system to go to sleep if it's tired.
                if (PreventSystemSleep)
                {
                    ExceptionHandler.Log("Clearing EXECUTION_STATE flags");
                    ExecutionState.SetThreadExecutionState(ExecutionState.EXECUTION_STATE.ES_CONTINUOUS);
                }

                // check if app was started with a form and use it's exit behaviour
                if (Application.OpenForms.Count > 0)
                {
                    ExceptionHandler.Log("Trying to exit application via main form");
                    Application.OpenForms[0].Invoke((MethodInvoker)(() =>
                    {
                        Application.Exit();
                    }));
                }

                // exit the application if nothing happens // no main form exists
                System.Threading.Thread.Sleep(1000);
                ExceptionHandler.Log("Exit via main form apparently failed, using Environment.Exit()");
                ExceptionHandler.CreateAutoLogIfApplicable();
                Environment.Exit(0);
            }
        }

        private static void ExecutePowerAction(string choosenAction)
        {
            ExceptionHandler.Log($"Processing action: {Action}");

            switch (choosenAction)
            {
                case "Shutdown":
                    ExitWindows.Shutdown(!Graceful);
                    break;

                case "Restart":
                    ExitWindows.Reboot(!Graceful);
                    break;

                case "Hibernate":
                    Application.SetSuspendState(PowerState.Hibernate, false, false);
                    break;

                case "Sleep":
                    Application.SetSuspendState(PowerState.Suspend, false, false);
                    break;

                case "Logout":
                    ExitWindows.LogOff(!Graceful);
                    break;

                case "Lock":
                    ExitWindows.Lock();
                    break;

                case "Custom Command":
                    try
                    {
                        Process.Start(Command);
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.Log("Error starting a process of the custom command.");
                        ExceptionHandler.Log("Custom command: " + Command);
                        ExceptionHandler.Log("Exception: " + ex.ToString());
                        MessageBox.Show("There was an error executing your custom command.\n\nYour custom command: " + Command + "\nError: " + ex.Message, "Countdown Update", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;

                default:
                    ExceptionHandler.Log("ERROR: Action was invalid");
                    break;
            }
        }
    }
}
