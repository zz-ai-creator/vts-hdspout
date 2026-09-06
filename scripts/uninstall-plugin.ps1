param(
    [string]$VtsRoot = "C:\Program Files (x86)\Steam\steamapps\common\VTube Studio",
    [switch]$StopVts,
    [switch]$KeepConfig
)

$ErrorActionPreference = "Stop"

$vtsExe = Join-Path $VtsRoot "VTube Studio.exe"
$pluginDir = Join-Path $VtsRoot "BepInEx\plugins\VTS_HDOutput"
$legacyPluginDir = Join-Path $VtsRoot "BepInEx\plugins\VTS_HDSpout"
$configPath = Join-Path $VtsRoot "BepInEx\config\me.codex.plugin.vts.hdoutput.cfg"

if (-not (Test-Path -LiteralPath $vtsExe)) {
    throw "VTube Studio.exe was not found: $vtsExe"
}

if ($StopVts) {
    $vtsProcesses = Get-Process -Name "VTube Studio" -ErrorAction SilentlyContinue
    if ($vtsProcesses) {
        $vtsProcesses | Stop-Process -Force
        Start-Sleep -Seconds 2
        Write-Host "Stopped running VTube Studio process."
    }
}

if (Test-Path -LiteralPath $pluginDir) {
    Remove-Item -LiteralPath $pluginDir -Recurse -Force
    Write-Host "Removed plugin folder: $pluginDir"
}
else {
    Write-Host "Plugin folder was not present: $pluginDir"
}

if (Test-Path -LiteralPath $legacyPluginDir) {
    Remove-Item -LiteralPath $legacyPluginDir -Recurse -Force
    Write-Host "Removed legacy plugin folder: $legacyPluginDir"
}

if ($KeepConfig) {
    Write-Host "Kept plugin config: $configPath"
}
elseif (Test-Path -LiteralPath $configPath) {
    Remove-Item -LiteralPath $configPath -Force
    Write-Host "Removed plugin config: $configPath"
}
else {
    Write-Host "Plugin config was not present: $configPath"
}

Write-Host "VTS_HDOutput uninstalled. BepInEx was left installed."
