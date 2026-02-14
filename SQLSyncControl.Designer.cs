using System.Drawing;
using System.Windows.Forms;

namespace AutoLinkCore
{
    partial class SQLSyncControl
    {
        private System.ComponentModel.IContainer components = null;
        
        private Button btnBack;
        private Label lblTitle;
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
        
        private GroupBox grpHandshake;
        private Label lblLogBit;
        private TextBox txtLogBit;
        private Label lblConfirmBit;
        private TextBox txtConfirmBit;
        private Label lblDataBlock;
        private TextBox txtDataBlock;
        
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
            
            this.grpHandshake = new GroupBox();
            this.lblLogBit = new Label();
            this.txtLogBit = new TextBox();
            this.lblConfirmBit = new Label();
            this.txtConfirmBit = new TextBox();
            this.lblDataBlock = new Label();
            this.txtDataBlock = new TextBox();
            
            this.grpMonitoring = new GroupBox();
            this.lblPLCStatus = new Label();
            this.lblSQLStatus = new Label();
            this.btnStartMonitoring = new Button();
            this.btnClearLogs = new Button();
            this.txtLogs = new TextBox();
            
            this.grpSQLConfig.SuspendLayout();
            this.grpHandshake.SuspendLayout();
            this.grpMonitoring.SuspendLayout();
            this.SuspendLayout();
            
            // btnBack
            this.btnBack.Location = new Point(10, 10);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new Size(80, 30);
            this.btnBack.Text = "← Back";
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            
            // lblTitle
            this.lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            this.lblTitle.Location = new Point(300, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new Size(300, 30);
            this.lblTitle.Text = "SQL-PLC Synchronization";
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            
            // grpSQLConfig
            this.grpSQLConfig.Location = new Point(10, 50);
            this.grpSQLConfig.Name = "grpSQLConfig";
            this.grpSQLConfig.Size = new Size(380, 300);
            this.grpSQLConfig.Text = "SQL Server Configuration";
            
            this.lblServer.Location = new Point(10, 25);
            this.lblServer.Size = new Size(80, 20);
            this.lblServer.Text = "Server:";
            
            this.txtServer.Location = new Point(100, 25);
            this.txtServer.Size = new Size(260, 20);
            this.txtServer.Text = "localhost";
            
            this.lblUser.Location = new Point(10, 55);
            this.lblUser.Size = new Size(80, 20);
            this.lblUser.Text = "User ID:";
            
            this.txtUser.Location = new Point(100, 55);
            this.txtUser.Size = new Size(260, 20);
            this.txtUser.Text = "sa";
            
            this.lblPassword.Location = new Point(10, 85);
            this.lblPassword.Size = new Size(80, 20);
            this.lblPassword.Text = "Password:";
            
            this.txtPassword.Location = new Point(100, 85);
            this.txtPassword.Size = new Size(260, 20);
            this.txtPassword.PasswordChar = '*';
            
            this.rbUseExisting.Location = new Point(10, 120);
            this.rbUseExisting.Size = new Size(150, 20);
            this.rbUseExisting.Text = "Use Existing Database";
            this.rbUseExisting.Checked = true;
            this.rbUseExisting.CheckedChanged += new System.EventHandler(this.rbUseExisting_CheckedChanged);
            
            this.cmbDatabases.Location = new Point(30, 145);
            this.cmbDatabases.Size = new Size(330, 25);
            this.cmbDatabases.DropDownStyle = ComboBoxStyle.DropDownList;
            
            this.rbCreateNew.Location = new Point(10, 180);
            this.rbCreateNew.Size = new Size(150, 20);
            this.rbCreateNew.Text = "Create New Database";
            this.rbCreateNew.CheckedChanged += new System.EventHandler(this.rbCreateNew_CheckedChanged);
            
            this.txtNewDBName.Location = new Point(30, 205);
            this.txtNewDBName.Size = new Size(330, 20);
            this.txtNewDBName.Enabled = false;
            this.txtNewDBName.Text = "PLCDataSync";
            
            this.btnTestSQL.Location = new Point(10, 240);
            this.btnTestSQL.Size = new Size(175, 35);
            this.btnTestSQL.Text = "Test Connection";
            this.btnTestSQL.BackColor = AppTheme.AccentBlue;
            this.btnTestSQL.ForeColor = Color.White;
            this.btnTestSQL.Click += new System.EventHandler(this.btnTestSQL_Click);
            
            this.btnInitializeDB.Location = new Point(195, 240);
            this.btnInitializeDB.Size = new Size(175, 35);
            this.btnInitializeDB.Text = "Initialize Database";
            this.btnInitializeDB.BackColor = AppTheme.SuccessGreen;
            this.btnInitializeDB.ForeColor = Color.White;
            this.btnInitializeDB.Click += new System.EventHandler(this.btnInitializeDB_Click);
            
            this.grpSQLConfig.Controls.Add(this.lblServer);
            this.grpSQLConfig.Controls.Add(this.txtServer);
            this.grpSQLConfig.Controls.Add(this.lblUser);
            this.grpSQLConfig.Controls.Add(this.txtUser);
            this.grpSQLConfig.Controls.Add(this.lblPassword);
            this.grpSQLConfig.Controls.Add(this.txtPassword);
            this.grpSQLConfig.Controls.Add(this.rbUseExisting);
            this.grpSQLConfig.Controls.Add(this.cmbDatabases);
            this.grpSQLConfig.Controls.Add(this.rbCreateNew);
            this.grpSQLConfig.Controls.Add(this.txtNewDBName);
            this.grpSQLConfig.Controls.Add(this.btnTestSQL);
            this.grpSQLConfig.Controls.Add(this.btnInitializeDB);
            
            // grpHandshake
            this.grpHandshake.Location = new Point(400, 50);
            this.grpHandshake.Name = "grpHandshake";
            this.grpHandshake.Size = new Size(380, 180);
            this.grpHandshake.Text = "Handshake Configuration";
            
            this.lblLogBit.Location = new Point(10, 30);
            this.lblLogBit.Size = new Size(120, 20);
            this.lblLogBit.Text = "Log Trigger Bit:";
            
            this.txtLogBit.Location = new Point(140, 30);
            this.txtLogBit.Size = new Size(220, 20);
            this.txtLogBit.Text = "DB1.DBX10.0";
            
            this.lblConfirmBit.Location = new Point(10, 70);
            this.lblConfirmBit.Size = new Size(120, 20);
            this.lblConfirmBit.Text = "Confirmation Bit:";
            
            this.txtConfirmBit.Location = new Point(140, 70);
            this.txtConfirmBit.Size = new Size(220, 20);
            this.txtConfirmBit.Text = "DB1.DBX10.1";
            
            this.lblDataBlock.Location = new Point(10, 110);
            this.lblDataBlock.Size = new Size(120, 20);
            this.lblDataBlock.Text = "Data Block Range:";
            
            this.txtDataBlock.Location = new Point(140, 110);
            this.txtDataBlock.Size = new Size(220, 20);
            this.txtDataBlock.Text = "DB1.DBB0-20";
            
            Label lblHelp = new Label();
            lblHelp.Location = new Point(10, 140);
            lblHelp.Size = new Size(360, 30);
            lblHelp.Text = "Format: DB[num].DBX[byte].[bit] for bits\r\n         DB[num].DBB[start]-[length] for data";
            lblHelp.Font = new Font("Segoe UI", 7.5F);
            lblHelp.ForeColor = Color.Gray;
            
            this.grpHandshake.Controls.Add(this.lblLogBit);
            this.grpHandshake.Controls.Add(this.txtLogBit);
            this.grpHandshake.Controls.Add(this.lblConfirmBit);
            this.grpHandshake.Controls.Add(this.txtConfirmBit);
            this.grpHandshake.Controls.Add(this.lblDataBlock);
            this.grpHandshake.Controls.Add(this.txtDataBlock);
            this.grpHandshake.Controls.Add(lblHelp);
            
            // grpMonitoring
            this.grpMonitoring.Location = new Point(10, 360);
            this.grpMonitoring.Name = "grpMonitoring";
            this.grpMonitoring.Size = new Size(770, 280);
            this.grpMonitoring.Text = "Monitoring & Logs";
            
            this.lblPLCStatus.Location = new Point(10, 25);
            this.lblPLCStatus.Size = new Size(150, 25);
            this.lblPLCStatus.Text = "PLC: Disconnected";
            this.lblPLCStatus.ForeColor = AppTheme.ErrorRed;
            this.lblPLCStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            
            this.lblSQLStatus.Location = new Point(170, 25);
            this.lblSQLStatus.Size = new Size(150, 25);
            this.lblSQLStatus.Text = "SQL: Disconnected";
            this.lblSQLStatus.ForeColor = AppTheme.ErrorRed;
            this.lblSQLStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            
            this.btnStartMonitoring.Location = new Point(500, 20);
            this.btnStartMonitoring.Size = new Size(130, 35);
            this.btnStartMonitoring.Text = "Start Monitoring";
            this.btnStartMonitoring.BackColor = AppTheme.SuccessGreen;
            this.btnStartMonitoring.ForeColor = Color.White;
            this.btnStartMonitoring.Click += new System.EventHandler(this.btnStartMonitoring_Click);
            
            this.btnClearLogs.Location = new Point(640, 20);
            this.btnClearLogs.Size = new Size(110, 35);
            this.btnClearLogs.Text = "Clear Logs";
            this.btnClearLogs.BackColor = Color.Gray;
            this.btnClearLogs.ForeColor = Color.White;
            this.btnClearLogs.Click += new System.EventHandler(this.btnClearLogs_Click);
            
            this.txtLogs.Location = new Point(10, 65);
            this.txtLogs.Size = new Size(750, 200);
            this.txtLogs.Multiline = true;
            this.txtLogs.ScrollBars = ScrollBars.Vertical;
            this.txtLogs.ReadOnly = true;
            this.txtLogs.BackColor = Color.Black;
            this.txtLogs.ForeColor = Color.Lime;
            this.txtLogs.Font = new Font("Consolas", 9F);
            
            this.grpMonitoring.Controls.Add(this.lblPLCStatus);
            this.grpMonitoring.Controls.Add(this.lblSQLStatus);
            this.grpMonitoring.Controls.Add(this.btnStartMonitoring);
            this.grpMonitoring.Controls.Add(this.btnClearLogs);
            this.grpMonitoring.Controls.Add(this.txtLogs);
            
            // grpHandshake additional info
            Label lblHandshakeInfo = new Label();
            lblHandshakeInfo.Location = new Point(400, 240);
            lblHandshakeInfo.Size = new Size(380, 110);
            lblHandshakeInfo.Text = "Handshake Protocol:\r\n" +
                "1. PLC sets Log Bit = 1\r\n" +
                "2. App reads data → writes to SQL\r\n" +
                "3. App sets Confirm Bit = 1\r\n" +
                "4. PLC sets Log Bit = 0\r\n" +
                "5. App sets Confirm Bit = 0";
            lblHandshakeInfo.Font = new Font("Segoe UI", 8.5F);
            lblHandshakeInfo.ForeColor = AppTheme.AccentBlue;
            lblHandshakeInfo.BorderStyle = BorderStyle.FixedSingle;
            lblHandshakeInfo.Padding = new Padding(5);
            
            // SQLSyncControl
            this.BackColor = AppTheme.BackgroundLight;
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.grpSQLConfig);
            this.Controls.Add(this.grpHandshake);
            this.Controls.Add(this.grpMonitoring);
            this.Controls.Add(lblHandshakeInfo);
            this.Name = "SQLSyncControl";
            this.Size = new Size(800, 650);
            
            this.grpSQLConfig.ResumeLayout(false);
            this.grpHandshake.ResumeLayout(false);
            this.grpMonitoring.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
