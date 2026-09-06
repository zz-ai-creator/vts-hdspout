@echo off
setlocal EnableExtensions

set "SCRIPT_DIR=%~dp0"
set "DEFAULT_VTS_ROOT=C:\Program Files (x86)\Steam\steamapps\common\VTube Studio"

if /I "%~1"=="--help" goto :usage
if /I "%~1"=="-h" goto :usage
if /I "%~1"=="/?" goto :usage

if "%~1"=="" (
    set "VTS_ROOT=%DEFAULT_VTS_ROOT%"
) else (
    set "VTS_ROOT=%~1"
)

echo.
echo VTS_HDOutput one-click uninstaller
echo ----------------------------------
echo VTS root: "%VTS_ROOT%"
echo.

set "POWERSHELL_EXE=%SystemRoot%\System32\WindowsPowerShell\v1.0\powershell.exe"

if not exist "%VTS_ROOT%\VTube Studio.exe" (
    echo ERROR: VTube Studio.exe was not found in:
    echo "%VTS_ROOT%"
    echo.
    echo Usage:
    echo   uninstall-vts-hdoutput.bat
    echo   uninstall-vts-hdoutput.bat "D:\SteamLibrary\steamapps\common\VTube Studio"
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

pushd "%SCRIPT_DIR%" >nul
if errorlevel 1 (
    echo ERROR: Cannot enter uninstaller directory:
    echo "%SCRIPT_DIR%"
    call :finish 1
    exit /b 1
)

echo [1/2] Stopping VTube Studio if it is running...
"%POWERSHELL_EXE%" -NoProfile -ExecutionPolicy Bypass -Command "Get-Process -Name 'VTube Studio' -ErrorAction SilentlyContinue | Stop-Process -Force; Start-Sleep -Seconds 2"
if errorlevel 1 goto :failed

echo.
echo [2/2] Removing VTS_HDOutput plugin files...
"%POWERSHELL_EXE%" -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT_DIR%scripts\uninstall-plugin.ps1" -VtsRoot "%VTS_ROOT%"
if errorlevel 1 goto :failed

echo.
echo Uninstalled successfully.
echo BepInEx was left installed because it may be used by other plugins.
popd >nul
call :finish 0
exit /b %ERRORLEVEL%

:failed
set "UNINSTALL_EXIT=%ERRORLEVEL%"
echo.
echo ERROR: Uninstall failed with exit code %UNINSTALL_EXIT%.
popd >nul
call :finish %UNINSTALL_EXIT%
exit /b %ERRORLEVEL%

:usage
echo VTS_HDOutput one-click uninstaller
echo.
echo Usage:
echo   uninstall-vts-hdoutput.bat
echo   uninstall-vts-hdoutput.bat "D:\SteamLibrary\steamapps\common\VTube Studio"
echo.
echo Default VTS root:
echo   %DEFAULT_VTS_ROOT%
echo.
echo Removes the VTS_HDOutput plugin folder and config file.
echo BepInEx is not removed.
echo Set VTS_HDOUTPUT_NO_PAUSE=1 to skip the final pause.
exit /b 0

:finish
set "FINISH_EXIT=%~1"
if not "%VTS_HDOUTPUT_NO_PAUSE%"=="1" (
    echo.
    pause
)
exit /b %FINISH_EXIT%
