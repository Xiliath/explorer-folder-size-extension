@echo off
REM Installation script for Folder Size Extension
REM Must be run as Administrator

echo.
echo ========================================
echo Folder Size Extension - Installer
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

echo Installing Folder Size Extension...
echo.

REM Set paths
set INSTALL_DIR=%ProgramFiles%\FolderSizeCalculator
set EXE_NAME=FolderSizeCalculator.exe
set BUILD_DIR=%~dp0FolderSizeExtension\bin\Release\net8.0-windows

REM Check if executable exists
if not exist "%BUILD_DIR%\%EXE_NAME%" (
    echo ERROR: Folder Size Calculator executable not found.
    echo Please build the project in Release mode first.
    echo Expected location: %BUILD_DIR%\%EXE_NAME%
    pause
    exit /b 1
)

REM Create installation directory
echo Creating installation directory...
if not exist "%INSTALL_DIR%" mkdir "%INSTALL_DIR%"

REM Copy files
echo Copying application files...
xcopy /Y /I "%BUILD_DIR%\*.*" "%INSTALL_DIR%\"

REM Register context menu entries
echo.
echo Registering context menu entries...
cd /d "%~dp0"

REM Import registry entries
regedit /s AddToContextMenu.reg

if %errorLevel% neq 0 (
    echo ERROR: Registry import failed
    echo You may need to manually import AddToContextMenu.reg
    pause
    exit /b 1
)

echo.
echo ========================================
echo Installation Complete!
echo ========================================
echo.
echo The Folder Size Extension has been installed.
echo.
echo Usage:
echo 1. Navigate to a folder in Windows Explorer
echo 2. Right-click on any folder
echo 3. Select "Calculate Folder Size" or "Calculate Folder Size (All Subfolders)"
echo 4. A progress window will show calculation status
echo 5. Results are cached for future reference
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
echo Installation complete!
pause
