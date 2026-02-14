using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using S7.Net;

namespace AutoLinkCore
{
    // Data mapping class for PLC to SQL column mapping
    public class PLCDataMapping
    {
        public string MemoryArea { get; set; }  // DB, M, I, Q
        public string DataType { get; set; }     // Bool, Int, Real
        public int DBNumber { get; set; }        // DB number (if Memory Area = DB)
        public int Offset { get; set; }          // Byte offset
        public int Bit { get; set; }             // Bit number (if DataType = Bool)
        public string SQLColumnName { get; set; } // Target SQL column name
        public Panel CardPanel { get; set; }     // Reference to the UI card
    }

    public partial class SQLSyncControl : UserControl
    {
        private Plc plc;
        private SqlConnection sqlConnection;
        private CancellationTokenSource cancellationTokenSource;
        private bool isMonitoring = false;
        
        // Handshake state tracking
        private bool lastLogBitState = false;
        private bool confirmBitState = false;
        
        // Dynamic data mappings
        private List<PLCDataMapping> dataMappings = new List<PLCDataMapping>();
        
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
                    
                    // Check if PLCLogs table exists
                    string checkTableQuery = "SELECT COUNT(*) FROM sys.tables WHERE name = 'PLCLogs'";
                    bool tableExists = false;
                    using (SqlCommand checkCmd = new SqlCommand(checkTableQuery, dbConn))
                    {
                        tableExists = ((int)checkCmd.ExecuteScalar() > 0);
                    }
                    
                    if (tableExists)
                    {
                        LogMessage("PLCLogs table already exists. Dropping to recreate with new mappings...", false);
                        using (SqlCommand dropCmd = new SqlCommand("DROP TABLE PLCLogs", dbConn))
                        {
                            dropCmd.ExecuteNonQuery();
                        }
                    }
                    
                    // Build dynamic CREATE TABLE with columns from mappings
                    var createTableBuilder = new System.Text.StringBuilder();
                    createTableBuilder.Append("CREATE TABLE PLCLogs (");
                    createTableBuilder.Append("ID INT IDENTITY(1,1) PRIMARY KEY, ");
                    createTableBuilder.Append("Timestamp DATETIME NOT NULL DEFAULT GETDATE()");
                    
                    if (dataMappings.Count == 0)
                    {
                        LogMessage("WARNING: No data mappings configured. Creating table with Timestamp only.", true);
                    }
                    
                    foreach (var mapping in dataMappings)
                    {
                        string sqlType = GetSQLType(mapping.DataType);
                        createTableBuilder.AppendFormat(", {0} {1}", mapping.SQLColumnName, sqlType);
                    }
                    
                    createTableBuilder.Append(")");
                    
                    using (SqlCommand cmd = new SqlCommand(createTableBuilder.ToString(), dbConn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    
                    LogMessage("Database initialized: PLCLogs table with " + dataMappings.Count + " mapped columns", false);
                    UpdateSQLStatus(true);
                }
            }
            catch (Exception ex)
            {
                LogMessage("Database initialization failed: " + ex.Message, true);
                UpdateSQLStatus(false);
            }
        }
        
        private string GetSQLType(string plcDataType)
        {
            switch (plcDataType)
            {
                case "Bool":
                    return "BIT";
                case "Int":
                    return "SMALLINT";
                case "DInt":
                    return "INT";
                case "Real":
                    return "REAL";
                default:
                    return "NVARCHAR(255)";
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
                if (dataMappings.Count == 0)
                {
                    LogMessage("ERROR: No data mappings configured", true);
                    return false;
                }
                
                LogMessage(">>> BULK READ: Reading " + dataMappings.Count + " PLC addresses...", false);
                
                // Step 1: Read all PLC values
                var plcValues = new Dictionary<string, object>();
                foreach (var mapping in dataMappings)
                {
                    try
                    {
                        object value = ReadPLCAddress(mapping);
                        plcValues[mapping.SQLColumnName] = value;
                        LogMessage("    Read " + mapping.SQLColumnName + " = " + value, false);
                    }
                    catch (Exception ex)
                    {
                        LogMessage("    ERROR reading " + mapping.SQLColumnName + ": " + ex.Message, true);
                        return false;
                    }
                }
                
                // Step 2: Build and execute SQL INSERT with all columns
                string dbName = rbCreateNew.Checked ? txtNewDBName.Text : cmbDatabases.Text;
                string dbConnectionString = string.Format("Server={0};Database={1};User Id={2};Password={3};Connect Timeout=5;",
                    txtServer.Text, dbName, txtUser.Text, txtPassword.Text);
                
                using (SqlConnection conn = new SqlConnection(dbConnectionString))
                {
                    await conn.OpenAsync();
                    
                    // Build dynamic INSERT query
                    var columnNames = new System.Text.StringBuilder("Timestamp");
                    var paramNames = new System.Text.StringBuilder("@Timestamp");
                    
                    foreach (var kvp in plcValues)
                    {
                        columnNames.Append(", " + kvp.Key);
                        paramNames.Append(", @" + kvp.Key);
                    }
                    
                    string insertQuery = string.Format("INSERT INTO PLCLogs ({0}) VALUES ({1})",
                        columnNames.ToString(), paramNames.ToString());
                    
                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                        
                        foreach (var kvp in plcValues)
                        {
                            cmd.Parameters.AddWithValue("@" + kvp.Key, kvp.Value);
                        }
                        
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                
                LogMessage("SQL Write SUCCESS - " + plcValues.Count + " columns inserted", false);
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
        
        private object ReadPLCAddress(PLCDataMapping mapping)
        {
            DataType dataType;
            int dbNumber = 0;
            
            // Determine S7.Net DataType from memory area
            switch (mapping.MemoryArea)
            {
                case "DB":
                    dataType = DataType.DataBlock;
                    dbNumber = mapping.DBNumber;
                    break;
                case "M":
                    dataType = DataType.Memory;
                    break;
                case "I":
                    dataType = DataType.Input;
                    break;
                case "Q":
                    dataType = DataType.Output;
                    break;
                default:
                    throw new Exception("Unsupported memory area: " + mapping.MemoryArea);
            }
            
            // Read based on data type
            switch (mapping.DataType)
            {
                case "Bool":
                    byte[] boolData = (byte[])plc.Read(dataType, dbNumber, mapping.Offset, VarType.Byte, 1);
                    return (boolData[0] & (1 << mapping.Bit)) != 0;
                    
                case "Int":
                    return (short)plc.Read(dataType, dbNumber, mapping.Offset, VarType.Int, 1);
                    
                case "DInt":
                    return (int)plc.Read(dataType, dbNumber, mapping.Offset, VarType.DInt, 1);
                    
                case "Real":
                    return (float)plc.Read(dataType, dbNumber, mapping.Offset, VarType.Real, 1);
                    
                default:
                    throw new Exception("Unsupported data type: " + mapping.DataType);
            }
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
        // ========== DYNAMIC DATA MAPPING METHODS ==========
        
        private void btnAddMapping_Click(object sender, EventArgs e)
        {
            AddMappingRow();
        }
        
        private void AddMappingRow()
        {
            var mapping = new PLCDataMapping
            {
                MemoryArea = "DB",
                DataType = "Int",
                DBNumber = 1,
                Offset = 0,
                Bit = 0,
                SQLColumnName = "Value" + (dataMappings.Count + 1)
            };
            
            Panel card = CreateMappingCard(mapping);
            mapping.CardPanel = card;
            dataMappings.Add(mapping);
            
            flowMappings.Controls.Add(card);
            LogMessage("Added new data mapping row", false);
        }
        
        private Panel CreateMappingCard(PLCDataMapping mapping)
        {
            Panel card = new Panel
            {
                Width = flowMappings.Width - 25,
                Height = 100,
                Margin = new Padding(5),
                BackColor = Color.FromArgb(45, 45, 48),
                BorderStyle = BorderStyle.FixedSingle
            };
            
            // Memory Area dropdown
            Label lblMemArea = new Label
            {
                Text = "Memory:",
                Location = new Point(10, 12),
                Size = new Size(60, 20),
                ForeColor = Color.White
            };
            ComboBox cmbMemArea = new ComboBox
            {
                Location = new Point(75, 10),
                Size = new Size(60, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Tag = "MemoryArea"
            };
            cmbMemArea.Items.AddRange(new object[] { "DB", "M", "I", "Q" });
            cmbMemArea.SelectedIndex = 0;
            cmbMemArea.SelectedIndexChanged += delegate(object s, EventArgs e) {
                mapping.MemoryArea = cmbMemArea.Text;
                // Show/hide DB Number field
                foreach (Control ctrl in card.Controls)
                {
                    if (ctrl.Tag != null && ctrl.Tag.ToString() == "DBNum")
                    {
                        ctrl.Visible = (cmbMemArea.Text == "DB");
                    }
                }
            };
            
            // Data Type dropdown
            Label lblDataType = new Label
            {
                Text = "Type:",
                Location = new Point(145, 12),
                Size = new Size(40, 20),
                ForeColor = Color.White
            };
            ComboBox cmbDataType = new ComboBox
            {
                Location = new Point(190, 10),
                Size = new Size(70, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Tag = "DataType"
            };
            cmbDataType.Items.AddRange(new object[] { "Bool", "Int", "Real", "DInt" });
            cmbDataType.SelectedIndex = 1;
            cmbDataType.SelectedIndexChanged += delegate(object s, EventArgs e) {
                mapping.DataType = cmbDataType.Text;
                // Show/hide Bit field
                foreach (Control ctrl in card.Controls)
                {
                    if (ctrl.Tag != null && ctrl.Tag.ToString() == "Bit")
                    {
                        ctrl.Visible = (cmbDataType.Text == "Bool");
                    }
                }
            };
            
            // DB Number (visible only for DB memory area)
            Label lblDBNum = new Label
            {
                Text = "DB:",
                Location = new Point(270, 12),
                Size = new Size(25, 20),
                ForeColor = Color.White,
                Tag = "DBNum"
            };
            TextBox txtDBNum = new TextBox
            {
                Location = new Point(300, 10),
                Size = new Size(40, 25),
                Text = "1",
                Tag = "DBNum"
            };
            txtDBNum.TextChanged += delegate(object s, EventArgs e) {
                int dbNum;
                if (int.TryParse(txtDBNum.Text, out dbNum))
                    mapping.DBNumber = dbNum;
            };
            
            // Offset
            Label lblOffset = new Label
            {
                Text = "Offset:",
                Location = new Point(350, 12),
                Size = new Size(45, 20),
                ForeColor = Color.White
            };
            TextBox txtOffset = new TextBox
            {
                Location = new Point(400, 10),
                Size = new Size(50, 25),
                Text = "0"
            };
            txtOffset.TextChanged += delegate(object s, EventArgs e) {
                int offset;
                if (int.TryParse(txtOffset.Text, out offset))
                    mapping.Offset = offset;
            };
            
            // Bit (visible only for Bool type)
            Label lblBit = new Label
            {
                Text = "Bit:",
                Location = new Point(460, 12),
                Size = new Size(30, 20),
                ForeColor = Color.White,
                Tag = "Bit",
                Visible = false
            };
            TextBox txtBit = new TextBox
            {
                Location = new Point(495, 10),
                Size = new Size(30, 25),
                Text = "0",
                Tag = "Bit",
                Visible = false
            };
            txtBit.TextChanged += delegate(object s, EventArgs e) {
                int bit;
                if (int.TryParse(txtBit.Text, out bit))
                    mapping.Bit = bit;
            };
            
            // SQL Column Name
            Label lblSQLCol = new Label
            {
                Text = "SQL Column:",
                Location = new Point(10, 52),
                Size = new Size(80, 20),
                ForeColor = Color.White
            };
            TextBox txtSQLCol = new TextBox
            {
                Location = new Point(95, 50),
                Size = new Size(200, 25),
                Text = mapping.SQLColumnName
            };
            txtSQLCol.TextChanged += delegate(object s, EventArgs e) {
                mapping.SQLColumnName = txtSQLCol.Text;
            };
            
            // Remove button
            Button btnRemove = new Button
            {
                Text = "✕ Remove",
                Location = new Point(card.Width - 100, 50),
                Size = new Size(85, 30),
                BackColor = AppTheme.ErrorRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRemove.Click += delegate(object s, EventArgs e) {
                RemoveMappingRow(mapping, card);
            };
            
            card.Controls.AddRange(new Control[] {
                lblMemArea, cmbMemArea, lblDataType, cmbDataType,
                lblDBNum, txtDBNum, lblOffset, txtOffset,
                lblBit, txtBit, lblSQLCol, txtSQLCol, btnRemove
            });
            
            return card;
        }
        
        private void RemoveMappingRow(PLCDataMapping mapping, Panel card)
        {
            dataMappings.Remove(mapping);
            flowMappings.Controls.Remove(card);
            card.Dispose();
            LogMessage("Removed data mapping row", false);
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
