$ErrorActionPreference = "Stop"

$projectRoot = Split-Path -Parent $PSScriptRoot
$apiPath = Join-Path $projectRoot "backend\distometer-api"
$venvPath = Join-Path $apiPath ".venv"

Write-Host "=== Dist-O-Meter Setup ==="

# --------------------------------------------------
# Find Python 3.11
# --------------------------------------------------

Write-Host "Checking for Python 3.11..."

$pythonCommand = $null

# First try the Windows Python launcher
if (Get-Command py -ErrorAction SilentlyContinue) {
    try {
        py -3.11 --version | Out-Null

        if ($LASTEXITCODE -eq 0) {
            $pythonCommand = "py"
        }
    }
    catch {
        # Python 3.11 was not found through py
    }
}

if ($null -eq $pythonCommand) {
    Write-Host ""
    Write-Host "ERROR: Python 3.11 was not found."
    Write-Host "Please install Python 3.11 and run this script again."
    exit 1
}

# --------------------------------------------------
# Create virtual environment
# --------------------------------------------------

if (-not (Test-Path $venvPath)) {

    Write-Host "Creating Python virtual environment..."

    & $pythonCommand -3.11 -m venv $venvPath

}
else {

    Write-Host "Python virtual environment already exists."

}

# --------------------------------------------------
# Install dependencies
# --------------------------------------------------

$venvPython = Join-Path $venvPath "Scripts\python.exe"
$requirements = Join-Path $apiPath "requirements.txt"

Write-Host "Installing API dependencies..."

& $venvPython -m pip install --upgrade pip
& $venvPython -m pip install -r $requirements

Write-Host ""
Write-Host "=== Setup completed successfully ==="