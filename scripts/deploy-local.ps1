# This script builds the Unity WebGL project locally and deploys it to Azure Static Web Apps.
# Usage: .\scripts\deploy-local.ps1 -Environment test

Param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("test", "prod")]
    [string]$Environment = "test"
)

# --- Configuration ---
$UnityVersion = "6000.3.9f1"
$UnityPath = "C:\Program Files\Unity\Hub\Editor\$UnityVersion\Editor\Unity.exe"
$BuildDir = "Build\WebGL"
$ConfigPath = "$BuildDir\StreamingAssets\config.json"

# Environment Specific URLs (Update these to match your Azure resources)
$EnvConfigs = @{
    "test" = @{
        "ApiBaseUrl" = "https://your-test-api.azurewebsites.net/api"
        "ChatHubUrl" = "https://your-test-api.azurewebsites.net/chatHub"
    }
    "prod" = @{
        "ApiBaseUrl" = "https://your-prod-api.azurewebsites.net/api"
        "ChatHubUrl" = "https://your-prod-api.azurewebsites.net/chatHub"
    }
}

# --- 1. Build Unity ---
Write-Host "Starting Unity WebGL Build ($Environment)..." -ForegroundColor Cyan

if (-not (Test-Path $UnityPath)) {
    Write-Error "Unity Editor not found at $UnityPath. Please update the UnityPath variable in this script."
    exit 1
}

$UnityArgs = "-batchmode -nographics -projectPath . -executeMethod BuildScript.BuildWebGL -quit -logFile unity_build.log"
Start-Process -FilePath $UnityPath -ArgumentList $UnityArgs -Wait -NoNewWindow

if ($LASTEXITCODE -ne 0) {
    Write-Error "Unity build failed. Check unity_build.log for details."
    exit 1
}

Write-Host "Unity Build Complete." -ForegroundColor Green

# --- 2. Inject Configuration ---
if (Test-Path $ConfigPath) {
    Write-Host "Injecting configuration for $Environment..." -ForegroundColor Cyan
    $Config = Get-Content $ConfigPath | ConvertFrom-Json
    $Config.ApiBaseUrl = $EnvConfigs[$Environment].ApiBaseUrl
    $Config.ChatHubUrl = $EnvConfigs[$Environment].ChatHubUrl
    $Config | ConvertTo-Json | Set-Content $ConfigPath
    Write-Host "Configuration updated in $ConfigPath" -ForegroundColor Green
} else {
    Write-Warning "Config file not found at $ConfigPath. Skipping injection."
}

# --- 3. Deploy to Azure SWA ---
Write-Host "Deploying to Azure Static Web Apps ($Environment)..." -ForegroundColor Cyan

# Note: This assumes you have run 'swa login' previously
# The --env flag helps SWA CLI find the right resource if you have multiple
swa deploy $BuildDir --env $Environment

if ($LASTEXITCODE -eq 0) {
    Write-Host "Deployment Successful!" -ForegroundColor Green
} else {
    Write-Error "SWA Deployment failed."
    exit 1
}
