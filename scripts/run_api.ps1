$ErrorActionPreference = "Stop"

$projectRoot = Split-Path -Parent $PSScriptRoot
$apiPath = Join-Path $projectRoot "backend\distometer-api"
$venvPython = Join-Path $apiPath ".venv\Scripts\python.exe"

if (-not (Test-Path $venvPython)) {
    Write-Host "ERROR: Python environment not found."
    Write-Host "Run .\scripts\setup.ps1 first."
    exit 1
}

Write-Host "Starting Dist-O-Meter API..."
Write-Host "API: http://127.0.0.1:8000"
Write-Host "Swagger: http://127.0.0.1:8000/docs"
Write-Host ""

Set-Location $apiPath

& $venvPython -m uvicorn main:app --host 127.0.0.1 --port 8000