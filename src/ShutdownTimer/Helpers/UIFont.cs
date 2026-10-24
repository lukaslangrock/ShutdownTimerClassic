using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace ShutdownTimer.Helpers
{
    /// <summary>
    /// Replaces the font the forms are designed with by one that can render the script of the
    /// current UI culture. Cultures the design font already covers are left untouched.
    /// </summary>
    public static class UIFont
    {
        private const string DesignFontFamily = "Microsoft Sans Serif";

        // Ordered by preference. The "* UI" variants exist since Windows 8, the plain families ship
        // with Windows 7 as well, so both have to be listed.
        private static readonly string[] SimplifiedChineseFamilies = { "Microsoft YaHei UI", "Microsoft YaHei" };

        private static readonly string[] TraditionalChineseFamilies = { "Microsoft JhengHei UI", "Microsoft JhengHei", "Microsoft YaHei UI", "Microsoft YaHei" };

        private static readonly string[] JapaneseFamilies = { "Yu Gothic UI", "Meiryo UI", "MS UI Gothic" };

        private static readonly string[] KoreanFamilies = { "Malgun Gothic" };

        // The name the design family reports at runtime. Resolved through the same API that
        // ReplaceFont uses for comparison, so the two always agree, whatever the UI language is.
        private static readonly string DesignFontName = ResolveDesignFontName();

        /// <summary>
        /// Applies the family required by the current UI culture to the form and all of its children.
        /// </summary>
        public static void Apply(Form form)
        {
            if (form == null) { return; }

            string family = RequiredFontFamily(CultureInfo.CurrentUICulture);
            if (family == null) { return; }

            ReplaceFont(form, family);
        }

        // Returns the family to use for the given culture, or null when the design font already
        // covers its script and nothing should be changed.
        private static string RequiredFontFamily(CultureInfo culture)
        {
            if (culture == null) { return null; }

            string[] candidates = PreferredFamilies(culture);
            if (candidates == null) { return null; }

            foreach (string candidate in candidates)
            {
                if (IsInstalled(candidate)) { return candidate; }
            }

            // The system UI font always covers the language of the shell, so it is a safe last
            // resort. On a Chinese system this is SimSun, which is not the design family, but it
            // keeps the text readable instead of leaving it to font linking.
            Font systemDefault = SystemFonts.DefaultFont;
            return systemDefault != null ? systemDefault.FontFamily.Name : null;
        }

        // Maps a culture to the families which can render its script. Returns null if no replacement
        // is needed.
        private static string[] PreferredFamilies(CultureInfo culture)
        {
            switch (culture.Name)
            {
                case "zh":
                case "zh-Hans":
                case "zh-CN":
                case "zh-SG":
                case "zh-MY":
                    return SimplifiedChineseFamilies;

                case "zh-Hant":
                case "zh-TW":
                case "zh-HK":
                case "zh-MO":
                    return TraditionalChineseFamilies;

                case "ja":
                case "ja-JP":
                    return JapaneseFamilies;

                case "ko":
                case "ko-KR":
                    return KoreanFamilies;
            }

            // Cultures not listed above. For Chinese the script subtag decides; the neutral "zh"
            // culture without one resolves to Simplified, which is the .NET default as well.
            switch (culture.TwoLetterISOLanguageName)
            {
                case "zh":
                    return IsTraditional(culture) ? TraditionalChineseFamilies : SimplifiedChineseFamilies;

                case "ja":
                    return JapaneseFamilies;

                case "ko":
                    return KoreanFamilies;
            }

            return null;
        }

        private static bool IsTraditional(CultureInfo culture)
        {
            string name = culture.Name;
            return name.StartsWith("zh-Hant", StringComparison.OrdinalIgnoreCase)
                || name.StartsWith("zh-TW", StringComparison.OrdinalIgnoreCase)
                || name.StartsWith("zh-HK", StringComparison.OrdinalIgnoreCase)
                || name.StartsWith("zh-MO", StringComparison.OrdinalIgnoreCase);
        }

        // Probes by constructing a FontFamily, which fails with an ArgumentException when the family
        // is missing. Constructing a Font instead would not work: GDI+ substitutes a fallback and
        // FontFamily.Name then reports that substitute in the language of the current UI, so it
        // never compares equal to the ASCII name that was asked for.
        private static bool IsInstalled(string familyName)
        {
            try
            {
                using (new FontFamily(familyName))
                {
                    return true;
                }
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        private static string ResolveDesignFontName()
        {
            try
            {
                using (FontFamily design = new FontFamily(DesignFontFamily))
                {
                    return design.Name;
                }
            }
            catch (ArgumentException)
            {
                return DesignFontFamily;
            }
        }

        // Replaces the design font on the control and recursively on its children. Font is an
        // ambient property, so a child which never assigned one reports the value inherited from its
        // parent. The parent is replaced first, which means such a child already reports the new
        // family and is skipped - the intended result, since it inherits the replacement. Only
        // children which assigned the design font explicitly are rewritten here.
        private static void ReplaceFont(Control control, string family)
        {
            Font current = control.Font;
            if (current != null &&
                string.Equals(current.FontFamily.Name, DesignFontName, StringComparison.OrdinalIgnoreCase))
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
