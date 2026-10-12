using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

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

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool OpenProcessToken(IntPtr ProcessHandle, int DesiredAccess, out IntPtr TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, [MarshalAs(UnmanagedType.Bool)] bool DisableAllPrivileges,
          ref TOKEN_PRIVILEGES NewState, UInt32 BufferLength, IntPtr PreviousState, IntPtr ReturnLength);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, out LUID lpLuid);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ExitWindowsEx(uint uFlags, uint dwReason);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
        const short SE_PRIVILEGE_ENABLED = 2;
        const short TOKEN_ADJUST_PRIVILEGES = 32;
        const short TOKEN_QUERY = 8;
        const int ERROR_NOT_ALL_ASSIGNED = 1300;

        const ushort EWX_LOGOFF = 0;
        const ushort EWX_POWEROFF = 0x00000008;
        const ushort EWX_REBOOT = 0x00000002;
        const ushort EWX_SHUTDOWN = 0x00000001;
        const ushort EWX_FORCE = 0x00000004;
        const ushort EWX_FORCEIFHUNG = 0x00000010;

        const uint SHTDN_REASON_FLAG_PLANNED = 0x80000000;

        public static void GetPrivileges()
        {
            ExceptionHandler.Log("Getting privileges");
            TOKEN_PRIVILEGES tkp;
            if (!OpenProcessToken(Process.GetCurrentProcess().Handle, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, out IntPtr hToken))
            {
                int error = Marshal.GetLastWin32Error();
                ExceptionHandler.Log($"OpenProcessToken failed to open the current process token, Win32 error code {error}");
                throw new Win32Exception(error);
            }

            tkp.PrivilegeCount = 1;
            tkp.Privileges.Attributes = SE_PRIVILEGE_ENABLED;

            try
            {
                if (!LookupPrivilegeValue("", SE_SHUTDOWN_NAME, out tkp.Privileges.pLuid))
                {
                    int error = Marshal.GetLastWin32Error();
                    ExceptionHandler.Log($"LookupPrivilegeValue failed for privilege {SE_SHUTDOWN_NAME}, Win32 error code {error}");
                    throw new Win32Exception(error);
                }
                if (!AdjustTokenPrivileges(hToken, false, ref tkp, 0U, IntPtr.Zero, IntPtr.Zero))
                {
                    int error = Marshal.GetLastWin32Error();
                    ExceptionHandler.Log($"AdjustTokenPrivileges failed to enable {SE_SHUTDOWN_NAME}, Win32 error code {error}");
                    throw new Win32Exception(error);
                }
                else
                {
                    // Even if AdjustTokenPrivileges succeeds, the user may not have the privileges we wanted.
                    int error = Marshal.GetLastWin32Error();
                    if (error == ERROR_NOT_ALL_ASSIGNED)
                    {
                        ExceptionHandler.Log("SeShutdownPrivilege couldn't be assigned! (is this system configured with stronger security policies?)");
                        throw new Win32Exception(error);
                    }
                }
            }
            finally
            {
                CloseHandle(hToken);
            }

            ExceptionHandler.Log("Required privileges have been enabled");
        }

        private static void CallExitWindowsEx(uint flags)
        {
            if (!ExitWindowsEx(flags, SHTDN_REASON_FLAG_PLANNED))
            {
                int error = Marshal.GetLastWin32Error();
                ExceptionHandler.Log($"ExitWindowsEx rejected the requested flags 0x{flags:X}, Win32 error code {error}");
                throw new Win32Exception(error);
            }
            ExceptionHandler.Log("Requested power action was accepted by Windows");
        }

        public static void Shutdown() { Shutdown(false); }
        public static void Shutdown(bool force)
        {
            GetPrivileges();
            ExceptionHandler.Log("Initiating shutdown");
            CallExitWindowsEx(EWX_SHUTDOWN |
              (uint)(force ? (SettingsProvider.Settings.ForceIfHungFlag ? EWX_FORCEIFHUNG : EWX_FORCE) : 0) | EWX_POWEROFF);
        }

        public static void Reboot() { Reboot(false); }
        public static void Reboot(bool force)
        {
            GetPrivileges();
            ExceptionHandler.Log("Initiating reboot");
            CallExitWindowsEx(EWX_REBOOT |
              (uint)(force ? (SettingsProvider.Settings.ForceIfHungFlag ? EWX_FORCEIFHUNG : EWX_FORCE) : 0));
        }

        public static void LogOff() { LogOff(false); }
        public static void LogOff(bool force)
        {
            GetPrivileges();
            ExceptionHandler.Log("Initiating logoff");
            CallExitWindowsEx(EWX_LOGOFF |
              (uint)(force ? (SettingsProvider.Settings.ForceIfHungFlag ? EWX_FORCEIFHUNG : EWX_FORCE) : 0));
        }

        // Credits: https://codehill.com/2009/01/lock-sleep-or-hibernate-windows-using-c/
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool LockWorkStation();

        public static void Lock()
        {
            ExceptionHandler.Log("Initiating workstation lock");

            if (!LockWorkStation())
            {
                int error = Marshal.GetLastWin32Error();
                ExceptionHandler.Log($"LockWorkStation failed, Win32 error code {error}");
                throw new Win32Exception(error);
            }
            else { ExceptionHandler.Log("Requested workstation lock was accepted by Windows"); }
        }

        public static void Sleep()
        {
            ExceptionHandler.Log("Initiating sleep");
            if (!Application.SetSuspendState(PowerState.Suspend, false, false))
            {
                ExceptionHandler.Log("SetSuspendState failed to set the system into suspend power state");
                throw new InvalidOperationException("Request for setting the system into suspended state did not suceed.");
            }
        }

        public static void Hibernate()
        {
            ExceptionHandler.Log("Initiating hibernation");
            if (!Application.SetSuspendState(PowerState.Hibernate, false, false))
            {
                ExceptionHandler.Log("SetSuspendState failed to set the system into hibernate power state");
                throw new InvalidOperationException("Request for setting the system into hibernation state did not suceed.");
            }
        }
    }
}
