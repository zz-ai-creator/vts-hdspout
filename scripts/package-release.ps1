param(
    [string]$Configuration = "Release",
    [string]$PackageVersion = "0.1.0"
)

$ErrorActionPreference = "Stop"

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$project = Join-Path $repoRoot "src\VTS_HDOutput\VTS_HDOutput.csproj"
$nugetConfig = Join-Path $repoRoot "NuGet.config"
$pluginDll = Join-Path $repoRoot "src\VTS_HDOutput\bin\$Configuration\VTS_HDOutput.dll"
$bepInExZipName = "BepInEx_win_x64_5.4.23.4.zip"
$bepInExZip = Join-Path $repoRoot ".downloads\$bepInExZipName"
$tempBepInExZip = Join-Path $env:TEMP "bepinex_5_4_23_4_check\$bepInExZipName"
$packageName = "VTS_HDOutput-$PackageVersion-installer"
$stagingRoot = Join-Path $repoRoot "artifacts\$packageName"
$distRoot = Join-Path $repoRoot "dist"
$zipPath = Join-Path $distRoot "$packageName.zip"

$env:NUGET_PACKAGES = Join-Path $repoRoot ".nuget\packages"
$env:DOTNET_CLI_HOME = Join-Path $repoRoot ".dotnet"

New-Item -ItemType Directory -Force -Path $env:NUGET_PACKAGES, $env:DOTNET_CLI_HOME | Out-Null

dotnet restore $project --configfile $nugetConfig
if ($LASTEXITCODE -ne 0) {
    throw "dotnet restore failed with exit code $LASTEXITCODE"
}

dotnet build $project -c $Configuration --no-restore
if ($LASTEXITCODE -ne 0) {
    throw "dotnet build failed with exit code $LASTEXITCODE"
}

if (-not (Test-Path -LiteralPath $pluginDll)) {
    throw "VTS_HDOutput.dll was not built: $pluginDll"
}

if (-not (Test-Path -LiteralPath $bepInExZip)) {
    if (Test-Path -LiteralPath $tempBepInExZip) {
        New-Item -ItemType Directory -Force -Path (Split-Path -Parent $bepInExZip) | Out-Null
        Copy-Item -LiteralPath $tempBepInExZip -Destination $bepInExZip -Force
    }
    else {
        throw "BepInEx package was not found: $bepInExZip"
    }
}

if (Test-Path -LiteralPath $stagingRoot) {
    Remove-Item -LiteralPath $stagingRoot -Recurse -Force
}

New-Item -ItemType Directory -Force -Path `
    $stagingRoot, `
    (Join-Path $stagingRoot "plugin"), `
    (Join-Path $stagingRoot "scripts"), `
    (Join-Path $stagingRoot "bepinex"), `
    $distRoot | Out-Null

Copy-Item -LiteralPath (Join-Path $repoRoot "README.md") -Destination $stagingRoot -Force
Copy-Item -LiteralPath (Join-Path $repoRoot "install-vts-hdoutput.bat") -Destination $stagingRoot -Force
Copy-Item -LiteralPath (Join-Path $repoRoot "uninstall-vts-hdoutput.bat") -Destination $stagingRoot -Force
Copy-Item -LiteralPath (Join-Path $repoRoot "scripts\install-bepinex.ps1") -Destination (Join-Path $stagingRoot "scripts") -Force
Copy-Item -LiteralPath (Join-Path $repoRoot "scripts\deploy-plugin.ps1") -Destination (Join-Path $stagingRoot "scripts") -Force
Copy-Item -LiteralPath (Join-Path $repoRoot "scripts\uninstall-plugin.ps1") -Destination (Join-Path $stagingRoot "scripts") -Force
Copy-Item -LiteralPath $pluginDll -Destination (Join-Path $stagingRoot "plugin") -Force
Copy-Item -LiteralPath $bepInExZip -Destination (Join-Path $stagingRoot "bepinex") -Force

if (Test-Path -LiteralPath $zipPath) {
    Remove-Item -LiteralPath $zipPath -Force
}

Compress-Archive -Path (Join-Path $stagingRoot "*") -DestinationPath $zipPath -CompressionLevel Optimal

Write-Host "Created package: $zipPath"
