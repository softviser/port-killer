using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace PortKiller;

internal sealed record Palette(
    bool Dark, Color Back, Color Surface, Color Field, Color FieldBorder, Color Line,
    Color Text, Color Muted, Color Accent, Color Selection, Color Danger);

/// <summary>Windows'un "uygulama modu" (açık/koyu) ve vurgu rengine göre renkler.</summary>
internal static class Theme
{
    /// <summary>Sistem ayarını yok sayıp temayı zorlar (testlerde kullanılır).</summary>
    internal static bool? ForceDark { get; set; }

    public static Palette Current()
    {
        bool dark = ForceDark ?? Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 1) is 0;
        var accent = Accent();
        return dark
            ? new(true, Back: Rgb(0x202020), Surface: Rgb(0x2B2B2B), Field: Rgb(0x2D2D2D), FieldBorder: Rgb(0x555555), Line: Rgb(0x353535),
                Text: Rgb(0xF0F0F0), Muted: Rgb(0x9D9D9D), Accent: Mix(accent, Color.White, 0.7), Selection: Mix(accent, Rgb(0x202020), 0.35), Danger: Rgb(0xFF6B6B))
            : new(false, Back: Color.White, Surface: Rgb(0xF3F3F3), Field: Rgb(0xFBFBFB), FieldBorder: Rgb(0xC4C4C4), Line: Rgb(0xE5E5E5),
                Text: Rgb(0x1A1A1A), Muted: Rgb(0x6B6B6B), Accent: accent, Selection: Mix(accent, Color.White, 0.18), Danger: Rgb(0xC42B1C));
    }

    // DWM AccentColor 0xAABBGGRR olarak saklanır; yoksa sistem vurgu rengi.
    private static Color Accent() =>
        Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "AccentColor", null) is int abgr
            ? Color.FromArgb(abgr & 0xFF, (abgr >> 8) & 0xFF, (abgr >> 16) & 0xFF)
            : SystemColors.Highlight;

    private static Color Rgb(int rgb) => Color.FromArgb((rgb >> 16) & 0xFF, (rgb >> 8) & 0xFF, rgb & 0xFF);

    private static Color Mix(Color a, Color b, double t) =>
        Color.FromArgb((int)(a.R * t + b.R * (1 - t)), (int)(a.G * t + b.G * (1 - t)), (int)(a.B * t + b.B * (1 - t)));

    /// <summary>Başlık çubuğu ve kaydırma çubuklarını temaya uydurur (Windows 10 1809+).</summary>
    public static void ApplyChrome(Control c, bool dark)
    {
        int on = dark ? 1 : 0;
        DwmSetWindowAttribute(c.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref on, sizeof(int));
        SetWindowTheme(c.Handle, dark ? "DarkMode_Explorer" : "Explorer", null);
    }

    private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int size);

    [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
    private static extern int SetWindowTheme(IntPtr hwnd, string? appName, string? idList);

    /// <summary>Tepsi menüsü renkleri.</summary>
    public sealed class MenuColors(Palette p) : ProfessionalColorTable
    {
        public override Color ToolStripDropDownBackground => p.Back;
        public override Color ImageMarginGradientBegin => p.Back;
        public override Color ImageMarginGradientMiddle => p.Back;
        public override Color ImageMarginGradientEnd => p.Back;
        public override Color MenuBorder => p.FieldBorder;
        public override Color MenuItemBorder => p.Selection;
        public override Color MenuItemSelected => p.Selection;
        public override Color SeparatorDark => p.Line;
        public override Color SeparatorLight => p.Line;
    }
}
