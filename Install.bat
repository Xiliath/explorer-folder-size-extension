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
set INSTALL_DIR=%ProgramFiles%\FolderSizeExtension
set DLL_NAME=FolderSizeExtension.dll
set BUILD_DIR=%~dp0FolderSizeExtension\bin\Release\net6.0-windows

REM Check if DLL exists
if not exist "%BUILD_DIR%\%DLL_NAME%" (
    echo ERROR: Extension DLL not found.
    echo Please build the project in Release mode first.
    echo Expected location: %BUILD_DIR%\%DLL_NAME%
    pause
    exit /b 1
)

REM Create installation directory
echo Creating installation directory...
if not exist "%INSTALL_DIR%" mkdir "%INSTALL_DIR%"

REM Copy files
echo Copying extension files...
xcopy /Y /I "%BUILD_DIR%\*.*" "%INSTALL_DIR%\"

REM Register the COM server using SharpShell
echo.
echo Registering COM server...
cd /d "%INSTALL_DIR%"

REM Use regasm for .NET COM registration
"%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\regasm.exe" /codebase "%INSTALL_DIR%\%DLL_NAME%"

if %errorLevel% neq 0 (
    echo ERROR: COM registration failed
    pause
    exit /b 1
)

REM Alternatively, if SharpShell Server Registration Manager is available
REM Use: srm install "%INSTALL_DIR%\%DLL_NAME%" -codebase

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
echo 3. Select "Calculate Folder Size" from the context menu
echo 4. The size will be calculated and cached
echo 5. Refresh the Explorer window (F5) to see the results in tooltips
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
