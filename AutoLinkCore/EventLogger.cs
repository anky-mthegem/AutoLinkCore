using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace AutoLinkCore
{
    /// <summary>
    /// Enterprise event logging system for diagnostics and troubleshooting
    /// Uses file-based logging with daily rotation and in-memory circular buffer
    /// </summary>
    public static class EventLogger
    {
        private static readonly object _lockObject = new object();
        private static List<DiagnosticEvent> _eventBuffer = new List<DiagnosticEvent>(1000);
        private static string _logDirectory;
        private static StreamWriter _currentLogWriter;
        private static DateTime _currentLogDate;
        private const int MAX_BUFFER_EVENTS = 1000;

        // Event types
        public const string EVENT_CONNECTION = "Connection";
        public const string EVENT_ERROR = "Error";
        public const string EVENT_DATA_SYNC = "DataSync";
        public const string EVENT_WATCHDOG = "Watchdog";
        public const string EVENT_SQL = "SQL";
        public const string EVENT_INFO = "Info";

        public delegate void EventAddedDelegate(DiagnosticEvent evt);
        public static event EventAddedDelegate OnEventAdded;

        /// <summary>
        /// Initialize logging system with file storage in AppData
        /// </summary>
        public static void Initialize()
        {
            lock (_lockObject)
            {
                try
                {
                    _logDirectory = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "AutoLinkCore",
                        "Logs"
                    );

                    if (!Directory.Exists(_logDirectory))
                        Directory.CreateDirectory(_logDirectory);

                    _currentLogDate = DateTime.Now;
                    _currentLogWriter = GetOrCreateLogWriter();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"EventLogger init error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Log an event to buffer and file
        /// </summary>
        public static void Log(string eventType, string message, string source = "System", int severity = 0)
        {
            if (_logDirectory == null)
                Initialize();

            lock (_lockObject)
            {
                try
                {
                    // Create event
                    var evt = new DiagnosticEvent
                    {
                        Timestamp = DateTime.Now,
                        EventType = eventType,
                        Message = message,
                        Source = source,
                        Severity = severity // 0=Info, 1=Warning, 2=Error
                    };

                    // Add to circular buffer
                    _eventBuffer.Add(evt);
                    if (_eventBuffer.Count > MAX_BUFFER_EVENTS)
                        _eventBuffer.RemoveAt(0);

                    // Write to file (check date for rotation)
                    if (DateTime.Now.Date > _currentLogDate.Date)
                    {
                        _currentLogWriter?.Close();
                        _currentLogDate = DateTime.Now;
                        _currentLogWriter = GetOrCreateLogWriter();
                    }

                    string logLine = $"[{evt.Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{eventType}] {message}";
                    _currentLogWriter?.WriteLine(logLine);
                    _currentLogWriter?.Flush();

                    // Notify subscribers
                    OnEventAdded?.Invoke(evt);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"EventLogger.Log error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Get recent events from buffer
        /// </summary>
        public static List<DiagnosticEvent> GetRecentEvents(int count = 100)
        {
            lock (_lockObject)
            {
                return _eventBuffer.Skip(Math.Max(0, _eventBuffer.Count - count))
                                  .ToList();
            }
        }

        /// <summary>
        /// Clear event buffer
        /// </summary>
        public static void ClearBuffer()
        {
            lock (_lockObject)
            {
                _eventBuffer.Clear();
            }
        }

        /// <summary>
        /// Get log directory path
        /// </summary>
        public static string GetLogDirectory()
        {
            return _logDirectory;
        }

        /// <summary>
        /// Cleanup logging
        /// </summary>
        public static void Shutdown()
        {
            lock (_lockObject)
            {
                _currentLogWriter?.Flush();
                _currentLogWriter?.Close();
                _currentLogWriter = null;
            }
        }

        private static StreamWriter GetOrCreateLogWriter()
        {
            string logFilePath = Path.Combine(
                _logDirectory,
                $"AutoLinkCore_{DateTime.Now:yyyy-MM-dd}.log"
            );

            return new StreamWriter(logFilePath, true)
            {
                AutoFlush = false
            };
        }
    }

    /// <summary>
    /// Diagnostic event data structure
    /// </summary>
    public class DiagnosticEvent
    {
        public DateTime Timestamp { get; set; }
        public string EventType { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public int Severity { get; set; } // 0=Info, 1=Warning, 2=Error

        public override string ToString()
        {
            return $"[{Timestamp:HH:mm:ss}] {EventType}: {Message}";
        }
    }
}
