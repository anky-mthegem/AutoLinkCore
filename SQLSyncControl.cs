using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using S7.Net;

namespace AutoLinkCore
{
    public partial class SQLSyncControl : UserControl
    {
        private Plc plc;
        private SqlConnection sqlConnection;
        private CancellationTokenSource cancellationTokenSource;
        private bool isMonitoring = false;
        
        // Handshake state tracking
        private bool lastLogBitState = false;
        private bool confirmBitState = false;
        
        public SQLSyncControl()
        {
            InitializeComponent();
            AppTheme.ApplyTheme(this);
        }
        
        public void SetPLC(Plc plcConnection)
        {
            plc = plcConnection;
            UpdatePLCStatus();
        }
        
        private void UpdatePLCStatus()
        {
            if (plc != null && plc.IsConnected)
            {
                lblPLCStatus.Text = "PLC: Connected";
                lblPLCStatus.ForeColor = AppTheme.SuccessGreen;
            }
            else
            {
                lblPLCStatus.Text = "PLC: Disconnected";
                lblPLCStatus.ForeColor = AppTheme.ErrorRed;
            }
        }
        
        private void UpdateSQLStatus(bool connected)
        {
            if (connected)
            {
                lblSQLStatus.Text = "SQL: Connected";
                lblSQLStatus.ForeColor = AppTheme.SuccessGreen;
            }
            else
            {
                lblSQLStatus.Text = "SQL: Disconnected";
                lblSQLStatus.ForeColor = AppTheme.ErrorRed;
            }
        }
        
        private void btnTestSQL_Click(object sender, EventArgs e)
        {
            TestSQLConnection();
        }
        
        private void TestSQLConnection()
        {
            try
            {
                string connectionString = BuildConnectionString();
                using (SqlConnection testConn = new SqlConnection(connectionString))
                {
                    testConn.Open();
                    LogMessage("SQL Server connection successful", false);
                    UpdateSQLStatus(true);
                    
                    // Load databases if using existing
                    if (rbUseExisting.Checked)
                    {
                        LoadDatabases(testConn);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage("SQL Connection Failed: " + ex.Message, true);
                UpdateSQLStatus(false);
            }
        }
        
        private void LoadDatabases(SqlConnection connection)
        {
            try
            {
                DataTable databases = connection.GetSchema("Databases");
                cmbDatabases.Items.Clear();
                foreach (DataRow row in databases.Rows)
                {
                    string dbName = row["database_name"].ToString();
                    if (dbName != "master" && dbName != "tempdb" && dbName != "model" && dbName != "msdb")
                    {
                        cmbDatabases.Items.Add(dbName);
                    }
                }
                if (cmbDatabases.Items.Count > 0)
                {
                    cmbDatabases.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                LogMessage("Error loading databases: " + ex.Message, true);
            }
        }
        
        private string BuildConnectionString()
        {
            return string.Format("Server={0};User Id={1};Password={2};Connect Timeout=5;",
                txtServer.Text, txtUser.Text, txtPassword.Text);
        }
        
        private void btnInitializeDB_Click(object sender, EventArgs e)
        {
            InitializeDatabase();
        }
        
        private void InitializeDatabase()
        {
            try
            {
                string dbName = rbCreateNew.Checked ? txtNewDBName.Text : cmbDatabases.Text;
                
                if (string.IsNullOrWhiteSpace(dbName))
                {
                    MessageBox.Show("Please enter a database name", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                string connectionString = BuildConnectionString();
                
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    
                    // Create database if it doesn't exist
                    if (rbCreateNew.Checked)
                    {
                        string createDBQuery = string.Format("IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{0}') CREATE DATABASE [{0}]", dbName);
                        using (SqlCommand cmd = new SqlCommand(createDBQuery, conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        LogMessage("Database created: " + dbName, false);
                    }
                }
                
                // Connect to the specific database
                string dbConnectionString = string.Format("Server={0};Database={1};User Id={2};Password={3};",
                    txtServer.Text, dbName, txtUser.Text, txtPassword.Text);
                
                using (SqlConnection dbConn = new SqlConnection(dbConnectionString))
                {
                    dbConn.Open();
                    
                    // Create PLCLogs table
                    string createTableQuery = @"
                        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PLCLogs')
                        CREATE TABLE PLCLogs (
                            ID INT IDENTITY(1,1) PRIMARY KEY,
                            Timestamp DATETIME NOT NULL DEFAULT GETDATE(),
                            TagName NVARCHAR(100) NOT NULL,
                            Value NVARCHAR(255),
                            Status NVARCHAR(50) DEFAULT 'Success',
                            ErrorMessage NVARCHAR(MAX)
                        )";
                    
                    using (SqlCommand cmd = new SqlCommand(createTableQuery, dbConn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    
                    LogMessage("Database initialized successfully: PLCLogs table ready", false);
                    UpdateSQLStatus(true);
                }
            }
            catch (Exception ex)
            {
                LogMessage("Database initialization failed: " + ex.Message, true);
                UpdateSQLStatus(false);
            }
        }
        
        private void btnStartMonitoring_Click(object sender, EventArgs e)
        {
            if (!isMonitoring)
            {
                StartMonitoring();
            }
            else
            {
                StopMonitoring();
            }
        }
        
        private void StartMonitoring()
        {
            if (plc == null || !plc.IsConnected)
            {
                LogMessage("ERROR: PLC not connected", true);
                return;
            }
            
            if (string.IsNullOrWhiteSpace(txtLogBit.Text) || string.IsNullOrWhiteSpace(txtConfirmBit.Text))
            {
                LogMessage("ERROR: Please configure handshake bits", true);
                return;
            }
            
            cancellationTokenSource = new CancellationTokenSource();
            isMonitoring = true;
            btnStartMonitoring.Text = "Stop Monitoring";
            btnStartMonitoring.BackColor = AppTheme.ErrorRed;
            
            LogMessage("=== Monitoring Started ===", false);
            LogMessage("Log Bit: " + txtLogBit.Text, false);
            LogMessage("Confirm Bit: " + txtConfirmBit.Text, false);
            
            Task.Run(() => MonitoringLoop(cancellationTokenSource.Token));
        }
        
        private void StopMonitoring()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }
            
            isMonitoring = false;
            btnStartMonitoring.Text = "Start Monitoring";
            btnStartMonitoring.BackColor = AppTheme.SuccessGreen;
            
            LogMessage("=== Monitoring Stopped ===", false);
        }
        
        private async Task MonitoringLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    // Parse Log Bit address (e.g., "DB1.DBX10.0")
                    var logBitAddress = ParseAddress(txtLogBit.Text);
                    
                    if (logBitAddress == null)
                    {
                        LogMessage("ERROR: Invalid Log Bit address format", true);
                        await Task.Delay(1000, token);
                        continue;
                    }
                    
                    // Read Log Bit
                    bool currentLogBit = ReadBit(logBitAddress);
                    
                    // Handshake Protocol: Rising edge detection
                    if (currentLogBit && !lastLogBitState)
                    {
                        LogMessage(">>> Log Bit DETECTED (Rising Edge)", false);
                        
                        // Read PLC Data
                        bool dataReadSuccess = await ReadAndLogPLCData();
                        
                        if (dataReadSuccess)
                        {
                            // Set Confirm Bit
                            var confirmBitAddress = ParseAddress(txtConfirmBit.Text);
                            if (confirmBitAddress != null)
                            {
                                WriteBit(confirmBitAddress, true);
                                confirmBitState = true;
                                LogMessage("<<< Confirm Bit SET (Write Success)", false);
                            }
                        }
                        else
                        {
                            LogMessage("!!! SQL Write Failed - Confirm Bit NOT SET", true);
                        }
                    }
                    // Wait for PLC to reset Log Bit
                    else if (!currentLogBit && lastLogBitState && confirmBitState)
                    {
                        LogMessage(">>> Log Bit RESET by PLC", false);
                        
                        // Reset Confirm Bit
                        var confirmBitAddress = ParseAddress(txtConfirmBit.Text);
                        if (confirmBitAddress != null)
                        {
                            WriteBit(confirmBitAddress, false);
                            confirmBitState = false;
                            LogMessage("<<< Confirm Bit RESET (Handshake Complete)", false);
                        }
                    }
                    
                    lastLogBitState = currentLogBit;
                    
                    await Task.Delay(100, token); // Poll every 100ms
                }
                catch (Exception ex)
                {
                    LogMessage("Monitoring Error: " + ex.Message, true);
                    Task.Delay(1000, token).Wait();
                }
            }
        }
        
        private async Task<bool> ReadAndLogPLCData()
        {
            try
            {
                // Parse data block range
                var dataAddress = ParseDataBlockRange(txtDataBlock.Text);
                if (dataAddress == null)
                {
                    LogMessage("ERROR: Invalid Data Block Range", true);
                    return false;
                }
                
                // Read data from PLC
                byte[] data = (byte[])plc.Read(DataType.DataBlock, dataAddress.DB, dataAddress.StartByte, 
                    VarType.Byte, dataAddress.Length);
                
                // Get database connection string
                string dbName = rbCreateNew.Checked ? txtNewDBName.Text : cmbDatabases.Text;
                string dbConnectionString = string.Format("Server={0};Database={1};User Id={2};Password={3};Connect Timeout=5;",
                    txtServer.Text, dbName, txtUser.Text, txtPassword.Text);
                
                // Write to SQL
                using (SqlConnection conn = new SqlConnection(dbConnectionString))
                {
                    await conn.OpenAsync();
                    
                    // Convert byte array to hex string for logging
                    string hexValue = BitConverter.ToString(data).Replace("-", " ");
                    
                    string insertQuery = @"INSERT INTO PLCLogs (Timestamp, TagName, Value, Status) 
                                          VALUES (@Timestamp, @TagName, @Value, @Status)";
                    
                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                        cmd.Parameters.AddWithValue("@TagName", txtDataBlock.Text);
                        cmd.Parameters.AddWithValue("@Value", hexValue);
                        cmd.Parameters.AddWithValue("@Status", "Success");
                        
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                
                LogMessage("SQL Write SUCCESS - Data logged", false);
                return true;
            }
            catch (Exception ex)
            {
                LogMessage("SQL Write FAILED: " + ex.Message, true);
                return false;
            }
        }
        
        private bool ReadBit(PLCAddress address)
        {
            byte[] data = (byte[])plc.Read(DataType.DataBlock, address.DB, address.Byte, VarType.Byte, 1);
            return (data[0] & (1 << address.Bit)) != 0;
        }
        
        private void WriteBit(PLCAddress address, bool value)
        {
            byte[] data = (byte[])plc.Read(DataType.DataBlock, address.DB, address.Byte, VarType.Byte, 1);
            
            if (value)
            {
                data[0] |= (byte)(1 << address.Bit);  // Set bit
            }
            else
            {
                data[0] &= (byte)(~(1 << address.Bit));  // Clear bit
            }
            
            plc.Write(DataType.DataBlock, address.DB, address.Byte, data);
        }
        
        private PLCAddress ParseAddress(string address)
        {
            try
            {
                // Format: DB1.DBX10.0
                string[] parts = address.Replace("DB", "").Replace("DBX", ".").Split('.');
                if (parts.Length >= 3)
                {
                    return new PLCAddress
                    {
                        DB = int.Parse(parts[0]),
                        Byte = int.Parse(parts[1]),
                        Bit = int.Parse(parts[2])
                    };
                }
            }
            catch { }
            return null;
        }
        
        private DataBlockAddress ParseDataBlockRange(string range)
        {
            try
            {
                // Format: DB1.DBB0-20 (Read 20 bytes starting from DB1.DBB0)
                string[] parts = range.Replace("DB", "").Replace("DBB", ".").Split(new[] { '.', '-' });
                if (parts.Length >= 3)
                {
                    return new DataBlockAddress
                    {
                        DB = int.Parse(parts[0]),
                        StartByte = int.Parse(parts[1]),
                        Length = int.Parse(parts[2])
                    };
                }
            }
            catch { }
            return null;
        }
        
        private void LogMessage(string message, bool isError)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string, bool>(LogMessage), message, isError);
                return;
            }
            
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string logEntry = string.Format("[{0}] {1}", timestamp, message);
            
            if (isError)
            {
                logEntry = "ERROR: " + logEntry;
            }
            
            txtLogs.AppendText(logEntry + Environment.NewLine);
            txtLogs.SelectionStart = txtLogs.Text.Length;
            txtLogs.ScrollToCaret();
        }
        
        private void btnClearLogs_Click(object sender, EventArgs e)
        {
            txtLogs.Clear();
        }
        
        private void rbUseExisting_CheckedChanged(object sender, EventArgs e)
        {
            cmbDatabases.Enabled = rbUseExisting.Checked;
            txtNewDBName.Enabled = !rbUseExisting.Checked;
        }
        
        private void rbCreateNew_CheckedChanged(object sender, EventArgs e)
        {
            txtNewDBName.Enabled = rbCreateNew.Checked;
            cmbDatabases.Enabled = !rbCreateNew.Checked;
        }
        
        private void btnBack_Click(object sender, EventArgs e)
        {
            StopMonitoring();
            MainForm mainForm = this.FindForm() as MainForm;
            if (mainForm != null)
            {
                mainForm.ShowMainDashboard();
            }
        }
        
        private class PLCAddress
        {
            public int DB { get; set; }
            public int Byte { get; set; }
            public int Bit { get; set; }
        }
        
        private class DataBlockAddress
        {
            public int DB { get; set; }
            public int StartByte { get; set; }
            public int Length { get; set; }
        }
    }
}
