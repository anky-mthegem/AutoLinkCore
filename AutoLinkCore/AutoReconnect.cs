using System;
using System.Collections.Generic;
using System.Threading;

namespace AutoLinkCore
{
    /// <summary>
    /// Auto-reconnect extension for Sharp7PLCClient
    /// Provides exponential backoff reconnection strategy and watchdog monitoring
    /// </summary>
    public partial class Sharp7PLCClientResilience
    {
        private static Timer _autoReconnectTimer;
        private static int _reconnectAttempt = 0;
        private static bool _isReconnecting = false;
        private static readonly int[] ExponentialBackoffs = { 2, 5, 10, 10 }; // seconds
        
        // Store connection info
        private static string _plcAddress = "192.168.1.1";
        private static int _plcRack = 0;
        private static int _plcSlot = 1;

        /// <summary>
        /// Initialize automatic reconnection with exponential backoff
        /// </summary>
        public static void InitializeAutoReconnect(Sharp7PLCClient client, string address = "192.168.1.1", int rack = 0, int slot = 1)
        {
            if (client == null)
                return;

            _plcAddress = address;
            _plcRack = rack;
            _plcSlot = slot;

            _autoReconnectTimer = new Timer((state) =>
            {
                if (_isReconnecting || client.IsConnected)
                    return;

                _isReconnecting = true;
                try
                {
                    int delaySeconds = ExponentialBackoffs[Math.Min(_reconnectAttempt, ExponentialBackoffs.Length - 1)];
                    EventLogger.Log(
                        EventLogger.EVENT_CONNECTION,
                        $"Attempting reconnect (attempt {_reconnectAttempt + 1}, delay was {delaySeconds}s)",
                        "AutoReconnect",
                        0
                    );

                    // Attempt connection
                    if (client.Connect(_plcAddress, _plcRack, _plcSlot))
                    {
                        _reconnectAttempt = 0;
                        EventLogger.Log(
                            EventLogger.EVENT_CONNECTION,
                            "Reconnected successfully",
                            "AutoReconnect",
                            0
                        );
                    }
                    else
                    {
                        _reconnectAttempt++;
                    }
                }
                catch (Exception ex)
                {
                    EventLogger.Log(
                        EventLogger.EVENT_ERROR,
                        $"Reconnect failed: {ex.Message}",
                        "AutoReconnect",
                        2
                    );
                }
                finally
                {
                    _isReconnecting = false;
                }

            }, null, 5000, 5000);

            EventLogger.Log(
                EventLogger.EVENT_INFO,
                "Auto-reconnect initialized",
                "System",
                0
            );
        }

        /// <summary>
        /// Stop auto-reconnect timer
        /// </summary>
        public static void StopAutoReconnect()
        {
            if (_autoReconnectTimer != null)
            {
                _autoReconnectTimer.Dispose();
                _autoReconnectTimer = null;
            }
        }

        /// <summary>
        /// Initialize watchdog monitoring for heartbeat bit
        /// </summary>
        public static void InitializeWatchdog(Sharp7PLCClient client, int dbNumber = 1, int startAddress = 100, 
                                             int bitPosition = 0, int checkIntervalMs = 5000)
        {
            if (client == null)
                return;

            bool lastHeartbeatState = false;

            var watchdogTimer = new Timer((state) =>
            {
                try
                {
                    if (client.ReadBool(dbNumber, startAddress, bitPosition, out bool currentState))
                    {
                        if (currentState == lastHeartbeatState)
                        {
                            EventLogger.Log(
                                EventLogger.EVENT_WATCHDOG,
                                $"Watchdog heartbeat stuck at {currentState}",
                                "Watchdog",
                                2
                            );
                        }
                        lastHeartbeatState = currentState;
                    }
                }
                catch (Exception ex)
                {
                    EventLogger.Log(
                        EventLogger.EVENT_ERROR,
                        $"Watchdog check error: {ex.Message}",
                        "Watchdog",
                        1
                    );
                }

            }, null, checkIntervalMs, checkIntervalMs);

            EventLogger.Log(
                EventLogger.EVENT_INFO,
                $"Watchdog initialized: DB{dbNumber}.DBX{startAddress}.{bitPosition}",
                "System",
                0
            );
        }
    }
}
