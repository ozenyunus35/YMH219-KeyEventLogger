namespace KeyEventLogger;

partial class AboutForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        headerPanel = new Panel();
        lblVersion = new Label();
        lblName = new Label();
        lblPrivacyTitle = new Label();
        listPanel = new Panel();
        btnOk = new Controls.FlatButton();
        headerPanel.SuspendLayout();
        SuspendLayout();
        //
        // headerPanel
        //
        headerPanel.Controls.Add(lblVersion);
        headerPanel.Controls.Add(lblName);
        headerPanel.Dock = DockStyle.Top;
        headerPanel.Location = new Point(0, 0);
        headerPanel.Name = "headerPanel";
        headerPanel.Size = new Size(470, 96);
        headerPanel.TabIndex = 0;
        headerPanel.Paint += headerPanel_Paint;
        //
        // lblVersion
        //
        lblVersion.AutoSize = true;
        lblVersion.BackColor = Color.Transparent;
        lblVersion.Font = new Font("Segoe UI", 9F);
        lblVersion.ForeColor = Color.FromArgb(237, 233, 254);
        lblVersion.Location = new Point(90, 53);
        lblVersion.Name = "lblVersion";
        lblVersion.Size = new Size(60, 15);
        lblVersion.TabIndex = 1;
        lblVersion.Text = "Sürüm";
        //
        // lblName
        //
        lblName.AutoSize = true;
        lblName.BackColor = Color.Transparent;
        lblName.Font = new Font("Segoe UI Semibold", 16F);
        lblName.ForeColor = Color.White;
        lblName.Location = new Point(87, 22);
        lblName.Name = "lblName";
        lblName.Size = new Size(180, 30);
        lblName.TabIndex = 0;
        lblName.Text = "Key Event Logger";
        //
        // lblPrivacyTitle
        //
        lblPrivacyTitle.AutoSize = true;
        lblPrivacyTitle.Font = new Font("Segoe UI Semibold", 10.5F);
        lblPrivacyTitle.ForeColor = Color.FromArgb(15, 23, 42);
        lblPrivacyTitle.Location = new Point(24, 114);
        lblPrivacyTitle.Name = "lblPrivacyTitle";
        lblPrivacyTitle.Size = new Size(170, 19);
        lblPrivacyTitle.TabIndex = 1;
        lblPrivacyTitle.Text = "Kapsam ve gizlilik";
        //
        // listPanel
        //
        listPanel.Font = new Font("Segoe UI", 9.5F);
        listPanel.Location = new Point(26, 144);
        listPanel.Name = "listPanel";
        listPanel.Size = new Size(420, 172);
        listPanel.TabIndex = 2;
        listPanel.Paint += listPanel_Paint;
        //
        // btnOk
        //
        btnOk.Location = new Point(346, 326);
        btnOk.Name = "btnOk";
        btnOk.Size = new Size(100, 34);
        btnOk.TabIndex = 3;
        btnOk.Text = "Tamam";
        btnOk.Variant = KeyEventLogger.Controls.ButtonVariant.Primary;
        btnOk.Click += btnOk_Click;
        //
        // AboutForm
        //
        AcceptButton = btnOk;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.White;
        CancelButton = btnOk;
        ClientSize = new Size(470, 378);
        Controls.Add(btnOk);
        Controls.Add(listPanel);
        Controls.Add(lblPrivacyTitle);
        Controls.Add(headerPanel);
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "AboutForm";
        ShowIcon = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Hakkında";
        headerPanel.ResumeLayout(false);
        headerPanel.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Panel headerPanel;
    private Label lblName;
    private Label lblVersion;
    private Label lblPrivacyTitle;
    private Panel listPanel;
    private Controls.FlatButton btnOk;
}
