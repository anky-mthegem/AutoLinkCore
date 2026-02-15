using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace AutoLinkCore
{
    /// <summary>
    /// Advanced SQL synchronization with deadband filtering and bulk operations
    /// Reduces database load by filtering redundant updates and batching operations
    /// </summary>
    public class AdvancedSqlSyncManager
    {
        private readonly Dictionary<string, object> _lastValues = new Dictionary<string, object>();
        private readonly Dictionary<string, double> _deadbands = new Dictionary<string, double>();
        private readonly object _lockObject = new object();

        /// <summary>
        /// Sets deadband threshold for a tag
        /// </summary>
        public void SetDeadband(string tagName, double deadband)
        {
            lock (_lockObject)
            {
                _deadbands[tagName] = Math.Abs(deadband);
                EventLogger.Log(EventLogger.EVENT_INFO, 
                    $"Deadband set: {tagName} = {deadband}", "AdvancedSqlSync", 0);
            }
        }

        /// <summary>
        /// Checks if value should be updated based on deadband filtering
        /// </summary>
        public bool ShouldUpdateValue(string tagName, object newValue)
        {
            lock (_lockObject)
            {
                if (!_lastValues.ContainsKey(tagName))
                {
                    _lastValues[tagName] = newValue;
                    return true;
                }

                if (!_deadbands.ContainsKey(tagName))
                {
                    bool shouldUpdate = !Equals(_lastValues[tagName], newValue);
                    if (shouldUpdate)
                        _lastValues[tagName] = newValue;
                    return shouldUpdate;
                }

                // Check numeric deadband
                try
                {
                    double oldVal = Convert.ToDouble(_lastValues[tagName]);
                    double newVal = Convert.ToDouble(newValue);
                    double change = Math.Abs(newVal - oldVal);

                    if (change >= _deadbands[tagName])
                    {
                        _lastValues[tagName] = newValue;
                        return true;
                    }
                }
                catch
                {
                    // If conversion fails, fall back to equality check
                    bool shouldUpdate = !Equals(_lastValues[tagName], newValue);
                    if (shouldUpdate)
                        _lastValues[tagName] = newValue;
                    return shouldUpdate;
                }

                return false;
            }
        }

        /// <summary>
        /// Performs bulk insert into SQL Server
        /// </summary>
        public bool BulkInsertSync(string connectionString, string tableName, DataTable dataTable)
        {
            if (dataTable == null || dataTable.Rows.Count == 0)
                return true;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        bulkCopy.DestinationTableName = tableName;
                        bulkCopy.BulkCopyTimeout = 30;
                        bulkCopy.BatchSize = 1000;

                        foreach (DataColumn column in dataTable.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                        }

                        bulkCopy.WriteToServer(dataTable);
                    }
                }

                EventLogger.Log(EventLogger.EVENT_DATA_SYNC,
                    $"Bulk inserted {dataTable.Rows.Count} rows to {tableName}",
                    "AdvancedSqlSync", 0);
                return true;
            }
            catch (Exception ex)
            {
                EventLogger.Log(EventLogger.EVENT_ERROR,
                    $"Bulk insert error: {ex.Message}",
                    "AdvancedSqlSync", 2);
                return false;
            }
        }

        /// <summary>
        /// Check if SQL Server is available
        /// </summary>
        public bool IsSqlServerAvailable(string connectionString)
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString)
                {
                    ConnectTimeout = 5
                };
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Clear all deadbands
        /// </summary>
        public void ClearDeadbands()
        {
            lock (_lockObject)
            {
                _deadbands.Clear();
                _lastValues.Clear();
            }
        }
    }
}
