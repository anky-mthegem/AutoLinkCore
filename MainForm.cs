using System;
using System.Windows.Forms;

namespace AutoLinkCore
{
    public partial class MainForm : Form
    {
        private Panel contentPanel;
        private ConnectionSettingsControl connectionSettingsControl;
        private PLCDiagnosticsControl diagnosticsControl;
        private SQLSyncControl sqlSyncControl;
        
        public MainForm()
        {
            InitializeComponent();
            InitializeNavigationPanel();
            AppTheme.ApplyTheme(this);
        }
        
        private void InitializeNavigationPanel()
        {
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.BackgroundLight
            };
            this.Controls.Add(contentPanel);
            contentPanel.BringToFront();
            
            connectionSettingsControl = new ConnectionSettingsControl
            {
                Dock = DockStyle.Fill,
                Visible = false
            };
            contentPanel.Controls.Add(connectionSettingsControl);
            
            diagnosticsControl = new PLCDiagnosticsControl
            {
                Dock = DockStyle.Fill,
                Visible = false
            };
            contentPanel.Controls.Add(diagnosticsControl);
            
            sqlSyncControl = new SQLSyncControl
            {
                Dock = DockStyle.Fill,
                Visible = false
            };
            contentPanel.Controls.Add(sqlSyncControl);
        }
        
        public void ShowConnectionSettings()
        {
            diagnosticsControl.Visible = false;
            sqlSyncControl.Visible = false;
            connectionSettingsControl.Visible = true;
            connectionSettingsControl.BringToFront();
        }
        
        public void ShowDiagnostics(S7.Net.Plc plc, string ipAddress, short rack, short slot)
        {
            connectionSettingsControl.Visible = false;
            sqlSyncControl.Visible = false;
            diagnosticsControl.SetConnectionInfo(ipAddress, rack, slot);
            diagnosticsControl.SetPLC(plc);
            diagnosticsControl.Visible = true;
            diagnosticsControl.BringToFront();
        }
        
        public void ShowMainDashboard()
        {
            connectionSettingsControl.Visible = false;
            diagnosticsControl.Visible = false;
            sqlSyncControl.Visible = false;
        }
        
        private void connectDisconnectPLCMenuItem_Click(object sender, EventArgs e)
        {
            ShowConnectionSettings();
        }
        
        private void dashboardMenuItem_Click(object sender, EventArgs e)
        {
            ShowMainDashboard();
        }
        
        private void connectionSettingsMenuItem_Click(object sender, EventArgs e)
        {
            ShowConnectionSettings();
        }
        
        private void diagnosticsMenuItem_Click(object sender, EventArgs e)
        {
            connectionSettingsControl.Visible = false;
            sqlSyncControl.Visible = false;
            diagnosticsControl.Visible = true;
            diagnosticsControl.BringToFront();
        }
        
        private void sqlSyncMenuItem_Click(object sender, EventArgs e)
        {
            connectionSettingsControl.Visible = false;
            diagnosticsControl.Visible = false;
            sqlSyncControl.Visible = true;
            sqlSyncControl.BringToFront();
            
            // Pass PLC connection if available
            if (connectionSettingsControl.PLCConnection != null)
            {
                sqlSyncControl.SetPLC(connectionSettingsControl.PLCConnection);
            }
        }
    }
}
