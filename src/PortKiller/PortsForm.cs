namespace PortKiller;

/// <summary>Tepsi simgesinin üstünde açılan, gruplu ve aranabilir port tablosu.</summary>
internal sealed class PortsForm : Form
{
    // Windows 10/11 ile gelen ikon fontu; metin yerine evrensel simgeler.
    private const string GlyphFont = "Segoe MDL2 Assets";
    private static readonly string SearchGlyph = Glyph(0xE721), RefreshGlyph = Glyph(0xE72C), KillGlyph = Glyph(0xE711), InfoGlyph = Glyph(0xE946);
    private const int AddressColumn = 3;
    private const int KillColumn = 4;

    private readonly DataGridView grid = new()
    {
        Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
        AllowUserToResizeRows = false, RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        MultiSelect = false, BorderStyle = BorderStyle.None, CellBorderStyle = DataGridViewCellBorderStyle.Single,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, EnableHeadersVisualStyles = false,
        ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing, ColumnHeadersHeight = 30,
    };
    private readonly TextBox search = new() { Dock = DockStyle.Fill, BorderStyle = BorderStyle.None };
    private readonly Panel top, field, fieldInner;
    private readonly Label searchIcon;
    private readonly Button refresh, info;
    private readonly Font bold;
    private Palette p = Theme.Current();
    private IReadOnlyList<Listener> all = [];

    public event Action<Listener>? KillRequested;
    public event Action? RefreshRequested;
    public event Action? AboutRequested;

    public PortsForm()
    {
        Text = "Port Killer";
        Font = new Font("Segoe UI", 9f);
        bold = new Font(Font, FontStyle.Bold);
        // Standart çerçeve: araç penceresinin küçük soluk ✕'i yerine büyük, üzerine gelince kırmızı kapatma düğmesi.
        FormBorderStyle = FormBorderStyle.Sizable;
        MaximizeBox = MinimizeBox = ShowIcon = false;
        ShowInTaskbar = false;
        TopMost = true;
        StartPosition = FormStartPosition.Manual;
        KeyPreview = true;
        Size = new Size(560, 560);
        MinimumSize = new Size(400, 220);
        if (S.IsRtl) { RightToLeft = RightToLeft.Yes; RightToLeftLayout = true; }

        var glyphs = new Font(GlyphFont, 10f);
        var tip = new ToolTip();

        // Üst çubuk: [ 🔍 arama alanı ................ ] ⟳
        search.PlaceholderText = S.Search;
        search.TextChanged += (_, _) => Apply();
        search.GotFocus += (_, _) => UpdateSearchFocus();
        search.LostFocus += (_, _) => UpdateSearchFocus();
        searchIcon = new Label { Text = SearchGlyph, Font = glyphs, Dock = DockStyle.Left, Width = 26, TextAlign = ContentAlignment.MiddleCenter };
        searchIcon.Click += (_, _) => search.Focus();
        fieldInner = new Panel { Dock = DockStyle.Fill, Padding = new Padding(2, 6, 6, 0) };
        fieldInner.Controls.Add(search); // Fill önce eklenir ki kenarlara yerleşenlerden kalan alanı alsın
        fieldInner.Controls.Add(searchIcon);
        fieldInner.Click += (_, _) => search.Focus();
        field = new Panel { Dock = DockStyle.Fill, Padding = new Padding(1) }; // arka plan rengi = kenarlık
        field.Controls.Add(fieldInner);

        refresh = new Button { Text = RefreshGlyph, Font = glyphs, Dock = DockStyle.Right, Width = 34, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        refresh.FlatAppearance.BorderSize = 0;
        refresh.Click += (_, _) => RefreshRequested?.Invoke();
        tip.SetToolTip(refresh, $"{S.Refresh} (F5)");
        info = new Button { Text = InfoGlyph, Font = glyphs, Dock = DockStyle.Right, Width = 34, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        info.FlatAppearance.BorderSize = 0;
        info.Click += (_, _) => AboutRequested?.Invoke();
        tip.SetToolTip(info, S.About);

        top = new Panel { Dock = DockStyle.Top, Height = 46, Padding = new Padding(10, 8, 8, 8) };
        top.Controls.Add(field);
        top.Controls.Add(new Panel { Dock = DockStyle.Right, Width = 6 });
        top.Controls.Add(refresh);
        top.Controls.Add(info);

        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Port", FillWeight = 14, DefaultCellStyle = { Font = bold } });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = S.App, FillWeight = 34 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "PID", FillWeight = 14 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = S.Address, FillWeight = 38 });
        grid.Columns.Add(new DataGridViewButtonColumn
        {
            Text = KillGlyph, UseColumnTextForButtonValue = true, FlatStyle = FlatStyle.Flat, ToolTipText = S.Kill,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None, Width = 38,
            DefaultCellStyle = { Font = glyphs, Alignment = DataGridViewContentAlignment.MiddleCenter },
        });
        foreach (DataGridViewColumn c in grid.Columns) c.SortMode = DataGridViewColumnSortMode.NotSortable;
        grid.RowTemplate.Height = 26;

        grid.CellContentClick += (_, e) =>
        {
            if (e.ColumnIndex == KillColumn && e.RowIndex >= 0 && grid.Rows[e.RowIndex].Tag is Listener l) KillRequested?.Invoke(l);
        };
        grid.CellToolTipTextNeeded += (_, e) =>
        {
            if (e.ColumnIndex == KillColumn && e.RowIndex >= 0 && grid.Rows[e.RowIndex].Tag is Listener l) e.ToolTipText = $"{S.Kill}: {l.Name} · PID {l.Pid} · :{l.Port}";
        };
        grid.CellPainting += PaintCell;
        grid.RowPostPaint += PaintGroupText;
        grid.HandleCreated += (_, _) => ThemeGridChrome();

        Controls.Add(grid);
        Controls.Add(top);

        KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.F5) RefreshRequested?.Invoke();
            else if (e.Control && e.KeyCode == Keys.F) search.Focus();
            else if (e.KeyCode == Keys.Delete && grid.Focused && grid.CurrentRow?.Tag is Listener l) KillRequested?.Invoke(l);
            else if (e.KeyCode == Keys.Escape) { if (search.TextLength > 0) search.Clear(); else Hide(); }
            else return;
            e.Handled = true;
        };

        ApplyTheme();
    }

    /// <summary>Windows açık/koyu tema veya vurgu rengi değişince yeniden çağrılır.</summary>
    public void ApplyTheme()
    {
        p = Theme.Current();
        BackColor = top.BackColor = p.Back;
        foreach (var b in new[] { refresh, info })
        {
            b.BackColor = p.Back;
            b.ForeColor = p.Text;
            b.FlatAppearance.MouseOverBackColor = p.Surface;
            b.FlatAppearance.MouseDownBackColor = p.Line;
        }
        fieldInner.BackColor = search.BackColor = p.Field;
        search.ForeColor = p.Text;
        UpdateSearchFocus();

        grid.BackgroundColor = p.Back;
        grid.GridColor = p.Line;
        var cells = grid.DefaultCellStyle;
        cells.BackColor = p.Back;
        cells.ForeColor = cells.SelectionForeColor = p.Text;
        cells.SelectionBackColor = p.Selection;
        var address = grid.Columns[AddressColumn].DefaultCellStyle;
        address.ForeColor = address.SelectionForeColor = p.Muted;
        var kill = grid.Columns[KillColumn].DefaultCellStyle;
        kill.ForeColor = kill.SelectionForeColor = p.Danger;

        if (IsHandleCreated) { Theme.ApplyChrome(this, p.Dark); ThemeGridChrome(); }
        Invalidate(true);
    }

    // shortcut: DataGridView'in kaydırma çubukları ayrı alt kontroller; SetWindowTheme onları koyulaştırmıyor,
    // koyu temada açık renk kalıyorlar. Gerekirse kendi çizilen bir kaydırma çubuğuyla değiştirilmeli.
    private void ThemeGridChrome() => Theme.ApplyChrome(grid, p.Dark);

    // Arama alanı: odaktayken vurgu renginde kenarlık ve kalın alt çizgi (Windows 11 giriş kutusu gibi).
    private void UpdateSearchFocus()
    {
        bool focused = search.Focused;
        field.BackColor = focused ? p.Accent : p.FieldBorder;
        field.Padding = new Padding(1, 1, 1, focused ? 2 : 1);
        searchIcon.ForeColor = focused ? p.Accent : p.Muted;
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        Theme.ApplyChrome(this, p.Dark);
    }

    protected override void OnVisibleChanged(EventArgs e)
    {
        base.OnVisibleChanged(e);
        if (Visible) ActiveControl = search; // açılınca doğrudan yazmaya başlanabilsin
    }

    public void Render(IReadOnlyList<Listener> list)
    {
        all = list;
        Apply();
    }

    private void Apply()
    {
        var q = search.Text.Trim().TrimStart(':'); // ":3000" de "3000" gibi aransın
        var shown = q.Length == 0 ? all : all.Where(l =>
            $"{l.Port} {l.Pid} {l.Name} {l.Group} {l.Address}".Contains(q, StringComparison.CurrentCultureIgnoreCase)).ToList();

        int scroll = grid.FirstDisplayedScrollingRowIndex;
        grid.Rows.Clear();

        if (shown.Count == 0)
            AddGroupRow(S.Empty, muted: true);

        // Geliştirme araçları önce, her biri kendi grubunda; geri kalan her şey en altta.
        foreach (var group in shown.GroupBy(l => l.Group).OrderBy(g => !g.First().IsDev).ThenBy(g => g.Key))
        {
            AddGroupRow($"{group.Key}  ({group.Count()})", muted: !group.First().IsDev);
            foreach (var l in group)
                grid.Rows[grid.Rows.Add(l.Port, l.Name, l.Pid, l.Address)].Tag = l;
        }

        if (scroll >= 0 && scroll < grid.Rows.Count) grid.FirstDisplayedScrollingRowIndex = scroll;
        grid.ClearSelection();
    }

    private void AddGroupRow(string text, bool muted) => grid.Rows[grid.Rows.Add()].Tag = (text, muted);

    private static string Glyph(int code) => ((char)code).ToString();

    private void PaintCell(object? sender, DataGridViewCellPaintingEventArgs e)
    {
        var g = e.Graphics!;
        var b = e.CellBounds;
        var flags = TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;
        using var line = new Pen(p.Line);

        if (e.RowIndex < 0)
        {
            // Sütun başlıkları: varsayılan siyah 3B kenarlık yerine tema renkleri.
            using (var back = new SolidBrush(p.Surface)) g.FillRectangle(back, b);
            g.DrawLine(line, b.Left, b.Bottom - 1, b.Right, b.Bottom - 1);
            int edge = S.IsRtl ? b.Left : b.Right - 1;
            g.DrawLine(line, edge, b.Top + 6, edge, b.Bottom - 7);
            TextRenderer.DrawText(g, e.Value as string, Font, Rectangle.Inflate(b, -6, 0), p.Muted,
                flags | (S.IsRtl ? TextFormatFlags.Right | TextFormatFlags.RightToLeft : TextFormatFlags.Left));
            e.Handled = true;
        }
        else if (S.IsRtl && e.ColumnIndex == AddressColumn && grid.Rows[e.RowIndex].Tag is Listener)
        {
            // RTL okumada "[::1]" → "[1::]" olarak ters dönüyor; adresi soldan sağa çiz, sağa yasla.
            e.Paint(e.ClipBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground);
            TextRenderer.DrawText(g, e.FormattedValue as string, e.CellStyle!.Font, Rectangle.Inflate(b, -4, 0), p.Muted, flags | TextFormatFlags.Right);
            e.Handled = true;
        }
        else if (grid.Rows[e.RowIndex].Tag is ValueTuple<string, bool>)
        {
            // Grup satırları tek parça bir başlık gibi görünsün: hücre içi ve dikey çizgiler çizilmez.
            using (var back = new SolidBrush(p.Surface)) g.FillRectangle(back, b);
            g.DrawLine(line, b.Left, b.Bottom - 1, b.Right, b.Bottom - 1);
            e.Handled = true;
        }
    }

    private void PaintGroupText(object? sender, DataGridViewRowPostPaintEventArgs e)
    {
        if (grid.Rows[e.RowIndex].Tag is not (string text, bool muted)) return;
        var r = new Rectangle(e.RowBounds.X + 8, e.RowBounds.Y, grid.DisplayRectangle.Width - 16, e.RowBounds.Height);
        var flags = TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | (S.IsRtl ? TextFormatFlags.Right | TextFormatFlags.RightToLeft : TextFormatFlags.Left);
        TextRenderer.DrawText(e.Graphics, text, bold, r, muted ? p.Muted : p.Text, flags);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        // Pencereyi kapatmak uygulamayı değil yalnızca listeyi kapatır; çıkış tepsi menüsünden.
        if (e.CloseReason == CloseReason.UserClosing)
        {
            e.Cancel = true;
            Hide();
        }
        base.OnFormClosing(e);
    }
}
