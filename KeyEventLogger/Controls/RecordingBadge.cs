using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace KeyEventLogger.Controls;

public enum BadgeState
{
    Stopped,
    Recording,
    NoFocus     // kayıt açık ama pencere odakta değil -> olay gelmiyor
}

/// <summary>
/// Sağ üstteki "KAYIT AÇIK / KAPALI" göstergesi. Kayıttayken nokta yanıp sönüyor.
/// </summary>
public class RecordingBadge : Control
{
    private BadgeState state = BadgeState.Stopped;
    private bool blinkOn = true;
    private readonly Font font = new("Segoe UI Semibold", 9.5F);

    public RecordingBadge()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                 ControlStyles.SupportsTransparentBackColor, true);
        SetStyle(ControlStyles.Selectable, false);
        TabStop = false;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public BadgeState State
    {
        get => state;
        set { state = value; blinkOn = true; Invalidate(); }
    }

    public string StateText => state switch
    {
        BadgeState.Recording => "KAYIT AÇIK",
        BadgeState.NoFocus => "ODAK DIŞI",
        _ => "KAYIT KAPALI"
    };

    /// <summary>Formdaki timer tarafından çağrılıyor.</summary>
    public void Blink()
    {
        if (state == BadgeState.Stopped)
            return;
        blinkOn = !blinkOn;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Parent?.BackColor ?? Color.White);

        (Color back, Color fore) = state switch
        {
            BadgeState.Recording => (Color.FromArgb(255, 228, 230), Color.FromArgb(225, 29, 72)),
            BadgeState.NoFocus => (Color.FromArgb(254, 243, 199), Color.FromArgb(180, 83, 9)),
            _ => (Color.FromArgb(241, 245, 249), Color.FromArgb(100, 116, 139))
        };

        var r = new Rectangle(0, 0, Width - 1, Height - 1);
        int d = r.Height;
        using (var path = new GraphicsPath())
        using (var brush = new SolidBrush(back))
        using (var pen = new Pen(Color.FromArgb(60, fore)))
        {
            path.AddArc(r.X, r.Y, d, d, 90, 180);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 180);
            path.CloseFigure();
            g.FillPath(brush, path);
            g.DrawPath(pen, path);
        }

        int dot = 10;
        var dotRect = new Rectangle(14, (Height - dot) / 2, dot, dot);
        using (var dotBrush = new SolidBrush(blinkOn ? fore : Color.FromArgb(80, fore)))
        {
            if (state == BadgeState.Stopped)
                g.FillRectangle(dotBrush, dotRect);   // durdurulmuş: kare
            else
                g.FillEllipse(dotBrush, dotRect);     // kayıtta: yuvarlak
        }

        TextRenderer.DrawText(g, StateText, font, new Rectangle(30, 0, Width - 36, Height), fore,
            TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            font.Dispose();
        base.Dispose(disposing);
    }
}
