using System;
using System.Data;
using System.Windows.Forms;

namespace AutoLinkCore
{
    /// <summary>
    /// Example usage of new enterprise features
    /// Shows how to integrate EventLogger, AutoReconnect, Watchdog, and AdvancedSqlSync
    /// </summary>
    public class EnterpriseFeatureExample
    {
        // Example: Initialize at application startup
        public void ApplicationStartup()
        {
            // 1. Initialize event logging
            EventLogger.Initialize();
            EventLogger.Log(EventLogger.EVENT_INFO, "=== AutoLinkCore Started ===", "System", 0);

            // 2. Create PLC client
            var plcClient = new Sharp7PLCClient();
            
            // 3. Connect to PLC
            if (plcClient.Connect("192.168.1.1", 0, 1))
            {
                EventLogger.Log(EventLogger.EVENT_CONNECTION, "Connected to PLC", "Main", 0);
                
                // 4. Enable auto-reconnect (will attempt reconnect if connection drops)
                Sharp7PLCClientResilience.InitializeAutoReconnect(plcClient, "192.168.1.1", 0, 1);
                
                // 5. Start watchdog monitoring heartbeat bit
                Sharp7PLCClientResilience.InitializeWatchdog(plcClient, dbNumber: 1, startAddress: 100, bitPosition: 0);
            }
            else
            {
                EventLogger.Log(EventLogger.EVENT_ERROR, "Failed to connect to PLC", "Main", 2);
            }
        }

        // Example: Read PLC data with deadband filtering
        public void ReadAndFilterData(Sharp7PLCClient plcClient)
        {
            var syncManager = new AdvancedSqlSyncManager();

            // Configure deadbands - only update if change exceeds threshold
            syncManager.SetDeadband("Temperature", 0.5);        // ±0.5° tolerance
            syncManager.SetDeadband("Pressure", 1.0);           // ±1.0 bar tolerance
            syncManager.SetDeadband("ProductionCount", 0);      // No tolerance for counts

            // Read temperature from PLC
            if (plcClient.ReadReal(1, 0, out float temperature))
            {
                // Check if update should occur based on deadband
                if (syncManager.ShouldUpdateValue("Temperature", temperature))
                {
                    EventLogger.Log(EventLogger.EVENT_DATA_SYNC, 
                        $"Temperature update: {temperature}°", "DataSync", 0);
                    // Add to sync queue...
                }
            }

            // Read pressure
            if (plcClient.ReadReal(1, 4, out float pressure))
            {
                if (syncManager.ShouldUpdateValue("Pressure", pressure))
                {
                    EventLogger.Log(EventLogger.EVENT_DATA_SYNC, 
                        $"Pressure update: {pressure} bar", "DataSync", 0);
                }
            }
        }

        // Example: Bulk sync multiple values to SQL Server
        public void BulkSyncToDatabase(string connectionString)
        {
            var syncManager = new AdvancedSqlSyncManager();

            try
            {
                // Check if SQL Server is available (5-second timeout)
                if (!syncManager.IsSqlServerAvailable(connectionString))
                {
                    EventLogger.Log(EventLogger.EVENT_ERROR, 
                        "SQL Server unavailable, data will be queued", "Sync", 1);
                    return;
                }

                // Build DataTable with PLC tag values
                DataTable dt = new DataTable("TagValues");
                dt.Columns.Add("TagName", typeof(string));
                dt.Columns.Add("Value", typeof(double));
                dt.Columns.Add("Timestamp", typeof(DateTime));

                // Add your tag data
                dt.Rows.Add("Temperature", 23.5, DateTime.Now);
                dt.Rows.Add("Pressure", 2.1, DateTime.Now);
                dt.Rows.Add("ProductionCount", 1234, DateTime.Now);
                dt.Rows.Add("RunStatus", 1, DateTime.Now);

                // Bulk insert (100x faster than individual INSERTs)
                if (syncManager.BulkInsertSync(connectionString, "TagValues", dt))
                {
                    EventLogger.Log(EventLogger.EVENT_DATA_SYNC, 
                        $"Synced {dt.Rows.Count} rows to SQL Server", "Sync", 0);
                }
                else
                {
                    EventLogger.Log(EventLogger.EVENT_ERROR, 
                        "Bulk insert failed", "Sync", 2);
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log(EventLogger.EVENT_ERROR, 
                    $"Sync error: {ex.Message}", "Sync", 2);
            }
        }

        // Example: Check event log in UI
        public void DisplayEventLog()
        {
            var recentEvents = EventLogger.GetRecentEvents(count: 100);

            MessageBox.Show($"Last 100 events:\n\n" + 
                string.Join("\n", recentEvents.ConvertAll(e => e.ToString())),
                "Event Log");

            // Log location
            string logPath = EventLogger.GetLogDirectory();
            MessageBox.Show($"Logs stored at: {logPath}", "Log Directory");
        }

        // Example: Graceful shutdown
        public void ApplicationShutdown()
        {
            // Stop auto-reconnect timer
            Sharp7PLCClientResilience.StopAutoReconnect();

            // Shutdown logging
            EventLogger.Log(EventLogger.EVENT_INFO, "=== Application Closing ===", "System", 0);
            EventLogger.Shutdown();
        }

        // Example: Handle connection loss in UI
        public void MonitorConnectionStatus(Sharp7PLCClient plcClient, Label statusLabel)
        {
            var timer = new Timer();
            timer.Interval = 2000; // Check every 2 seconds

            timer.Tick += (sender, e) =>
            {
                if (plcClient.IsConnected)
                {
                    statusLabel.Text = "Status: Connected";
                    statusLabel.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    statusLabel.Text = "Status: Disconnected (Auto-Reconnecting)";
                    statusLabel.ForeColor = System.Drawing.Color.Red;
                }
            };

            timer.Start();
        }
    }

    // Example UI control for event log display
    public partial class EventLogViewerForm : Form
    {
        private ListBox lstEvents;
        private Button btnClear;
        private Button btnExport;

        public EventLogViewerForm()
        {
            InitializeComponent();
            RefreshEventLog();

            // Subscribe to new events
            EventLogger.OnEventAdded += (evt) =>
            {
                this.Invoke((Action)(() => AddEventToList(evt)));
            };
        }

        private void InitializeComponent()
        {
            lstEvents = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Consolas", 9),
            };

            btnClear = new Button
            {
                Text = "Clear",
                Dock = DockStyle.Bottom,
                Height = 30
            };
            btnClear.Click += (s, e) => EventLogger.ClearBuffer();

            btnExport = new Button
            {
                Text = "Export Logs",
                Dock = DockStyle.Bottom,
                Height = 30
            };
            btnExport.Click += (s, e) => 
            {
                string path = EventLogger.GetLogDirectory();
                System.Diagnostics.Process.Start(path);
            };

            Controls.Add(lstEvents);
            Controls.Add(btnClear);
            Controls.Add(btnExport);

            Text = "Event Log Viewer";
            Size = new System.Drawing.Size(800, 600);
        }

        private void AddEventToList(DiagnosticEvent evt)
        {
            lstEvents.Items.Insert(0, evt.ToString()); // Most recent first

            // Limit to 500 items for performance
            if (lstEvents.Items.Count > 500)
            {
                lstEvents.Items.RemoveAt(lstEvents.Items.Count - 1);
            }
        }

        private void RefreshEventLog()
        {
            lstEvents.Items.Clear();
            var events = EventLogger.GetRecentEvents(100);
            foreach (var evt in events)
            {
                lstEvents.Items.Add(evt.ToString());
            }
        }
    }
}
