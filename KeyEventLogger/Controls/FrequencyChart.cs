using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace KeyEventLogger.Controls;

/// <summary>
/// En sık basılan tuşları yatay çubuk grafik olarak çizer.
/// Solda tuş "keycap" şeklinde, sağda kaç kez basıldığı yazıyor.
/// </summary>
public class FrequencyChart : Control
{
    private IReadOnlyList<(string Key, int Count)> items = Array.Empty<(string, int)>();

    private readonly Font keyFont = new("Segoe UI Semibold", 8.5F);
    private readonly Font countFont = new("Segoe UI", 8.5F);
    private readonly Font emptyFont = new("Segoe UI", 9F, FontStyle.Italic);

    public FrequencyChart()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        SetStyle(ControlStyles.Selectable, false);
        TabStop = false;
        BackColor = Color.White;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IReadOnlyList<(string Key, int Count)> Items
    {
        get => items;
        set { items = value ?? Array.Empty<(string, int)>(); Invalidate(); }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(BackColor);

        if (items.Count == 0)
        {
            TextRenderer.DrawText(g, "Henüz veri yok.\nKayıt başlatıp birkaç tuşa basın.", emptyFont,
                ClientRectangle, Color.FromArgb(148, 163, 184),
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);
            return;
        }

        int max = items.Max(i => i.Count);
        const int rowHeight = 26;
        const int keyWidth = 64;
        const int countWidth = 38;
        int barMaxWidth = Width - keyWidth - countWidth - 12;

        for (int i = 0; i < items.Count; i++)
        {
            var (key, count) = items[i];
            int y = i * rowHeight + 2;
            if (y + rowHeight > Height)
                break;

            // keycap
            var cap = new Rectangle(0, y, keyWidth - 8, rowHeight - 6);
            using (var path = Round(cap, 5))
            using (var capFill = new SolidBrush(Color.FromArgb(248, 250, 252)))
            using (var capBorder = new Pen(Color.FromArgb(203, 213, 225)))
            {
                g.FillPath(capFill, path);
                g.DrawPath(capBorder, path);
                using var shadow = new Pen(Color.FromArgb(203, 213, 225), 2);
                g.DrawLine(shadow, cap.X + 4, cap.Bottom, cap.Right - 4, cap.Bottom);
            }
            TextRenderer.DrawText(g, key, keyFont, cap, Color.FromArgb(30, 41, 59),
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // çubuk
            int barWidth = Math.Max(6, (int)(barMaxWidth * (count / (double)max)));
            var bar = new Rectangle(keyWidth, y + 5, barWidth, rowHeight - 16);
            Color start = Color.FromArgb(99, 102, 241);
            Color end = Color.FromArgb(168, 85, 247);
            using (var path = Round(bar, bar.Height / 2))
            using (var fill = new LinearGradientBrush(new Rectangle(bar.X - 1, bar.Y, bar.Width + 2, bar.Height), start, end, 0f))
                g.FillPath(fill, path);

            TextRenderer.DrawText(g, count.ToString(), countFont,
                new Rectangle(keyWidth + barWidth + 6, y, countWidth + 20, rowHeight - 6),
                Color.FromArgb(71, 85, 105), TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
        }
    }

    private static GraphicsPath Round(Rectangle r, int radius)
    {
        int d = Math.Max(1, Math.Min(radius * 2, Math.Min(r.Width, r.Height)));
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
            keyFont.Dispose();
            countFont.Dispose();
            emptyFont.Dispose();
        }
        base.Dispose(disposing);
    }
}
