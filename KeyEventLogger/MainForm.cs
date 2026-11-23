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
    private bool statsDirty = true;
    private string? currentFilePath;

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

        DateTime today = DateTime.Today;
        dtpFrom.Value = today;
        dtpTo.Value = today.AddDays(1).AddSeconds(-1);

        toolTip.SetToolTip(btnStart, "Klavye olaylarını kaydetmeye başla");
        toolTip.SetToolTip(btnStop, "Kaydı durdur (mevcut kayıtlar silinmez)");
        toolTip.SetToolTip(btnClear, "Ekrandaki ve bellekteki tüm kayıtları sil");
        toolTip.SetToolTip(btnSave, "Kayıtları .txt dosyasına kaydet");
        toolTip.SetToolTip(btnOpen, "Daha önce kaydedilmiş bir .txt dosyasını aç");
        toolTip.SetToolTip(btnApplyFilter, "Sadece seçilen zaman aralığındaki kayıtları göster");

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

        if (m.Msg == WM_SYSCOMMAND && ((int)m.WParam & 0xFFF0) == SC_KEYMENU && m.LParam == IntPtr.Zero)
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
        {
            MessageBox.Show(this, "Kaydedilecek kayıt yok.", "Kaydet", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dialog = new SaveFileDialog
        {
            Title = "Kayıtları kaydet",
            Filter = "Metin dosyası (*.txt)|*.txt",
            FileName = LogFileService.SuggestFileName(DateTime.Now),
            AddExtension = true,
            DefaultExt = "txt"
        };

        if (dialog.ShowDialog(this) == DialogResult.OK && SaveToFile(dialog.FileName, showErrors: true))
        {
            MessageBox.Show(this, $"{session.Entries.Count} kayıt kaydedildi:\n{dialog.FileName}", "Kaydet",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        capturePad.Focus();
    }

    internal bool SaveToFile(string path, bool showErrors)
    {
        try
        {
            LogFileService.Save(path, session.Entries.ToList());
            currentFilePath = path;
            lblFile.Text = "Dosya: " + Path.GetFileName(path);
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
            Filter = "Metin dosyası (*.txt)|*.txt|Tüm dosyalar (*.*)|*.*"
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            LoadResult? result = LoadFromFile(dialog.FileName, showErrors: true);
            if (result != null)
            {
                string message = $"{result.Entries.Count} kayıt yüklendi.";
                if (result.SkippedLines > 0)
                    message += $"\n{result.SkippedLines} satır okunamadığı için atlandı.";
                MessageBox.Show(this, message, "Dosya açıldı", MessageBoxButtons.OK,
                    result.SkippedLines > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
            }
        }

        capturePad.Focus();
    }

    internal LoadResult? LoadFromFile(string path, bool showErrors)
    {
        try
        {
            LoadResult result = LogFileService.Load(path);
            session.Import(result.Entries);
            currentFilePath = path;
            lblFile.Text = "Dosya: " + Path.GetFileName(path);
            return result;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
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
        MessageBox.Show(this,
            "Key Event Logger  ·  YMH219 Nesne Tabanlı Programlama\n\n" +
            "Bu uygulama yalnızca KENDİ penceresi odaktayken klavye olaylarını (KeyDown, KeyUp, KeyPress) gösterir.\n\n" +
            "• Global klavye kancası (hook) kullanılmaz\n" +
            "• Arka planda / gizli çalışmaz, başlangıçta otomatik açılmaz\n" +
            "• Başka uygulamalardan veri toplamaz\n" +
            "• Kayıtları ağ üzerinden hiçbir yere göndermez\n" +
            "• Kayıtlar yalnızca kullanıcı isterse yerel .txt dosyasına yazılır",
            "Hakkında ve Gizlilik", MessageBoxButtons.OK, MessageBoxIcon.Information);
        capturePad.Focus();
    }

    // ===================== Ekran güncelleme =====================

    private void Session_EntryAdded(object? sender, KeyLogEntry entry)
    {
        statsDirty = true;
        if (IsVisible(entry))
        {
            AppendEntry(entry);
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
        if (filterActive && (entry.Timestamp < dtpFrom.Value || entry.Timestamp > dtpTo.Value))
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
        var shown = visible.Skip(Math.Max(0, visible.Count - MaxVisibleLines));

        rtbLog.SuspendLayout();
        rtbLog.Clear();
        foreach (var entry in shown)
            AppendEntry(entry);
        rtbLog.ResumeLayout();
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
            lblEmpty.Text = session.Entries.Count > 0
                ? "Seçili filtreye uyan kayıt yok."
                : "Henüz kayıt yok.\r\n\"Başlat\" butonuna basın ve yukarıdaki alana tıklayıp yazmaya başlayın.";
        }
        lblEmpty.Visible = empty;
        rtbLog.Visible = !empty;
    }

    private IEnumerable<KeyLogEntry> EntriesForStats() =>
        filterActive ? session.Entries.Where(e => e.Timestamp >= dtpFrom.Value && e.Timestamp <= dtpTo.Value) : session.Entries;

    private void UpdateStats()
    {
        if (!statsDirty)
            return;
        statsDirty = false;

        var entries = EntriesForStats().ToList();
        tileTotal.Value = KeyStatistics.TotalKeyPresses(entries).ToString();
        tileKpm.Value = KeyStatistics.KeysPerMinute(entries).ToString("0");
        tileDistinct.Value = KeyStatistics.DistinctKeys(entries).ToString();
        frequencyChart.Items = KeyStatistics.TopKeys(entries, 10);
    }

    private void UpdateCounts()
    {
        lblCount.Text = $"{session.Entries.Count} kayıt";
        if (filterActive)
        {
            int inRange = session.Entries.Count(e => e.Timestamp >= dtpFrom.Value && e.Timestamp <= dtpTo.Value);
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
            lblDuration.Text = $"Kayıt süresi: {elapsed:hh\\:mm\\:ss}";
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
