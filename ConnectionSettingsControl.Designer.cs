namespace AutoLinkCore
{
    partial class ConnectionSettingsControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (plc != null && plc.IsConnected)
                {
                    plc.Close();
                }
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnBack = new System.Windows.Forms.Button();
            this.cardPanel = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblPLCName = new System.Windows.Forms.Label();
            this.txtPLCName = new System.Windows.Forms.TextBox();
            this.lblIPAddress = new System.Windows.Forms.Label();
            this.txtIPAddress = new System.Windows.Forms.TextBox();
            this.lblRack = new System.Windows.Forms.Label();
            this.txtRack = new System.Windows.Forms.TextBox();
            this.lblSlot = new System.Windows.Forms.Label();
            this.txtSlot = new System.Windows.Forms.TextBox();
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblCPUState = new System.Windows.Forms.Label();
            this.progressSpinner = new System.Windows.Forms.ProgressBar();
            this.lblErrorMessage = new System.Windows.Forms.Label();
            this.btnPing = new System.Windows.Forms.Button();
            this.lblPingStatus = new System.Windows.Forms.Label();
            this.picPLC = new System.Windows.Forms.PictureBox();
            this.cardPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // cardPanel
            // 
            this.cardPanel.BackColor = System.Drawing.Color.White;
            this.cardPanel.Controls.Add(this.lblTitle);
            this.cardPanel.Controls.Add(this.lblPLCName);
            this.cardPanel.Controls.Add(this.txtPLCName);
            this.cardPanel.Controls.Add(this.lblIPAddress);
            this.cardPanel.Controls.Add(this.txtIPAddress);
            this.cardPanel.Controls.Add(this.lblRack);
            this.cardPanel.Controls.Add(this.txtRack);
            this.cardPanel.Controls.Add(this.lblSlot);
            this.cardPanel.Controls.Add(this.txtSlot);
            this.cardPanel.Controls.Add(this.btnTestConnection);
            this.cardPanel.Controls.Add(this.lblStatus);
            this.cardPanel.Controls.Add(this.lblCPUState);
            this.cardPanel.Controls.Add(this.progressSpinner);
            this.cardPanel.Controls.Add(this.lblErrorMessage);
            this.cardPanel.Controls.Add(this.btnPing);
            this.cardPanel.Controls.Add(this.lblPingStatus);
            this.cardPanel.Controls.Add(this.picPLC);
            this.cardPanel.Location = new System.Drawing.Point(150, 50);
            this.cardPanel.Name = "cardPanel";
            this.cardPanel.Size = new System.Drawing.Size(500, 480);
            this.cardPanel.TabIndex = 0;
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnBack.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnBack.ForeColor = System.Drawing.Color.White;
            this.btnBack.Location = new System.Drawing.Point(20, 20);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(100, 35);
            this.btnBack.TabIndex = 1;
            this.btnBack.Text = "← Back";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F);
            this.lblTitle.Location = new System.Drawing.Point(30, 30);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(250, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "PLC Connection Settings";
            // 
            // lblPLCName
            // 
            this.lblPLCName.AutoSize = true;
            this.lblPLCName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblPLCName.Location = new System.Drawing.Point(30, 100);
            this.lblPLCName.Name = "lblPLCName";
            this.lblPLCName.Size = new System.Drawing.Size(75, 19);
            this.lblPLCName.TabIndex = 1;
            this.lblPLCName.Text = "PLC Name:";
            // 
            // txtPLCName
            // 
            this.txtPLCName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtPLCName.Location = new System.Drawing.Point(180, 97);
            this.txtPLCName.Name = "txtPLCName";
            this.txtPLCName.Size = new System.Drawing.Size(290, 25);
            this.txtPLCName.TabIndex = 2;
            this.txtPLCName.Text = "S7-1200";
            // 
            // lblIPAddress
            // 
            this.lblIPAddress.AutoSize = true;
            this.lblIPAddress.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblIPAddress.Location = new System.Drawing.Point(30, 145);
            this.lblIPAddress.Name = "lblIPAddress";
            this.lblIPAddress.Size = new System.Drawing.Size(75, 19);
            this.lblIPAddress.TabIndex = 3;
            this.lblIPAddress.Text = "IP Address:";
            // 
            // txtIPAddress
            // 
            this.txtIPAddress.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtIPAddress.Location = new System.Drawing.Point(180, 142);
            this.txtIPAddress.Name = "txtIPAddress";
            this.txtIPAddress.Size = new System.Drawing.Size(200, 25);
            this.txtIPAddress.TabIndex = 4;
            this.txtIPAddress.Text = "192.168.0.1";
            // 
            // btnPing
            // 
            this.btnPing.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnPing.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnPing.ForeColor = System.Drawing.Color.White;
            this.btnPing.Location = new System.Drawing.Point(385, 142);
            this.btnPing.Name = "btnPing";
            this.btnPing.Size = new System.Drawing.Size(85, 25);
            this.btnPing.TabIndex = 14;
            this.btnPing.Text = "Ping";
            this.btnPing.UseVisualStyleBackColor = false;
            this.btnPing.Click += new System.EventHandler(this.btnPing_Click);
            // 
            // lblRack
            // 
            this.lblRack.AutoSize = true;
            this.lblRack.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblRack.Location = new System.Drawing.Point(30, 190);
            this.lblRack.Name = "lblRack";
            this.lblRack.Size = new System.Drawing.Size(40, 19);
            this.lblRack.TabIndex = 5;
            this.lblRack.Text = "Rack:";
            // 
            // txtRack
            // 
            this.txtRack.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtRack.Location = new System.Drawing.Point(180, 187);
            this.txtRack.Name = "txtRack";
            this.txtRack.Size = new System.Drawing.Size(290, 25);
            this.txtRack.TabIndex = 6;
            this.txtRack.Text = "0";
            // 
            // lblSlot
            // 
            this.lblSlot.AutoSize = true;
            this.lblSlot.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSlot.Location = new System.Drawing.Point(30, 235);
            this.lblSlot.Name = "lblSlot";
            this.lblSlot.Size = new System.Drawing.Size(35, 19);
            this.lblSlot.TabIndex = 7;
            this.lblSlot.Text = "Slot:";
            // 
            // txtSlot
            // 
            this.txtSlot.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtSlot.Location = new System.Drawing.Point(180, 232);
            this.txtSlot.Name = "txtSlot";
            this.txtSlot.Size = new System.Drawing.Size(290, 25);
            this.txtSlot.TabIndex = 8;
            this.txtSlot.Text = "1";
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnTestConnection.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnTestConnection.ForeColor = System.Drawing.Color.White;
            this.btnTestConnection.Location = new System.Drawing.Point(180, 280);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(290, 35);
            this.btnTestConnection.TabIndex = 9;
            this.btnTestConnection.Text = "Test Connection";
            this.btnTestConnection.UseVisualStyleBackColor = false;
            this.btnTestConnection.Click += new System.EventHandler(this.btnTestConnection_Click);
            // 
            // progressSpinner
            // 
            this.progressSpinner.Location = new System.Drawing.Point(180, 325);
            this.progressSpinner.Name = "progressSpinner";
            this.progressSpinner.Size = new System.Drawing.Size(290, 10);
            this.progressSpinner.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressSpinner.TabIndex = 10;
            this.progressSpinner.Visible = false;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblStatus.Location = new System.Drawing.Point(180, 345);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 21);
            this.lblStatus.TabIndex = 11;
            // 
            // lblCPUState
            // 
            this.lblCPUState.AutoSize = true;
            this.lblCPUState.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblCPUState.Location = new System.Drawing.Point(180, 375);
            this.lblCPUState.Name = "lblCPUState";
            this.lblCPUState.Size = new System.Drawing.Size(0, 19);
            this.lblCPUState.TabIndex = 12;
            // 
            // lblErrorMessage
            // 
            this.lblErrorMessage.AutoSize = true;
            this.lblErrorMessage.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblErrorMessage.Location = new System.Drawing.Point(180, 405);
            this.lblErrorMessage.Name = "lblErrorMessage";
            this.lblErrorMessage.Size = new System.Drawing.Size(0, 15);
            this.lblErrorMessage.TabIndex = 13;
            this.lblErrorMessage.Visible = false;
            // 
            // lblPingStatus
            // 
            this.lblPingStatus.AutoSize = true;
            this.lblPingStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPingStatus.Location = new System.Drawing.Point(180, 170);
            this.lblPingStatus.Name = "lblPingStatus";
            this.lblPingStatus.Size = new System.Drawing.Size(0, 15);
            this.lblPingStatus.TabIndex = 15;
            // 
            // picPLC
            // 
            this.picPLC.Location = new System.Drawing.Point(30, 280);
            this.picPLC.Name = "picPLC";
            this.picPLC.Size = new System.Drawing.Size(440, 180);
            this.picPLC.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picPLC.TabIndex = 16;
            this.picPLC.TabStop = false;
            // 
            // ConnectionSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.Controls.Add(this.cardPanel);
            this.Controls.Add(this.btnBack);
            this.Name = "ConnectionSettingsControl";
            this.Size = new System.Drawing.Size(800, 600);
            ((System.ComponentModel.ISupportInitialize)(this.picPLC)).EndInit();
            this.cardPanel.ResumeLayout(false);
            this.cardPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Panel cardPanel;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblPLCName;
        private System.Windows.Forms.TextBox txtPLCName;
        private System.Windows.Forms.Label lblIPAddress;
        private System.Windows.Forms.TextBox txtIPAddress;
        private System.Windows.Forms.Label lblRack;
        private System.Windows.Forms.TextBox txtRack;
        private System.Windows.Forms.Label lblSlot;
        private System.Windows.Forms.TextBox txtSlot;
        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblCPUState;
        private System.Windows.Forms.ProgressBar progressSpinner;
        private System.Windows.Forms.Label lblErrorMessage;
        private System.Windows.Forms.Button btnPing;
        private System.Windows.Forms.Label lblPingStatus;
        private System.Windows.Forms.PictureBox picPLC;
    }
}
