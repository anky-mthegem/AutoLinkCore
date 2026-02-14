namespace AutoLinkCore
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileMenu;
        private System.Windows.Forms.ToolStripMenuItem connectDisconnectPLCMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportLogsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem userLoginLogoutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewMenu;
        private System.Windows.Forms.ToolStripMenuItem dashboardMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectionSettingsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem diagnosticsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sqlSyncMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fullscreenModeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toggleSidebarMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsMenu;
        private System.Windows.Forms.ToolStripMenuItem plcIPAddressMenuItem;
        private System.Windows.Forms.ToolStripMenuItem communicationTimeoutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem databaseConfigMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsMenu;
        private System.Windows.Forms.ToolStripMenuItem manualOperationMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ioSimulatorMenuItem;
        private System.Windows.Forms.ToolStripMenuItem backupRestoreRecipeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpMenu;
        private System.Windows.Forms.ToolStripMenuItem documentationMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem licenseInfoMenuItem;

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
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.connectDisconnectPLCMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportLogsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userLoginLogoutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.dashboardMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectionSettingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.diagnosticsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sqlSyncMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fullscreenModeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleSidebarMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.plcIPAddressMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.communicationTimeoutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.databaseConfigMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.manualOperationMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ioSimulatorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backupRestoreRecipeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.documentationMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.licenseInfoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenu,
            this.viewMenu,
            this.settingsMenu,
            this.toolsMenu,
            this.helpMenu});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(800, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip";
            // 
            // fileMenu
            // 
            this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectDisconnectPLCMenuItem,
            this.exportLogsMenuItem,
            this.userLoginLogoutMenuItem,
            this.exitMenuItem});
            this.fileMenu.Name = "fileMenu";
            this.fileMenu.Size = new System.Drawing.Size(37, 20);
            this.fileMenu.Text = "File";
            // 
            // connectDisconnectPLCMenuItem
            // 
            this.connectDisconnectPLCMenuItem.Name = "connectDisconnectPLCMenuItem";
            this.connectDisconnectPLCMenuItem.Size = new System.Drawing.Size(197, 22);
            this.connectDisconnectPLCMenuItem.Text = "Connect/Disconnect PLC";
            this.connectDisconnectPLCMenuItem.Click += new System.EventHandler(this.connectDisconnectPLCMenuItem_Click);
            // 
            // exportLogsMenuItem
            // 
            this.exportLogsMenuItem.Name = "exportLogsMenuItem";
            this.exportLogsMenuItem.Size = new System.Drawing.Size(197, 22);
            this.exportLogsMenuItem.Text = "Export Logs (.csv/.pdf)";
            // 
            // userLoginLogoutMenuItem
            // 
            this.userLoginLogoutMenuItem.Name = "userLoginLogoutMenuItem";
            this.userLoginLogoutMenuItem.Size = new System.Drawing.Size(197, 22);
            this.userLoginLogoutMenuItem.Text = "User Login/Logout";
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(197, 22);
            this.exitMenuItem.Text = "Exit";
            // 
            // viewMenu
            // 
            this.viewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dashboardMenuItem,
            this.connectionSettingsMenuItem,
            this.diagnosticsMenuItem,
            this.sqlSyncMenuItem,
            this.fullscreenModeMenuItem,
            this.toggleSidebarMenuItem});
            this.viewMenu.Name = "viewMenu";
            this.viewMenu.Size = new System.Drawing.Size(44, 20);
            this.viewMenu.Text = "View";
            // 
            // dashboardMenuItem
            // 
            this.dashboardMenuItem.Name = "dashboardMenuItem";
            this.dashboardMenuItem.Size = new System.Drawing.Size(180, 22);
            this.dashboardMenuItem.Text = "Dashboard";
            this.dashboardMenuItem.Click += new System.EventHandler(this.dashboardMenuItem_Click);
            // 
            // connectionSettingsMenuItem
            // 
            this.connectionSettingsMenuItem.Name = "connectionSettingsMenuItem";
            this.connectionSettingsMenuItem.Size = new System.Drawing.Size(180, 22);
            this.connectionSettingsMenuItem.Text = "Connection Settings";
            this.connectionSettingsMenuItem.Click += new System.EventHandler(this.connectionSettingsMenuItem_Click);
            // 
            // diagnosticsMenuItem
            // 
            this.diagnosticsMenuItem.Name = "diagnosticsMenuItem";
            this.diagnosticsMenuItem.Size = new System.Drawing.Size(180, 22);
            this.diagnosticsMenuItem.Text = "Diagnostics";
            this.diagnosticsMenuItem.Click += new System.EventHandler(this.diagnosticsMenuItem_Click);
            // 
            // sqlSyncMenuItem
            // 
            this.sqlSyncMenuItem.Name = "sqlSyncMenuItem";
            this.sqlSyncMenuItem.Size = new System.Drawing.Size(180, 22);
            this.sqlSyncMenuItem.Text = "SQL Synchronization";
            this.sqlSyncMenuItem.Click += new System.EventHandler(this.sqlSyncMenuItem_Click);
            // 
            // fullscreenModeMenuItem
            // 
            this.fullscreenModeMenuItem.Name = "fullscreenModeMenuItem";
            this.fullscreenModeMenuItem.Size = new System.Drawing.Size(157, 22);
            this.fullscreenModeMenuItem.Text = "Fullscreen Mode";
            // 
            // toggleSidebarMenuItem
            // 
            this.toggleSidebarMenuItem.Name = "toggleSidebarMenuItem";
            this.toggleSidebarMenuItem.Size = new System.Drawing.Size(157, 22);
            this.toggleSidebarMenuItem.Text = "Toggle Sidebar";
            // 
            // settingsMenu
            // 
            this.settingsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.plcIPAddressMenuItem,
            this.communicationTimeoutMenuItem,
            this.databaseConfigMenuItem});
            this.settingsMenu.Name = "settingsMenu";
            this.settingsMenu.Size = new System.Drawing.Size(61, 20);
            this.settingsMenu.Text = "Settings";
            // 
            // plcIPAddressMenuItem
            // 
            this.plcIPAddressMenuItem.Name = "plcIPAddressMenuItem";
            this.plcIPAddressMenuItem.Size = new System.Drawing.Size(215, 22);
            this.plcIPAddressMenuItem.Text = "PLC IP Address";
            // 
            // communicationTimeoutMenuItem
            // 
            this.communicationTimeoutMenuItem.Name = "communicationTimeoutMenuItem";
            this.communicationTimeoutMenuItem.Size = new System.Drawing.Size(215, 22);
            this.communicationTimeoutMenuItem.Text = "Communication Timeout";
            // 
            // databaseConfigMenuItem
            // 
            this.databaseConfigMenuItem.Name = "databaseConfigMenuItem";
            this.databaseConfigMenuItem.Size = new System.Drawing.Size(215, 22);
            this.databaseConfigMenuItem.Text = "Database (MS SQL) Config";
            // 
            // toolsMenu
            // 
            this.toolsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.manualOperationMenuItem,
            this.ioSimulatorMenuItem,
            this.backupRestoreRecipeMenuItem});
            this.toolsMenu.Name = "toolsMenu";
            this.toolsMenu.Size = new System.Drawing.Size(46, 20);
            this.toolsMenu.Text = "Tools";
            // 
            // manualOperationMenuItem
            // 
            this.manualOperationMenuItem.Name = "manualOperationMenuItem";
            this.manualOperationMenuItem.Size = new System.Drawing.Size(186, 22);
            this.manualOperationMenuItem.Text = "Manual Operation";
            // 
            // ioSimulatorMenuItem
            // 
            this.ioSimulatorMenuItem.Name = "ioSimulatorMenuItem";
            this.ioSimulatorMenuItem.Size = new System.Drawing.Size(186, 22);
            this.ioSimulatorMenuItem.Text = "I/O Simulator";
            // 
            // backupRestoreRecipeMenuItem
            // 
            this.backupRestoreRecipeMenuItem.Name = "backupRestoreRecipeMenuItem";
            this.backupRestoreRecipeMenuItem.Size = new System.Drawing.Size(186, 22);
            this.backupRestoreRecipeMenuItem.Text = "Backup/Restore Recipe";
            // 
            // helpMenu
            // 
            this.helpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.documentationMenuItem,
            this.aboutMenuItem,
            this.licenseInfoMenuItem});
            this.helpMenu.Name = "helpMenu";
            this.helpMenu.Size = new System.Drawing.Size(44, 20);
            this.helpMenu.Text = "Help";
            // 
            // documentationMenuItem
            // 
            this.documentationMenuItem.Name = "documentationMenuItem";
            this.documentationMenuItem.Size = new System.Drawing.Size(180, 22);
            this.documentationMenuItem.Text = "Documentation";
            // 
            // aboutMenuItem
            // 
            this.aboutMenuItem.Name = "aboutMenuItem";
            this.aboutMenuItem.Size = new System.Drawing.Size(180, 22);
            this.aboutMenuItem.Text = "About (App Version)";
            // 
            // licenseInfoMenuItem
            // 
            this.licenseInfoMenuItem.Name = "licenseInfoMenuItem";
            this.licenseInfoMenuItem.Size = new System.Drawing.Size(180, 22);
            this.licenseInfoMenuItem.Text = "License Info";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "AutoLink Core";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
