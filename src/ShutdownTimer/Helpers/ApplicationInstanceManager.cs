using System.Diagnostics;

namespace ShutdownTimer.Helpers
{
    public static class ApplicationInstanceManager
    {
        public static bool IsSingleInstance()
        {
            return Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length == 1;
        }
    }
}
