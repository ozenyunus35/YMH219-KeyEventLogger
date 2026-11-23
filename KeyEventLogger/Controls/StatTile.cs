using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace KeyEventLogger.Controls;

/// <summary>Büyük bir sayı ve altında açıklama gösteren küçük istatistik kutusu.</summary>
public class StatTile : Control
{
    private string value = "0";
    private string caption = "";
    private Color accent = Color.FromArgb(79, 70, 229);

    private readonly Font valueFont = new("Segoe UI Semibold", 17F);
    private readonly Font captionFont = new("Segoe UI", 8.25F);

    public StatTile()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        SetStyle(ControlStyles.Selectable, false);
        TabStop = false;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Value
    {
        get => value;
        set { this.value = value; Invalidate(); }
    }

    [Category("Appearance")]
    [DefaultValue("")]
    public string Caption
    {
        get => caption;
        set { caption = value; Invalidate(); }
    }

    [Category("Appearance")]
    public Color Accent
    {
        get => accent;
        set { accent = value; Invalidate(); }
    }

    private bool ShouldSerializeAccent() => accent != Color.FromArgb(79, 70, 229);
    private void ResetAccent() => Accent = Color.FromArgb(79, 70, 229);

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Parent?.BackColor ?? Color.White);

        var r = new Rectangle(0, 0, Width - 1, Height - 1);
        using (var path = new GraphicsPath())
        {
            int d = 14;
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            using var fill = new SolidBrush(Color.FromArgb(18, accent));
            g.FillPath(fill, path);
        }

        TextRenderer.DrawText(g, value, valueFont, new Rectangle(0, 6, Width, Height / 2 + 6), accent,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        TextRenderer.DrawText(g, caption, captionFont, new Rectangle(0, Height / 2 + 6, Width, Height / 2 - 10),
            Color.FromArgb(100, 116, 139), TextFormatFlags.HorizontalCenter | TextFormatFlags.Top);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            valueFont.Dispose();
            captionFont.Dispose();
        }
        base.Dispose(disposing);
    }
}
