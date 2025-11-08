@echo off
REM Uninstallation script for Folder Size Extension
REM Must be run as Administrator

echo.
echo ========================================
echo Folder Size Extension - Uninstaller
echo ========================================
echo.

REM Check for administrator privileges
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo ERROR: This script must be run as Administrator
    echo Right-click on this file and select "Run as administrator"
    pause
    exit /b 1
)

echo Uninstalling Folder Size Extension...
echo.

REM Set paths
set INSTALL_DIR=%ProgramFiles%\FolderSizeCalculator
set EXE_NAME=FolderSizeCalculator.exe

REM Check if installed
if not exist "%INSTALL_DIR%\%EXE_NAME%" (
    echo Extension does not appear to be installed.
    echo Installation directory not found: %INSTALL_DIR%
    pause
    exit /b 1
)

REM Remove context menu entries
echo Removing context menu entries...
cd /d "%~dp0"
regedit /s RemoveFromContextMenu.reg

REM Remove installation directory
echo Removing installation files...
rd /s /q "%INSTALL_DIR%"

REM Clean up cache (optional)
set CACHE_DIR=%APPDATA%\FolderSizeExtension
if exist "%CACHE_DIR%" (
    echo Removing cache directory...
    rd /s /q "%CACHE_DIR%"
)

echo.
echo ========================================
echo Uninstallation Complete!
echo ========================================
echo.
echo The Folder Size Extension has been removed.
echo.
echo Note: You may need to restart Windows Explorer for changes to take effect.
echo Press any key to restart Explorer now, or close this window to restart later.
echo.
pause

REM Restart Windows Explorer
echo Restarting Windows Explorer...
taskkill /f /im explorer.exe
start explorer.exe

echo.
echo Uninstallation complete!
pause
