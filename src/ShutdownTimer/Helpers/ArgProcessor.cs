using System;

namespace ShutdownTimer.Helpers
{
    public static class ArgProcessor
    {
        public static string argMode = "Prefill";
        public static string argTime;
        public static string argAction = "Shutdown";
        public static bool argGraceful = false;
        public static bool argPreventSleep = true;
        public static bool argBackground = false;
        public static bool argNoSettings = false;
        public static int argTimeH;
        public static int argTimeM;
        public static int argTimeS;
        public static TimeSpan argTimeTS;

        public static void ProcessArgs(string[] args)
        {
            ExceptionHandler.LogEvent("[ArgProcessor] Processing args...");
            ReadArgs(args);
            ExportTimeToInt();
            ExportTimeToTimeSpan();
            ExceptionHandler.LogEvent("[ArgProcessor] Processed args");
        }

        private static void ReadArgs(string[] args)
        {
            ExceptionHandler.LogEvent("[ArgProcessor] Reading args...");

            //Control Modes:
            //Prefill:      Prefills settings but let user manually change them too. Timer won't start automatically.
            //Lock:         Overrides settings so the user can not change them. Timer won't start automatically.
            //Launch:       Overrides settings and starts the timer.
            //ForcedLaunch: Overrides settings and starts the timer. Disables all UI controls and exit dialogs.

            // Read args and do some processing
            for (var i = 0; i < args.Length; i++)
            {
                ExceptionHandler.LogEvent("[ArgProcessor] Arg " + i + " = " + args[i]);
                switch (args[i])
                {
                    case "/SetTime":
                        argTime = args[i + 1];
                        break;

                    case "/SetAction":
                        argAction = args[i + 1];
                        break;

                    case "/SetMode":
                        argMode = args[i + 1];
                        break;

                    case "/Graceful":
                        argGraceful = true;
                        break;

                    case "/AllowSleep":
                        argPreventSleep = false;
                        break;

                    case "/Background":
                        argBackground = true;
                        break;

                    case "/NoSettings":
                        argNoSettings = true;
                        break;
                }
            }

            ExceptionHandler.LogEvent("[ArgProcessor] Read args");
        }

        private static void ExportTimeToInt()
        {
            ExceptionHandler.LogEvent("[ArgProcessor] Exporting to Int...");

            if (!string.IsNullOrWhiteSpace(argTime))
            {
                if (!argTime.Contains(":"))
                {
                    // time in seconds
                    argTimeS = Convert.ToInt32(argTime);
                }
                else
                {
                    string[] splittedTimeArg = argTime.Split(':');
                    int count = splittedTimeArg.Length - 1; // Count number of colons
                    switch (count)
                    {
                        case 0:
                            ExceptionHandler.LogEvent("[ArgProcessor] Invalid time args");
                            break;

                        case 1:
                            // Assuming HH:mm
                            argTimeH = Convert.ToInt32(splittedTimeArg[0]);
                            argTimeM = Convert.ToInt32(splittedTimeArg[1]);
                            break;

                        case 2:
                            // Assuming HH:mm:ss
                            argTimeH = Convert.ToInt32(splittedTimeArg[0]);
                            argTimeM = Convert.ToInt32(splittedTimeArg[1]);
                            argTimeS = Convert.ToInt32(splittedTimeArg[2]);
                            break;
                    }
                }
            }

            ExceptionHandler.LogEvent("[ArgProcessor] Exported to Int");
        }

        private static void ExportTimeToTimeSpan()
        {
            ExceptionHandler.LogEvent("[ArgProcessor] Exporting to TimeSpan...");

            argTimeTS = new TimeSpan(argTimeH, argTimeM, argTimeS);

            ExceptionHandler.LogEvent("[ArgProcessor] Exported to TimeSpan");
        }
    }
}
