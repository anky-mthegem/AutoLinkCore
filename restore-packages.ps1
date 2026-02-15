param([string]$packagesDir = "AutoLinkCore\packages")

# Create packages directory
if (-not (Test-Path $packagesDir)) {
    New-Item -ItemType Directory -Path $packagesDir -Force | Out-Null
}

# Define packages to download
$packages = @(
    @{Id = "Sharp7"; Version = "1.4.10"; URL = "https://www.nuget.org/api/v2/package/Sharp7/1.4.10"},
    @{Id = "System.Buffers"; Version = "4.5.1"; URL = "https://www.nuget.org/api/v2/package/System.Buffers/4.5.1"},
    @{Id = "System.Memory"; Version = "4.5.5"; URL = "https://www.nuget.org/api/v2/package/System.Memory/4.5.5"},
    @{Id = "System.Numerics.Vectors"; Version = "4.5.0"; URL = "https://www.nuget.org/api/v2/package/System.Numerics.Vectors/4.5.0"},
    @{Id = "System.Runtime.CompilerServices.Unsafe"; Version = "4.5.3"; URL = "https://www.nuget.org/api/v2/package/System.Runtime.CompilerServices.Unsafe/4.5.3"}
)

foreach ($pkg in $packages) {
    $pkgDir = Join-Path $packagesDir "$($pkg.Id).$($pkg.Version)"
    $pkgFile = "$pkgDir.nupkg"
    
    Write-Host "Downloading $($pkg.Id) v$($pkg.Version)..."
    
    try {
        Invoke-WebRequest -Uri $pkg.URL -OutFile $pkgFile -UseBasicParsing
        if (Test-Path $pkgFile) {
            Expand-Archive -Path $pkgFile -DestinationPath $pkgDir -Force
            Write-Host "  OK: Extracted to $pkgDir"
        }
    }
    catch {
        Write-Host "  ERROR: $_"
    }
}

Write-Host "Done!"
