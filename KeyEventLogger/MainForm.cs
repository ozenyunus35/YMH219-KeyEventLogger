using System.Drawing.Drawing2D;
using KeyEventLogger.Controls;
using KeyEventLogger.Helpers;
using KeyEventLogger.Models;
using KeyEventLogger.Services;

namespace KeyEventLogger;

public partial class MainForm : Form
{
    private static readonly Color TimeColor = Color.FromArgb(148, 163, 184);
    private static readonly Color KeyDownColor = Color.FromArgb(79, 70, 229);
    private static readonly Color KeyUpColor = Color.FromArgb(100, 116, 139);
    private static readonly Color KeyPressColor = Color.FromArgb(13, 148, 136);
    private static readonly Color TextColor = Color.FromArgb(30, 41, 59);
    private static readonly Color RepeatColor = Color.FromArgb(160, 170, 185);

    // Çok uzun kayıtlarda RichTextBox yavaşlamasın diye ekranda en fazla bu kadar satır
    private const int MaxVisibleLines = 3000;

    private readonly KeyLogSession session = new();
    private bool windowFocused = true;
    private bool filterActive;
    // Filtre "Uygula"ya basıldığı andaki aralık; tarih kutuları değiştirilip uygulanmadıysa liste ve sayılar değişmesin
    private DateTime filterFrom, filterTo;
    private int visibleLineCount;
    private bool statsDirty = true;
    private string? currentFilePath;
    // Son kaydetmeden/açmadan sonra yeni kayıt geldi mi? (kapatırken uyarmak için)
    private bool dirty;
    private string lastFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

    public MainForm()
    {
        InitializeComponent();

        try
        {
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        }
        catch (Exception)
        {
            // ikon bulunamazsa varsayılan ikon kalsın
        }

        session.EntryAdded += Session_EntryAdded;
        session.RecordingStateChanged += (_, _) => UpdateRecordingUi();
        session.Cleared += (_, _) => RebuildLog();
        session.Imported += (_, _) => RebuildLog();
        session.EntryAdded += (_, _) => MarkDirty(true);
        session.Cleared += (_, _) => MarkDirty(false);

        // Tarih kutusundan sonra yazılan tuşlar tarihi değiştirmesin: seçim bitince odak kayıt alanına dönüyor
        dtpFrom.CloseUp += (_, _) => BeginInvoke(() => capturePad.Focus());
        dtpTo.CloseUp += (_, _) => BeginInvoke(() => capturePad.Focus());
        dtpFrom.KeyDown += DatePicker_KeyDown;
        dtpTo.KeyDown += DatePicker_KeyDown;
        FormClosing += MainForm_FormClosing;
        statusStrip.Paint += (_, e) =>
        {
            using var line = new Pen(Color.FromArgb(226, 232, 240));
            e.Graphics.DrawLine(line, 0, 0, statusStrip.Width, 0);
        };
        Load += (_, _) => FitToScreen();

        DateTime today = DateTime.Today;
        dtpFrom.Value = today;
        dtpTo.Value = today.AddDays(1).AddSeconds(-1);

        toolTip.SetToolTip(btnStart, "Klavye olaylarını kaydetmeye başla (F9)");
        toolTip.SetToolTip(btnStop, "Kaydı durdur, mevcut kayıtlar silinmez (F9)");
        toolTip.SetToolTip(btnClear, "Ekrandaki ve bellekteki tüm kayıtları sil");
        toolTip.SetToolTip(btnSave, "Kayıtları .txt dosyasına kaydet (Ctrl+S)");
        toolTip.SetToolTip(btnOpen, "Daha önce kaydedilmiş bir .txt dosyasını aç (Ctrl+O)");
        toolTip.SetToolTip(btnApplyFilter, "Sadece seçilen zaman aralığındaki kayıtları göster (tarih kutusunda Enter)");
        toolTip.SetToolTip(btnClearFilter, "Zaman filtresini kaldır, tüm kayıtları göster");
        toolTip.SetToolTip(btnAbout, "Uygulama ve gizlilik bilgisi (F1)");

        UpdateRecordingUi();
        RebuildLog();
        uiTimer.Start();
    }

    // ===================== Klavye olayları =====================
    // KeyPreview = true olduğu için hangi kontrol odakta olursa olsun önce form görüyor.

    private void MainForm_KeyDown(object? sender, KeyEventArgs e)
    {
        KeyLogEntry? entry = session.RecordKeyDown(e.KeyCode, e.Modifiers);
        if (entry != null && !entry.IsRepeat)
            capturePad.LastKeyText = KeyNameFormatter.FormatCombination(e.KeyCode, e.Modifiers);

        // Kısayollar: tuş yine de kayda düşüyor, sonra ilgili işlem yapılıyor
        if (entry is { IsRepeat: true })
            return;
        switch (e.KeyData)
        {
            case Keys.F9:
                (session.IsRecording ? btnStop : btnStart).PerformClick();
                break;
            case Keys.Control | Keys.S:
                BeginInvoke(() => btnSave.PerformClick());
                break;
            case Keys.Control | Keys.O:
                BeginInvoke(() => btnOpen.PerformClick());
                break;
            case Keys.F1:
                BeginInvoke(() => btnAbout.PerformClick());
                break;
        }
    }

    private void DatePicker_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true;
            btnApplyFilter.PerformClick();
        }
    }

    private void MarkDirty(bool value)
    {
        if (dirty == value)
            return;
        dirty = value;
        UpdateFileLabel();
    }

    private void UpdateFileLabel()
    {
        string name = currentFilePath is null ? "kaydedilmedi" : Path.GetFileName(currentFilePath);
        lblFile.Text = "Dosya: " + name + (dirty && currentFilePath != null ? "  (kaydedilmemiş değişiklik var)" : "");
        lblFile.ToolTipText = currentFilePath ?? "Kayıtlar henüz bir dosyaya kaydedilmedi";
        lblFile.ForeColor = dirty && session.Entries.Count > 0 ? Color.FromArgb(180, 83, 9) : Color.FromArgb(100, 116, 139);
    }

    /// <summary>Yüksek DPI'da (ör. %150) pencere ekrandan taşmasın.</summary>
    private void FitToScreen()
    {
        Rectangle area = Screen.FromPoint(Cursor.Position).WorkingArea;
        MinimumSize = new Size(Math.Min(MinimumSize.Width, area.Width), Math.Min(MinimumSize.Height, area.Height));
        if (Width > area.Width || Height > area.Height)
        {
            Size = new Size(Math.Min(Width, area.Width), Math.Min(Height, area.Height));
            StartPosition = FormStartPosition.Manual;
            Location = new Point(area.Left + (area.Width - Width) / 2, area.Top + (area.Height - Height) / 2);
        }
    }

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (e.CloseReason != CloseReason.UserClosing || !dirty || session.Entries.Count == 0)
            return;
        var answer = MessageBox.Show(this,
            $"{session.Entries.Count} kayıt henüz bir dosyaya kaydedilmedi.\nKaydetmeden çıkmak istiyor musunuz?",
            "Çıkış", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
        if (answer != DialogResult.Yes)
        {
            e.Cancel = true;
            capturePad.Focus();
        }
    }

    private void MainForm_KeyUp(object? sender, KeyEventArgs e)
    {
        session.RecordKeyUp(e.KeyCode, e.Modifiers);
    }

    private void MainForm_KeyPress(object? sender, KeyPressEventArgs e)
    {
        session.RecordKeyPress(e.KeyChar);
    }

    // Tek başına Alt veya F10'a basınca Windows pencere menüsüne geçip
    // sonraki tuşları yutuyor. Bunu engelliyoruz ki her tuş kayda düşsün.
    protected override void WndProc(ref Message m)
    {
        const int WM_SYSCOMMAND = 0x0112;
        const int SC_KEYMENU = 0xF100;

        if (m.Msg == WM_SYSCOMMAND && ((long)m.WParam & 0xFFF0) == SC_KEYMENU && m.LParam == IntPtr.Zero)
            return;

        base.WndProc(ref m);
    }

    // ===================== Odak =====================

    private void MainForm_Shown(object? sender, EventArgs e)
    {
        capturePad.Focus();
    }

    private void MainForm_Activated(object? sender, EventArgs e)
    {
        windowFocused = true;
        UpdateRecordingUi();
    }

    private void MainForm_Deactivate(object? sender, EventArgs e)
    {
        // Pencere arka plana geçti: artık hiçbir tuş gelmeyecek (global hook yok)
        windowFocused = false;
        session.ResetPressedKeys();
        UpdateRecordingUi();
    }

    // ===================== Butonlar =====================

    private void btnStart_Click(object? sender, EventArgs e)
    {
        session.Start();
        capturePad.Focus();
    }

    private void btnStop_Click(object? sender, EventArgs e)
    {
        session.Stop();
        capturePad.Focus();
    }

    private void btnClear_Click(object? sender, EventArgs e)
    {
        ClearLog(askFirst: true);
        capturePad.Focus();
    }

    internal void ClearLog(bool askFirst)
    {
        if (session.Entries.Count == 0)
            return;

        if (askFirst)
        {
            var answer = MessageBox.Show(this,
                $"{session.Entries.Count} kayıt silinecek. Emin misiniz?\n(Dosyaya kaydedilmemiş kayıtlar kaybolur.)",
                "Kayıtları temizle", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (answer != DialogResult.Yes)
                return;
        }

        session.Clear();
        capturePad.LastKeyText = "";
    }

    private void btnSave_Click(object? sender, EventArgs e)
    {
        if (session.Entries.Count == 0)
            return;

        using var dialog = new SaveFileDialog
        {
            Title = "Kayıtları kaydet",
            Filter = "Metin dosyası (*.txt)|*.txt",
            FileName = LogFileService.SuggestFileName(DateTime.Now),
            InitialDirectory = lastFolder,
            RestoreDirectory = true,
            OverwritePrompt = true,
            AddExtension = true,
            DefaultExt = "txt"
        };

        if (dialog.ShowDialog(this) == DialogResult.OK && SaveToFile(dialog.FileName, showErrors: true))
        {
            // Başarı için ayrı pencere açmak yerine durum çubuğunda gösteriliyor
            lblFile.Text = $"✓ {session.Entries.Count} kayıt kaydedildi: {Path.GetFileName(dialog.FileName)}";
            lblFile.ForeColor = Color.FromArgb(22, 163, 74);
        }

        capturePad.Focus();
    }

    internal bool SaveToFile(string path, bool showErrors)
    {
        try
        {
            LogFileService.Save(path, session.Entries.ToList());
            currentFilePath = path;
            lastFolder = Path.GetDirectoryName(path) ?? lastFolder;
            dirty = false;
            UpdateFileLabel();
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
        {
            if (showErrors)
                MessageBox.Show(this, "Dosya kaydedilemedi:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            lblFile.Text = "Dosya: kaydedilemedi!";
            return false;
        }
    }

    private void btnOpen_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Kayıt dosyası aç",
            Filter = "Metin dosyası (*.txt)|*.txt|Tüm dosyalar (*.*)|*.*",
            InitialDirectory = lastFolder,
            RestoreDirectory = true
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            bool replace = false;
            if (session.Entries.Count > 0)
            {
                var replaceButton = new TaskDialogCommandLinkButton("Mevcut kayıtları sil ve dosyayı aç",
                    dirty ? "Kaydedilmemiş kayıtlar kaybolur." : null);
                var appendButton = new TaskDialogCommandLinkButton("Dosyadaki kayıtları mevcutlara ekle",
                    "Kayıtlar zaman sırasına göre birleştirilir.");
                var page = new TaskDialogPage
                {
                    Caption = "Dosya aç",
                    Heading = $"Ekranda {session.Entries.Count} kayıt var. Dosya nasıl açılsın?",
                    Icon = TaskDialogIcon.Information,
                    Buttons = { replaceButton, appendButton, TaskDialogButton.Cancel },
                    DefaultButton = appendButton
                };
                var choice = TaskDialog.ShowDialog(this, page);
                if (choice != replaceButton && choice != appendButton)
                {
                    capturePad.Focus();
                    return;
                }
                replace = choice == replaceButton;
            }

            LoadResult? result = LoadFromFile(dialog.FileName, showErrors: true, replace);
            if (result != null)
            {
                if (result.SkippedLines > 0)
                    MessageBox.Show(this, $"{result.Entries.Count} kayıt yüklendi.\n{result.SkippedLines} satır okunamadığı için atlandı.",
                        "Dosya açıldı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else if (result.Entries.Count == 0)
                    MessageBox.Show(this, "Dosyada kayıt bulunamadı.", "Dosya açıldı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        capturePad.Focus();
    }

    internal LoadResult? LoadFromFile(string path, bool showErrors, bool replace = false)
    {
        try
        {
            LoadResult result = LogFileService.Load(path);
            bool hadOtherEntries = !replace && session.Entries.Count > 0;
            session.Import(result.Entries, replace);
            currentFilePath = path;
            lastFolder = Path.GetDirectoryName(path) ?? lastFolder;
            // Başka kayıtlarla birleştirildiyse ekrandaki liste dosyayla aynı değil
            dirty = hadOtherEntries;
            UpdateFileLabel();
            return result;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException or OutOfMemoryException)
        {
            if (showErrors)
                MessageBox.Show(this, "Dosya açılamadı:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return null;
        }
    }

    private void displayOption_CheckedChanged(object? sender, EventArgs e)
    {
        RebuildLog();
        capturePad.Focus();
    }

    private void btnApplyFilter_Click(object? sender, EventArgs e)
    {
        ApplyFilter(dtpFrom.Value, dtpTo.Value, showErrors: true);
        capturePad.Focus();
    }

    internal bool ApplyFilter(DateTime from, DateTime to, bool showErrors)
    {
        if (to < from)
        {
            if (showErrors)
                MessageBox.Show(this, "Bitiş zamanı başlangıç zamanından önce olamaz.", "Filtre",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        dtpFrom.Value = from;
        dtpTo.Value = to;
        filterFrom = from;
        // Saat kutusu saniye gösteriyor: "23:59:59" o saniyenin tamamını kapsasın (23:59:59.500 de dahil)
        filterTo = to.AddSeconds(1);
        filterActive = true;
        RebuildLog();
        return true;
    }

    private void btnClearFilter_Click(object? sender, EventArgs e)
    {
        filterActive = false;
        RebuildLog();
        capturePad.Focus();
    }

    private void btnAbout_Click(object? sender, EventArgs e)
    {
        using (var about = new AboutForm())
            about.ShowDialog(this);
        capturePad.Focus();
    }

    // ===================== Ekran güncelleme =====================

    private void Session_EntryAdded(object? sender, KeyLogEntry entry)
    {
        statsDirty = true;
        if (IsVisible(entry))
        {
            // Uzun kayıtta kutu sınırsız büyümesin: sınır biraz aşılınca son satırlarla yeniden çiziliyor
            if (visibleLineCount >= MaxVisibleLines + 500)
            {
                RebuildLog();
                return;
            }
            AppendEntry(entry);
            visibleLineCount++;
            rtbLog.ScrollToCaret();
            ShowEmptyState(false);
        }
        UpdateCounts();
    }

    private bool IsVisible(KeyLogEntry entry)
    {
        if (entry.Kind == KeyEventKind.KeyUp && !chkShowKeyUp.Checked)
            return false;
        if (entry.Kind == KeyEventKind.KeyPress && !chkShowKeyPress.Checked)
            return false;
        if (filterActive && !InFilter(entry))
            return false;
        return true;
    }

    private void AppendEntry(KeyLogEntry entry)
    {
        (string arrow, string kind, Color color) = entry.Kind switch
        {
            KeyEventKind.KeyDown => ("↓", "KeyDown ", KeyDownColor),
            KeyEventKind.KeyUp => ("↑", "KeyUp   ", KeyUpColor),
            _ => ("•", "KeyPress", KeyPressColor)
        };

        AppendColored(entry.Timestamp.ToString("HH:mm:ss.fff") + "   ", TimeColor);
        AppendColored($"{arrow} {kind}   ", color);
        if (entry.IsRepeat)
            AppendColored(entry.DisplayText + "  (tekrar)", RepeatColor);
        else
            AppendColored(entry.DisplayText, TextColor);
        AppendColored(Environment.NewLine, TextColor);
    }

    private void AppendColored(string text, Color color)
    {
        rtbLog.SelectionStart = rtbLog.TextLength;
        rtbLog.SelectionLength = 0;
        rtbLog.SelectionColor = color;
        rtbLog.AppendText(text);
    }

    private void RebuildLog()
    {
        var visible = session.Entries.Where(IsVisible).ToList();
        var shown = visible.Skip(Math.Max(0, visible.Count - MaxVisibleLines)).ToList();

        // SuspendLayout RichTextBox'ın her satırda yeniden çizilmesini durdurmuyor; WM_SETREDRAW durduruyor
        SetRedraw(rtbLog, false);
        try
        {
            rtbLog.Clear();
            foreach (var entry in shown)
                AppendEntry(entry);
        }
        finally
        {
            SetRedraw(rtbLog, true);
            rtbLog.Invalidate();
        }
        visibleLineCount = shown.Count;
        rtbLog.ScrollToCaret();

        ShowEmptyState(visible.Count == 0);
        statsDirty = true;
        UpdateCounts();
        UpdateStats();
    }

    private void ShowEmptyState(bool empty)
    {
        if (empty)
        {
            lblEmpty.Text = session.Entries.Count == 0
                ? "Henüz kayıt yok.\r\n\"Başlat\" butonuna (F9) basın ve yukarıdaki alana tıklayıp yazmaya başlayın."
                : filterActive
                    ? "Seçili filtreye uyan kayıt yok."
                    : "Bu görünüm ayarlarında gösterilecek kayıt yok.\r\n(KeyUp / KeyPress kutularını işaretleyin.)";
        }
        lblEmpty.Visible = empty;
        rtbLog.Visible = !empty;
    }

    private bool InFilter(KeyLogEntry e) => e.Timestamp >= filterFrom && e.Timestamp < filterTo;

    private IEnumerable<KeyLogEntry> EntriesForStats() =>
        filterActive ? session.Entries.Where(InFilter) : session.Entries;

    private const int WM_SETREDRAW = 0x000B;

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    private static void SetRedraw(Control control, bool on)
    {
        if (control.IsHandleCreated)
            SendMessage(control.Handle, WM_SETREDRAW, on ? 1 : 0, IntPtr.Zero);
    }

    private void UpdateStats()
    {
        if (!statsDirty)
            return;
        statsDirty = false;

        var entries = EntriesForStats().ToList();
        tileTotal.Value = KeyStatistics.TotalKeyPresses(entries).ToString("N0");
        tileKpm.Value = KeyStatistics.KeysPerMinute(entries).ToString("0");
        tileDistinct.Value = KeyStatistics.DistinctKeys(entries).ToString();
        frequencyChart.Items = KeyStatistics.TopKeys(entries, 10);
    }

    private void UpdateCounts()
    {
        lblCount.Text = $"{session.Entries.Count:N0} kayıt";
        btnSave.Enabled = btnClear.Enabled = session.Entries.Count > 0;
        btnClearFilter.Enabled = filterActive;
        if (filterActive)
        {
            int inRange = session.Entries.Count(InFilter);
            lblFilter.Text = $"Filtre: {inRange} / {session.Entries.Count}";
            lblFilter.ForeColor = KeyDownColor;
        }
        else
        {
            lblFilter.Text = "Filtre kapalı";
            lblFilter.ForeColor = SystemColors.ControlText;
        }
    }

    private void UpdateRecordingUi()
    {
        bool recording = session.IsRecording;

        btnStart.Enabled = !recording;
        btnStop.Enabled = recording;
        capturePad.Recording = recording;

        recordingBadge.State = !recording ? BadgeState.Stopped
            : windowFocused ? BadgeState.Recording
            : BadgeState.NoFocus;

        Text = recording ? "● Kayıt açık - Key Event Logger" : "Key Event Logger";

        if (windowFocused)
        {
            lblFocus.Text = "● Pencere odakta";
            lblFocus.ForeColor = Color.FromArgb(22, 163, 74);
        }
        else
        {
            lblFocus.Text = "● Pencere odakta değil - olay yakalanmıyor";
            lblFocus.ForeColor = Color.FromArgb(180, 83, 9);
        }

        UpdateDuration();
    }

    private void UpdateDuration()
    {
        if (session.IsRecording && session.RecordingStartedAt is DateTime start)
        {
            TimeSpan elapsed = DateTime.Now - start;
            lblDuration.Text = $"Kayıt süresi: {(int)elapsed.TotalHours:00}:{elapsed.Minutes:00}:{elapsed.Seconds:00}";
        }
        else
        {
            lblDuration.Text = "Kayıt durduruldu";
        }
    }

    private void uiTimer_Tick(object? sender, EventArgs e)
    {
        recordingBadge.Blink();
        UpdateDuration();
        UpdateStats();
    }

    private void headerPanel_Paint(object? sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Logo: mor kare içinde bir klavye tuşu
        var logo = new Rectangle(20, 16, 40, 40);
        using (var path = Rounded(logo, 10))
        using (var brush = new LinearGradientBrush(logo, Color.FromArgb(99, 102, 241), Color.FromArgb(168, 85, 247), 45f))
            g.FillPath(brush, path);

        var key = new Rectangle(29, 24, 22, 20);
        using (var path = Rounded(key, 4))
        using (var fill = new SolidBrush(Color.White))
        using (var shadow = new Pen(Color.FromArgb(120, 255, 255, 255), 2))
        {
            g.FillPath(fill, path);
            g.DrawLine(shadow, key.X + 3, key.Bottom + 3, key.Right - 3, key.Bottom + 3);
        }
        using (var font = new Font("Segoe UI", 9F, FontStyle.Bold))
            TextRenderer.DrawText(g, "K", font, key, Color.FromArgb(79, 70, 229),
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

        using var line = new Pen(Color.FromArgb(226, 232, 240));
        g.DrawLine(line, 0, headerPanel.Height - 1, headerPanel.Width, headerPanel.Height - 1);
    }

    private static GraphicsPath Rounded(Rectangle r, int radius)
    {
        int d = radius * 2;
        var path = new GraphicsPath();
        path.AddArc(r.X, r.Y, d, d, 180, 90);
        path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }
}
