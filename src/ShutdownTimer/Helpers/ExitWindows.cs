using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ShutdownTimer.Helpers;

namespace ShutdownTimer.Helpers
{
    public static class ExitWindows
    {
        // Credits: https://miromannino.com/blog/exitwindowsex-in-c/ (slightly modified)
        // Thank you so much!!

        private struct LUID
        {
            public int LowPart;
            public int HighPart;
        }
        private struct LUID_AND_ATTRIBUTES
        {
            public LUID pLuid;
            public int Attributes;
        }
        private struct TOKEN_PRIVILEGES
        {
            public int PrivilegeCount;
            public LUID_AND_ATTRIBUTES Privileges;
        }

        [DllImport("advapi32.dll")]
        static extern int OpenProcessToken(IntPtr ProcessHandle,
          int DesiredAccess, out IntPtr TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AdjustTokenPrivileges(IntPtr TokenHandle,
          [MarshalAs(UnmanagedType.Bool)] bool DisableAllPrivileges,
          ref TOKEN_PRIVILEGES NewState,
          UInt32 BufferLength,
          IntPtr PreviousState,
          IntPtr ReturnLength);

        [DllImport("advapi32.dll")]
        static extern int LookupPrivilegeValue(string lpSystemName,
          string lpName, out LUID lpLuid);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int ExitWindowsEx(uint uFlags, uint dwReason);

        const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
        const short SE_PRIVILEGE_ENABLED = 2;
        const short TOKEN_ADJUST_PRIVILEGES = 32;
        const short TOKEN_QUERY = 8;

        const ushort EWX_LOGOFF = 0;
        const ushort EWX_POWEROFF = 0x00000008;
        const ushort EWX_REBOOT = 0x00000002;
        const ushort EWX_SHUTDOWN = 0x00000001;
        const ushort EWX_FORCE = 0x00000004;
        const ushort EWX_FORCEIFHUNG = 0x00000010;

        const uint SHTDN_REASON_FLAG_PLANNED = 0x80000000;

        private static void GetPrivileges()
        {
            ExceptionHandler.LogEvent("[ExitWindows] Getting privileges...");
            TOKEN_PRIVILEGES tkp;
            OpenProcessToken(Process.GetCurrentProcess().Handle,
              TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, out IntPtr hToken);
            tkp.PrivilegeCount = 1;
            tkp.Privileges.Attributes = SE_PRIVILEGE_ENABLED;
            LookupPrivilegeValue("", SE_SHUTDOWN_NAME,
              out tkp.Privileges.pLuid);
            AdjustTokenPrivileges(hToken, false, ref tkp,
              0U, IntPtr.Zero, IntPtr.Zero);
        }

        public static void Shutdown() { Shutdown(false); }
        public static void Shutdown(bool force)
        {
            GetPrivileges();
            ExceptionHandler.LogEvent("[ExitWindows] Shutting down...");
            ExitWindowsEx(EWX_SHUTDOWN |
              (uint)(force ? (SettingsProvider.Settings.ForceIfHungFlag ? EWX_FORCEIFHUNG : EWX_FORCE) : 0) | EWX_POWEROFF, SHTDN_REASON_FLAG_PLANNED);
        }

        public static void Reboot() { Reboot(false); }
        public static void Reboot(bool force)
        {
            GetPrivileges();
            ExceptionHandler.LogEvent("[ExitWindows] Rebooting...");
            ExitWindowsEx(EWX_REBOOT |
              (uint)(force ? (SettingsProvider.Settings.ForceIfHungFlag ? EWX_FORCEIFHUNG : EWX_FORCE) : 0), SHTDN_REASON_FLAG_PLANNED);
        }

        public static void LogOff() { LogOff(false); }
        public static void LogOff(bool force)
        {
            GetPrivileges();
            ExceptionHandler.LogEvent("[ExitWindows] Logging off...");
            ExitWindowsEx(EWX_LOGOFF |
              (uint)(force ? (SettingsProvider.Settings.ForceIfHungFlag ? EWX_FORCEIFHUNG : EWX_FORCE) : 0), SHTDN_REASON_FLAG_PLANNED);
        }

        // Credits: https://codehill.com/2009/01/lock-sleep-or-hibernate-windows-using-c/
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool LockWorkStation();

        public static void Lock()
        {
            ExceptionHandler.LogEvent("[ExitWindows] Trying to lock...");

            bool result = LockWorkStation();

            if (result == false)
            {
                // An error occurred
                ExceptionHandler.LogEvent("[ExitWindows] Error locking workstation!");
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            else { ExceptionHandler.LogEvent("[ExitWindows] Workstation lock successful"); }
        }
    }
}
