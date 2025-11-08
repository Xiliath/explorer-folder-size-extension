@echo off
REM Build script for Folder Size Extension

echo.
echo ========================================
echo Folder Size Extension - Build Script
echo ========================================
echo.

REM Check if dotnet is available
where dotnet >nul 2>&1
if %errorLevel% neq 0 (
    echo ERROR: .NET SDK not found
    echo Please install .NET 8.0 SDK or later from:
    echo https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo Restoring NuGet packages...
dotnet restore FolderSizeExtension.sln

if %errorLevel% neq 0 (
    echo ERROR: NuGet restore failed
    pause
    exit /b 1
)

echo.
echo Building solution (Release configuration)...
dotnet build FolderSizeExtension.sln -c Release

if %errorLevel% neq 0 (
    echo ERROR: Build failed
    pause
    exit /b 1
)

echo.
echo ========================================
echo Build Complete!
echo ========================================
echo.
echo The application has been built successfully.
echo Output location: FolderSizeExtension\bin\Release\net8.0-windows\
echo.
echo Next steps:
echo 1. Run Install.bat as Administrator to install the extension
echo.
pause
