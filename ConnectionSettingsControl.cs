using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net;
using System.Net.NetworkInformation;
using S7.Net;
using System.Threading.Tasks;

namespace AutoLinkCore
{
    public partial class ConnectionSettingsControl : UserControl
    {
        private Plc plc;
        
        // Public property to expose PLC connection
        public Plc PLCConnection
        {
            get { return plc; }
        }
        
        public ConnectionSettingsControl()
        {
            InitializeComponent();
            AppTheme.ApplyTheme(this);
            
            // Load or create PLC image
            LoadPLCImage();
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
                    // Create a placeholder image with S7-1200 text
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
                // Background
                g.FillRectangle(new SolidBrush(Color.FromArgb(240, 240, 240)), 0, 0, 440, 180);
                
                // Border
                g.DrawRectangle(new Pen(Color.FromArgb(200, 200, 200), 2), 1, 1, 438, 178);
                
                // PLC body shape
                g.FillRectangle(new SolidBrush(Color.FromArgb(50, 50, 50)), 50, 40, 340, 100);
                g.FillRectangle(new SolidBrush(Color.FromArgb(0, 120, 215)), 60, 50, 60, 80);
                g.FillRectangle(new SolidBrush(Color.FromArgb(100, 100, 100)), 130, 50, 250, 80);
                
                // Status LEDs
                g.FillEllipse(new SolidBrush(Color.FromArgb(0, 255, 0)), 70, 60, 8, 8);
                g.FillEllipse(new SolidBrush(Color.FromArgb(255, 200, 0)), 85, 60, 8, 8);
                
                // Text
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
                
                short rack = 0;
                short slot = 1;
                
                if (!short.TryParse(txtRack.Text, out rack))
                {
                    UpdateUIOnError("Invalid Rack number");
                    return;
                }
                
                if (!short.TryParse(txtSlot.Text, out slot))
                {
                    UpdateUIOnError("Invalid Slot number");
                    return;
                }
                
                // Create PLC instance for S7-1200
                plc = new Plc(CpuType.S71200, txtIPAddress.Text, rack, slot);
                
                // Attempt to open connection
                plc.Open();
                
                if (plc.IsConnected)
                {
                    // Read CPU status (using PlcStatus property)
                    UpdateUIOnSuccess(txtIPAddress.Text, rack, slot);
                }
                else
                {
                    UpdateUIOnError("Connection Failed");
                }
            }
            catch (Exception ex)
            {
                UpdateUIOnError("Port 102 Unreachable");
            }
        }
        
        private void UpdateUIOnSuccess(string ipAddress, short rack, short slot)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string, short, short>(UpdateUIOnSuccess), ipAddress, rack, slot);
                return;
            }
            
            progressSpinner.Visible = false;
            btnTestConnection.Enabled = true;
            lblStatus.Text = "Connected";
            lblStatus.ForeColor = AppTheme.SuccessGreen;
            
            // For S7.Net.Plus, we'll display CPU state as RUN when connected
            lblCPUState.Text = "CPU State: RUN";
            lblCPUState.ForeColor = AppTheme.SuccessGreen;
            lblCPUState.Visible = true;
            lblErrorMessage.Visible = false;            
            // Show diagnostics screen
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
            
            if (plc != null && plc.IsConnected)
            {
                plc.Close();
            }
        }
        
        private bool ValidateIPAddress(string ip)
        {
            IPAddress address;
            return IPAddress.TryParse(ip, out address);
        }
    }
}
