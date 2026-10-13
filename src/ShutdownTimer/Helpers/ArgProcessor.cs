using System;

namespace ShutdownTimer.Helpers
{
    public static class ArgProcessor
    {
        public static string argMode;
        public static string argTime;
        public static string argAction = "Shutdown";
        public static string argCommand;
        public static string argPassword;
        public static bool argGraceful = false;
        public static bool argPreventSleep = true;
        public static bool argBackground = false;
        public static bool argUseSettings = false;
        public static bool argUseTimeOfDay = false;
        public static int argTimeH;
        public static int argTimeM;
        public static int argTimeS;
        public static TimeSpan argTimeTS;

        public static void ProcessArgs(string[] args)
        {
            ExceptionHandler.Log("Processing CLI args");
            ReadArgs(args);
            ExportTimeToInt();
            CalculateTimeSpan();
            ExceptionHandler.Log("CLI args processed");
        }

        private static void ReadArgs(string[] args)
        {
            ExceptionHandler.Log("Reading CLI args");

            for (var i = 0; i < args.Length; i++)
            {
                ExceptionHandler.Log("Arg " + i + " = " + args[i]);
            }

            // Read args and do some processing
            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "/SetTime":
                        if (i + 1 >= args.Length || args[i + 1].StartsWith("/")) { throw new ArgumentException("Missing value for /SetTime."); }
                        argTime = args[++i];
                        break;

                    case "/SetAction":
                        if (i + 1 >= args.Length || args[i + 1].StartsWith("/")) { throw new ArgumentException("Missing value for /SetAction."); }
                        argAction = args[++i];
                        break;

                    case "/SetCommand":
                        if (i + 1 >= args.Length) { throw new ArgumentException("Missing value for /SetCommand."); }
                        argCommand = args[++i];
                        break;

                    case "/SetMode":
                        if (i + 1 >= args.Length || args[i + 1].StartsWith("/")) { throw new ArgumentException("Missing value for /SetMode."); }
                        argMode = args[++i];
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

                    case "/UseSettings":
                        argUseSettings = true;
                        break;

                    case "/TargetTimeOfDay":
                        argUseTimeOfDay = true;
                        break;

                    case "/SetPassword":
                        if (i + 1 >= args.Length) { throw new ArgumentException("Missing value for /SetPassword."); }
                        argPassword = args[++i];
                        break;
                }
            }

            ExceptionHandler.Log("Args read");
        }

        private static void ExportTimeToInt()
        {
            ExceptionHandler.Log("Parsing time argument");

            if (argTime != null)
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

                        default:
                            throw new ArgumentException("Invalid /SetTime format. Expected seconds, HH:mm, or HH:mm:ss.");
                    }
                }

                if (argTimeH < 0 || argTimeM < 0 || argTimeS < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(argTime), "Time values cannot be negative.");
                }

                if (argUseTimeOfDay && (argTimeH > 23 || argTimeM > 59 || argTimeS > 59))
                {
                    throw new ArgumentOutOfRangeException(nameof(argTime), "Target time of day must be a valid 24-hour time (00:00:00 to 23:59:59).");
                }
            }

            ExceptionHandler.Log("Time argument parsed");
        }

        private static void CalculateTimeSpan()
        {
            ExceptionHandler.Log("Calculating TimeSpan");
            argTimeTS = Time.CalculateCountdownTimeSpan(argTimeH, argTimeM, argTimeS, argUseTimeOfDay, DateTime.Now);
        }
    }
}
