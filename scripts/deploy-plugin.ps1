param(
    [string]$VtsRoot = "C:\Program Files (x86)\Steam\steamapps\common\VTube Studio",
    [string]$Configuration = "Release",
    [string]$PluginDllPath,
    [switch]$NoBuild,
    [switch]$StopVts,
    [switch]$StartVts,
    [switch]$RestartVts
)

$ErrorActionPreference = "Stop"

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$project = Join-Path $repoRoot "src\VTS_HDOutput\VTS_HDOutput.csproj"
$output = Join-Path $repoRoot "src\VTS_HDOutput\bin\$Configuration"
$builtPluginDll = Join-Path $output "VTS_HDOutput.dll"
$nugetConfig = Join-Path $repoRoot "NuGet.config"
$env:NUGET_PACKAGES = Join-Path $repoRoot ".nuget\packages"
$env:DOTNET_CLI_HOME = Join-Path $repoRoot ".dotnet"
$pluginDir = Join-Path $VtsRoot "BepInEx\plugins\VTS_HDOutput"
$legacyPluginDir = Join-Path $VtsRoot "BepInEx\plugins\VTS_HDSpout"
$vtsExe = Join-Path $VtsRoot "VTube Studio.exe"
$shouldStopVts = $StopVts -or $RestartVts
$shouldStartVts = $StartVts -or $RestartVts

if ($shouldStopVts) {
    $vtsProcesses = Get-Process -Name "VTube Studio" -ErrorAction SilentlyContinue
    if ($vtsProcesses) {
        $vtsProcesses | Stop-Process -Force
        Start-Sleep -Seconds 2
        Write-Host "Stopped running VTube Studio process."
    }
}

if (-not $NoBuild) {
    New-Item -ItemType Directory -Force -Path $env:NUGET_PACKAGES, $env:DOTNET_CLI_HOME | Out-Null

    dotnet restore $project --configfile $nugetConfig
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet restore failed with exit code $LASTEXITCODE"
    }

    dotnet build $project -c $Configuration --no-restore
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet build failed with exit code $LASTEXITCODE"
    }
}

$sourcePluginDll = $builtPluginDll
if ($PluginDllPath) {
    if (-not (Test-Path -LiteralPath $PluginDllPath)) {
        throw "VTS_HDOutput.dll was not found: $PluginDllPath"
    }

    $sourcePluginDll = (Resolve-Path -LiteralPath $PluginDllPath).Path
}

if (-not (Test-Path -LiteralPath $sourcePluginDll)) {
    throw "VTS_HDOutput.dll was not found: $sourcePluginDll"
}

New-Item -ItemType Directory -Force -Path $pluginDir | Out-Null

try {
    Copy-Item -LiteralPath $sourcePluginDll -Destination $pluginDir -Force
}
catch [System.IO.IOException] {
    throw "Failed to copy VTS_HDOutput.dll. Close VTube Studio or rerun with -RestartVts. Original error: $($_.Exception.Message)"
}

Write-Host "Deployed VTS_HDOutput to $pluginDir"
Write-Host "Using VTS bundled native KlakSpout.dll from VTube Studio_Data\Plugins\x86_64"

if (Test-Path -LiteralPath $legacyPluginDir) {
    Remove-Item -LiteralPath $legacyPluginDir -Recurse -Force
    Write-Host "Removed legacy VTS_HDSpout plugin folder: $legacyPluginDir"
}

if ($shouldStartVts) {
    if (-not (Test-Path -LiteralPath $vtsExe)) {
        throw "Cannot start VTube Studio because the executable was not found: $vtsExe"
    }

    Start-Process -FilePath $vtsExe -ArgumentList "-nosteam" -WorkingDirectory $VtsRoot -WindowStyle Normal
    Write-Host "Started VTube Studio with -nosteam."
}
