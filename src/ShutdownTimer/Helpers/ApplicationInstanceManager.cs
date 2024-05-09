using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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
