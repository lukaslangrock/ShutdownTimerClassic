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
                ExceptionHandler.Log("Timer already started; aborting");
                return;
            }

            ExceptionHandler.Log("Start timer");
            isActive = true;
            clock.Start();

            countdownForm = new Countdown
            {
                IsForegroundUI = formInForeground,
                Password = formPassword,
                IsReadOnly = argForced,
                IsUserLaunched = userLaunch
            };

            ExceptionHandler.Log("Show countdown form");
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
                ExceptionHandler.Log("Start looping timer evaluation task");
                while (isActive)
                {
                    try
                    {
                        countdownForm.UpdateExternal(GetTimeRemaining());
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.Log("Exception encountered while updating the UI:");
                        ExceptionHandler.Log(ex.ToString());
                        ExceptionHandler.Log(ex.StackTrace.ToString());
                    }

                    EvaluateTimerLoop();

                    // repeat loop each 150ms
                    await Task.Delay(150);
                }
            }).ContinueWith(t =>
            {
                // a fault in the timer evaluation or power action execution is critical and must not be swallowed by the background task
                Exception e = t.Exception.Flatten().InnerException ?? t.Exception;
                ExceptionHandler.Log("Critical exception in the looping timer evaluation task, forwarding to exception handler");
                ExceptionHandler.HandleException(e, "UnhandledTaskException", true);
            }, TaskContinuationOptions.OnlyOnFaulted);
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
                ExceptionHandler.Log("Countdown reached zero");
                isActive = false; // end looped thread
                clock.Stop();
                countdownForm.ExitExternal(); // close countdown window
                ExecutePowerAction(Action);

                // check if app was started with a form and use its exit behaviour
                if (Application.OpenForms.Count > 0)
                {
                    ExceptionHandler.Log("Exit via main form");
                    Application.OpenForms[0].Invoke((MethodInvoker)(() =>
                    {
                        Application.Exit();
                    }));
                }

                // exit the application if nothing happens // no main form exists
                System.Threading.Thread.Sleep(1000);
                ExceptionHandler.Log("Fallback to Environment.Exit");
                ExceptionHandler.CreateAutoLogIfApplicable();
                Environment.Exit(0);
            }
        }

        private static void ExecutePowerAction(string chosenAction)
        {
            ExceptionHandler.Log($"Executing action: {Action}");

            switch (chosenAction)
            {
                case "Shutdown":
                    ExitWindows.Shutdown(!Graceful);
                    break;

                case "Restart":
                    ExitWindows.Reboot(!Graceful);
                    break;

                case "Hibernate":
                    if (!Application.SetSuspendState(PowerState.Hibernate, false, false))
                    {
                        throw new InvalidOperationException("Request for setting the system into hibernation state did not suceed.");
                    }
                    break;

                case "Sleep":
                    if (!Application.SetSuspendState(PowerState.Suspend, false, false))
                    {
                        throw new InvalidOperationException("Request for setting the system into suspended state did not suceed.");
                    }
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
                        ExceptionHandler.Log("Failed to start custom command process");
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
