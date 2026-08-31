$ErrorActionPreference = "Stop"

$projectRoot = Split-Path -Parent $PSScriptRoot

$apiScript = Join-Path $PSScriptRoot "run_api.ps1"

$guiProject = Join-Path `
    $projectRoot `
    "web\DistOMeter\DistOMeter.GUI\DistOMeter.GUI.csproj"

$localDotnet = Join-Path `
    $projectRoot `
    ".tools\dotnet\dotnet.exe"

Write-Host "=== Starting Dist-O-Meter ==="

# --------------------------------------------------
# Find .NET
# --------------------------------------------------

$dotnetCommand = $null

if (Test-Path $localDotnet) {
    $dotnetCommand = $localDotnet
}
else {
    $globalDotnet = Get-Command dotnet -ErrorAction SilentlyContinue

    if ($globalDotnet) {
        $dotnetCommand = $globalDotnet.Source
    }
}

if ($null -eq $dotnetCommand) {
    Write-Host ""
    Write-Host "ERROR: .NET was not found."
    Write-Host "Run .\scripts\setup.ps1 first."
    exit 1
}

# --------------------------------------------------
# Check Python environment
# --------------------------------------------------

$venvPython = Join-Path `
    $projectRoot `
    "backend\distometer-api\.venv\Scripts\python.exe"

if (-not (Test-Path $venvPython)) {
    Write-Host ""
    Write-Host "ERROR: Python environment was not found."
    Write-Host "Run .\scripts\setup.ps1 first."
    exit 1
}

# --------------------------------------------------
# Start API
# --------------------------------------------------

Write-Host "Starting API..."

Start-Process `
    powershell `
    -ArgumentList "-NoExit", "-ExecutionPolicy", "Bypass", "-File", "`"$apiScript`""

# Give FastAPI a moment to start
Start-Sleep -Seconds 2

# --------------------------------------------------
# Start GUI
# --------------------------------------------------

Write-Host "Starting web application..."

Start-Process `
    $dotnetCommand `
    -ArgumentList "run", "--project", "`"$guiProject`"", "--urls", "http://127.0.0.1:5000"

# --------------------------------------------------
# Wait for GUI
# --------------------------------------------------

Write-Host "Waiting for Dist-O-Meter..."

$url = "http://127.0.0.1:5000"
$started = $false

for ($i = 0; $i -lt 30; $i++) {

    try {
        $response = Invoke-WebRequest `
            -Uri $url `
            -UseBasicParsing `
            -TimeoutSec 1

        if ($response.StatusCode -eq 200) {
            $started = $true
            break
        }
    }
    catch {
        Start-Sleep -Seconds 1
    }
}

if (-not $started) {
    Write-Host ""
    Write-Host "ERROR: Web application did not start."
    exit 1
}

# --------------------------------------------------
# Open browser
# --------------------------------------------------

Write-Host ""
Write-Host "Dist-O-Meter is running:"
Write-Host $url
Write-Host ""

Start-Process $url