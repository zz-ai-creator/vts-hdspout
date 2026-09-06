param(
    [string]$VtsRoot = "C:\Program Files (x86)\Steam\steamapps\common\VTube Studio"
)

$ErrorActionPreference = "Stop"

$url = "https://github.com/BepInEx/BepInEx/releases/download/v5.4.23.4/BepInEx_win_x64_5.4.23.4.zip"
$packageBepInExZip = Join-Path $PSScriptRoot "..\bepinex\BepInEx_win_x64_5.4.23.4.zip"
$downloads = Join-Path $PSScriptRoot "..\.downloads"
$zipPath = Join-Path $downloads "BepInEx_win_x64_5.4.23.4.zip"
$tempZipPath = Join-Path $env:TEMP "bepinex_5_4_23_4_check\BepInEx_win_x64_5.4.23.4.zip"
$bepInExCore = Join-Path $VtsRoot "BepInEx\core\BepInEx.dll"
$winHttp = Join-Path $VtsRoot "winhttp.dll"
$doorstopConfig = Join-Path $VtsRoot "doorstop_config.ini"
$doorstopVersion = Join-Path $VtsRoot ".doorstop_version"

$requiredFiles = @($bepInExCore, $winHttp, $doorstopConfig, $doorstopVersion)
$missingFiles = $requiredFiles | Where-Object { -not (Test-Path -LiteralPath $_) }

if (-not $missingFiles) {
    Write-Host "BepInEx is already installed at $VtsRoot"
    return
}

New-Item -ItemType Directory -Force -Path $downloads | Out-Null

if (-not (Test-Path -LiteralPath $zipPath)) {
    if (Test-Path -LiteralPath $packageBepInExZip) {
        Copy-Item -LiteralPath $packageBepInExZip -Destination $zipPath -Force
    }
    elseif (Test-Path -LiteralPath $tempZipPath) {
        Copy-Item -LiteralPath $tempZipPath -Destination $zipPath -Force
    }
    else {
        Invoke-WebRequest -Uri $url -OutFile $zipPath
    }
}

Expand-Archive -LiteralPath $zipPath -DestinationPath $VtsRoot -Force

Write-Host "BepInEx files installed to $VtsRoot"
Write-Host "Launch VTS once with: $VtsRoot\start_without_steam.bat"
Write-Host "After first launch, verify: $VtsRoot\BepInEx\core\BepInEx.dll"
