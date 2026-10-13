using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace ShutdownTimer.Helpers
{
    public static class ApplicationInstanceManager
    {
        private static Mutex mutex;
        private static bool hasLock;

        public static bool IsSingleInstance()
        {
            if (mutex != null)
            {
                return hasLock;
            }
            else
            {
                try
                {
                    return OpenMutex();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Log("Failed to acquire Mutex: " + ex.Message);

                    // Fallback to simple process name check if Mutex creation fails
                    return Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length == 1;
                }
            }
        }

        /// <summary>
        /// Opens a Mutex in kernel memory to check if another instance is already running.
        /// </summary>
        /// <returns>true if we acquire the mutex</returns>
        private static bool OpenMutex()
        {
            mutex = new Mutex(true, $"Local\\{Application.ProductName}Mutex", out hasLock);
            return hasLock;
        }

        /// <summary>
        /// Release the Mutex used to track application instances. Critical to execute before restarting the application.
        /// Calling Application.Restart will start another process before the current one exits, thus the Mutex must be released to allow the new instance to acquire it.
        /// </summary>
        public static void ReleaseMutex()
        {
            if (mutex != null && hasLock)
            {
                try
                {
                    mutex.ReleaseMutex();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Log("Failed to release Mutex: " + ex.Message);
                }
                finally { 
                    mutex.Dispose();
                    mutex = null;
                    hasLock = false;
                }
            }
        }
    }
}
