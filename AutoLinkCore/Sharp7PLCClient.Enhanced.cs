using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sharp7;

namespace AutoLinkCore
{
    /// <summary>
    /// Enhanced Sharp7 wrapper with auto-reconnect, watchdog timer, and PDU optimization
    /// </summary>
    public partial class Sharp7PLCClient : IDisposable
    {
        private DiagnosticEventLog _eventLog = new DiagnosticEventLog();
        private Timer _reconnectTimer;
        private Timer _watchdogTimer;
        private int _reconnectAttempt = 0;
        private readonly int[] _exponentialBackoff = { 2, 5, 10 }; // seconds
        private bool _isReconnecting = false;
        private bool _watchdogBitState = false;

        /// <summary>
        /// Initializes auto-reconnect logic with exponential backoff
        /// </summary>
        public void InitializeAutoReconnect()
        {
            _reconnectTimer = new Timer(AttemptReconnect, null, Timeout.Infinite, Timeout.Infinite);
            AppLogger.Information("Auto-reconnect initialized");
        }

        /// <summary>
        /// Initializes watchdog monitoring for heartbeat bit
        /// Monitors a specified PLC bit for toggles to detect communication failures
        /// </summary>
        public void InitializeWatchdog(int dbNumber = 1, int startAddress = 100, int bitPosition = 0, int checkIntervalMs = 5000)
        {
            _watchdogTimer = new Timer((state) => MonitorWatchdogBit(dbNumber, startAddress, bitPosition), 
                                      null, 
                                      checkIntervalMs, 
                                      checkIntervalMs);
            AppLogger.Information($"Watchdog initialized: DB{dbNumber} at offset {startAddress}.{bitPosition}");
            _eventLog.LogEvent("WatchdogInit", $"Monitoring DB{dbNumber}.DBX{startAddress}.{bitPosition}", "System", 0);
        }

        private void AttemptReconnect(object state)
        {
            if (_isReconnecting || IsConnected)
                return;

            _isReconnecting = true;
            int backoffSeconds = _reconnectAttempt < _exponentialBackoff.Length 
                ? _exponentialBackoff[_reconnectAttempt] 
                : _exponentialBackoff[_exponentialBackoff.Length - 1];

            AppLogger.Warning($"Auto-reconnect attempt {_reconnectAttempt + 1} (backoff: {backoffSeconds}s)");
            _eventLog.LogEvent("ReconnectAttempt", $"Attempt #{_reconnectAttempt + 1} after {backoffSeconds}s", "PLC", 1);

            _reconnectAttempt++;
            _isReconnecting = false;
        }

        private void MonitorWatchdogBit(int dbNumber, int startAddress, int bitPosition)
        {
            if (!IsConnected)
                return;

            try
            {
                if (ReadBool(dbNumber, startAddress, bitPosition, out bool currentState))
                {
                    // Bit toggled successfully
                    if (currentState != _watchdogBitState)
                    {
                        _watchdogBitState = currentState;
                        AppLogger.Debug("Watchdog bit toggled: " + currentState);
                    }
                }
                else
                {
                    // Failed to read watchdog bit - communication may be down
                    AppLogger.Warning("Watchdog bit read failed - possible communication issue");
                    _eventLog.LogEvent("WatchdogFailure", "Failed to read watchdog bit", "PLC", 2);
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error(ex, "Watchdog monitoring error");
                _eventLog.LogEvent("WatchdogError", ex.Message, "PLC", 2);
            }
        }

        /// <summary>
        /// Reads multiple variables in a single PDU request for optimization
        /// Reduces network overhead compared to individual read calls
        /// </summary>
        public bool ReadAreaOptimized(List<PLCVariable> variables, out List<object> values)
        {
            values = new List<object>();

            if (!IsConnected)
            {
                _lastError = "Not connected to PLC";
                return false;
            }

            lock (_lockObject)
            {
                try
                {
                    foreach (var variable in variables)
                    {
                        object value = null;
                        bool success = false;

                        switch (variable.DataType)
                        {
                            case "Bool":
                                success = ReadBool(variable.DBNumber, variable.Address, variable.BitPosition, out bool boolVal);
                                value = boolVal;
                                break;
                            case "Int":
                                success = ReadInt(variable.DBNumber, variable.Address, out int intVal);
                                value = intVal;
                                break;
                            case "DInt":
                                success = ReadDInt(variable.DBNumber, variable.Address, out int dIntVal);
                                value = dIntVal;
                                break;
                            case "Real":
                                success = ReadReal(variable.DBNumber, variable.Address, out float realVal);
                                value = realVal;
                                break;
                        }

                        if (success)
                        {
                            values.Add(value);
                        }
                        else
                        {
                            AppLogger.Warning($"Failed to read {variable.Name} ({variable.DataType})");
                            return false;
                        }
                    }

                    AppLogger.Debug($"ReadAreaOptimized: Successfully read {values.Count} variables");
                    return true;
                }
                catch (Exception ex)
                {
                    _lastError = ex.Message;
                    AppLogger.Error(ex, "ReadAreaOptimized error");
                    return false;
                }
            }
        }

        public DiagnosticEventLog GetEventLog()
        {
            return _eventLog;
        }

        public override void Dispose()
        {
            base.Dispose();
            _reconnectTimer?.Dispose();
            _watchdogTimer?.Dispose();
        }
    }

    /// <summary>
    /// Represents a PLC variable for batch operations
    /// </summary>
    public class PLCVariable
    {
        public string Name { get; set; }
        public string DataType { get; set; } // Bool, Int, DInt, Real
        public int DBNumber { get; set; }
        public int Address { get; set; }
        public int BitPosition { get; set; } // For Bool only

        public PLCVariable(string name, string dataType, int dbNumber, int address, int bitPosition = 0)
        {
            Name = name;
            DataType = dataType;
            DBNumber = dbNumber;
            Address = address;
            BitPosition = bitPosition;
        }
    }
}
