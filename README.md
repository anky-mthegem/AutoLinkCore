# AutoLink Core - Siemens S7-1200 PLC Management Application

## Description
AutoLink Core is a Windows .NET Framework 4.8 application for managing and monitoring Siemens S7-1200 PLCs. Built by Singh Automation.

## Features
- **Authentication**: Secure login system (default: admin/123456)
- **PLC Connection**: Real-time connection to S7-1200 PLCs via Ethernet (S7.Net.Plus)
- **Network Diagnostics**: Ping testing and connection validation
- **PLC Diagnostics**: Comprehensive PLC status monitoring and information
- **Modern UI**: Light theme with 3D buttons and professional design
- **Navigation**: Single-window design with panel-based navigation

## System Requirements
- Windows 7 or higher
- .NET Framework 4.8
- Network access to Siemens S7-1200 PLC (Port 102)

## How to Run

### Option 1: Run from Project Directory (Recommended)
1. Extract the downloaded ZIP file
2. Open PowerShell/Command Prompt
3. Navigate to the `AutoLinkCore` folder
4. Run: `.\bin\Debug\AutoLinkCore.exe`

### Option 2: Build from Source
1. Extract the downloaded ZIP file
2. Open PowerShell/Command Prompt
3. Navigate to the `AutoLinkCore` folder
4. Build: `C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe AutoLinkCore.csproj /p:Configuration=Debug`
5. Run: `.\bin\Debug\AutoLinkCore.exe`

### Option 3: Run EXE Directly
1. Extract the downloaded ZIP file
2. Navigate to `AutoLinkCore\bin\Debug\`
3. Double-click `AutoLinkCore.exe`
4. Allow Windows Defender if prompted

**Note**: If the app doesn't start, logo.png might be missing. The app will now run without it.

## Default Login Credentials
- **Username**: admin
- **Password**: 123456

## PLC Connection Settings
- **CPU Type**: S7-1200
- **Default IP**: 192.168.0.1
- **Default Rack**: 0
- **Default Slot**: 1
- **Protocol**: ISO over TCP (RFC1006)
- **Port**: 102

## Application Structure
```
AutoLinkCore/
├── bin/Debug/              # Compiled executable and dependencies
├── Properties/             # Assembly information
├── packages/               # NuGet packages (S7.Net.Plus, etc.)
├── *.cs                    # C# source files
├── *.Designer.cs           # Windows Forms designer files
├── *.resx                  # Resource files
├── AutoLinkCore.csproj     # Project file
└── packages.config         # NuGet package configuration
```

## Main Components
- **LoginForm.cs**: User authentication screen
- **MainForm.cs**: Main application window with menu system
- **ConnectionSettingsControl.cs**: PLC connection configuration and testing
- **PLCDiagnosticsControl.cs**: PLC status and diagnostic information display
- **AppTheme.cs**: Light theme styling system

## Menu Structure
- **File**: Connect/Disconnect PLC, Export Logs, User Login/Logout, Exit
- **View**: Dashboard, Connection Settings, Diagnostics, Fullscreen Mode, Toggle Sidebar
- **Settings**: PLC IP Address, Communication Timeout, Database Configuration
- **Tools**: Manual Operation, I/O Simulator, Backup/Restore Recipe
- **Help**: Documentation, About, License Info

## Libraries Used
- **S7.Net.Plus v0.20.0**: Siemens S7 PLC communication
- **System.Buffers v4.5.1**: Memory management
- **System.Memory v4.5.5**: Span and Memory types
- **System.Numerics.Vectors v4.5.0**: SIMD vector operations
- **System.Runtime.CompilerServices.Unsafe v4.5.3**: Low-level memory operations

## Troubleshooting

### App doesn't start after allowing Windows Defender
- The logo.png file is missing. This has been fixed in the latest version.
- Update to the latest code from GitHub.

### Connection to PLC fails
- Verify PLC IP address is correct
- Ensure Port 102 is not blocked by firewall
- Check PLC is in RUN mode and Ethernet cable is connected
- Verify PLC permits PUT/GET communication (TIA Portal settings)

### Build errors
- Ensure .NET Framework 4.8 is installed
- Verify all NuGet packages are restored
- Run MSBuild from the AutoLinkCore directory

## Development
- **Language**: C# 5.0 compatible syntax
- **Framework**: .NET Framework 4.8
- **IDE**: Visual Studio Code / Visual Studio
- **Build Tool**: MSBuild v4.0.30319

## Author
Singh Automation

## License
Copyright © 2026 Singh Automation. All rights reserved.

## Repository
https://github.com/anky-mthegem/AutoLinkCore.git
