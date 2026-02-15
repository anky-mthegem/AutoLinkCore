// ============================================================================
// Sharp7PLCClient - Production-Ready Usage Examples
// ============================================================================
// This document provides practical examples for using the Sharp7PLCClient wrapper
// for communicating with Siemens S7-1200 PLCs.
// ============================================================================

using System;
using AutoLinkCore;

namespace Sharp7ClientExamples
{
    public class Sharp7ClientUsageExamples
    {
        // ======================================================================
        // Example 1: Basic Connection and Status Check
        // ======================================================================
        public void Example_BasicConnectionAndStatus()
        {
            Console.WriteLine("\n=== Example 1: Basic Connection and Status ===");

            using (var client = new Sharp7PLCClient())
            {
                // Set connection timeout
                client.ConnectionTimeout = 5000;

                // Connect to PLC
                string plcIP = "10.208.19.184";
                int rack = 0;
                int slot = 1;

                if (client.Connect(plcIP, rack, slot))
                {
                    Console.WriteLine($"✓ Connected to PLC at {plcIP}");

                    // Check CPU status
                    if (client.GetCPUStatus(out bool isRunning))
                    {
                        string status = isRunning ? "RUNNING" : "STOPPED";
                        Console.WriteLine($"✓ CPU Status: {status}");
                    }
                    else
                    {
                        Console.WriteLine($"✗ Failed to get CPU status: {client.LastError}");
                    }

                    // Disconnect
                    client.Disconnect();
                    Console.WriteLine("✓ Disconnected");
                }
                else
                {
                    Console.WriteLine($"✗ Connection failed: {client.LastError}");
                }
            }
        }

        // ======================================================================
        // Example 2: Reading Single Values (REAL and BOOL)
        // ======================================================================
        public void Example_ReadingSingleValues()
        {
            Console.WriteLine("\n=== Example 2: Reading Single Values ===");

            using (var client = new Sharp7PLCClient())
            {
                if (!client.Connect("10.208.19.184", 0, 1))
                {
                    Console.WriteLine($"Connection failed: {client.LastError}");
                    return;
                }

                // Read REAL value from DB1, DBD10 (4 bytes starting at byte 10)
                if (client.ReadReal(1, 10, out float realValue))
                {
                    Console.WriteLine($"✓ DB1.DBD10 (REAL): {realValue}");
                }
                else
                {
                    Console.WriteLine($"✗ Failed to read REAL: {client.LastError}");
                }

                // Read BOOL value from DB1, DBX0.0 (byte 0, bit 0)
                if (client.ReadBool(1, 0, 0, out bool boolValue))
                {
                    Console.WriteLine($"✓ DB1.DBX0.0 (BOOL): {(boolValue ? "TRUE" : "FALSE")}");
                }
                else
                {
                    Console.WriteLine($"✗ Failed to read BOOL: {client.LastError}");
                }

                // Read other data types
                if (client.ReadInt(1, 0, out short intValue))
                {
                    Console.WriteLine($"✓ DB1.DBW0 (INT): {intValue}");
                }

                if (client.ReadByte(1, 0, out byte byteValue))
                {
                    Console.WriteLine($"✓ DB1.DBB0 (BYTE): {byteValue}");
                }

                client.Disconnect();
            }
        }

        // ======================================================================
        // Example 3: Writing Values to PLC
        // ======================================================================
        public void Example_WritingValues()
        {
            Console.WriteLine("\n=== Example 3: Writing Values to PLC ===");

            using (var client = new Sharp7PLCClient())
            {
                if (!client.Connect("10.208.19.184", 0, 1))
                {
                    Console.WriteLine($"Connection failed: {client.LastError}");
                    return;
                }

                // Write REAL value (42.5) to DB1, DBD20
                if (client.WriteReal(1, 20, 42.5f))
                {
                    Console.WriteLine("✓ Successfully wrote REAL value 42.5 to DB1.DBD20");
                }
                else
                {
                    Console.WriteLine($"✗ Failed to write REAL: {client.LastError}");
                }

                // Write BOOL value (true) to DB1, DBX1.3 (byte 1, bit 3)
                if (client.WriteBool(1, 1, 3, true))
                {
                    Console.WriteLine("✓ Successfully wrote BOOL value TRUE to DB1.DBX1.3");
                }
                else
                {
                    Console.WriteLine($"✗ Failed to write BOOL: {client.LastError}");
                }

                client.Disconnect();
            }
        }

        // ======================================================================
        // Example 4: Reading Protection Information
        // ======================================================================
        public void Example_ReadingProtectionInfo()
        {
            Console.WriteLine("\n=== Example 4: Reading Protection Information ===");

            using (var client = new Sharp7PLCClient())
            {
                if (!client.Connect("10.208.19.184", 0, 1))
                {
                    Console.WriteLine($"Connection failed: {client.LastError}");
                    return;
                }

                if (client.GetProtectionInfo(out S7ProtectionInfo protInfo))
                {
                    Console.WriteLine($"✓ {protInfo}");
                    Console.WriteLine($"  Protection info retrieved (simplified without S7Protect access)");
                }
                else
                {
                    Console.WriteLine($"✗ Failed to read protection info: {client.LastError}");
                }

                client.Disconnect();
            }
        }

        // ======================================================================
        // Example 5: Reading Raw Binary Data
        // ======================================================================
        public void Example_ReadingRawData()
        {
            Console.WriteLine("\n=== Example 5: Reading Raw Binary Data ===");

            using (var client = new Sharp7PLCClient())
            {
                if (!client.Connect("10.208.19.184", 0, 1))
                {
                    Console.WriteLine($"Connection failed: {client.LastError}");
                    return;
                }

                // Read 16 bytes from DB1, starting at byte 0
                if (client.ReadRaw(1, 0, 16, out byte[] buffer))
                {
                    Console.WriteLine("✓ Read 16 bytes from DB1.0:");
                    Console.WriteLine($"  Hex: {BitConverter.ToString(buffer)}");
                    Console.WriteLine($"  Values: {string.Join(", ", buffer)}");
                }
                else
                {
                    Console.WriteLine($"✗ Failed to read raw data: {client.LastError}");
                }

                client.Disconnect();
            }
        }

        // ======================================================================
        // Example 6: Communication Testing
        // ======================================================================
        public void Example_CommunicationTest()
        {
            Console.WriteLine("\n=== Example 6: Communication Test ===");

            using (var client = new Sharp7PLCClient())
            {
                if (!client.Connect("10.208.19.184", 0, 1))
                {
                    Console.WriteLine($"Connection failed: {client.LastError}");
                    return;
                }

                if (client.TestCommunication())
                {
                    Console.WriteLine($"✓ {client.LastError}");
                }
                else
                {
                    Console.WriteLine($"✗ {client.LastError}");
                }

                client.Disconnect();
            }
        }

        // ======================================================================
        // Example 7: Error Handling and Retry Logic
        // ======================================================================
        public void Example_ErrorHandlingAndRetry()
        {
            Console.WriteLine("\n=== Example 7: Error Handling and Retry Logic ===");

            using (var client = new Sharp7PLCClient())
            {
                client.ConnectionTimeout = 3000;

                int maxRetries = 3;
                int retryCount = 0;
                bool connected = false;

                while (retryCount < maxRetries && !connected)
                {
                    if (client.Connect("10.208.19.184", 0, 1))
                    {
                        connected = true;
                        Console.WriteLine($"✓ Connected on attempt {retryCount + 1}");
                    }
                    else
                    {
                        retryCount++;
                        Console.WriteLine($"✗ Attempt {retryCount} failed: {client.LastError}");

                        if (retryCount < maxRetries)
                        {
                            System.Threading.Thread.Sleep(1000); // Wait 1 second before retry
                        }
                    }
                }

                if (connected)
                {
                    // Read value with error handling
                    if (client.ReadReal(1, 10, out float value))
                    {
                        Console.WriteLine($"✓ Read value: {value}");
                    }
                    else
                    {
                        Console.WriteLine($"✗ Read failed: {client.LastError}");
                    }

                    client.Disconnect();
                }
                else
                {
                    Console.WriteLine($"✗ Could not connect after {maxRetries} attempts");
                }
            }
        }

        // ======================================================================
        // Example 8: Continuous Monitoring Loop
        // ======================================================================
        public void Example_ContinuousMonitoring()
        {
            Console.WriteLine("\n=== Example 8: Continuous Monitoring Loop ===");

            using (var client = new Sharp7PLCClient())
            {
                if (!client.Connect("10.208.19.184", 0, 1))
                {
                    Console.WriteLine($"Connection failed: {client.LastError}");
                    return;
                }

                // Monitor for 10 seconds
                DateTime endTime = DateTime.Now.AddSeconds(10);
                int readCount = 0;

                while (DateTime.Now < endTime)
                {
                    if (client.IsConnected)
                    {
                        // Read current values
                        if (client.ReadReal(1, 10, out float realValue) &&
                            client.ReadBool(1, 0, 0, out bool boolValue))
                        {
                            readCount++;
                            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Read #{readCount}: " +
                                            $"REAL={realValue:F2}, BOOL={boolValue}");
                        }
                        else
                        {
                            Console.WriteLine($"✗ Read error: {client.LastError}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("✗ Connection lost!");
                        break;
                    }

                    System.Threading.Thread.Sleep(500); // Read every 500ms
                }

                client.Disconnect();
            }
        }

        // ======================================================================
        // Example 9: Working with Multiple Database Objects
        // ======================================================================
        public void Example_MultipleDataValues()
        {
            Console.WriteLine("\n=== Example 9: Multiple Data Values ===");

            using (var client = new Sharp7PLCClient())
            {
                if (!client.Connect("10.208.19.184", 0, 1))
                {
                    Console.WriteLine($"Connection failed: {client.LastError}");
                    return;
                }

                // Define a struct-like data mapping
                var dataPoints = new[]
                {
                    new { DB = 1, Address = 0, Type = "BOOL_0", Bit = 0 },
                    new { DB = 1, Address = 0, Type = "BOOL_1", Bit = 1 },
                    new { DB = 1, Address = 10, Type = "REAL", Bit = 0 },
                    new { DB = 1, Address = 20, Type = "INT", Bit = 0 },
                };

                foreach (var point in dataPoints)
                {
                    try
                    {
                        if (point.Type == "REAL")
                        {
                            if (client.ReadReal(point.DB, point.Address, out float value))
                                Console.WriteLine($"✓ {point.Type} @ DB{point.DB}.{point.Address}: {value}");
                        }
                        else if (point.Type.StartsWith("BOOL"))
                        {
                            if (client.ReadBool(point.DB, point.Address, point.Bit, out bool value))
                                Console.WriteLine($"✓ {point.Type} @ DB{point.DB}.{point.Address}.{point.Bit}: {value}");
                        }
                        else if (point.Type == "INT")
                        {
                            if (client.ReadInt(point.DB, point.Address, out short value))
                                Console.WriteLine($"✓ {point.Type} @ DB{point.DB}.{point.Address}: {value}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"✗ Error reading {point.Type}: {ex.Message}");
                    }
                }

                client.Disconnect();
            }
        }

        // ======================================================================
        // Example 10: Connection Status Monitoring
        // ======================================================================
        public void Example_ConnectionStatusMonitoring()
        {
            Console.WriteLine("\n=== Example 10: Connection Status Monitoring ===");

            using (var client = new Sharp7PLCClient())
            {
                Console.WriteLine($"Before connect - IsConnected: {client.IsConnected}");

                if (client.Connect("10.208.19.184", 0, 1))
                {
                    Console.WriteLine($"After connect - IsConnected: {client.IsConnected}");

                    if (client.GetCPUStatus(out bool isRunning))
                    {
                        Console.WriteLine($"CPU is in {(isRunning ? "RUN" : "STOP")} mode");
                    }

                    client.Disconnect();
                    Console.WriteLine($"After disconnect - IsConnected: {client.IsConnected}");
                }
                else
                {
                    Console.WriteLine($"Connection failed: {client.LastError}");
                }
            }
        }
    }
}
