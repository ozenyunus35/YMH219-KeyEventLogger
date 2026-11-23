using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace KeyEventLogger.Controls;

public enum ButtonVariant
{
    Primary,
    Danger,
    Secondary,
    Ghost
}

/// <summary>
/// Köşeleri yuvarlatılmış, üzerine gelince rengi değişen buton.
/// Standart Button'dan türetildi, böylece Click, Enabled vs. aynı şekilde çalışıyor.
/// </summary>
public class FlatButton : Button
{
    private ButtonVariant variant = ButtonVariant.Secondary;
    private bool hover;
    private bool pressed;

    public FlatButton()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        Cursor = Cursors.Hand;
        Font = new Font("Segoe UI Semibold", 9.5F);
        TabStop = false;
    }

    [Category("Appearance")]
    [DefaultValue(ButtonVariant.Secondary)]
    public ButtonVariant Variant
    {
        get => variant;
        set { variant = value; Invalidate(); }
    }

    protected override void OnMouseEnter(EventArgs e) { hover = true; Invalidate(); base.OnMouseEnter(e); }
    protected override void OnMouseLeave(EventArgs e) { hover = false; pressed = false; Invalidate(); base.OnMouseLeave(e); }
    protected override void OnMouseDown(MouseEventArgs e) { pressed = true; Invalidate(); base.OnMouseDown(e); }
    protected override void OnMouseUp(MouseEventArgs e) { pressed = false; Invalidate(); base.OnMouseUp(e); }
    protected override void OnEnabledChanged(EventArgs e) { Invalidate(); base.OnEnabledChanged(e); }

    private (Color back, Color fore, Color border) Colors()
    {
        if (!Enabled)
            return (Color.FromArgb(241, 245, 249), Color.FromArgb(148, 163, 184), Color.FromArgb(226, 232, 240));

        Color back, fore, border;
        switch (variant)
        {
            case ButtonVariant.Primary:
                back = Color.FromArgb(22, 163, 74); fore = Color.White; border = back;
                break;
            case ButtonVariant.Danger:
                back = Color.FromArgb(225, 29, 72); fore = Color.White; border = back;
                break;
            case ButtonVariant.Ghost:
                back = Color.Transparent; fore = Color.FromArgb(79, 70, 229); border = Color.Transparent;
                break;
            default:
                back = Color.White; fore = Color.FromArgb(30, 41, 59); border = Color.FromArgb(203, 213, 225);
                break;
        }

        if (pressed) back = Shade(back, variant is ButtonVariant.Secondary or ButtonVariant.Ghost ? 0.90 : 0.80);
        else if (hover) back = Shade(back, variant is ButtonVariant.Secondary or ButtonVariant.Ghost ? 0.96 : 0.90);

        return (back, fore, border);
    }

    private static Color Shade(Color c, double factor)
    {
        if (c == Color.Transparent)
            return Color.FromArgb(25, 79, 70, 229);
        return Color.FromArgb(c.A, (int)(c.R * factor), (int)(c.G * factor), (int)(c.B * factor));
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Parent?.BackColor ?? SystemColors.Control);

        var (back, fore, border) = Colors();
        var r = new Rectangle(0, 0, Width - 1, Height - 1);

        using (GraphicsPath path = Round(r, 7))
        {
            if (back != Color.Transparent)
                using (var b = new SolidBrush(back)) g.FillPath(b, path);
            if (border != Color.Transparent)
                using (var p = new Pen(border)) g.DrawPath(p, path);
        }

        TextRenderer.DrawText(g, Text, Font, r, fore,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
    }

    private static GraphicsPath Round(Rectangle r, int radius)
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
