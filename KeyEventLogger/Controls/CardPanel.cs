using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace KeyEventLogger.Controls;

/// <summary>
/// Beyaz, köşeleri yuvarlak, başlıklı kart. İçindeki kontroller Padding sayesinde
/// başlığın altından başlıyor.
/// </summary>
public class CardPanel : Panel
{
    private string title = "";
    private readonly Font titleFont = new("Segoe UI Semibold", 10.5F);

    public CardPanel()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        BackColor = Color.White;
        Padding = new Padding(16, 44, 16, 14);
    }

    [Category("Appearance")]
    [DefaultValue("")]
    public string Title
    {
        get => title;
        set { title = value; Invalidate(); }
    }

    [Category("Appearance")]
    [DefaultValue("")]
    public string Subtitle { get; set; } = "";

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Parent?.BackColor ?? SystemColors.Control);

        var r = new Rectangle(0, 0, Width - 1, Height - 1);
        int d = 20;
        using (var path = new GraphicsPath())
        {
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            using var fill = new SolidBrush(BackColor);
            using var border = new Pen(Color.FromArgb(226, 232, 240));
            g.FillPath(fill, path);
            g.DrawPath(border, path);
        }

        if (!string.IsNullOrEmpty(title))
        {
            TextRenderer.DrawText(g, title, titleFont, new Point(14, 14), Color.FromArgb(30, 41, 59));

            if (!string.IsNullOrEmpty(Subtitle))
            {
                Size titleSize = TextRenderer.MeasureText(title, titleFont);
                TextRenderer.DrawText(g, Subtitle, Font, new Point(18 + titleSize.Width, 16), Color.FromArgb(100, 116, 139));
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            titleFont.Dispose();
        base.Dispose(disposing);
    }
}
