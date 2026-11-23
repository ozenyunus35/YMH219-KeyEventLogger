namespace KeyEventLogger;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        headerPanel = new Panel();
        btnAbout = new Controls.FlatButton();
        recordingBadge = new Controls.RecordingBadge();
        lblSubtitle = new Label();
        lblTitle = new Label();
        toolbarPanel = new FlowLayoutPanel();
        btnStart = new Controls.FlatButton();
        btnStop = new Controls.FlatButton();
        toolbarSeparator = new Panel();
        btnClear = new Controls.FlatButton();
        btnSave = new Controls.FlatButton();
        btnOpen = new Controls.FlatButton();
        chkShowKeyUp = new CheckBox();
        chkShowKeyPress = new CheckBox();
        sidePanel = new Panel();
        cardTopKeys = new Controls.CardPanel();
        frequencyChart = new Controls.FrequencyChart();
        sideSpacer = new Panel();
        cardStats = new Controls.CardPanel();
        statsTable = new TableLayoutPanel();
        tileTotal = new Controls.StatTile();
        tileKpm = new Controls.StatTile();
        tileDistinct = new Controls.StatTile();
        contentPanel = new Panel();
        cardLog = new Controls.CardPanel();
        rtbLog = new RichTextBox();
        lblEmpty = new Label();
        btnClearFilter = new Controls.FlatButton();
        btnApplyFilter = new Controls.FlatButton();
        dtpTo = new DateTimePicker();
        lblDash = new Label();
        dtpFrom = new DateTimePicker();
        contentSpacer = new Panel();
        capturePad = new Controls.KeyCapturePad();
        statusStrip = new StatusStrip();
        lblFocus = new ToolStripStatusLabel();
        lblCount = new ToolStripStatusLabel();
        lblFilter = new ToolStripStatusLabel();
        lblDuration = new ToolStripStatusLabel();
        lblFile = new ToolStripStatusLabel();
        uiTimer = new System.Windows.Forms.Timer(components);
        toolTip = new ToolTip(components);
        headerPanel.SuspendLayout();
        toolbarPanel.SuspendLayout();
        sidePanel.SuspendLayout();
        cardTopKeys.SuspendLayout();
        cardStats.SuspendLayout();
        statsTable.SuspendLayout();
        contentPanel.SuspendLayout();
        cardLog.SuspendLayout();
        statusStrip.SuspendLayout();
        SuspendLayout();
        //
        // headerPanel
        //
        headerPanel.BackColor = Color.White;
        headerPanel.Controls.Add(btnAbout);
        headerPanel.Controls.Add(recordingBadge);
        headerPanel.Controls.Add(lblSubtitle);
        headerPanel.Controls.Add(lblTitle);
        headerPanel.Dock = DockStyle.Top;
        headerPanel.Location = new Point(0, 0);
        headerPanel.Name = "headerPanel";
        headerPanel.Size = new Size(1040, 72);
        headerPanel.TabIndex = 0;
        headerPanel.Paint += headerPanel_Paint;
        //
        // btnAbout
        //
        btnAbout.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnAbout.Location = new Point(740, 20);
        btnAbout.Name = "btnAbout";
        btnAbout.Size = new Size(96, 32);
        btnAbout.TabIndex = 3;
        btnAbout.Text = "Hakkında";
        btnAbout.Variant = KeyEventLogger.Controls.ButtonVariant.Ghost;
        btnAbout.Click += btnAbout_Click;
        //
        // recordingBadge
        //
        recordingBadge.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        recordingBadge.Location = new Point(848, 20);
        recordingBadge.Name = "recordingBadge";
        recordingBadge.Size = new Size(172, 32);
        recordingBadge.TabIndex = 2;
        //
        // lblSubtitle
        //
        lblSubtitle.AutoSize = true;
        lblSubtitle.Font = new Font("Segoe UI", 9F);
        lblSubtitle.ForeColor = Color.FromArgb(100, 116, 139);
        lblSubtitle.Location = new Point(73, 41);
        lblSubtitle.Name = "lblSubtitle";
        lblSubtitle.Size = new Size(330, 15);
        lblSubtitle.TabIndex = 1;
        lblSubtitle.Text = "Klavye olaylarını yakalama ve inceleme aracı  ·  YMH219";
        //
        // lblTitle
        //
        lblTitle.AutoSize = true;
        lblTitle.Font = new Font("Segoe UI Semibold", 15F);
        lblTitle.ForeColor = Color.FromArgb(15, 23, 42);
        lblTitle.Location = new Point(70, 11);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(176, 28);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "Key Event Logger";
        //
        // toolbarPanel
        //
        toolbarPanel.BackColor = Color.FromArgb(245, 247, 251);
        toolbarPanel.Controls.Add(btnStart);
        toolbarPanel.Controls.Add(btnStop);
        toolbarPanel.Controls.Add(toolbarSeparator);
        toolbarPanel.Controls.Add(btnClear);
        toolbarPanel.Controls.Add(btnSave);
        toolbarPanel.Controls.Add(btnOpen);
        toolbarPanel.Controls.Add(chkShowKeyUp);
        toolbarPanel.Controls.Add(chkShowKeyPress);
        toolbarPanel.Dock = DockStyle.Top;
        toolbarPanel.Location = new Point(0, 72);
        toolbarPanel.Name = "toolbarPanel";
        toolbarPanel.Padding = new Padding(17, 12, 17, 0);
        toolbarPanel.Size = new Size(1040, 60);
        toolbarPanel.TabIndex = 1;
        toolbarPanel.WrapContents = false;
        //
        // btnStart
        //
        btnStart.Location = new Point(20, 15);
        btnStart.Name = "btnStart";
        btnStart.Size = new Size(112, 36);
        btnStart.TabIndex = 0;
        btnStart.Text = "▶  Başlat";
        btnStart.Variant = KeyEventLogger.Controls.ButtonVariant.Primary;
        btnStart.Click += btnStart_Click;
        //
        // btnStop
        //
        btnStop.Enabled = false;
        btnStop.Location = new Point(138, 15);
        btnStop.Name = "btnStop";
        btnStop.Size = new Size(112, 36);
        btnStop.TabIndex = 1;
        btnStop.Text = "■  Durdur";
        btnStop.Variant = KeyEventLogger.Controls.ButtonVariant.Danger;
        btnStop.Click += btnStop_Click;
        //
        // toolbarSeparator
        //
        toolbarSeparator.BackColor = Color.FromArgb(203, 213, 225);
        toolbarSeparator.Location = new Point(266, 19);
        toolbarSeparator.Margin = new Padding(13, 7, 13, 0);
        toolbarSeparator.Name = "toolbarSeparator";
        toolbarSeparator.Size = new Size(1, 28);
        toolbarSeparator.TabIndex = 2;
        //
        // btnClear
        //
        btnClear.Location = new Point(283, 15);
        btnClear.Name = "btnClear";
        btnClear.Size = new Size(96, 36);
        btnClear.TabIndex = 3;
        btnClear.Text = "Temizle";
        btnClear.Click += btnClear_Click;
        //
        // btnSave
        //
        btnSave.Location = new Point(385, 15);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(124, 36);
        btnSave.TabIndex = 4;
        btnSave.Text = "Kaydet (.txt)";
        btnSave.Click += btnSave_Click;
        //
        // btnOpen
        //
        btnOpen.Location = new Point(515, 15);
        btnOpen.Name = "btnOpen";
        btnOpen.Size = new Size(118, 36);
        btnOpen.TabIndex = 5;
        btnOpen.Text = "Dosyadan Aç";
        btnOpen.Click += btnOpen_Click;
        //
        // chkShowKeyUp
        //
        chkShowKeyUp.AutoSize = true;
        chkShowKeyUp.Checked = true;
        chkShowKeyUp.CheckState = CheckState.Checked;
        chkShowKeyUp.Font = new Font("Segoe UI", 9F);
        chkShowKeyUp.ForeColor = Color.FromArgb(51, 65, 85);
        chkShowKeyUp.Location = new Point(655, 24);
        chkShowKeyUp.Margin = new Padding(19, 12, 3, 0);
        chkShowKeyUp.Name = "chkShowKeyUp";
        chkShowKeyUp.Size = new Size(96, 19);
        chkShowKeyUp.TabIndex = 6;
        chkShowKeyUp.TabStop = false;
        chkShowKeyUp.Text = "KeyUp göster";
        chkShowKeyUp.UseVisualStyleBackColor = true;
        chkShowKeyUp.CheckedChanged += displayOption_CheckedChanged;
        //
        // chkShowKeyPress
        //
        chkShowKeyPress.AutoSize = true;
        chkShowKeyPress.Checked = true;
        chkShowKeyPress.CheckState = CheckState.Checked;
        chkShowKeyPress.Font = new Font("Segoe UI", 9F);
        chkShowKeyPress.ForeColor = Color.FromArgb(51, 65, 85);
        chkShowKeyPress.Location = new Point(757, 24);
        chkShowKeyPress.Margin = new Padding(3, 12, 3, 0);
        chkShowKeyPress.Name = "chkShowKeyPress";
        chkShowKeyPress.Size = new Size(108, 19);
        chkShowKeyPress.TabIndex = 7;
        chkShowKeyPress.TabStop = false;
        chkShowKeyPress.Text = "KeyPress göster";
        chkShowKeyPress.UseVisualStyleBackColor = true;
        chkShowKeyPress.CheckedChanged += displayOption_CheckedChanged;
        //
        // sidePanel
        //
        sidePanel.BackColor = Color.FromArgb(245, 247, 251);
        sidePanel.Controls.Add(cardTopKeys);
        sidePanel.Controls.Add(sideSpacer);
        sidePanel.Controls.Add(cardStats);
        sidePanel.Dock = DockStyle.Right;
        sidePanel.Location = new Point(720, 132);
        sidePanel.Name = "sidePanel";
        sidePanel.Padding = new Padding(0, 4, 20, 16);
        sidePanel.Size = new Size(320, 506);
        sidePanel.TabIndex = 3;
        //
        // cardTopKeys
        //
        cardTopKeys.Controls.Add(frequencyChart);
        cardTopKeys.Dock = DockStyle.Fill;
        cardTopKeys.Location = new Point(0, 136);
        cardTopKeys.Name = "cardTopKeys";
        cardTopKeys.Size = new Size(300, 354);
        cardTopKeys.TabIndex = 2;
        cardTopKeys.Title = "En sık kullanılan tuşlar";
        //
        // frequencyChart
        //
        frequencyChart.Dock = DockStyle.Fill;
        frequencyChart.Location = new Point(16, 44);
        frequencyChart.Name = "frequencyChart";
        frequencyChart.Size = new Size(268, 296);
        frequencyChart.TabIndex = 0;
        //
        // sideSpacer
        //
        sideSpacer.Dock = DockStyle.Top;
        sideSpacer.Location = new Point(0, 124);
        sideSpacer.Name = "sideSpacer";
        sideSpacer.Size = new Size(300, 12);
        sideSpacer.TabIndex = 1;
        //
        // cardStats
        //
        cardStats.Controls.Add(statsTable);
        cardStats.Dock = DockStyle.Top;
        cardStats.Location = new Point(0, 4);
        cardStats.Name = "cardStats";
        cardStats.Padding = new Padding(12, 44, 12, 12);
        cardStats.Size = new Size(300, 120);
        cardStats.TabIndex = 0;
        cardStats.Title = "Özet";
        //
        // statsTable
        //
        statsTable.ColumnCount = 3;
        statsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        statsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        statsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));
        statsTable.Controls.Add(tileTotal, 0, 0);
        statsTable.Controls.Add(tileKpm, 1, 0);
        statsTable.Controls.Add(tileDistinct, 2, 0);
        statsTable.Dock = DockStyle.Fill;
        statsTable.Location = new Point(12, 44);
        statsTable.Name = "statsTable";
        statsTable.RowCount = 1;
        statsTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        statsTable.Size = new Size(276, 64);
        statsTable.TabIndex = 0;
        //
        // tileTotal
        //
        tileTotal.Caption = "Toplam tuş";
        tileTotal.Dock = DockStyle.Fill;
        tileTotal.Location = new Point(3, 3);
        tileTotal.Name = "tileTotal";
        tileTotal.Size = new Size(85, 58);
        tileTotal.TabIndex = 0;
        //
        // tileKpm
        //
        tileKpm.Accent = Color.FromArgb(22, 163, 74);
        tileKpm.Caption = "Tuş / dakika";
        tileKpm.Dock = DockStyle.Fill;
        tileKpm.Location = new Point(94, 3);
        tileKpm.Name = "tileKpm";
        tileKpm.Size = new Size(85, 58);
        tileKpm.TabIndex = 1;
        //
        // tileDistinct
        //
        tileDistinct.Accent = Color.FromArgb(217, 119, 6);
        tileDistinct.Caption = "Farklı tuş";
        tileDistinct.Dock = DockStyle.Fill;
        tileDistinct.Location = new Point(185, 3);
        tileDistinct.Name = "tileDistinct";
        tileDistinct.Size = new Size(88, 58);
        tileDistinct.TabIndex = 2;
        //
        // contentPanel
        //
        contentPanel.BackColor = Color.FromArgb(245, 247, 251);
        contentPanel.Controls.Add(cardLog);
        contentPanel.Controls.Add(contentSpacer);
        contentPanel.Controls.Add(capturePad);
        contentPanel.Dock = DockStyle.Fill;
        contentPanel.Location = new Point(0, 132);
        contentPanel.Name = "contentPanel";
        contentPanel.Padding = new Padding(20, 4, 16, 16);
        contentPanel.Size = new Size(720, 506);
        contentPanel.TabIndex = 2;
        //
        // cardLog
        //
        cardLog.Controls.Add(rtbLog);
        cardLog.Controls.Add(lblEmpty);
        cardLog.Controls.Add(btnClearFilter);
        cardLog.Controls.Add(btnApplyFilter);
        cardLog.Controls.Add(dtpTo);
        cardLog.Controls.Add(lblDash);
        cardLog.Controls.Add(dtpFrom);
        cardLog.Dock = DockStyle.Fill;
        cardLog.Location = new Point(20, 162);
        cardLog.Name = "cardLog";
        cardLog.Padding = new Padding(16, 52, 8, 12);
        cardLog.Size = new Size(684, 328);
        cardLog.TabIndex = 2;
        cardLog.Title = "Olay günlüğü";
        //
        // rtbLog
        //
        rtbLog.BackColor = Color.White;
        rtbLog.BorderStyle = BorderStyle.None;
        rtbLog.DetectUrls = false;
        rtbLog.Dock = DockStyle.Fill;
        rtbLog.Font = new Font("Consolas", 10F);
        rtbLog.ForeColor = Color.FromArgb(30, 41, 59);
        rtbLog.HideSelection = false;
        rtbLog.Location = new Point(16, 52);
        rtbLog.Name = "rtbLog";
        rtbLog.ReadOnly = true;
        rtbLog.ScrollBars = RichTextBoxScrollBars.Vertical;
        rtbLog.Size = new Size(660, 264);
        rtbLog.TabIndex = 5;
        rtbLog.TabStop = false;
        rtbLog.Text = "";
        rtbLog.WordWrap = false;
        //
        // lblEmpty
        //
        lblEmpty.Dock = DockStyle.Fill;
        lblEmpty.Font = new Font("Segoe UI", 10F);
        lblEmpty.ForeColor = Color.FromArgb(148, 163, 184);
        lblEmpty.Location = new Point(16, 52);
        lblEmpty.Name = "lblEmpty";
        lblEmpty.Size = new Size(660, 264);
        lblEmpty.TabIndex = 6;
        lblEmpty.Text = "Henüz kayıt yok.\r\n\"Başlat\" butonuna basın ve yukarıdaki alana tıklayıp yazmaya başlayın.";
        lblEmpty.TextAlign = ContentAlignment.MiddleCenter;
        //
        // btnClearFilter
        //
        btnClearFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnClearFilter.Font = new Font("Segoe UI Semibold", 9F);
        btnClearFilter.Location = new Point(590, 10);
        btnClearFilter.Name = "btnClearFilter";
        btnClearFilter.Size = new Size(80, 28);
        btnClearFilter.TabIndex = 4;
        btnClearFilter.Text = "Kaldır";
        btnClearFilter.Variant = KeyEventLogger.Controls.ButtonVariant.Ghost;
        btnClearFilter.Click += btnClearFilter_Click;
        //
        // btnApplyFilter
        //
        btnApplyFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnApplyFilter.Font = new Font("Segoe UI Semibold", 9F);
        btnApplyFilter.Location = new Point(508, 10);
        btnApplyFilter.Name = "btnApplyFilter";
        btnApplyFilter.Size = new Size(78, 28);
        btnApplyFilter.TabIndex = 3;
        btnApplyFilter.Text = "Filtrele";
        btnApplyFilter.Click += btnApplyFilter_Click;
        //
        // dtpTo
        //
        dtpTo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        dtpTo.CustomFormat = "dd.MM.yyyy HH:mm:ss";
        dtpTo.Font = new Font("Segoe UI", 9F);
        dtpTo.Format = DateTimePickerFormat.Custom;
        dtpTo.Location = new Point(338, 12);
        dtpTo.Name = "dtpTo";
        dtpTo.Size = new Size(164, 23);
        dtpTo.TabIndex = 2;
        dtpTo.TabStop = false;
        //
        // lblDash
        //
        lblDash.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblDash.AutoSize = true;
        lblDash.ForeColor = Color.FromArgb(100, 116, 139);
        lblDash.Location = new Point(322, 16);
        lblDash.Name = "lblDash";
        lblDash.Size = new Size(12, 15);
        lblDash.TabIndex = 1;
        lblDash.Text = "–";
        //
        // dtpFrom
        //
        dtpFrom.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        dtpFrom.CustomFormat = "dd.MM.yyyy HH:mm:ss";
        dtpFrom.Font = new Font("Segoe UI", 9F);
        dtpFrom.Format = DateTimePickerFormat.Custom;
        dtpFrom.Location = new Point(152, 12);
        dtpFrom.Name = "dtpFrom";
        dtpFrom.Size = new Size(164, 23);
        dtpFrom.TabIndex = 0;
        dtpFrom.TabStop = false;
        //
        // contentSpacer
        //
        contentSpacer.Dock = DockStyle.Top;
        contentSpacer.Location = new Point(20, 148);
        contentSpacer.Name = "contentSpacer";
        contentSpacer.Size = new Size(684, 14);
        contentSpacer.TabIndex = 1;
        //
        // capturePad
        //
        capturePad.Dock = DockStyle.Top;
        capturePad.Location = new Point(20, 4);
        capturePad.Name = "capturePad";
        capturePad.Size = new Size(684, 144);
        capturePad.TabIndex = 0;
        //
        // statusStrip
        //
        statusStrip.BackColor = Color.White;
        statusStrip.Items.AddRange(new ToolStripItem[] { lblFocus, lblCount, lblFilter, lblDuration, lblFile });
        statusStrip.Location = new Point(0, 638);
        statusStrip.Name = "statusStrip";
        statusStrip.Size = new Size(1040, 22);
        statusStrip.SizingGrip = false;
        statusStrip.TabIndex = 4;
        //
        // lblFocus
        //
        lblFocus.Name = "lblFocus";
        lblFocus.Size = new Size(110, 17);
        lblFocus.Text = "Pencere odakta";
        //
        // lblCount
        //
        lblCount.BorderSides = ToolStripStatusLabelBorderSides.Left;
        lblCount.Name = "lblCount";
        lblCount.Size = new Size(70, 17);
        lblCount.Text = "0 kayıt";
        //
        // lblFilter
        //
        lblFilter.BorderSides = ToolStripStatusLabelBorderSides.Left;
        lblFilter.Name = "lblFilter";
        lblFilter.Size = new Size(85, 17);
        lblFilter.Text = "Filtre kapalı";
        //
        // lblDuration
        //
        lblDuration.BorderSides = ToolStripStatusLabelBorderSides.Left;
        lblDuration.Name = "lblDuration";
        lblDuration.Size = new Size(600, 17);
        lblDuration.Spring = true;
        lblDuration.Text = "Süre: -";
        lblDuration.TextAlign = ContentAlignment.MiddleLeft;
        //
        // lblFile
        //
        lblFile.ForeColor = Color.FromArgb(100, 116, 139);
        lblFile.Name = "lblFile";
        lblFile.Size = new Size(120, 17);
        lblFile.Text = "Dosya: kaydedilmedi";
        //
        // uiTimer
        //
        uiTimer.Interval = 500;
        uiTimer.Tick += uiTimer_Tick;
        //
        // MainForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(245, 247, 251);
        ClientSize = new Size(1040, 660);
        Controls.Add(contentPanel);
        Controls.Add(sidePanel);
        Controls.Add(toolbarPanel);
        Controls.Add(headerPanel);
        Controls.Add(statusStrip);
        Font = new Font("Segoe UI", 9F);
        KeyPreview = true;
        MinimumSize = new Size(900, 600);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Key Event Logger";
        Activated += MainForm_Activated;
        Deactivate += MainForm_Deactivate;
        Shown += MainForm_Shown;
        KeyDown += MainForm_KeyDown;
        KeyPress += MainForm_KeyPress;
        KeyUp += MainForm_KeyUp;
        headerPanel.ResumeLayout(false);
        headerPanel.PerformLayout();
        toolbarPanel.ResumeLayout(false);
        toolbarPanel.PerformLayout();
        sidePanel.ResumeLayout(false);
        cardTopKeys.ResumeLayout(false);
        cardStats.ResumeLayout(false);
        statsTable.ResumeLayout(false);
        contentPanel.ResumeLayout(false);
        cardLog.ResumeLayout(false);
        cardLog.PerformLayout();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Panel headerPanel;
    private Label lblTitle;
    private Label lblSubtitle;
    private Controls.RecordingBadge recordingBadge;
    private Controls.FlatButton btnAbout;
    private FlowLayoutPanel toolbarPanel;
    private Controls.FlatButton btnStart;
    private Controls.FlatButton btnStop;
    private Panel toolbarSeparator;
    private Controls.FlatButton btnClear;
    private Controls.FlatButton btnSave;
    private Controls.FlatButton btnOpen;
    private CheckBox chkShowKeyUp;
    private CheckBox chkShowKeyPress;
    private Panel sidePanel;
    private Controls.CardPanel cardStats;
    private TableLayoutPanel statsTable;
    private Controls.StatTile tileTotal;
    private Controls.StatTile tileKpm;
    private Controls.StatTile tileDistinct;
    private Panel sideSpacer;
    private Controls.CardPanel cardTopKeys;
    private Controls.FrequencyChart frequencyChart;
    private Panel contentPanel;
    private Controls.KeyCapturePad capturePad;
    private Panel contentSpacer;
    private Controls.CardPanel cardLog;
    private RichTextBox rtbLog;
    private Label lblEmpty;
    private DateTimePicker dtpFrom;
    private Label lblDash;
    private DateTimePicker dtpTo;
    private Controls.FlatButton btnApplyFilter;
    private Controls.FlatButton btnClearFilter;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel lblFocus;
    private ToolStripStatusLabel lblCount;
    private ToolStripStatusLabel lblFilter;
    private ToolStripStatusLabel lblDuration;
    private ToolStripStatusLabel lblFile;
    private System.Windows.Forms.Timer uiTimer;
    private ToolTip toolTip;
}
