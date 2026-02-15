using System;
using System.IO;
using Serilog;
using Serilog.Core;

namespace AutoLinkCore
{
    /// <summary>
    /// Centralized logging utility using Serilog
    /// Automatically rotates logs by day and maintains file structure
    /// </summary>
    public static class AppLogger
    {
        private static ILogger _logger;
        private static readonly string LogDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "AutoLinkCore",
            "Logs"
        );

        static AppLogger()
        {
            InitializeLogger();
        }

        public static void InitializeLogger()
        {
            // Create logs directory if it doesn't exist
            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }

            string logFilePath = Path.Combine(LogDirectory, "AutoLinkCore-.log");

            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(
                    logFilePath,
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    retainedFileCountLimit: 30 // Keep last 30 days of logs
                )
                .CreateLogger();

            System.Diagnostics.Debug.WriteLine("[AppLogger] Initialized - Logs at: " + LogDirectory);
        }

        public static void Information(string message, params object[] args)
        {
            _logger?.Information(message, args);
        }

        public static void Warning(string message, params object[] args)
        {
            _logger?.Warning(message, args);
        }

        public static void Error(string message, params object[] args)
        {
            _logger?.Error(message, args);
        }

        public static void Error(Exception ex, string message, params object[] args)
        {
            _logger?.Error(ex, message, args);
        }

        public static void Debug(string message, params object[] args)
        {
            _logger?.Debug(message, args);
        }

        public static void Dispose()
        {
            Log.CloseAndFlush();
        }

        public static string GetLogDirectory()
        {
            return LogDirectory;
        }
    }
}
