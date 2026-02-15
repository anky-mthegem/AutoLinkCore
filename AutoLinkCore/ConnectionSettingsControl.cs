using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net;
using System.Net.NetworkInformation;
using Sharp7;
using System.Threading.Tasks;

namespace AutoLinkCore
{
    public partial class ConnectionSettingsControl : UserControl
    {
        private Sharp7PLCClient plc;
        private Timer statusRefreshTimer;
        
        public Sharp7PLCClient PLCConnection
        {
            get { return plc; }
        }
        
        public ConnectionSettingsControl()
        {
            InitializeComponent();
            AppTheme.ApplyTheme(this);
            LoadPLCImage();
            
            // Initialize status refresh timer
            statusRefreshTimer = new Timer();
            statusRefreshTimer.Interval = 2000; // Refresh every 2 seconds
            statusRefreshTimer.Tick += (sender, e) => RefreshCPUStatus();
            statusRefreshTimer.Start(); // Start timer immediately to monitor connection status
        }
        
        private void LoadPLCImage()
        {
            try
            {
                string imagePath = System.IO.Path.Combine(Application.StartupPath, @"..\..\..\..\s7-1200.png");
                if (System.IO.File.Exists(imagePath))
                {
                    picPLC.Image = Image.FromFile(imagePath);
                }
                else
                {
                    CreatePlaceholderImage();
                }
            }
            catch
            {
                CreatePlaceholderImage();
            }
        }
        
        private void CreatePlaceholderImage()
        {
            Bitmap bmp = new Bitmap(440, 180);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(240, 240, 240)), 0, 0, 440, 180);
                g.DrawRectangle(new Pen(Color.FromArgb(200, 200, 200), 2), 1, 1, 438, 178);
                g.FillRectangle(new SolidBrush(Color.FromArgb(50, 50, 50)), 50, 40, 340, 100);
                g.FillRectangle(new SolidBrush(Color.FromArgb(0, 120, 215)), 60, 50, 60, 80);
                g.FillRectangle(new SolidBrush(Color.FromArgb(100, 100, 100)), 130, 50, 250, 80);
                g.FillEllipse(new SolidBrush(Color.FromArgb(0, 255, 0)), 70, 60, 8, 8);
                g.FillEllipse(new SolidBrush(Color.FromArgb(255, 200, 0)), 85, 60, 8, 8);
                Font titleFont = new Font("Segoe UI", 16, FontStyle.Bold);
                Font subtitleFont = new Font("Segoe UI", 10);
                g.DrawString("SIEMENS S7-1200", titleFont, Brushes.Black, new PointF(120, 150));
                g.DrawString("Placeholder Image", subtitleFont, Brushes.Gray, new PointF(160, 10));
                titleFont.Dispose();
                subtitleFont.Dispose();
            }
            picPLC.Image = bmp;
        }
        
        private void btnBack_Click(object sender, EventArgs e)
        {
            MainForm mainForm = this.FindForm() as MainForm;
            if (mainForm != null)
            {
                mainForm.ShowMainDashboard();
            }
        }
        
        private async void btnPing_Click(object sender, EventArgs e)
        {
            if (!ValidateIPAddress(txtIPAddress.Text))
            {
                lblPingStatus.Text = "Invalid IP Address";
                lblPingStatus.ForeColor = AppTheme.ErrorRed;
                return;
            }
            
            lblPingStatus.Text = "Pinging...";
            lblPingStatus.ForeColor = AppTheme.AccentBlue;
            btnPing.Enabled = false;
            
            await Task.Run(() => PingHost());
        }
        
        private void PingHost()
        {
            try
            {
                using (Ping pingSender = new Ping())
                {
                    PingReply reply = pingSender.Send(txtIPAddress.Text, 3000);
                    
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(() => UpdatePingUI(reply)));
                    }
                    else
                    {
                        UpdatePingUI(reply);
                    }
                }
            }
            catch (Exception ex)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => UpdatePingUIError(ex.Message)));
                }
                else
                {
                    UpdatePingUIError(ex.Message);
                }
            }
        }
        
        private void UpdatePingUI(PingReply reply)
        {
            btnPing.Enabled = true;
            
            if (reply.Status == IPStatus.Success)
            {
                lblPingStatus.Text = "Ping successful - " + reply.RoundtripTime + " ms";
                lblPingStatus.ForeColor = AppTheme.SuccessGreen;
            }
            else
            {
                lblPingStatus.Text = "Ping failed - " + reply.Status.ToString();
                lblPingStatus.ForeColor = AppTheme.ErrorRed;
            }
        }
        
        private void UpdatePingUIError(string error)
        {
            btnPing.Enabled = true;
            lblPingStatus.Text = "Ping failed - " + error;
            lblPingStatus.ForeColor = AppTheme.ErrorRed;
        }
        
        private async void btnTestConnection_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Connecting...";
            lblStatus.ForeColor = AppTheme.AccentBlue;
            lblCPUState.Visible = false;
            lblErrorMessage.Visible = false;
            progressSpinner.Visible = true;
            btnTestConnection.Enabled = false;
            
            await Task.Run(() => TestPLCConnection());
        }
        
        private void TestPLCConnection()
        {
            try
            {
                if (!ValidateIPAddress(txtIPAddress.Text))
                {
                    UpdateUIOnError("Invalid IP Address");
                    return;
                }
                
                int rack = 0;
                int slot = 1;
                
                if (!int.TryParse(txtRack.Text, out rack))
                {
                    UpdateUIOnError("Invalid Rack number");
                    return;
                }
                
                if (!int.TryParse(txtSlot.Text, out slot))
                {
                    UpdateUIOnError("Invalid Slot number");
                    return;
                }
                
                plc = new Sharp7PLCClient();
                
                if (plc.Connect(txtIPAddress.Text, rack, slot))
                {
                    UpdateUIOnSuccess(txtIPAddress.Text, rack, slot);
                }
                else
                {
                    UpdateUIOnError(plc.LastError);
                }
            }
            catch (Exception ex)
            {
                UpdateUIOnError("Port 102 Unreachable");
            }
        }
        
        private void UpdateUIOnSuccess(string ipAddress, int rack, int slot)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string, int, int>(UpdateUIOnSuccess), ipAddress, rack, slot);
                return;
            }
            
            progressSpinner.Visible = false;
            btnTestConnection.Enabled = true;
            lblStatus.Text = "Connected";
            lblStatus.ForeColor = AppTheme.SuccessGreen;
            
            // Get actual CPU state instead of hardcoding RUN
            if (plc != null && plc.GetCPUStatus(out bool isRunning))
            {
                string cpuState = isRunning ? "RUN" : "STOP";
                lblCPUState.Text = $"CPU State: {cpuState}";
                lblCPUState.ForeColor = isRunning ? AppTheme.SuccessGreen : AppTheme.ErrorRed;
            }
            else
            {
                lblCPUState.Text = "CPU State: UNKNOWN";
                lblCPUState.ForeColor = AppTheme.ForegroundDark;
            }
            
            lblCPUState.Visible = true;
            lblErrorMessage.Visible = false;
            
            MainForm mainForm = this.FindForm() as MainForm;
            if (mainForm != null && plc != null && plc.IsConnected)
            {
                mainForm.ShowDiagnostics(plc, ipAddress, rack, slot);
            }
        }
        
        private void UpdateUIOnError(string errorMessage)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(UpdateUIOnError), errorMessage);
                return;
            }
            
            progressSpinner.Visible = false;
            btnTestConnection.Enabled = true;
            lblStatus.Text = "Connection Failed";
            lblStatus.ForeColor = AppTheme.ErrorRed;
            lblErrorMessage.Text = errorMessage;
            lblErrorMessage.ForeColor = AppTheme.ErrorRed;
            lblErrorMessage.Visible = true;
            lblCPUState.Visible = false;
            // Timer continues running to monitor for reconnection
        }
        
        private void RefreshCPUStatus()
        {
            // Check connection status first
            if (plc == null)
            {
                if (statusRefreshTimer != null && statusRefreshTimer.Enabled)
                {
                    statusRefreshTimer.Stop();
                }
                return;
            }
            
            // Update connection status and CPU state periodically
            Task.Run(() =>
            {
                bool isConnected = plc.IsConnected;
                
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        // Update connection status
                        if (isConnected)
                        {
                            lblStatus.Text = "Connected";
                            lblStatus.ForeColor = AppTheme.SuccessGreen;
                        }
                        else
                        {
                            lblStatus.Text = "Disconnected";
                            lblStatus.ForeColor = AppTheme.ErrorRed;
                            lblCPUState.Visible = false;
                        }
                    }));
                }
                else
                {
                    if (isConnected)
                    {
                        lblStatus.Text = "Connected";
                        lblStatus.ForeColor = AppTheme.SuccessGreen;
                    }
                    else
                    {
                        lblStatus.Text = "Disconnected";
                        lblStatus.ForeColor = AppTheme.ErrorRed;
                        lblCPUState.Visible = false;
                    }
                }
                
                // If still connected, update CPU status
                if (isConnected && plc.GetCPUStatus(out bool isRunning))
                {
                    string cpuState = isRunning ? "RUN" : "STOP";
                    Color stateColor = isRunning ? AppTheme.SuccessGreen : AppTheme.ErrorRed;
                    
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(() =>
                        {
                            lblCPUState.Text = $"CPU State: {cpuState}";
                            lblCPUState.ForeColor = stateColor;
                            lblCPUState.Visible = true;
                        }));
                    }
                    else
                    {
                        lblCPUState.Text = $"CPU State: {cpuState}";
                        lblCPUState.ForeColor = stateColor;
                        lblCPUState.Visible = true;
                    }
                }
                else if (!isConnected && statusRefreshTimer != null && statusRefreshTimer.Enabled)
                {
                    // Stop timer if disconnected
                    statusRefreshTimer.Stop();
                }
            });
        }
        
        private bool ValidateIPAddress(string ip)
        {
            IPAddress address;
            return IPAddress.TryParse(ip, out address);
        }
    }
}
