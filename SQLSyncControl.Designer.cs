using System.Drawing;
using System.Windows.Forms;

namespace AutoLinkCore
{
    partial class SQLSyncControl
    {
        private System.ComponentModel.IContainer components = null;
        
        private Button btnBack;
        private Label lblTitle;
        
        // SQL Configuration
        private GroupBox grpSQLConfig;
        private Label lblServer;
        private TextBox txtServer;
        private Label lblUser;
        private TextBox txtUser;
        private Label lblPassword;
        private TextBox txtPassword;
        private RadioButton rbUseExisting;
        private RadioButton rbCreateNew;
        private ComboBox cmbDatabases;
        private TextBox txtNewDBName;
        private Button btnTestSQL;
        private Button btnInitializeDB;
        
        // Global Handshake Settings
        private GroupBox grpGlobalHandshake;
        private Label lblLogBit;
        private ComboBox cmbLogMemArea;
        private TextBox txtLogDBNum;
        private TextBox txtLogOffset;
        private TextBox txtLogBitNum;
        private Label lblConfirmBit;
        private ComboBox cmbConfirmMemArea;
        private TextBox txtConfirmDBNum;
        private TextBox txtConfirmOffset;
        private TextBox txtConfirmBitNum;
        private Label lblHandshakeInfo;
        
        // Dynamic Data Mapping
        private GroupBox grpDataMapping;
        private Button btnAddMapping;
        private FlowLayoutPanel flowMappings;
        
        // Monitoring
        private GroupBox grpMonitoring;
        private Label lblPLCStatus;
        private Label lblSQLStatus;
        private Button btnStartMonitoring;
        private Button btnClearLogs;
        private TextBox txtLogs;
        
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        
        private void InitializeComponent()
        {
            this.btnBack = new Button();
            this.lblTitle = new Label();
            
            this.grpSQLConfig = new GroupBox();
            this.lblServer = new Label();
            this.txtServer = new TextBox();
            this.lblUser = new Label();
            this.txtUser = new TextBox();
            this.lblPassword = new Label();
            this.txtPassword = new TextBox();
            this.rbUseExisting = new RadioButton();
            this.rbCreateNew = new RadioButton();
            this.cmbDatabases = new ComboBox();
            this.txtNewDBName = new TextBox();
            this.btnTestSQL = new Button();
            this.btnInitializeDB = new Button();
            
            this.grpGlobalHandshake = new GroupBox();
            this.lblLogBit = new Label();
            this.cmbLogMemArea = new ComboBox();
            this.txtLogDBNum = new TextBox();
            this.txtLogOffset = new TextBox();
            this.txtLogBitNum = new TextBox();
            this.lblConfirmBit = new Label();
            this.cmbConfirmMemArea = new ComboBox();
            this.txtConfirmDBNum = new TextBox();
            this.txtConfirmOffset = new TextBox();
            this.txtConfirmBitNum = new TextBox();
            this.lblHandshakeInfo = new Label();
            
            this.grpDataMapping = new GroupBox();
            this.btnAddMapping = new Button();
            this.flowMappings = new FlowLayoutPanel();
            
            this.grpMonitoring = new GroupBox();
            this.lblPLCStatus = new Label();
            this.lblSQLStatus = new Label();
            this.btnStartMonitoring = new Button();
            this.btnClearLogs = new Button();
            this.txtLogs = new TextBox();
            
            this.grpSQLConfig.SuspendLayout();
            this.grpGlobalHandshake.SuspendLayout();
            this.grpDataMapping.SuspendLayout();
            this.grpMonitoring.SuspendLayout();
            this.SuspendLayout();
            
            // 
            // btnBack
            // 
            this.btnBack.Location = new Point(10, 10);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new Size(80, 30);
            this.btnBack.Text = "← Back";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            this.lblTitle.Location = new Point(320, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new Size(350, 30);
            this.lblTitle.Text = "SQL-PLC Synchronization (Multi-Mapping)";
            this.lblTitle.TextAlign = ContentAlignment.MiddleLeft;
            
            // ========== SQL CONFIGURATION GROUP ==========
            // 
            // grpSQLConfig
            // 
            this.grpSQLConfig.BackColor = Color.FromArgb(245, 245, 245);
            this.grpSQLConfig.Controls.Add(this.lblServer);
            this.grpSQLConfig.Controls.Add(this.txtServer);
            this.grpSQLConfig.Controls.Add(this.lblUser);
            this.grpSQLConfig.Controls.Add(this.txtUser);
            this.grpSQLConfig.Controls.Add(this.lblPassword);
            this.grpSQLConfig.Controls.Add(this.txtPassword);
            this.grpSQLConfig.Controls.Add(this.rbUseExisting);
            this.grpSQLConfig.Controls.Add(this.rbCreateNew);
            this.grpSQLConfig.Controls.Add(this.cmbDatabases);
            this.grpSQLConfig.Controls.Add(this.txtNewDBName);
            this.grpSQLConfig.Controls.Add(this.btnTestSQL);
            this.grpSQLConfig.Controls.Add(this.btnInitializeDB);
            this.grpSQLConfig.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            this.grpSQLConfig.Location = new Point(10, 50);
            this.grpSQLConfig.Name = "grpSQLConfig";
            this.grpSQLConfig.Size = new Size(380, 260);
            this.grpSQLConfig.TabIndex = 0;
            this.grpSQLConfig.TabStop = false;
            this.grpSQLConfig.Text = "SQL Server Configuration";
            
            this.lblServer.Font = new Font("Segoe UI", 9F);
            this.lblServer.Location = new Point(15, 30);
            this.lblServer.Size = new Size(80, 20);
            this.lblServer.Text = "Server:";
            
            this.txtServer.Font = new Font("Segoe UI", 9F);
            this.txtServer.Location = new Point(100, 28);
            this.txtServer.Size = new Size(260, 25);
            this.txtServer.Text = "localhost";
            
            this.lblUser.Font = new Font("Segoe UI", 9F);
            this.lblUser.Location = new Point(15, 65);
            this.lblUser.Size = new Size(80, 20);
            this.lblUser.Text = "User:";
            
            this.txtUser.Font = new Font("Segoe UI", 9F);
            this.txtUser.Location = new Point(100, 63);
            this.txtUser.Size = new Size(260, 25);
            this.txtUser.Text = "sa";
            
            this.lblPassword.Font = new Font("Segoe UI", 9F);
            this.lblPassword.Location = new Point(15, 100);
            this.lblPassword.Size = new Size(80, 20);
            this.lblPassword.Text = "Password:";
            
            this.txtPassword.Font = new Font("Segoe UI", 9F);
            this.txtPassword.Location = new Point(100, 98);
            this.txtPassword.Size = new Size(260, 25);
            this.txtPassword.UseSystemPasswordChar = true;
            
            this.rbUseExisting.Font = new Font("Segoe UI", 9F);
            this.rbUseExisting.Location = new Point(15, 135);
            this.rbUseExisting.Size = new Size(150, 20);
            this.rbUseExisting.Text = "Use Existing Database";
            this.rbUseExisting.Checked = true;
            this.rbUseExisting.CheckedChanged += new System.EventHandler(this.rbUseExisting_CheckedChanged);
            
            this.cmbDatabases.Font = new Font("Segoe UI", 9F);
            this.cmbDatabases.Location = new Point(100, 160);
            this.cmbDatabases.Size = new Size(260, 25);
            this.cmbDatabases.DropDownStyle = ComboBoxStyle.DropDownList;
            
            this.rbCreateNew.Font = new Font("Segoe UI", 9F);
            this.rbCreateNew.Location = new Point(15, 190);
            this.rbCreateNew.Size = new Size(150, 20);
            this.rbCreateNew.Text = "Create New Database";
            this.rbCreateNew.CheckedChanged += new System.EventHandler(this.rbCreateNew_CheckedChanged);
            
            this.txtNewDBName.Font = new Font("Segoe UI", 9F);
            this.txtNewDBName.Location = new Point(170, 190);
            this.txtNewDBName.Size = new Size(190, 25);
            this.txtNewDBName.Text = "PLCDataLog";
            this.txtNewDBName.Enabled = false;
            
            this.btnTestSQL.BackColor = AppTheme.AccentBlue;
            this.btnTestSQL.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnTestSQL.ForeColor = Color.White;
            this.btnTestSQL.Location = new Point(15, 225);
            this.btnTestSQL.Size = new Size(175, 35);
            this.btnTestSQL.Text = "Test Connection";
            this.btnTestSQL.UseVisualStyleBackColor = false;
            this.btnTestSQL.Click += new System.EventHandler(this.btnTestSQL_Click);
            
            this.btnInitializeDB.BackColor = AppTheme.SuccessGreen;
            this.btnInitializeDB.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnInitializeDB.ForeColor = Color.White;
            this.btnInitializeDB.Location = new Point(195, 225);
            this.btnInitializeDB.Size = new Size(175, 35);
            this.btnInitializeDB.Text = "Initialize Database";
            this.btnInitializeDB.UseVisualStyleBackColor = false;
            this.btnInitializeDB.Click += new System.EventHandler(this.btnInitializeDB_Click);
            
            // ========== GLOBAL HANDSHAKE SETTINGS ==========
            // 
            // grpGlobalHandshake
            // 
            this.grpGlobalHandshake.BackColor = Color.FromArgb(245, 245, 245);
            this.grpGlobalHandshake.Controls.Add(this.lblLogBit);
            this.grpGlobalHandshake.Controls.Add(this.cmbLogMemArea);
            this.grpGlobalHandshake.Controls.Add(this.txtLogDBNum);
            this.grpGlobalHandshake.Controls.Add(this.txtLogOffset);
            this.grpGlobalHandshake.Controls.Add(this.txtLogBitNum);
            this.grpGlobalHandshake.Controls.Add(this.lblConfirmBit);
            this.grpGlobalHandshake.Controls.Add(this.cmbConfirmMemArea);
            this.grpGlobalHandshake.Controls.Add(this.txtConfirmDBNum);
            this.grpGlobalHandshake.Controls.Add(this.txtConfirmOffset);
            this.grpGlobalHandshake.Controls.Add(this.txtConfirmBitNum);
            this.grpGlobalHandshake.Controls.Add(this.lblHandshakeInfo);
            this.grpGlobalHandshake.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            this.grpGlobalHandshake.Location = new Point(400, 50);
            this.grpGlobalHandshake.Name = "grpGlobalHandshake";
            this.grpGlobalHandshake.Size = new Size(380, 180);
            this.grpGlobalHandshake.TabIndex = 1;
            this.grpGlobalHandshake.TabStop = false;
            this.grpGlobalHandshake.Text = "Global Handshake Settings";
            
            // Log Trigger Bit
            this.lblLogBit.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblLogBit.Location = new Point(15, 25);
            this.lblLogBit.Size = new Size(100, 20);
            this.lblLogBit.Text = "Log Trigger Bit:";
            
            this.cmbLogMemArea.Font = new Font("Segoe UI", 8F);
            this.cmbLogMemArea.Location = new Point(15, 48);
            this.cmbLogMemArea.Size = new Size(50, 22);
            this.cmbLogMemArea.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbLogMemArea.Items.AddRange(new object[] { "DB", "M", "I", "Q" });
            this.cmbLogMemArea.SelectedIndex = 0;
            
            this.txtLogDBNum.Font = new Font("Segoe UI", 8F);
            this.txtLogDBNum.Location = new Point(70, 48);
            this.txtLogDBNum.Size = new Size(40, 22);
            this.txtLogDBNum.Text = "1";
            
            this.txtLogOffset.Font = new Font("Segoe UI", 8F);
            this.txtLogOffset.Location = new Point(115, 48);
            this.txtLogOffset.Size = new Size(50, 22);
            this.txtLogOffset.Text = "10";
            
            this.txtLogBitNum.Font = new Font("Segoe UI", 8F);
            this.txtLogBitNum.Location = new Point(170, 48);
            this.txtLogBitNum.Size = new Size(35, 22);
            this.txtLogBitNum.Text = "0";
            
            // Confirmation Bit
            this.lblConfirmBit.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblConfirmBit.Location = new Point(15, 78);
            this.lblConfirmBit.Size = new Size(110, 20);
            this.lblConfirmBit.Text = "Confirmation Bit:";
            
            this.cmbConfirmMemArea.Font = new Font("Segoe UI", 8F);
            this.cmbConfirmMemArea.Location = new Point(15, 101);
            this.cmbConfirmMemArea.Size = new Size(50, 22);
            this.cmbConfirmMemArea.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbConfirmMemArea.Items.AddRange(new object[] { "DB", "M", "I", "Q" });
            this.cmbConfirmMemArea.SelectedIndex = 0;
            
            this.txtConfirmDBNum.Font = new Font("Segoe UI", 8F);
            this.txtConfirmDBNum.Location = new Point(70, 101);
            this.txtConfirmDBNum.Size = new Size(40, 22);
            this.txtConfirmDBNum.Text = "1";
            
            this.txtConfirmOffset.Font = new Font("Segoe UI", 8F);
            this.txtConfirmOffset.Location = new Point(115, 101);
            this.txtConfirmOffset.Size = new Size(50, 22);
            this.txtConfirmOffset.Text = "10";
            
            this.txtConfirmBitNum.Font = new Font("Segoe UI", 8F);
            this.txtConfirmBitNum.Location = new Point(170, 101);
            this.txtConfirmBitNum.Size = new Size(35, 22);
            this.txtConfirmBitNum.Text = "1";
            
            this.lblHandshakeInfo.Font = new Font("Segoe UI", 7.5F, FontStyle.Italic);
            this.lblHandshakeInfo.ForeColor = Color.DarkBlue;
            this.lblHandshakeInfo.Location = new Point(15, 130);
            this.lblHandshakeInfo.Size = new Size(350, 45);
            this.lblHandshakeInfo.Text = "Protocol: PLC sets Log Bit → App reads all mappings → App writes SQL\n→ App sets Confirm Bit → PLC resets Log Bit → App resets Confirm Bit";
            
            // ========== DYNAMIC DATA MAPPING ==========
            // 
            // grpDataMapping
            // 
            this.grpDataMapping.BackColor = Color.FromArgb(30, 30, 30);
            this.grpDataMapping.ForeColor = Color.White;
            this.grpDataMapping.Controls.Add(this.btnAddMapping);
            this.grpDataMapping.Controls.Add(this.flowMappings);
            this.grpDataMapping.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            this.grpDataMapping.Location = new Point(10, 320);
            this.grpDataMapping.Name = "grpDataMapping";
            this.grpDataMapping.Size = new Size(770, 250);
            this.grpDataMapping.TabIndex = 2;
            this.grpDataMapping.TabStop = false;
            this.grpDataMapping.Text = "PLC to SQL Data Mappings (Industrial Dark Mode)";
            
            this.btnAddMapping.BackColor = Color.FromArgb(0, 150, 0);
            this.btnAddMapping.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            this.btnAddMapping.ForeColor = Color.White;
            this.btnAddMapping.Location = new Point(10, 25);
            this.btnAddMapping.Size = new Size(750, 40);
            this.btnAddMapping.Text = "+ Add Data Mapping";
            this.btnAddMapping.UseVisualStyleBackColor = false;
            this.btnAddMapping.Click += new System.EventHandler(this.btnAddMapping_Click);
            
            this.flowMappings.AutoScroll = true;
            this.flowMappings.BackColor = Color.FromArgb(28, 28, 28);
            this.flowMappings.BorderStyle = BorderStyle.FixedSingle;
            this.flowMappings.FlowDirection = FlowDirection.TopDown;
            this.flowMappings.Location = new Point(10, 70);
            this.flowMappings.Size = new Size(750, 170);
            this.flowMappings.WrapContents = false;
            
            // ========== MONITORING GROUP ==========
            // 
            // grpMonitoring
            // 
            this.grpMonitoring.BackColor = Color.FromArgb(245, 245, 245);
            this.grpMonitoring.Controls.Add(this.lblPLCStatus);
            this.grpMonitoring.Controls.Add(this.lblSQLStatus);
            this.grpMonitoring.Controls.Add(this.btnStartMonitoring);
            this.grpMonitoring.Controls.Add(this.btnClearLogs);
            this.grpMonitoring.Controls.Add(this.txtLogs);
            this.grpMonitoring.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            this.grpMonitoring.Location = new Point(400, 240);
            this.grpMonitoring.Name = "grpMonitoring";
            this.grpMonitoring.Size = new Size(380, 100);
            this.grpMonitoring.TabIndex = 3;
            this.grpMonitoring.TabStop = false;
            this.grpMonitoring.Text = "Monitoring & Status";
            
            this.lblPLCStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblPLCStatus.Location = new Point(15, 25);
            this.lblPLCStatus.Size = new Size(150, 20);
            this.lblPLCStatus.Text = "PLC: Disconnected";
            this.lblPLCStatus.ForeColor = AppTheme.ErrorRed;
            
            this.lblSQLStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblSQLStatus.Location = new Point(15, 50);
            this.lblSQLStatus.Size = new Size(150, 20);
            this.lblSQLStatus.Text = "SQL: Disconnected";
            this.lblSQLStatus.ForeColor = AppTheme.ErrorRed;
            
            this.btnStartMonitoring.BackColor = AppTheme.SuccessGreen;
            this.btnStartMonitoring.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnStartMonitoring.ForeColor = Color.White;
            this.btnStartMonitoring.Location = new Point(180, 25);
            this.btnStartMonitoring.Size = new Size(120, 35);
            this.btnStartMonitoring.Text = "Start Monitoring";
            this.btnStartMonitoring.UseVisualStyleBackColor = false;
            this.btnStartMonitoring.Click += new System.EventHandler(this.btnStartMonitoring_Click);
            
            this.btnClearLogs.BackColor = Color.FromArgb(100, 100, 100);
            this.btnClearLogs.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnClearLogs.ForeColor = Color.White;
            this.btnClearLogs.Location = new Point(305, 25);
            this.btnClearLogs.Size = new Size(65, 35);
            this.btnClearLogs.Text = "Clear";
            this.btnClearLogs.UseVisualStyleBackColor = false;
            this.btnClearLogs.Click += new System.EventHandler(this.btnClearLogs_Click);
            
            this.txtLogs.BackColor = Color.Black;
            this.txtLogs.Font = new Font("Consolas", 9F);
            this.txtLogs.ForeColor = Color.Lime;
            this.txtLogs.Location = new Point(15, 70);
            this.txtLogs.Multiline = true;
            this.txtLogs.ReadOnly = true;
            this.txtLogs.ScrollBars = ScrollBars.Vertical;
            this.txtLogs.Size = new Size(355, 20);
            this.txtLogs.Visible = false;
            
            // 
            // SQLSyncControl
            // 
            this.BackColor = AppTheme.BackgroundLight;
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.grpSQLConfig);
            this.Controls.Add(this.grpGlobalHandshake);
            this.Controls.Add(this.grpDataMapping);
            this.Controls.Add(this.grpMonitoring);
            this.Name = "SQLSyncControl";
            this.Size = new Size(800, 600);
            
            this.grpSQLConfig.ResumeLayout(false);
            this.grpSQLConfig.PerformLayout();
            this.grpGlobalHandshake.ResumeLayout(false);
            this.grpGlobalHandshake.PerformLayout();
            this.grpDataMapping.ResumeLayout(false);
            this.grpMonitoring.ResumeLayout(false);
            this.grpMonitoring.PerformLayout();
            this.ResumeLayout(false);
        }
        
        private void rbUseExisting_CheckedChanged(object sender, System.EventArgs e)
        {
            cmbDatabases.Enabled = rbUseExisting.Checked;
            txtNewDBName.Enabled = !rbUseExisting.Checked;
        }
    }
}
