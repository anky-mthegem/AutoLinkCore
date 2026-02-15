using System;
using System.Collections.Generic;

namespace AutoLinkCore
{
    /// <summary>
    /// Represents a single diagnostic event with timestamp
    /// </summary>
    public class DiagnosticEvent
    {
        public DateTime Timestamp { get; set; }
        public string EventType { get; set; } // Connected, Disconnected, StateChange, Error, Warning, Info
        public string Message { get; set; }
        public string Source { get; set; } // PLC, SQL, Network, System
        public int SeverityLevel { get; set; } // 0=Info, 1=Warning, 2=Error

        public override string ToString()
        {
            return $"[{Timestamp:HH:mm:ss.fff}] ({EventType}) {Source}: {Message}";
        }
    }

    /// <summary>
    /// Manages diagnostic event history with in-memory buffer
    /// Provides thread-safe operations and event notifications
    /// </summary>
    public class DiagnosticEventLog
    {
        private readonly List<DiagnosticEvent> _events = new List<DiagnosticEvent>();
        private readonly int _maxEvents = 1000; // Keep last 1000 events
        private readonly object _lockObject = new object();

        public event EventHandler<DiagnosticEvent> EventAdded;

        public void LogEvent(string eventType, string message, string source = "System", int severity = 0)
        {
            var diagEvent = new DiagnosticEvent
            {
                Timestamp = DateTime.Now,
                EventType = eventType,
                Message = message,
                Source = source,
                SeverityLevel = severity
            };

            lock (_lockObject)
            {
                _events.Add(diagEvent);

                // Trim if exceeds max
                if (_events.Count > _maxEvents)
                {
                    _events.RemoveRange(0, _events.Count - _maxEvents);
                }
            }

            // Log to file
            if (severity == 2) // Error
            {
                AppLogger.Error($"[{source}] {eventType}: {message}");
            }
            else if (severity == 1) // Warning
            {
                AppLogger.Warning($"[{source}] {eventType}: {message}");
            }
            else
            {
                AppLogger.Information($"[{source}] {eventType}: {message}");
            }

            // Notify subscribers
            EventAdded?.Invoke(this, diagEvent);
        }

        public List<DiagnosticEvent> GetRecentEvents(int count = 50)
        {
            lock (_lockObject)
            {
                int startIndex = Math.Max(0, _events.Count - count);
                return _events.GetRange(startIndex, _events.Count - startIndex);
            }
        }

        public List<DiagnosticEvent> GetEventsByType(string eventType)
        {
            lock (_lockObject)
            {
                return _events.FindAll(e => e.EventType == eventType);
            }
        }

        public void Clear()
        {
            lock (_lockObject)
            {
                _events.Clear();
            }
        }

        public int GetEventCount()
        {
            lock (_lockObject)
            {
                return _events.Count;
            }
        }
    }
}
