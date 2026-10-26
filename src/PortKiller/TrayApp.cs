namespace PortKiller;

internal sealed class TrayApp : ApplicationContext
{
    private const int PollMs = 3000;

    private readonly NotifyIcon tray;
    private readonly ContextMenuStrip menu;
    private readonly PortsForm form = new();
    private readonly System.Windows.Forms.Timer timer = new() { Interval = PollMs };
    private HashSet<int>? knownPorts; // null = ilk tarama, bildirim üretme
    private string lastSignature = "";

    public TrayApp()
    {
        menu = new ContextMenuStrip { RightToLeft = S.IsRtl ? RightToLeft.Yes : RightToLeft.No };
        menu.Items.Add(S.Show, null, (_, _) => ShowForm());
        menu.Items.Add(S.Refresh, null, (_, _) => Poll(force: true));
        menu.Items.Add(S.About, null, (_, _) => ShowAbout());
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(S.Exit, null, (_, _) => ExitThread());

        tray = new NotifyIcon { Icon = MakeIcon(), Text = AppInfo.Name, ContextMenuStrip = menu, Visible = true };
        tray.MouseClick += (_, e) => { if (e.Button == MouseButtons.Left) ToggleForm(); };
        tray.BalloonTipClicked += (_, _) => ShowForm();

        form.KillRequested += Kill;
        form.RefreshRequested += () => Poll(force: true);
        form.AboutRequested += ShowAbout;
        ApplyMenuTheme();
        Microsoft.Win32.SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
        timer.Tick += (_, _) => Poll();
        timer.Start();
        Poll();
    }

    private void Poll(bool force = false)
    {
        List<Listener> now;
        try { now = Ports.Snapshot(); }
        catch { return; } // bir sonraki turda tekrar dener

        var ports = now.Select(l => l.Port).ToHashSet();
        if (knownPorts != null)
        {
            var fresh = now.Where(l => !knownPorts.Contains(l.Port)).ToList();
            if (fresh.Count > 0)
                tray.ShowBalloonTip(5000, fresh.Count == 1 ? S.NewPort : $"{S.NewPort} (+{fresh.Count})",
                    string.Join("\n", fresh.Take(4).Select(l => $":{l.Port}  {l.Group} · {l.Name} · PID {l.Pid}")), ToolTipIcon.Info);
        }
        knownPorts = ports;

        tray.Text = $"Port Killer · {now.Count(l => l.IsDev)} / {now.Count}";

        // Liste değişmediyse yeniden çizme; tıklama sırasında satırlar kaymasın.
        var signature = string.Join("|", now.Select(l => $"{l.Port}:{l.Pid}:{l.Address}"));
        if (!force && signature == lastSignature) return;
        lastSignature = signature;
        form.Render(now);
    }

    private void Kill(Listener l)
    {
        var what = $"{l.Name} · PID {l.Pid} · :{l.Port}";
        if (!l.IsDev && MessageBox.Show(form, $"{what}\n\n{S.ConfirmKill}", "Port Killer", MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2, S.MsgOptions) != DialogResult.Yes)
            return;

        try { Ports.Kill(l.Pid); }
        catch (Exception ex)
        {
            MessageBox.Show(form, $"{what}\n\n{S.KillFailed}\n\n{ex.Message}", "Port Killer", MessageBoxButtons.OK,
                MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, S.MsgOptions);
        }
        Poll();
    }

    // Ayarlar'da açık/koyu mod ya da vurgu rengi değişince "General" kategorisiyle gelir.
    private void OnUserPreferenceChanged(object? sender, Microsoft.Win32.UserPreferenceChangedEventArgs e)
    {
        if (e.Category is not (Microsoft.Win32.UserPreferenceCategory.General or Microsoft.Win32.UserPreferenceCategory.Color)) return;
        form.ApplyTheme();
        ApplyMenuTheme();
    }

    private void ApplyMenuTheme()
    {
        var p = Theme.Current();
        menu.Renderer = new ToolStripProfessionalRenderer(new Theme.MenuColors(p));
        menu.BackColor = p.Back;
        menu.ForeColor = p.Text;
    }

    private bool aboutOpen; // tepsiden ikinci kez tıklanınca ikinci pencere açılmasın

    private void ShowAbout()
    {
        if (aboutOpen) return;
        aboutOpen = true;
        using (var about = new AboutForm()) about.ShowDialog(form.Visible ? form : null);
        aboutOpen = false;
    }

    private void ToggleForm()
    {
        if (form.Visible) form.Hide();
        else ShowForm();
    }

    private void ShowForm()
    {
        var area = Screen.FromPoint(Cursor.Position).WorkingArea;
        form.Location = new Point(area.Right - form.Width - 8, area.Bottom - form.Height - 8);
        form.Show();
        form.Activate();
    }

    protected override void ExitThreadCore()
    {
        Microsoft.Win32.SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged; // statik olay; bırakılmazsa sızar
        timer.Dispose();
        tray.Visible = false;
        tray.Dispose();
        form.Dispose();
        base.ExitThreadCore();
    }

    private static Icon MakeIcon()
    {
        using var bmp = AppInfo.Logo(32);
        return Icon.FromHandle(bmp.GetHicon());
    }
}
