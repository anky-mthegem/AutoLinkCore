using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace AutoLinkCore
{
    /// <summary>
    /// Offline data buffering using SQLite
    /// Caches sync data when SQL Server is unavailable, then auto-flushes on reconnection
    /// </summary>
    public class OfflineBuffer
    {
        private readonly string _dbPath;
        private readonly string _connectionString;
        private const string TableName = "SyncBuffer";

        public OfflineBuffer(string bufferDirectory = null)
        {
            if (bufferDirectory == null)
            {
                bufferDirectory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "AutoLinkCore",
                    "Cache"
                );
            }

            if (!Directory.Exists(bufferDirectory))
                Directory.CreateDirectory(bufferDirectory);

            _dbPath = Path.Combine(bufferDirectory, "SyncBuffer.db");
            _connectionString = $"Data Source={_dbPath};Version=3;";

            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    string createTableSql = $@"
                        CREATE TABLE IF NOT EXISTS {TableName} (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
                            TagName TEXT NOT NULL,
                            Value TEXT NOT NULL,
                            DataType TEXT NOT NULL,
                            DBNumber INTEGER,
                            Address INTEGER
                        );
                        CREATE INDEX IF NOT EXISTS idx_timestamp ON {TableName}(Timestamp);
                    ";

                    using (var command = new SQLiteCommand(createTableSql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                AppLogger.Information("OfflineBuffer database initialized");
            }
            catch (Exception ex)
            {
                AppLogger.Error(ex, "Failed to initialize offline buffer database");
            }
        }

        /// <summary>
        /// Buffers a data point when SQL connection is down
        /// </summary>
        public bool BufferData(string tagName, object value, string dataType, int dbNumber = 0, int address = 0)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    string sql = $@"
                        INSERT INTO {TableName} (TagName, Value, DataType, DBNumber, Address)
                        VALUES (@tagName, @value, @dataType, @dbNumber, @address)
                    ";

                    using (var command = new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@tagName", tagName);
                        command.Parameters.AddWithValue("@value", value?.ToString() ?? "");
                        command.Parameters.AddWithValue("@dataType", dataType);
                        command.Parameters.AddWithValue("@dbNumber", dbNumber);
                        command.Parameters.AddWithValue("@address", address);

                        command.ExecuteNonQuery();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                AppLogger.Error(ex, $"Failed to buffer data for tag {tagName}");
                return false;
            }
        }

        /// <summary>
        /// Retrieves all buffered data points (typically for flushing to main database)
        /// </summary>
        public List<SyncBufferEntry> GetBufferedData(int limitRecords = 0)
        {
            var entries = new List<SyncBufferEntry>();

            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    string sql = $"SELECT Id, Timestamp, TagName, Value, DataType, DBNumber, Address FROM {TableName} ORDER BY Timestamp";
                    if (limitRecords > 0)
                        sql += $" LIMIT {limitRecords}";

                    using (var command = new SQLiteCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                entries.Add(new SyncBufferEntry
                                {
                                    Id = reader.GetInt32(0),
                                    Timestamp = reader.GetDateTime(1),
                                    TagName = reader.GetString(2),
                                    Value = reader.GetString(3),
                                    DataType = reader.GetString(4),
                                    DBNumber = reader.GetInt32(5),
                                    Address = reader.GetInt32(6)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error(ex, "Failed to retrieve buffered data");
            }

            return entries;
        }

        /// <summary>
        /// Clears buffered data after successful flush
        /// </summary>
        public bool ClearBufferedData(List<int> ids)
        {
            if (ids.Count == 0)
                return true;

            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    string placeholders = string.Join(",", ids);
                    string sql = $"DELETE FROM {TableName} WHERE Id IN ({placeholders})";

                    using (var command = new SQLiteCommand(sql, connection))
                    {
                        int deleted = command.ExecuteNonQuery();
                        AppLogger.Information($"Cleared {deleted} buffered entries");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error(ex, "Failed to clear buffered data");
                return false;
            }
        }

        public int GetBufferSize()
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new SQLiteCommand($"SELECT COUNT(*) FROM {TableName}", connection))
                    {
                        return (int)(long)command.ExecuteScalar();
                    }
                }
            }
            catch
            {
                return 0;
            }
        }
    }

    public class SyncBufferEntry
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string TagName { get; set; }
        public string Value { get; set; }
        public string DataType { get; set; }
        public int DBNumber { get; set; }
        public int Address { get; set; }
    }
}
