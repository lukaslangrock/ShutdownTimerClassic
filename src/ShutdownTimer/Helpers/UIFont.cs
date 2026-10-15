using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace ShutdownTimer.Helpers
{
    /// <summary>
    /// Chooses a font that can actually render the text the forms display.
    ///
    /// Every form assigned "Microsoft Sans Serif" explicitly to keep the layout stable, which is
    /// correct for Latin text only: that family has no glyphs for Chinese, Japanese or Korean, so
    /// each CJK character falls back to a different font mid-line. The fallback has different
    /// metrics, so the text overflows the box the designer measured it in (reported in #56).
    ///
    /// On cultures which need those scripts this replaces the design font with one that covers
    /// them; on every other culture nothing is touched at all.
    /// </summary>
    public static class UIFont
    {
        private const string DesignFontFamily = "Microsoft Sans Serif";

        private static readonly string[] SimplifiedChineseFamilies = { "Microsoft YaHei UI", "Microsoft YaHei", "Malgun Gothic", "Meiryo UI" };

        private static readonly string[] TraditionalChineseFamilies = { "Microsoft JhengHei UI", "Microsoft JhengHei", "Microsoft YaHei UI", "Malgun Gothic" };

        private static readonly string[] JapaneseFamilies = { "Yu Gothic UI", "Meiryo UI", "MS UI Gothic", "Microsoft YaHei UI" };

        private static readonly string[] KoreanFamilies = { "Malgun Gothic", "Microsoft YaHei UI" };

        /// <summary>
        /// Swaps the design font for a script capable one on the form and all of its children.
        /// Controls which inherit their font are left alone.
        /// </summary>
        public static void Apply(Form form)
        {
            if (form == null) { return; }

            string family = RequiredFontFamily();
            if (family == null) { return; }

            ReplaceFont(form, family);
        }

        private static string RequiredFontFamily()
        {
            CultureInfo uiCulture = CultureInfo.CurrentUICulture;
            string language = uiCulture.TwoLetterISOLanguageName;
            if (language != "zh" && language != "ja" && language != "ko")
            {
                return null;
            }

            foreach (string candidate in PreferredFamilies(language, uiCulture.Name))
            {
                if (IsInstallable(candidate)) { return candidate; }
            }

            Font systemDefault = SystemFonts.DefaultFont;
            return systemDefault != null ? systemDefault.FontFamily.Name : null;
        }

        private static string[] PreferredFamilies(string language, string cultureName)
        {
            if (language == "ja") { return JapaneseFamilies; }
            if (language == "ko") { return KoreanFamilies; }
            bool traditional = cultureName.StartsWith("zh-TW", StringComparison.OrdinalIgnoreCase)
                || cultureName.StartsWith("zh-Hant", StringComparison.OrdinalIgnoreCase)
                || cultureName.StartsWith("zh-HK", StringComparison.OrdinalIgnoreCase);
            return traditional ? TraditionalChineseFamilies : SimplifiedChineseFamilies;
        }

        private static bool IsInstallable(string family)
        {
            try
            {
                using (Font probe = new Font(family, 8.25f))
                {
                    // Font silently substitutes when the requested family is missing.
                    return string.Equals(probe.FontFamily.Name, family, StringComparison.OrdinalIgnoreCase);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void ReplaceFont(Control control, string family)
        {
            Font current = control.Font;
            if (current != null &&
                string.Equals(current.FontFamily.Name, DesignFontFamily, StringComparison.OrdinalIgnoreCase))
            {
                control.Font = new Font(family, current.Size, current.Style, current.Unit, current.GdiCharSet);
            }

            foreach (Control child in control.Controls)
            {
                ReplaceFont(child, family);
            }
        }
    }
}
