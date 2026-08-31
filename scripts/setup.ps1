$ErrorActionPreference = "Stop"

$projectRoot = Split-Path -Parent $PSScriptRoot
$apiPath = Join-Path $projectRoot "backend\distometer-api"
$venvPath = Join-Path $apiPath ".venv"
$toolsPath = Join-Path $projectRoot ".tools"
$uvPath = Join-Path $toolsPath "uv.exe"

Write-Host "=== Dist-O-Meter Setup ==="

# --------------------------------------------------
# Create tools directory
# --------------------------------------------------

if (-not (Test-Path $toolsPath)) {
    New-Item -ItemType Directory -Path $toolsPath | Out-Null
}

# --------------------------------------------------
# Find or install uv
# --------------------------------------------------

Write-Host "Checking for uv..."

$uvCommand = $null

# Prefer a global uv installation if one already exists
$globalUv = Get-Command uv -ErrorAction SilentlyContinue

if ($globalUv) {
    $uvCommand = $globalUv.Source
    Write-Host "Using installed uv."
}
elseif (Test-Path $uvPath) {
    $uvCommand = $uvPath
    Write-Host "Using local uv."
}
else {
    Write-Host "uv was not found. Downloading local copy..."

    $installScript = Invoke-RestMethod https://astral.sh/uv/install.ps1

    $env:UV_INSTALL_DIR = $toolsPath

    Invoke-Expression $installScript

    if (-not (Test-Path $uvPath)) {
        Write-Host ""
        Write-Host "ERROR: uv could not be installed."
        exit 1
    }

    $uvCommand = $uvPath
}

# --------------------------------------------------
# Ensure Python 3.11 exists
# --------------------------------------------------

Write-Host "Checking for Python 3.11..."

& $uvCommand python install 3.11

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "ERROR: Python 3.11 could not be prepared."
    exit 1
}

# --------------------------------------------------
# Create virtual environment
# --------------------------------------------------

if (-not (Test-Path $venvPath)) {
    Write-Host "Creating Python virtual environment..."

    & $uvCommand venv $venvPath --python 3.11
}
else {
    Write-Host "Python virtual environment already exists."
}

# --------------------------------------------------
# Install dependencies
# --------------------------------------------------

$venvPython = Join-Path $venvPath "Scripts\python.exe"
$requirements = Join-Path $apiPath "requirements.txt"

if (-not (Test-Path $venvPython)) {
    Write-Host ""
    Write-Host "ERROR: Python virtual environment is invalid."
    exit 1
}

Write-Host "Installing API dependencies..."

& $uvCommand pip install `
    --python $venvPython `
    -r $requirements

# --------------------------------------------------
# Find or install .NET 8 SDK
# --------------------------------------------------

Write-Host ""
Write-Host "Checking for .NET 8 SDK..."

$dotnetCommand = $null
$localDotnetPath = Join-Path $toolsPath "dotnet"
$localDotnetExe = Join-Path $localDotnetPath "dotnet.exe"

# First check global .NET
$globalDotnet = Get-Command dotnet -ErrorAction SilentlyContinue

if ($globalDotnet) {

    $hasDotnet8 = & $globalDotnet.Source --list-sdks |
        Where-Object { $_ -match "^8\." }

    if ($hasDotnet8) {
        $dotnetCommand = $globalDotnet.Source
        Write-Host "Using installed .NET 8 SDK."
    }
}

# Then check local .NET
if (($null -eq $dotnetCommand) -and (Test-Path $localDotnetExe)) {

    $hasDotnet8 = & $localDotnetExe --list-sdks |
        Where-Object { $_ -match "^8\." }

    if ($hasDotnet8) {
        $dotnetCommand = $localDotnetExe
        Write-Host "Using local .NET 8 SDK."
    }
}

# Download local .NET 8 if necessary
if ($null -eq $dotnetCommand) {

    Write-Host ".NET 8 SDK was not found. Downloading local copy..."

    $dotnetInstallScript =
        Join-Path $toolsPath "dotnet-install.ps1"

    Invoke-WebRequest `
        -Uri "https://dot.net/v1/dotnet-install.ps1" `
        -OutFile $dotnetInstallScript

    & $dotnetInstallScript `
        -Channel 8.0 `
        -InstallDir $localDotnetPath `
        -NoPath

    if (-not (Test-Path $localDotnetExe)) {
        Write-Host ""
        Write-Host "ERROR: .NET 8 SDK could not be installed."
        exit 1
    }

    $dotnetCommand = $localDotnetExe
}

# --------------------------------------------------
# Build C# solution
# --------------------------------------------------

$solutionPath =
    Join-Path $projectRoot "web\DistOMeter\DistOMeter.sln"

Write-Host ""
Write-Host "Building Dist-O-Meter web application..."

& $dotnetCommand build $solutionPath

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "ERROR: Dist-O-Meter could not be built."
    exit 1
}

Write-Host ""
Write-Host "=== Setup completed successfully ==="