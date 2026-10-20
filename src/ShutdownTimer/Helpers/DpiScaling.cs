using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ShutdownTimer.Helpers
{
    internal static class DpiScaling
    {
        private const int WmDpiChanged = 0x02E0;

        // A form first created on another monitor can retain the startup DPI in
        // WinForms even though Windows has already created its HWND at the new DPI.
        // Run after the Load handler has set the form's font and content.
        internal static void CorrectInitialDpi(Form form)
        {
            int actualDpi;
            try { actualDpi = (int)GetDpiForWindow(form.Handle); }
            catch (EntryPointNotFoundException) { return; } // Windows before version 10, 1607

            if (actualDpi <= 0 || form.DeviceDpi <= 0 || actualDpi == form.DeviceDpi) { return; }

            double scale = (double)actualDpi / form.DeviceDpi;
            List<FontSnapshot> fonts = new List<FontSnapshot>();
            CaptureFonts(form, fonts, true);
            Rectangle bounds = form.Bounds;
            NativeRectangle suggested = new NativeRectangle
            {
                Left = bounds.Left,
                Top = bounds.Top,
                Right = bounds.Left + Math.Max(1, (int)Math.Round(bounds.Width * scale)),
                Bottom = bounds.Top + Math.Max(1, (int)Math.Round(bounds.Height * scale))
            };

            // Update WinForms' DPI state and bounds through its normal handler.
            // This does not change the process or thread DPI-awareness mode.
            SendMessage(form.Handle, WmDpiChanged,
                new IntPtr(actualDpi | (actualDpi << 16)), ref suggested);

            // Initial HWND creation did not raise the child DPI-change messages
            // that normally rescale explicitly assigned fonts.
            List<Font> scaledFonts = new List<Font>();
            foreach (FontSnapshot snapshot in fonts)
            {
                Font scaled = new Font(snapshot.FamilyName, snapshot.Size * (float)scale,
                    snapshot.Style, snapshot.Unit, snapshot.CharSet, snapshot.VerticalFont);
                scaledFonts.Add(scaled);
                snapshot.Control.Font = scaled;
            }
            form.Disposed += delegate
            {
                foreach (Font font in scaledFonts) { font.Dispose(); }
            };
        }

        private static void CaptureFonts(Control control, List<FontSnapshot> fonts, bool isForm)
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(control)["Font"];
            if (isForm || (property != null && property.ShouldSerializeValue(control)))
            {
                // Resize handlers can replace and dispose fonts while handling WM_DPICHANGED.
                // Keep their values rather than references to those font objects.
                Font font = control.Font;
                fonts.Add(new FontSnapshot
                {
                    Control = control, FamilyName = font.FontFamily.Name, Size = font.Size,
                    Style = font.Style, Unit = font.Unit, CharSet = font.GdiCharSet,
                    VerticalFont = font.GdiVerticalFont
                });
            }
            foreach (Control child in control.Controls) { CaptureFonts(child, fonts, false); }
        }

        private sealed class FontSnapshot
        {
            internal Control Control;
            internal string FamilyName;
            internal float Size;
            internal FontStyle Style;
            internal GraphicsUnit Unit;
            internal byte CharSet;
            internal bool VerticalFont;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NativeRectangle
        {
            internal int Left;
            internal int Top;
            internal int Right;
            internal int Bottom;
        }

        [DllImport("user32.dll")]
        private static extern uint GetDpiForWindow(IntPtr window);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr window, int message, IntPtr wParam, ref NativeRectangle rectangle);
    }
}
