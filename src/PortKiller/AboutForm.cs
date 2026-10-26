namespace PortKiller;

/// <summary>Sürüm, lisans, web sitesi ve kaynak kod bağlantıları.</summary>
internal sealed class AboutForm : Form
{
    private readonly Palette p = Theme.Current();

    public AboutForm()
    {
        Text = S.About;
        Font = new Font("Segoe UI", 9f);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = MinimizeBox = ShowIcon = ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterScreen;
        TopMost = true;
        ClientSize = new Size(380, 224);
        BackColor = p.Back;
        ForeColor = p.Text;
        if (S.IsRtl) { RightToLeft = RightToLeft.Yes; RightToLeftLayout = true; }

        var logo = new PictureBox { Image = AppInfo.Logo(64), Size = new Size(64, 64), Location = new Point(24, 24) };
        var name = new Label { Text = AppInfo.Name, Font = new Font("Segoe UI Semibold", 16f), AutoSize = true, Location = new Point(104, 24) };
        var version = new Label { Text = $"v{AppInfo.Version}  ·  MIT License", ForeColor = p.Muted, AutoSize = true, Location = new Point(106, 62) };

        var glyphs = new Font("Segoe MDL2 Assets", 10f);
        Control[] Link(int y, int glyph, string url) =>
        [
            new Label { Text = ((char)glyph).ToString(), Font = glyphs, ForeColor = p.Muted, AutoSize = true, Location = new Point(106, y + 2) },
            MakeLink(url, new Point(130, y)),
        ];

        // Yapımcı, sol alt köşede.
        var maker = new Label { Text = $"© 2026 {AppInfo.Company}", ForeColor = p.Muted, AutoSize = true, Location = new Point(24, 192) };

        var ok = new Button
        {
            Text = "OK", Size = new Size(88, 30), Location = new Point(ClientSize.Width - 88 - 16, ClientSize.Height - 30 - 14),
            FlatStyle = FlatStyle.Flat, BackColor = p.Surface, ForeColor = p.Text, DialogResult = DialogResult.OK,
        };
        ok.FlatAppearance.BorderColor = p.FieldBorder;
        AcceptButton = CancelButton = ok;

        Controls.AddRange([logo, name, version, .. Link(100, 0xE774, AppInfo.Website), .. Link(126, 0xE943, AppInfo.Repository), maker, ok]);
    }

    private LinkLabel MakeLink(string url, Point at)
    {
        var link = new LinkLabel
        {
            Text = url.Replace("https://", ""), AutoSize = true, Location = at,
            LinkColor = p.Accent, ActiveLinkColor = p.Accent, VisitedLinkColor = p.Accent, LinkBehavior = LinkBehavior.HoverUnderline,
        };
        link.LinkClicked += (_, _) => AppInfo.Open(url);
        return link;
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        Theme.ApplyChrome(this, p.Dark);
    }
}
