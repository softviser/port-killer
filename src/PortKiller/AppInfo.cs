using System.Diagnostics;
using System.Reflection;

namespace PortKiller;

internal static class AppInfo
{
    public const string Name = "Port Killer";
    public const string Company = "Softviser Yazılım";
    public const string Website = "https://softviser.com.tr";
    public const string Repository = "https://github.com/softviser/port-killer";

    public static string Version { get; } =
        Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion.Split('+')[0] ?? "1.0.0";

    public static void Open(string url)
    {
        try { Process.Start(new ProcessStartInfo(url) { UseShellExecute = true }); }
        catch { /* tarayıcı açılamazsa yoksay */ }
    }

    /// <summary>Kırmızı daire içinde "PK"; tepsi simgesi ve Hakkında penceresi aynı çizimi kullanır.</summary>
    public static Bitmap Logo(int size)
    {
        var bmp = new Bitmap(size, size);
        using var g = Graphics.FromImage(bmp);
        using var font = new Font("Segoe UI", size * 15f / 32, FontStyle.Bold, GraphicsUnit.Pixel);
        using var brush = new SolidBrush(Color.FromArgb(220, 53, 69));
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
        g.FillEllipse(brush, size / 32f, size / 32f, size * 30f / 32, size * 30f / 32);
        TextRenderer.DrawText(g, "PK", font, new Rectangle(0, 0, size, size), Color.White,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        return bmp;
    }
}
