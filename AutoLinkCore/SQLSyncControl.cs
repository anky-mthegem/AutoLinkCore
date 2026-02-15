using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sharp7;

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
        private Sharp7PLCClient plc;
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
        
        public void SetPLC(Sharp7PLCClient plcConnection)
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
                
                // Read and display current value from data mappings
                Task.Run(() => ReadAndDisplayMappingValues());
            }
            else
            {
                lblPLCStatus.Text = "PLC: Disconnected";
                lblPLCStatus.ForeColor = AppTheme.ErrorRed;
            }
        }
        
        private void ReadAndDisplayMappingValues()
        {
            if (plc == null || !plc.IsConnected || dataMappings.Count == 0)
                return;
                
            try
            {
                foreach (var mapping in dataMappings)
                {
                    try
                    {
                        object value = null;
                        
                        if (mapping.DataType == "Bool")
                        {
                            if (plc.ReadBool(mapping.DBNumber, mapping.Offset, mapping.Bit, out bool bitValue))
                                value = bitValue ? "1" : "0";
                        }
                        else if (mapping.DataType == "Int")
                        {
                            if (plc.ReadInt(mapping.DBNumber, mapping.Offset, out short intValue))
                                value = intValue.ToString();
                        }
                        else if (mapping.DataType == "Real")
                        {
                            if (plc.ReadReal(mapping.DBNumber, mapping.Offset, out float realValue))
                                value = realValue.ToString("F2");
                        }
                        else if (mapping.DataType == "Byte")
                        {
                            if (plc.ReadByte(mapping.DBNumber, mapping.Offset, out byte byteValue))
                                value = byteValue.ToString();
                        }
                        else if (mapping.DataType == "DInt")
                        {
                            if (plc.ReadDInt(mapping.DBNumber, mapping.Offset, out int dintValue))
                                value = dintValue.ToString();
                        }
                        
                        // Update the UI with the value
                        if (mapping.CardPanel != null && value != null)
                        {
                            string displayText = $"Current Value: {value}";
                            if (this.InvokeRequired)
                            {
                                this.Invoke(new Action(() =>
                                {
                                    var lblValue = mapping.CardPanel.Controls.OfType<Label>()
                                        .FirstOrDefault(l => l.Name == "lblCurrentValue" || (l.Tag != null && l.Tag.ToString() == "CurrentValue"));
                                    if (lblValue != null)
                                    {
                                        lblValue.Text = displayText;
                                        lblValue.ForeColor = AppTheme.SuccessGreen;
                                    }
                                }));
                            }
                            else
                            {
                                var lblValue = mapping.CardPanel.Controls.OfType<Label>()
                                    .FirstOrDefault(l => l.Name == "lblCurrentValue" || (l.Tag != null && l.Tag.ToString() == "CurrentValue"));
                                if (lblValue != null)
                                {
                                    lblValue.Text = displayText;
                                    lblValue.ForeColor = AppTheme.SuccessGreen;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (mapping.CardPanel != null)
                        {
                            if (this.InvokeRequired)
                            {
                                this.Invoke(new Action(() =>
                                {
                                    var lblValue = mapping.CardPanel.Controls.OfType<Label>()
                                        .FirstOrDefault(l => l.Name == "lblCurrentValue" || (l.Tag != null && l.Tag.ToString() == "CurrentValue"));
                                    if (lblValue != null)
                                    {
                                        lblValue.Text = "Current Value: ERROR";
                                        lblValue.ForeColor = AppTheme.ErrorRed;
                                    }
                                }));
                            }
                        }
                    }
                }
            }
            catch
            {
                // Silently fail
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
            
            // Validate handshake bit configuration
            if (string.IsNullOrWhiteSpace(txtLogOffset.Text) || string.IsNullOrWhiteSpace(txtConfirmOffset.Text))
            {
                LogMessage("ERROR: Please configure handshake bits", true);
                return;
            }
            
            cancellationTokenSource = new CancellationTokenSource();
            isMonitoring = true;
            btnStartMonitoring.Text = "Stop Monitoring";
            btnStartMonitoring.BackColor = AppTheme.ErrorRed;
            
            string logBitAddr = BuildBitAddress(lblLogMemArea.Text, txtLogDBNum.Text, txtLogOffset.Text, txtLogBitNum.Text);
            string confirmBitAddr = BuildBitAddress(lblConfirmMemArea.Text, txtConfirmDBNum.Text, txtConfirmOffset.Text, txtConfirmBitNum.Text);
            
            LogMessage("=== Monitoring Started ===", false);
            LogMessage("Log Bit: " + logBitAddr, false);
            LogMessage("Confirm Bit: " + confirmBitAddr, false);
            
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
                    // Build Log Bit address from controls
                    var logBitAddress = new PLCAddress
                    {
                        DB = (lblLogMemArea.Text == "DB") ? int.Parse(txtLogDBNum.Text) : 0,
                        Byte = int.Parse(txtLogOffset.Text),
                        Bit = int.Parse(txtLogBitNum.Text)
                    };
                    
                    var confirmBitAddress = new PLCAddress
                    {
                        DB = (lblConfirmMemArea.Text == "DB") ? int.Parse(txtConfirmDBNum.Text) : 0,
                        Byte = int.Parse(txtConfirmOffset.Text),
                        Bit = int.Parse(txtConfirmBitNum.Text)
                    };
                    
                    // Read Log Bit
                    bool currentLogBit = ReadBit(logBitAddress);
                    
                    // Update status indicators
                    UpdateBitStatusIndicators(currentLogBit, logBitAddress, confirmBitAddress);
                    
                    // Handshake Protocol: Rising edge detection
                    if (currentLogBit && !lastLogBitState)
                    {
                        LogMessage(">>> Log Bit DETECTED (Rising Edge)", false);
                        
                        // Read PLC Data
                        bool dataReadSuccess = await ReadAndLogPLCData();
                        
                        if (dataReadSuccess)
                        {
                            // Set Confirm Bit
                            WriteBit(confirmBitAddress, true);
                            confirmBitState = true;
                            LogMessage("<<< Confirm Bit SET (Write Success)", false);
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
                        WriteBit(confirmBitAddress, false);
                        confirmBitState = false;
                        LogMessage("<<< Confirm Bit RESET (Handshake Complete)", false);
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
            if (plc.ReadBool(address.DB, address.Byte, address.Bit, out bool bitValue))
                return bitValue;
            return false;
        }
        
        private void UpdateBitStatusIndicators(bool logBitValue, PLCAddress logAddress, PLCAddress confirmAddress)
        {
            try
            {
                // Update Log Bit status indicator
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(delegate
                    {
                        lblLogBitStatus.BackColor = logBitValue ? Color.Green : Color.Red;
                    }));
                }
                else
                {
                    lblLogBitStatus.BackColor = logBitValue ? Color.Green : Color.Red;
                }
                
                // Read and update Confirm Bit status indicator
                bool confirmBitValue = ReadBit(confirmAddress);
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(delegate
                    {
                        lblConfirmBitStatus.BackColor = confirmBitValue ? Color.Green : Color.Red;
                    }));
                }
                else
                {
                    lblConfirmBitStatus.BackColor = confirmBitValue ? Color.Green : Color.Red;
                }
            }
            catch (Exception ex)
            {
                // Silently fail to avoid flooding logs during monitoring
                System.Diagnostics.Debug.WriteLine("Status indicator update failed: " + ex.Message);
            }
        }
        
        private void WriteBit(PLCAddress address, bool value)
        {
            plc.WriteBool(address.DB, address.Byte, address.Bit, value);
        }
        
        private object ReadPLCAddress(PLCDataMapping mapping)
        {
            // Read based on data type using Sharp7PLCClient methods
            switch (mapping.DataType)
            {
                case "Bool":
                    if (plc.ReadBool(mapping.DBNumber, mapping.Offset, mapping.Bit, out bool boolValue))
                        return boolValue;
                    break;
                    
                case "Int":
                    if (plc.ReadInt(mapping.DBNumber, mapping.Offset, out short intValue))
                        return intValue;
                    break;
                    
                case "DInt":
                    if (plc.ReadDInt(mapping.DBNumber, mapping.Offset, out int dintValue))
                        return dintValue;
                    break;
                    
                case "Real":
                    if (plc.ReadReal(mapping.DBNumber, mapping.Offset, out float realValue))
                        return realValue;
                    break;
                    
                case "Byte":
                    if (plc.ReadByte(mapping.DBNumber, mapping.Offset, out byte byteValue))
                        return byteValue;
                    break;
                    
                default:
                    throw new Exception("Unsupported data type: " + mapping.DataType);
            }
            
            return null;
        }
        
        private string BuildBitAddress(string memArea, string dbNum, string offset, string bit)
        {
            if (memArea == "DB")
            {
                return string.Format("DB{0}.DBX{1}.{2}", dbNum, offset, bit);
            }
            else
            {
                return string.Format("{0}{1}.{2}", memArea, offset, bit);
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
            
            // Current Value label
            Label lblCurrentValue = new Label
            {
                Text = "Current Value: --",
                Location = new Point(310, 52),
                Size = new Size(200, 25),
                ForeColor = AppTheme.SuccessGreen,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Name = "lblCurrentValue",
                Tag = "CurrentValue"
            };
            
            card.Controls.AddRange(new Control[] {
                lblMemArea, cmbMemArea, lblDataType, cmbDataType,
                lblDBNum, txtDBNum, lblOffset, txtOffset,
                lblBit, txtBit, lblSQLCol, txtSQLCol, lblCurrentValue, btnRemove
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
        
        private void PaintRoundIndicator(object sender, PaintEventArgs e)
        {
            Label indicator = sender as Label;
            if (indicator != null)
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    path.AddEllipse(0, 0, indicator.Width - 1, indicator.Height - 1);
                    indicator.Region = new Region(path);
                    using (SolidBrush brush = new SolidBrush(indicator.BackColor))
                    {
                        e.Graphics.FillEllipse(brush, 0, 0, indicator.Width - 1, indicator.Height - 1);
                    }
                }
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
