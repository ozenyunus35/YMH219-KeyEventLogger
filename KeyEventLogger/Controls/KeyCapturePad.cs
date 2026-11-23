using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace KeyEventLogger.Controls;

/// <summary>
/// Tuşların yakalandığı alan. Odak buradayken ok tuşları, Tab ve Enter dahil
/// her tuş KeyDown olarak geliyor (normalde form bunları gezinme için yutuyor).
/// Ortada en son basılan tuşu büyük harflerle gösteriyor.
/// </summary>
public class KeyCapturePad : Control
{
    private static readonly Color Accent = Color.FromArgb(79, 70, 229);
    private static readonly Color BorderIdle = Color.FromArgb(203, 213, 225);

    private string lastKeyText = "";
    private bool recording;

    private readonly Font bigFont = new("Segoe UI Semibold", 28F);
    private readonly Font hintFont = new("Segoe UI", 10F);

    public KeyCapturePad()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                 ControlStyles.Selectable, true);
        TabStop = true;
        BackColor = Color.White;
        Cursor = Cursors.IBeam;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string LastKeyText
    {
        get => lastKeyText;
        set { lastKeyText = value; Invalidate(); }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Recording
    {
        get => recording;
        set { recording = value; Invalidate(); }
    }

    protected override bool IsInputKey(Keys keyData) => true;

    protected override void OnMouseDown(MouseEventArgs e)
    {
        Focus();
        base.OnMouseDown(e);
    }

    protected override void OnGotFocus(EventArgs e) { base.OnGotFocus(e); Invalidate(); }
    protected override void OnLostFocus(EventArgs e) { base.OnLostFocus(e); Invalidate(); }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Parent?.BackColor ?? SystemColors.Control);

        var box = new Rectangle(1, 1, Width - 3, Height - 3);
        // Mor kesintisiz çerçeve sadece gerçekten dinlerken; kayıt kapalıyken "aktif" gibi görünmesin
        bool listening = recording && Focused;
        using (GraphicsPath path = RoundedRect(box, 12))
        using (var fill = new SolidBrush(BackColor))
        using (var pen = new Pen(listening ? Accent : BorderIdle, listening ? 2f : 1.5f))
        {
            if (!listening)
                pen.DashStyle = DashStyle.Dash;
            g.FillPath(fill, path);
            g.DrawPath(pen, path);
        }

        string hint;
        if (!recording)
            hint = "Kayıt kapalı - başlamak için \"Başlat\" butonuna basın";
        else if (!Focused)
            hint = "Buraya tıklayın ve yazmaya başlayın";
        else
            hint = "Dinleniyor... (yalnızca bu pencere odaktayken)";

        var center = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        string big = string.IsNullOrEmpty(lastKeyText) ? "—" : lastKeyText;

        using (var bigBrush = new SolidBrush(recording ? Color.FromArgb(30, 41, 59) : Color.FromArgb(148, 163, 184)))
            g.DrawString(big, bigFont, bigBrush, new RectangleF(0, 0, Width, Height - 26), center);

        using (var hintBrush = new SolidBrush(Color.FromArgb(100, 116, 139)))
            g.DrawString(hint, hintFont, hintBrush, new RectangleF(0, Height - 38, Width, 26), center);
    }

    private static GraphicsPath RoundedRect(Rectangle r, int radius)
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

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            bigFont.Dispose();
            hintFont.Dispose();
        }
        base.Dispose(disposing);
    }
}
