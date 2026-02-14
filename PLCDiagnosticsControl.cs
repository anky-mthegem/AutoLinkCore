using System;
using System.Drawing;
using System.Windows.Forms;
using S7.Net;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Text;

namespace AutoLinkCore
{
    public partial class PLCDiagnosticsControl : UserControl
    {
        private Plc plc;
        private string ipAddress;
        private short rack;
        private short slot;
        
        public PLCDiagnosticsControl()
        {
            InitializeComponent();
            AppTheme.ApplyTheme(this);
        }
        
        public void SetPLC(Plc plcConnection)
        {
            plc = plcConnection;
            LoadDiagnostics();
        }
        
        public void SetConnectionInfo(string ip, short rackNum, short slotNum)
        {
            ipAddress = ip;
            rack = rackNum;
            slot = slotNum;
        }
        
        private void btnBack_Click(object sender, EventArgs e)
        {
            MainForm mainForm = this.FindForm() as MainForm;
            if (mainForm != null)
            {
                mainForm.ShowConnectionSettings();
            }
        }
        
        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await Task.Run(() => LoadDiagnostics());
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
                
                // CPU Status
                diagnosticInfo.AppendLine("=== CPU STATUS ===");
                try
                {
                    // Check if PLC is still connected
                    if (plc.IsConnected)
                    {
                        diagnosticInfo.AppendLine("Operating State: Connected and Communicating");
                        
                        // Try to read a simple test value to confirm communication
                        try
                        {
                            // Read DB1.DBX0.0 as a communication test
                            var testRead = plc.Read(DataType.DataBlock, 1, 0, VarType.Bit, 1);
                            diagnosticInfo.AppendLine("Communication Test: OK");
                        }
                        catch
                        {
                            diagnosticInfo.AppendLine("Communication Test: Failed (check DB1 exists)");
                        }
                    }
                    else
                    {
                        diagnosticInfo.AppendLine("Operating State: Disconnected");
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
                diagnosticInfo.AppendLine("Library: S7.Net.Plus v0.20.0");
                diagnosticInfo.AppendLine("");
                
                // Diagnostic Notes
                diagnosticInfo.AppendLine("=== DIAGNOSTIC NOTES ===");
                diagnosticInfo.AppendLine("Note: S7.Net.Plus v0.20.0 provides basic read/write");
                diagnosticInfo.AppendLine("      operations. Advanced diagnostics (SZL reads,");
                diagnosticInfo.AppendLine("      LED status, diagnostic buffer) require newer");
                diagnosticInfo.AppendLine("      library versions or direct S7 protocol access.");
                diagnosticInfo.AppendLine("");
                diagnosticInfo.AppendLine("Recommended Operations:");
                diagnosticInfo.AppendLine("  - Test specific memory addresses");
                diagnosticInfo.AppendLine("  - Monitor I/O values");
                diagnosticInfo.AppendLine("  - Read/Write data blocks");
                diagnosticInfo.AppendLine("  - Use TIA Portal for advanced diagnostics");
                
                UpdateUI(diagnosticInfo.ToString());
            }
            catch (Exception ex)
            {
                UpdateUI("Error loading diagnostics: " + ex.Message);
            }
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
