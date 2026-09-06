@echo off
setlocal EnableExtensions

set "SCRIPT_DIR=%~dp0"
set "DEFAULT_VTS_ROOT=C:\Program Files (x86)\Steam\steamapps\common\VTube Studio"
set "PACKAGED_PLUGIN_DLL=%SCRIPT_DIR%plugin\VTS_HDOutput.dll"

if /I "%~1"=="--help" goto :usage
if /I "%~1"=="-h" goto :usage
if /I "%~1"=="/?" goto :usage

if "%~1"=="" (
    set "VTS_ROOT=%DEFAULT_VTS_ROOT%"
) else (
    set "VTS_ROOT=%~1"
)

echo.
echo VTS_HDOutput one-click installer
echo --------------------------------
echo VTS root: "%VTS_ROOT%"
echo.

set "POWERSHELL_EXE=%SystemRoot%\System32\WindowsPowerShell\v1.0\powershell.exe"

if not exist "%VTS_ROOT%\VTube Studio.exe" (
    echo ERROR: VTube Studio.exe was not found in:
    echo "%VTS_ROOT%"
    echo.
    echo Usage:
    echo   install-vts-hdoutput.bat
    echo   install-vts-hdoutput.bat "D:\SteamLibrary\steamapps\common\VTube Studio"
    call :finish 1
    exit /b 1
)

if not exist "%POWERSHELL_EXE%" (
    where powershell.exe >nul 2>nul
    if errorlevel 1 (
        set "POWERSHELL_EXE="
    ) else (
        set "POWERSHELL_EXE=powershell.exe"
    )
)

if "%POWERSHELL_EXE%"=="" (
    echo ERROR: powershell.exe was not found.
    call :finish 1
    exit /b 1
)

if exist "%PACKAGED_PLUGIN_DLL%" (
    set "INSTALL_MODE=package"
) else (
    set "INSTALL_MODE=source"
    where dotnet.exe >nul 2>nul
    if errorlevel 1 (
        echo ERROR: plugin\VTS_HDOutput.dll was not found and dotnet.exe is not available.
        echo Use the release zip package, or install the .NET SDK for source-tree installs.
        call :finish 1
        exit /b 1
    )
)

pushd "%SCRIPT_DIR%" >nul
if errorlevel 1 (
    echo ERROR: Cannot enter installer directory:
    echo "%SCRIPT_DIR%"
    call :finish 1
    exit /b 1
)

echo [1/4] Stopping VTube Studio if it is running...
"%POWERSHELL_EXE%" -NoProfile -ExecutionPolicy Bypass -Command "Get-Process -Name 'VTube Studio' -ErrorAction SilentlyContinue | Stop-Process -Force; Start-Sleep -Seconds 2"
if errorlevel 1 goto :failed

echo.
echo [2/4] Installing BepInEx...
"%POWERSHELL_EXE%" -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT_DIR%scripts\install-bepinex.ps1" -VtsRoot "%VTS_ROOT%"
if errorlevel 1 goto :failed

echo.
if /I "%INSTALL_MODE%"=="package" (
    echo [3/4] Deploying packaged VTS_HDOutput...
    "%POWERSHELL_EXE%" -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT_DIR%scripts\deploy-plugin.ps1" -VtsRoot "%VTS_ROOT%" -StopVts -NoBuild -PluginDllPath "%PACKAGED_PLUGIN_DLL%"
) else (
    echo [3/4] Building and deploying VTS_HDOutput...
    "%POWERSHELL_EXE%" -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT_DIR%scripts\deploy-plugin.ps1" -VtsRoot "%VTS_ROOT%" -StopVts
)
if errorlevel 1 goto :failed

echo.
echo [4/4] Starting VTube Studio...
if exist "%VTS_ROOT%\start_without_steam.bat" (
    start "" /D "%VTS_ROOT%" "%VTS_ROOT%\start_without_steam.bat"
) else (
    start "" /D "%VTS_ROOT%" "%VTS_ROOT%\VTube Studio.exe" -nosteam
)

echo.
echo Installed successfully.
echo In OBS, add a Spout2 source and select: VTS_HDOutput
popd >nul
call :finish 0
exit /b %ERRORLEVEL%

:failed
set "INSTALL_EXIT=%ERRORLEVEL%"
echo.
echo ERROR: Installation failed with exit code %INSTALL_EXIT%.
popd >nul
call :finish %INSTALL_EXIT%
exit /b %ERRORLEVEL%

:usage
echo VTS_HDOutput one-click installer
echo.
echo Usage:
echo   install-vts-hdoutput.bat
echo   install-vts-hdoutput.bat "D:\SteamLibrary\steamapps\common\VTube Studio"
echo.
echo Default VTS root:
echo   %DEFAULT_VTS_ROOT%
echo.
echo Set VTS_HDOUTPUT_NO_PAUSE=1 to skip the final pause.
exit /b 0

:finish
set "FINISH_EXIT=%~1"
if not "%VTS_HDOUTPUT_NO_PAUSE%"=="1" (
    echo.
    pause
)
exit /b %FINISH_EXIT%
