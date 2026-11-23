using System.Drawing.Drawing2D;
using System.Reflection;

namespace KeyEventLogger;

/// <summary>Uygulama bilgisi ve gizlilik kapsamını gösteren pencere.</summary>
public partial class AboutForm : Form
{
    private static readonly string[] PrivacyItems =
    {
        "Sadece bu pencere odaktayken olay yakalar",
        "Global klavye kancası (hook) kullanmaz",
        "Arka planda / gizli çalışmaz, otomatik başlamaz",
        "Başka uygulamalardan veri toplamaz",
        "Hiçbir veriyi ağ üzerinden göndermez",
        "Kayıtlar yalnızca istenirse yerel .txt dosyasına yazılır"
    };

    public AboutForm()
    {
        InitializeComponent();
        Version? version = Assembly.GetExecutingAssembly().GetName().Version;
        lblVersion.Text = $"Sürüm {version?.ToString(3) ?? "1.0.0"}  ·  YMH219 Nesne Tabanlı Programlama";
    }

    private void headerPanel_Paint(object? sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        using (var bg = new LinearGradientBrush(headerPanel.ClientRectangle,
                   Color.FromArgb(99, 102, 241), Color.FromArgb(168, 85, 247), 0f))
            g.FillRectangle(bg, headerPanel.ClientRectangle);

        var logo = new Rectangle(24, 22, 52, 52);
        using (var path = Rounded(logo, 12))
        using (var white = new SolidBrush(Color.FromArgb(235, 255, 255, 255)))
            g.FillPath(white, path);
        using (var font = new Font("Segoe UI", 18F, FontStyle.Bold))
            TextRenderer.DrawText(g, "K", font, logo, Color.FromArgb(79, 70, 229),
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
    }

    private void listPanel_Paint(object? sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var check = new Pen(Color.FromArgb(22, 163, 74), 2.2f);
        using var circle = new SolidBrush(Color.FromArgb(220, 252, 231));

        for (int i = 0; i < PrivacyItems.Length; i++)
        {
            int y = i * 28;
            g.FillEllipse(circle, 0, y + 2, 18, 18);
            g.DrawLines(check, new[] { new Point(5, y + 11), new Point(8, y + 14), new Point(14, y + 7) });
            TextRenderer.DrawText(g, PrivacyItems[i], listPanel.Font, new Point(28, y + 2), Color.FromArgb(51, 65, 85));
        }
    }

    private void btnOk_Click(object? sender, EventArgs e) => Close();

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
