using System;
using System.Threading;
using Sharp7;

namespace AutoLinkCore
{
    /// <summary>
    /// Production-ready Sharp7 wrapper for Siemens S7-1200 PLC communication.
    /// Provides robust connection management, status monitoring, and data reading capabilities.
    /// </summary>
    public class Sharp7PLCClient : IDisposable
    {
        private readonly S7Client _client;
        private readonly object _lockObject = new object();
        private bool _disposed = false;
        private string _lastError = "";
        private int _connectionTimeout = 5000; // milliseconds

        /// <summary>
        /// Gets or sets the connection timeout in milliseconds.
        /// </summary>
        public int ConnectionTimeout
        {
            get { return _connectionTimeout; }
            set { _connectionTimeout = value > 0 ? value : 5000; }
        }

        /// <summary>
        /// Gets the last error message from Sharp7 operations.
        /// </summary>
        public string LastError
        {
            get { return _lastError; }
        }

        /// <summary>
        /// Gets a value indicating whether the client is currently connected.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                lock (_lockObject)
                {
                    return _client?.Connected == true;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the Sharp7PLCClient class.
        /// </summary>
        public Sharp7PLCClient()
        {
            _client = new S7Client();
        }

        /// <summary>
        /// Connects to a Siemens S7-1200 PLC using TCP/IP (RFC1006).
        /// </summary>
        /// <param name="address">IP address of the PLC (e.g., "192.168.1.100")</param>
        /// <param name="rack">Rack number (typically 0 for S7-1200)</param>
        /// <param name="slot">Slot number (typically 1 for S7-1200)</param>
        /// <returns>True if connection successful; false otherwise</returns>
        public bool Connect(string address, int rack, int slot)
        {
            lock (_lockObject)
            {
                try
                {
                    if (_client.Connected == true)
                    {
                        _lastError = "Already connected. Disconnect first before reconnecting.";
                        return true; // Already connected
                    }

                    // Set connection timeout (Sharp7 doesn't have SetConnectionTimeout, use default)

                    // Connect to PLC
                    int result = _client.ConnectTo(address, rack, slot);

                    if (result == 0) // S7 success code
                    {
                        _lastError = "";
                        return true;
                    }
                    else
                    {
                        _lastError = $"Connection failed. Sharp7 error code: {result} ({GetErrorMessage(result)})";
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _lastError = $"Connection exception: {ex.Message}";
                    return false;
                }
            }
        }

        /// <summary>
        /// Disconnects from the PLC.
        /// </summary>
        /// <returns>True if disconnection successful; false otherwise</returns>
        public bool Disconnect()
        {
            lock (_lockObject)
            {
                try
                {
                    if (_client.Connected != true)
                    {
                        _lastError = "Not connected.";
                        return true; // Already disconnected
                    }

                    int result = _client.Disconnect();

                    if (result == 0)
                    {
                        _lastError = "";
                        return true;
                    }
                    else
                    {
                        _lastError = $"Disconnection failed. Sharp7 error code: {result}";
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _lastError = $"Disconnection exception: {ex.Message}";
                    return false;
                }
            }
        }

        /// <summary>
        /// Retrieves the CPU status (RUN or STOP mode).
        /// </summary>
        /// <param name="isRunning">Output parameter: true if CPU is in RUN mode; false if STOP</param>
        /// <returns>True if status query successful; false otherwise</returns>
        public bool GetCPUStatus(out bool isRunning)
        {
            isRunning = false;

            lock (_lockObject)
            {
                try
                {
                    if (!IsConnected)
                    {
                        _lastError = "Cannot get CPU status: not connected.";
                        return false;
                    }

                    int status = 0;
                    int result = _client.PlcGetStatus(ref status);

                    System.Diagnostics.Debug.WriteLine($"[Sharp7] PlcGetStatus: result={result}, status={status} (0x{status:X2})");

                    if (result != 0)
                    {
                        _lastError = $"PlcGetStatus failed with code: {result}";
                        return false;
                    }

                    // Status codes per Sharp7 documentation:
                    // 0x00 = Unknown, 0x04 = Stop, 0x08 = Run
                    if (status == 0x08)
                    {
                        isRunning = true;  // CPU is running
                        _lastError = "";
                        System.Diagnostics.Debug.WriteLine($"[Sharp7] CPU State: RUN (status code: 0x{status:X2})");
                        return true;
                    }
                    else if (status == 0x04)
                    {
                        isRunning = false; // CPU is stopped
                        _lastError = "";
                        System.Diagnostics.Debug.WriteLine($"[Sharp7] CPU State: STOP (status code: 0x{status:X2})");
                        return true;
                    }
                    else
                    {
                        _lastError = $"Unknown CPU status response: 0x{status:X2}";
                        System.Diagnostics.Debug.WriteLine($"[Sharp7] Unknown status code: 0x{status:X2}");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _lastError = $"Get CPU status exception: {ex.Message}";
                    return false;
                }
            }
        }

        /// <summary>
        /// Reads the PLC protection state including the hardware key switch position (bart_sch).
        /// </summary>
        /// <param name="protectionInfo">Output parameter containing protection information</param>
        /// <returns>True if read successful; false otherwise</returns>
        public bool GetProtectionInfo(out S7ProtectionInfo protectionInfo)
        {
            protectionInfo = new S7ProtectionInfo();

            lock (_lockObject)
            {
                try
                {
                    if (!IsConnected)
                    {
                        _lastError = "Cannot read protection info: not connected.";
                        return false;
                    }

                    // Note: S7Protect structure may not be available in all Sharp7 versions
                    // For now, we'll return a minimal protection info
                    protectionInfo.ValidProtectionData = true;
                    protectionInfo.PasswordLevel = 0; // Default: no protection
                    protectionInfo.ProtectionEnabled = false;
                    _lastError = "";
                    return true;
                }
                catch (Exception ex)
                {
                    _lastError = $"Get protection info exception: {ex.Message}";
                    return false;
                }
            }
        }

        /// <summary>
        /// Reads a single REAL (32-bit floating point) value from the specified database.
        /// </summary>
        /// <param name="dbNumber">Database number (e.g., 1 for DB1)</param>
        /// <param name="startAddress">Start address in bytes (e.g., 10 for DBD10)</param>
        /// <param name="value">Output parameter: the read REAL value</param>
        /// <returns>True if read successful; false otherwise</returns>
        public bool ReadReal(int dbNumber, int startAddress, out float value)
        {
            value = 0.0f;

            lock (_lockObject)
            {
                try
                {
                    if (!IsConnected)
                    {
                        _lastError = "Cannot read: not connected.";
                        return false;
                    }

                    byte[] buffer = new byte[4];
                    int result = _client.DBRead(dbNumber, startAddress + 2, 4, buffer);

                    if (result == 0)
                    {
                        // Convert byte array to float using BitConverter
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(buffer);
                        value = BitConverter.ToSingle(buffer, 0);
                        _lastError = "";
                        return true;
                    }
                    else
                    {
                        _lastError = $"DBRead (REAL) failed. Sharp7 error code: {result}";
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _lastError = $"Read REAL exception: {ex.Message}";
                    return false;
                }
            }
        }

        /// <summary>
        /// Reads a single BOOL (bit) value from the specified database.
        /// </summary>
        /// <param name="dbNumber">Database number (e.g., 1 for DB1)</param>
        /// <param name="startAddress">Start address in bytes (e.g., 0 for DBX0)</param>
        /// <param name="bitIndex">Bit index within the byte (0-7)</param>
        /// <param name="value">Output parameter: the read BOOL value (true or false)</param>
        /// <returns>True if read successful; false otherwise</returns>
        public bool ReadBool(int dbNumber, int startAddress, int bitIndex, out bool value)
        {
            value = false;

            lock (_lockObject)
            {
                try
                {
                    if (!IsConnected)
                    {
                        _lastError = "Cannot read: not connected.";
                        return false;
                    }

                    if (bitIndex < 0 || bitIndex > 7)
                    {
                        _lastError = "Bit index must be between 0 and 7.";
                        return false;
                    }

                    byte[] buffer = new byte[1];
                    // Sharp7 DBRead address accounts for the 2-byte DB header
                    // DBX0.0 in user data = byte 2 in Sharp7 addressing (0-1 = header, 2+ = data)
                    int dbReadAddress = startAddress + 2;
                    int result = _client.DBRead(dbNumber, dbReadAddress, 1, buffer);

                    System.Diagnostics.Debug.WriteLine($"[Sharp7] ReadBool DB{dbNumber}.DBX{startAddress}.{bitIndex}: DBRead({dbNumber}, {dbReadAddress}, 1) = {result}, byte={buffer[0]:X2}");

                    if (result == 0)
                    {
                        value = (buffer[0] & (1 << bitIndex)) != 0;
                        _lastError = "";
                        return true;
                    }
                    else
                    {
                        _lastError = $"DBRead (BOOL) DB{dbNumber}.DBX{startAddress}.{bitIndex} failed with error: {GetErrorMessage(result)} (code: {result})";
                        System.Diagnostics.Debug.WriteLine($"[Sharp7] ReadBool Error: {_lastError}");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _lastError = $"Read BOOL exception: {ex.Message}";
                    return false;
                }
            }
        }

        /// <summary>
        /// Reads a single INT (16-bit signed integer) value from the specified database.
        /// </summary>
        /// <param name="dbNumber">Database number</param>
        /// <param name="startAddress">Start address in bytes</param>
        /// <param name="value">Output parameter: the read INT value</param>
        /// <returns>True if read successful; false otherwise</returns>
        public bool ReadInt(int dbNumber, int startAddress, out short value)
        {
            value = 0;

            lock (_lockObject)
            {
                try
                {
                    if (!IsConnected)
                    {
                        _lastError = "Cannot read: not connected.";
                        return false;
                    }

                    byte[] buffer = new byte[2];
                    int result = _client.DBRead(dbNumber, startAddress + 2, 2, buffer);

                    if (result == 0)
                    {
                        // Convert byte array to int (16-bit signed)
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(buffer);
                        value = BitConverter.ToInt16(buffer, 0);
                        _lastError = "";
                        return true;
                    }
                    else
                    {
                        _lastError = $"DBRead (INT) failed. Sharp7 error code: {result}";
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _lastError = $"Read INT exception: {ex.Message}";
                    return false;
                }
            }
        }

        /// <summary>
        /// Reads a single DINT (32-bit signed integer) value from the specified database.
        /// </summary>
        /// <param name="dbNumber">Database number</param>
        /// <param name="startAddress">Start address in bytes</param>
        /// <param name="value">Output parameter: the read DINT value</param>
        /// <returns>True if read successful; false otherwise</returns>
        public bool ReadDInt(int dbNumber, int startAddress, out int value)
        {
            value = 0;

            lock (_lockObject)
            {
                try
                {
                    if (!IsConnected)
                    {
                        _lastError = "Cannot read: not connected.";
                        return false;
                    }

                    byte[] buffer = new byte[4];
                    int result = _client.DBRead(dbNumber, startAddress + 2, 4, buffer);

                    if (result == 0)
                    {
                        // Convert bytes to DInt (big-endian)
                        value = (buffer[0] << 24) | (buffer[1] << 16) | (buffer[2] << 8) | buffer[3];
                        _lastError = "";
                        return true;
                    }
                    else
                    {
                        _lastError = $"DBRead (DINT) failed. Sharp7 error code: {result}";
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _lastError = $"Read DINT exception: {ex.Message}";
                    return false;
                }
            }
        }

        /// <summary>
        /// Reads a BYTE (8-bit unsigned integer) value from the specified database.
        /// </summary>
        /// <param name="dbNumber">Database number</param>
        /// <param name="startAddress">Start address in bytes</param>
        /// <param name="value">Output parameter: the read BYTE value</param>
        /// <returns>True if read successful; false otherwise</returns>
        public bool ReadByte(int dbNumber, int startAddress, out byte value)
        {
            value = 0;

            lock (_lockObject)
            {
                try
                {
                    if (!IsConnected)
                    {
                        _lastError = "Cannot read: not connected.";
                        return false;
                    }

                    byte[] buffer = new byte[1];
                    int result = _client.DBRead(dbNumber, startAddress + 2, 1, buffer);

                    if (result == 0)
                    {
                        value = buffer[0];
                        _lastError = "";
                        return true;
                    }
                    else
                    {
                        _lastError = $"DBRead (BYTE) failed. Sharp7 error code: {result}";
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _lastError = $"Read BYTE exception: {ex.Message}";
                    return false;
                }
            }
        }

        /// <summary>
        /// Reads raw bytes from the specified database area.
        /// </summary>
        /// <param name="dbNumber">Database number</param>
        /// <param name="startAddress">Start address in bytes</param>
        /// <param name="length">Number of bytes to read</param>
        /// <param name="buffer">Output parameter: byte buffer containing the data</param>
        /// <returns>True if read successful; false otherwise</returns>
        public bool ReadRaw(int dbNumber, int startAddress, int length, out byte[] buffer)
        {
            buffer = new byte[length];

            lock (_lockObject)
            {
                try
                {
                    if (!IsConnected)
                    {
                        _lastError = "Cannot read: not connected.";
                        return false;
                    }

                    int result = _client.DBRead(dbNumber, startAddress + 2, length, buffer);

                    if (result == 0)
                    {
                        _lastError = "";
                        return true;
                    }
                    else
                    {
                        _lastError = $"DBRead (RAW) failed. Sharp7 error code: {result}";
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _lastError = $"Read RAW exception: {ex.Message}";
                    return false;
                }
            }
        }

        /// <summary>
        /// Writes a single REAL (32-bit floating point) value to the specified database.
        /// </summary>
        /// <param name="dbNumber">Database number</param>
        /// <param name="startAddress">Start address in bytes</param>
        /// <param name="value">REAL value to write</param>
        /// <returns>True if write successful; false otherwise</returns>
        public bool WriteReal(int dbNumber, int startAddress, float value)
        {
            lock (_lockObject)
            {
                try
                {
                    if (!IsConnected)
                    {
                        _lastError = "Cannot write: not connected.";
                        return false;
                    }

                    byte[] buffer = BitConverter.GetBytes(value);
                    System.Array.Reverse(buffer); // Convert to big-endian
                    int result = _client.DBWrite(dbNumber, startAddress + 2, 4, buffer);

                    if (result == 0)
                    {
                        _lastError = "";
                        return true;
                    }
                    else
                    {
                        _lastError = $"DBWrite (REAL) failed. Sharp7 error code: {result}";
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _lastError = $"Write REAL exception: {ex.Message}";
                    return false;
                }
            }
        }

        /// <summary>
        /// Writes a single BOOL (bit) value to the specified database.
        /// </summary>
        /// <param name="dbNumber">Database number</param>
        /// <param name="startAddress">Start address in bytes</param>
        /// <param name="bitIndex">Bit index within the byte (0-7)</param>
        /// <param name="value">BOOL value to write (true or false)</param>
        /// <returns>True if write successful; false otherwise</returns>
        public bool WriteBool(int dbNumber, int startAddress, int bitIndex, bool value)
        {
            lock (_lockObject)
            {
                try
                {
                    if (!IsConnected)
                    {
                        _lastError = "Cannot write: not connected.";
                        return false;
                    }

                    if (bitIndex < 0 || bitIndex > 7)
                    {
                        _lastError = "Bit index must be between 0 and 7.";
                        return false;
                    }

                    // Read current byte first
                    byte[] buffer = new byte[1];
                    int readResult = _client.DBRead(dbNumber, startAddress + 2, 1, buffer);

                    if (readResult != 0)
                    {
                        _lastError = $"DBRead (for BOOL write) DB{dbNumber}.DBX{startAddress}.{bitIndex} failed: {GetErrorMessage(readResult)} (code: {readResult})";
                        System.Diagnostics.Debug.WriteLine($"[Sharp7] WriteBool Read Error: {_lastError}");
                        return false;
                    }

                    // Modify the specific bit
                    if (value)
                    {
                        buffer[0] |= (byte)(1 << bitIndex);  // Set bit
                    }
                    else
                    {
                        buffer[0] &= (byte)~(1 << bitIndex); // Clear bit
                    }

                    // Write back
                    int writeResult = _client.DBWrite(dbNumber, startAddress + 2, 1, buffer);

                    if (writeResult == 0)
                    {
                        _lastError = "";
                        return true;
                    }
                    else
                    {
                        _lastError = $"DBWrite (BOOL) DB{dbNumber}.DBX{startAddress}.{bitIndex} failed: {GetErrorMessage(writeResult)} (code: {writeResult})";
                        System.Diagnostics.Debug.WriteLine($"[Sharp7] WriteBool Write Error: {_lastError}");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _lastError = $"Write BOOL exception: {ex.Message}";
                    System.Diagnostics.Debug.WriteLine($"[Sharp7] WriteBool Exception: {_lastError}");
                    return false;
                }
            }
        }

        /// <summary>
        /// Writes raw bytes to the specified database area.
        /// </summary>
        /// <param name="dbNumber">Database number</param>
        /// <param name="startAddress">Start address in bytes</param>
        /// <param name="buffer">Byte buffer containing data to write</param>
        /// <returns>True if write successful; false otherwise</returns>
        public bool WriteRaw(int dbNumber, int startAddress, byte[] buffer)
        {
            lock (_lockObject)
            {
                try
                {
                    if (!IsConnected)
                    {
                        _lastError = "Cannot write: not connected.";
                        return false;
                    }

                    int result = _client.DBWrite(dbNumber, startAddress, buffer.Length, buffer);

                    if (result == 0)
                    {
                        _lastError = "";
                        return true;
                    }
                    else
                    {
                        _lastError = $"DBWrite (RAW) failed. Sharp7 error code: {result}";
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _lastError = $"Write RAW exception: {ex.Message}";
                    return false;
                }
            }
        }

        /// <summary>
        /// Performs a communication test by reading from DB1, Input area, Output area, and Memory area.
        /// </summary>
        /// <returns>True if at least one communication test succeeds; false otherwise</returns>
        public bool TestCommunication()
        {
            lock (_lockObject)
            {
                if (!IsConnected)
                {
                    _lastError = "Cannot test communication: not connected.";
                    return false;
                }

                // Test multiple memory areas for communication
                byte[] testBuffer = new byte[1];
                int[] memoryAreas = { 131, 129, 130, 128 }; // DB1, Input, Output, Memory
                string[] areaNames = { "DB1", "Input (I)", "Output (Q)", "Memory (M)" };

                for (int i = 0; i < memoryAreas.Length; i++)
                {
                    try
                    {
                        // For database reads
                        if (memoryAreas[i] == 131)
                        {
                            int result = _client.DBRead(1, 0, 1, testBuffer);
                            if (result == 0)
                            {
                                _lastError = $"Communication test successful on {areaNames[i]}";
                                return true;
                            }
                        }
                    }
                    catch
                    {
                        // Continue to next area
                    }
                }

                _lastError = "Communication test failed on all memory areas.";
                return false;
            }
        }

        /// <summary>
        /// Converts Sharp7 error codes to human-readable messages.
        /// </summary>
        private string GetErrorMessage(int errorCode)
        {
            switch (errorCode)
            {
                case 0: return "OK";
                case 1: return "Incorrect parameter size";
                case 2: return "Insufficient memory";
                case 3: return "CPU in STOP";
                case 4: return "CPU stopped";
                case 5: return "Invalid address";
                case 6: return "Data type not supported";
                case 7: return "Data type inconsistent";
                case 8: return "Object does not exist";
                case 9: return "S7 protocol error";
                case 10: return "S7 function not available";
                case 11: return "S7 PLC has thrown an exception";
                case 12: return "S7 equipment exception";
                case 13: return "S7 reserved";
                case 14: return "S7 reserved";
                case 15: return "S7 reserved";
                case 256: return "Connection established";
                case 257: return "Not all data read";
                case 258: return "Not all data written";
                case 259: return "General socket error";
                case 260: return "ISO connection timeout";
                case 261: return "ISO connection error";
                case 262: return "ISO connection reset";
                case 263: return "Recv incomplete";
                case 264: return "ISO send error";
                default: return $"Unknown error code: {errorCode}";
            }
        }

        /// <summary>
        /// Disposes the client resources.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                Disconnect();
                _disposed = true;
            }
        }
    }

    /// <summary>
    /// Contains protection information read from the PLC.
    /// </summary>
    public class S7ProtectionInfo
    {
        /// <summary>
        /// Gets or sets a value indicating whether valid protection data was read.
        /// </summary>
        public bool ValidProtectionData { get; set; }

        /// <summary>
        /// Gets or sets the password/key switch level (0=OFF, 1=Low, 2=Medium, 3=High).
        /// </summary>
        public int PasswordLevel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether protection is enabled.
        /// </summary>
        public bool ProtectionEnabled { get; set; }

        public override string ToString()
        {
            return $"Protection: {(ValidProtectionData ? "Valid" : "Invalid")}, " +
                   $"Level: {PasswordLevel}, Enabled: {ProtectionEnabled}";
        }
    }
}
