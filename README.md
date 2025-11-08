# Folder Size Calculator for Windows Explorer

A modern .NET 8 application that adds folder size calculation capabilities to Windows Explorer through context menu integration. Calculate folder sizes on-demand with a clean progress interface and persistent caching.

## Features

- **Context Menu Integration**: Right-click on any folder to calculate its size
- **Individual or Batch Calculation**: Calculate a single folder or all subfolders at once
- **Persistent Cache**: Calculated sizes are cached and persist across sessions
- **Progress Display**: Real-time progress window with cancellation support
- **Manual Trigger**: Sizes are only calculated when you explicitly request them (no performance impact)
- **Modern .NET 8**: Built on the latest LTS version of .NET

## Architecture

This is a standalone .NET 8 Windows Forms application that integrates with Explorer via Windows Registry context menu entries.

**Components:**
1. **FolderSizeCalculator**: Background calculation engine with async/await support
2. **FolderSizeCache**: Thread-safe JSON-based persistent cache
3. **FolderSizeCalculatorForm**: WinForms UI for displaying progress
4. **FormatUtils**: Utility functions for formatting file sizes
5. **Registry Integration**: Context menu entries added via Windows Registry

## Requirements

- Windows 10 or later
- .NET 8 Runtime (installed automatically with .NET 8 SDK, or download separately)
- Administrator privileges for installation

## Building from Source

### Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022 (optional, can also build with command line)

### Build Steps

1. Clone the repository
2. Open a command prompt in the repository root
3. Run the build script:
   ```batch
   Build.bat
   ```

Or build manually:
```batch
dotnet restore FolderSizeExtension.sln
dotnet build FolderSizeExtension.sln -c Release
```

## Installation

1. Build the project (see above)
2. Right-click on `Install.bat` and select **Run as administrator**
3. Follow the on-screen instructions
4. Windows Explorer will restart automatically

The application will be installed to: `C:\Program Files\FolderSizeCalculator\`

## Usage

### Calculating Folder Sizes

1. Open Windows Explorer and navigate to any folder
2. Right-click on a folder you want to analyze
3. Select one of the context menu options:
   - **Calculate Folder Size**: Calculate only the selected folder
   - **Calculate Folder Size (All Subfolders)**: Calculate all immediate subfolders
4. A progress window will appear showing:
   - Current folder being scanned
   - Progress indicator
   - Real-time results
5. Click **Cancel** to stop calculation, or **Close** when done
6. Results are automatically cached

### Viewing Results

Results are displayed in the progress window showing:
- Total size in human-readable format (KB, MB, GB, TB)
- Total byte count
- Number of files
- Number of folders
- Total item count

Cached results are stored persistently and can be retrieved by running the calculation again (it will complete instantly if already cached).

## Uninstallation

1. Right-click on `Uninstall.bat` and select **Run as administrator**
2. Follow the on-screen instructions
3. Windows Explorer will restart automatically

This will remove:
- The application executable and dependencies
- Registry context menu entries
- Installation directory
- Cache files from AppData

## Technical Details

### How It Works

1. **Registry Integration**: Context menu entries are added to `HKEY_CLASSES_ROOT\Directory\shell`
2. **Standalone Application**: .NET 8 WinExe that receives folder path as command-line argument
3. **Async Calculation**: Uses `Task`-based async/await for non-blocking folder traversal
4. **Cache Storage**: Stores calculated sizes in JSON format at `%APPDATA%\FolderSizeExtension\`
5. **Error Handling**: Gracefully handles access denied errors for protected folders

### Performance Considerations

- Calculations are **manual only** - no automatic calculations or background scanning
- Large folders may take several minutes depending on size and file count
- Calculations run asynchronously with progress reporting
- Supports cancellation at any time
- Cache is loaded on-demand and saved after each calculation

### Limitations

- Only calculates folders (not individual files)
- Requires manual trigger for each calculation
- Does not update automatically when folder contents change (must recalculate)
- No Explorer column integration (registry-based approach limitation)
- Requires administrator privileges for installation

## Command Line Usage

You can also run the calculator directly from command line:

```batch
# Calculate single folder
FolderSizeCalculator.exe "C:\Path\To\Folder"

# Calculate all subfolders
FolderSizeCalculator.exe "C:\Path\To\Folder" --subfolders
```

## Troubleshooting

### Context menu doesn't appear
- Ensure you ran Install.bat as Administrator
- Try restarting Windows Explorer: `taskkill /f /im explorer.exe && start explorer.exe`
- Check if registry entries exist: Run `regedit` and navigate to `HKEY_CLASSES_ROOT\Directory\shell`

### Calculations are slow
- This is expected for large folders with many files
- You can cancel calculations at any time using the Cancel button
- Consider calculating subfolders individually instead of all at once
- Network drives and external drives may be slower

### Application doesn't start
- Ensure .NET 8 Runtime is installed: `dotnet --list-runtimes`
- Check Windows Event Viewer for application errors
- Verify application exists at `C:\Program Files\FolderSizeCalculator\FolderSizeCalculator.exe`

### Build errors
- Ensure .NET 8.0 SDK is installed: `dotnet --version` (should show 8.0.x)
- Try restoring NuGet packages: `dotnet restore`
- Clean and rebuild: `dotnet clean && dotnet build -c Release`

## Development

### Project Structure

```
explorer-folder-size-extension/
├── FolderSizeExtension/              # Main project
│   ├── Properties/                   # Assembly info
│   ├── FolderSizeCache.cs            # JSON-based caching system
│   ├── FolderSizeCalculator.cs       # Async calculation engine
│   ├── FolderSizeCalculatorForm.cs   # WinForms progress UI
│   ├── FormatUtils.cs                # Formatting utilities
│   ├── Program.cs                    # Application entry point
│   ├── app.manifest                  # Windows application manifest
│   └── FolderSizeExtension.csproj    # .NET 8 project file
├── AddToContextMenu.reg              # Registry script (install)
├── RemoveFromContextMenu.reg         # Registry script (uninstall)
├── Build.bat                         # Build script
├── Install.bat                       # Installation script
├── Uninstall.bat                     # Uninstallation script
├── FolderSizeExtension.sln           # Visual Studio solution
└── README.md                         # This file
```

### Contributing

Contributions are welcome! Please ensure:
- Code follows existing style and C# conventions
- Changes are tested on Windows 10 and 11
- Registry integration remains functional after changes
- Code is compatible with .NET 8

## License

This project is open source. See LICENSE file for details.

## Credits

Built using:
- **.NET 8** - Latest LTS version of .NET
- **Windows Forms** - Native Windows UI framework
- **System.Text.Json** - Modern JSON serialization
- **Windows Registry** - For context menu integration

## Version History

### 2.0.0 (Current)
- Migrated to .NET 8 (LTS)
- Removed SharpShell dependency
- Simplified architecture with registry-based context menu
- Modern async/await patterns
- Improved error handling
- Self-contained executable approach

### 1.0.0
- Initial release (.NET Framework 4.8)
- COM-based shell extension
- Basic folder size calculation
- Persistent caching
