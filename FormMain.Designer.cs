using System.Windows.Forms;

partial class FormMain : Form
{
    private TableLayoutPanel tableLayoutPanel;
    private Label labelDevices;
    private FlowLayoutPanel panelDevices; // Audio Devices (unpaired)
    private Label labelClients;
    private FlowLayoutPanel panelClients; // Clients (unpaired)
    private Label labelPairs;
    private FlowLayoutPanel panelPaired;  // Paired devices

    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.tableLayoutPanel = new TableLayoutPanel();
        this.labelDevices = new Label();
        this.panelDevices = new FlowLayoutPanel();
        this.labelClients = new Label();
        this.panelClients = new FlowLayoutPanel();
        this.labelPairs = new Label();
        this.panelPaired = new FlowLayoutPanel();

        this.SuspendLayout();

        // 
        // tableLayoutPanel
        // 
        this.tableLayoutPanel.ColumnCount = 1;
        this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        this.tableLayoutPanel.RowCount = 6;
        this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // Label Devices
        this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 30F)); // Panel Devices
        this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // Label Clients
        this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 30F)); // Panel Clients
        this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // Label Paired
        this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 40F)); // Panel Paired

        this.tableLayoutPanel.Dock = DockStyle.Fill;
        this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
        this.tableLayoutPanel.Name = "tableLayoutPanel";
        this.tableLayoutPanel.Size = new System.Drawing.Size(1024, 1024);
        this.tableLayoutPanel.TabIndex = 0;

        // 
        // labelDevices
        // 
        this.labelDevices.AutoSize = true;
        this.labelDevices.Dock = DockStyle.Fill;
        this.labelDevices.Text = "Unpaired Audio Devices";
        this.labelDevices.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        this.labelDevices.Padding = new Padding(5, 5, 0, 5);
        this.labelDevices.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);

        // 
        // panelDevices
        // 
        this.panelDevices.AutoScroll = true;
        this.panelDevices.Dock = DockStyle.Fill;
        this.panelDevices.FlowDirection = FlowDirection.TopDown;
        this.panelDevices.WrapContents = false;
        this.panelDevices.Name = "panelDevices";

        // 
        // labelClients
        // 
        this.labelClients.AutoSize = true;
        this.labelClients.Dock = DockStyle.Fill;
        this.labelClients.Text = "Unpaired Clients";
        this.labelClients.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        this.labelClients.Padding = new Padding(5, 5, 0, 5);
        this.labelClients.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);

        // 
        // panelClients
        // 
        this.panelClients.AutoScroll = true;
        this.panelClients.Dock = DockStyle.Fill;
        this.panelClients.FlowDirection = FlowDirection.TopDown;
        this.panelClients.WrapContents = false;
        this.panelClients.Name = "panelClients";

        // 
        // labelPairs
        // 
        this.labelPairs.AutoSize = true;
        this.labelPairs.Dock = DockStyle.Fill;
        this.labelPairs.Text = "Paired Devices";
        this.labelPairs.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        this.labelPairs.Padding = new Padding(5, 5, 0, 5);
        this.labelPairs.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);

        // 
        // panelPaired
        // 
        this.panelPaired.AutoScroll = true;
        this.panelPaired.Dock = DockStyle.Fill;
        this.panelPaired.FlowDirection = FlowDirection.TopDown;
        this.panelPaired.WrapContents = false;
        this.panelPaired.Name = "panelPaired";

        // 
        // Add controls to tableLayoutPanel
        // 
        this.tableLayoutPanel.Controls.Add(this.labelDevices, 0, 0);
        this.tableLayoutPanel.Controls.Add(this.panelDevices, 0, 1);
        this.tableLayoutPanel.Controls.Add(this.labelClients, 0, 2);
        this.tableLayoutPanel.Controls.Add(this.panelClients, 0, 3);
        this.tableLayoutPanel.Controls.Add(this.labelPairs, 0, 4);
        this.tableLayoutPanel.Controls.Add(this.panelPaired, 0, 5);

        // 
        // FormMain
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(1024, 1024);
        this.Controls.Add(this.tableLayoutPanel);
        this.FormBorderStyle = FormBorderStyle.Sizable;
        this.MaximizeBox = true;
        this.Name = "FormMain";
        this.Text = "LanMic Server";

        this.ResumeLayout(false);
    }
}
