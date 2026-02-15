using System;
using System.Drawing;
using System.Windows.Forms;
using Sharp7;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Text;

namespace AutoLinkCore
{
    public partial class PLCDiagnosticsControl : UserControl
    {
        private Sharp7PLCClient plc;
        private string ipAddress;
        private int rack;
        private int slot;
        private Timer autoRefreshTimer;
        
        public PLCDiagnosticsControl()
        {
            InitializeComponent();
            AppTheme.ApplyTheme(this);
            
            // Initialize auto-refresh timer
            autoRefreshTimer = new Timer();
            autoRefreshTimer.Interval = 2000; // Refresh every 2 seconds
            autoRefreshTimer.Tick += (sender, e) => 
            {
                // Run diagnostics update on background thread to prevent UI blocking
                Task.Run(() => LoadDiagnostics());
            };
            autoRefreshTimer.Start();
        }
        
        public void SetPLC(Sharp7PLCClient plcConnection)
        {
            plc = plcConnection;
            // Restart auto-refresh timer when diagnostics are shown
            if (autoRefreshTimer != null && !autoRefreshTimer.Enabled)
            {
                autoRefreshTimer.Start();
            }
            LoadDiagnostics();
        }
        
        public void SetConnectionInfo(string ip, int rackNum, int slotNum)
        {
            ipAddress = ip;
            rack = rackNum;
            slot = slotNum;
        }
        
        private void btnBack_Click(object sender, EventArgs e)
        {
            // Stop auto-refresh when navigating away
            if (autoRefreshTimer != null)
                autoRefreshTimer.Stop();
            
            MainForm mainForm = this.FindForm() as MainForm;
            if (mainForm != null)
            {
                mainForm.ShowConnectionSettings();
            }
        }
        
        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            // Manual refresh - force immediate update
            btnRefresh.Enabled = false;
            await Task.Run(() => LoadDiagnostics());
            btnRefresh.Enabled = true;
        }
        
        private void LoadDiagnostics()
        {
            if (plc == null || !plc.IsConnected)
            {
                UpdateUI("PLC not connected");
                return;
            }
            
            try
            {
                StringBuilder diagnosticInfo = new StringBuilder();
                
                // Connection Information
                diagnosticInfo.AppendLine("=== CONNECTION INFORMATION ===");
                diagnosticInfo.AppendLine("IP Address: " + (ipAddress ?? "Unknown"));
                diagnosticInfo.AppendLine("Rack: " + rack);
                diagnosticInfo.AppendLine("Slot: " + slot);
                diagnosticInfo.AppendLine("CPU Type: S7-1200");
                diagnosticInfo.AppendLine("Connection Status: Connected");
                diagnosticInfo.AppendLine("");
                
                // Network Ping Test
                diagnosticInfo.AppendLine("=== NETWORK STATUS ===");
                try
                {
                    if (!string.IsNullOrEmpty(ipAddress))
                    {
                        Ping pinger = new Ping();
                        PingReply reply = pinger.Send(ipAddress, 3000);
                        if (reply.Status == IPStatus.Success)
                        {
                            diagnosticInfo.AppendLine("Ping Status: Success");
                            diagnosticInfo.AppendLine("Roundtrip Time: " + reply.RoundtripTime + " ms");
                        }
                        else
                        {
                            diagnosticInfo.AppendLine("Ping Status: " + reply.Status);
                        }
                    }
                }
                catch (Exception ex)
                {
                    diagnosticInfo.AppendLine("Ping Error: " + ex.Message);
                }
                diagnosticInfo.AppendLine("");
                
                // CPU Status - Test multiple memory areas
                diagnosticInfo.AppendLine("=== CPU STATUS ===");
                try
                {
                    bool communicating = TestPLCCommunication();
                    
                    if (communicating && plc.GetCPUStatus(out bool isRunning))
                    {
                        string cpuState = isRunning ? "RUN" : "STOP";
                        diagnosticInfo.AppendLine("Operating State: " + cpuState);
                        diagnosticInfo.AppendLine("Communication Test: OK");
                        diagnosticInfo.AppendLine("PLC is responding to commands");
                        
                        // Add verification note
                        if (!isRunning)
                        {
                            diagnosticInfo.AppendLine("[INFO] CPU is in STOP mode - monitoring will not process data");
                        }
                    }
                    else if (!communicating)
                    {
                        diagnosticInfo.AppendLine("Operating State: OFFLINE");
                        diagnosticInfo.AppendLine("Communication Test: FAILED");
                        diagnosticInfo.AppendLine("Error: " + plc.LastError);
                    }
                    else
                    {
                        diagnosticInfo.AppendLine("Operating State: UNKNOWN");
                        diagnosticInfo.AppendLine("Communication Test: OK (but CPU status unavailable)");
                    }
                }
                catch (Exception ex)
                {
                    diagnosticInfo.AppendLine("Error reading CPU status: " + ex.Message);
                }
                diagnosticInfo.AppendLine("");
                
                // Available Read/Write Operations
                diagnosticInfo.AppendLine("=== AVAILABLE OPERATIONS ===");
                diagnosticInfo.AppendLine("Supported Data Types:");
                diagnosticInfo.AppendLine("  - Inputs (I)");
                diagnosticInfo.AppendLine("  - Outputs (Q)");
                diagnosticInfo.AppendLine("  - Flags/Merkers (M)");
                diagnosticInfo.AppendLine("  - Data Blocks (DB)");
                diagnosticInfo.AppendLine("  - Timers (T)");
                diagnosticInfo.AppendLine("  - Counters (C)");
                diagnosticInfo.AppendLine("");
                diagnosticInfo.AppendLine("Variable Types:");
                diagnosticInfo.AppendLine("  - Bit, Byte, Word, DWord");
                diagnosticInfo.AppendLine("  - Int, DInt, Real");
                diagnosticInfo.AppendLine("  - String, S7String, S7WString");
                diagnosticInfo.AppendLine("");
                
                // Memory Areas Information
                diagnosticInfo.AppendLine("=== MEMORY AREAS (S7-1200) ===");
                diagnosticInfo.AppendLine("Process Image Inputs (I): 1024 bytes");
                diagnosticInfo.AppendLine("Process Image Outputs (Q): 1024 bytes");
                diagnosticInfo.AppendLine("Bit Memory (M): 4096 bytes");
                diagnosticInfo.AppendLine("Data Blocks: User-defined");
                diagnosticInfo.AppendLine("");
                
                // Communication Protocol Info
                diagnosticInfo.AppendLine("=== PROTOCOL INFORMATION ===");
                diagnosticInfo.AppendLine("Protocol: ISO over TCP (RFC1006)");
                diagnosticInfo.AppendLine("Port: 102");
                diagnosticInfo.AppendLine("Library: Sharp7 v1.1.84");
                diagnosticInfo.AppendLine("");
                
                // Diagnostic Notes
                diagnosticInfo.AppendLine("=== DIAGNOSTIC NOTES ===");
                diagnosticInfo.AppendLine("Communication test attempts:");
                diagnosticInfo.AppendLine("  1. Read from DB1 (Data Block 1)");
                diagnosticInfo.AppendLine("  2. Read from I area (Inputs)");
                diagnosticInfo.AppendLine("  3. Read from Q area (Outputs)");
                diagnosticInfo.AppendLine("  4. Read from M area (Flags/Memory)");
                diagnosticInfo.AppendLine("");
                diagnosticInfo.AppendLine("If all tests fail, PLC is truly offline or in STOP mode.");
                diagnosticInfo.AppendLine("Status updates automatically every 2 seconds.");
                
                UpdateUI(diagnosticInfo.ToString());
            }
            catch (Exception ex)
            {
                UpdateUI("Error loading diagnostics: " + ex.Message);
            }
        }
        
        private bool TestPLCCommunication()
        {
            // Use Sharp7PLCClient's built-in communication test
            return plc.TestCommunication();
        }
        
        private void UpdateUI(string text)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(UpdateUI), text);
                return;
            }
            
            txtDiagnostics.Text = text;
        }
    }
}
