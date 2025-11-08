# Folder Size Extension for Windows Explorer

A Windows Shell Extension that adds folder size calculation capabilities to Windows Explorer. This extension integrates directly into Windows Explorer, allowing you to calculate and view folder sizes through context menus and tooltips.

## Features

- **Context Menu Integration**: Right-click on any folder to calculate its size
- **Individual or Batch Calculation**: Calculate a single folder or all subfolders at once
- **Persistent Cache**: Calculated sizes are cached and persist across sessions
- **Progress Display**: Visual progress window shows calculation status
- **Tooltip Integration**: Hover over folders to see cached size information
- **Manual Trigger**: Sizes are only calculated when you explicitly request them (no performance impact on browsing)

## Architecture

The extension consists of several components:

1. **FolderSizeContextMenu**: Context menu handler for triggering calculations
2. **FolderSizeCalculator**: Background calculation engine with cancellation support
3. **FolderSizeCache**: Thread-safe persistent cache for storing results
4. **FolderSizePropertyHandler**: Property handler for displaying sizes
5. **FolderSizeColumnProvider**: Info tip provider for hover tooltips
6. **FolderSizeCalculatorForm**: WinForms UI for displaying progress

## Requirements

- Windows 10 or later
- .NET Framework 4.8 (pre-installed on Windows 10/11)
- Administrator privileges for installation

## Building from Source

### Prerequisites

- Visual Studio 2022 or later (with .NET desktop development workload)
- .NET Framework 4.8 Developer Pack

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

The extension will be installed to: `C:\Program Files\FolderSizeExtension\`

## Usage

### Calculating Folder Sizes

1. Open Windows Explorer and navigate to any folder
2. Right-click on a folder you want to analyze
3. Select **Calculate Folder Size** from the context menu
4. Choose one of the options:
   - **Calculate Size (This Folder)**: Calculate only the selected folder
   - **Calculate Size (All Subfolders)**: Calculate all immediate subfolders
5. A progress window will appear showing the calculation status
6. Once complete, the size is cached

### Viewing Cached Sizes

After calculation, you can view folder sizes in two ways:

1. **Tooltips**: Hover your mouse over a folder to see a tooltip with:
   - Folder size (human-readable format)
   - Total item count
   - Last calculation date

2. **Details View** (Note: Requires additional registry configuration):
   - Switch to Details view in Explorer
   - Right-click column headers
   - Add custom "Folder Size" column

### Clearing Cache

To clear cached sizes:
1. Right-click on a folder
2. Select **Calculate Folder Size** > **Clear Cached Sizes**
3. Press F5 to refresh Explorer

## Uninstallation

1. Right-click on `Uninstall.bat` and select **Run as administrator**
2. Follow the on-screen instructions
3. Windows Explorer will restart automatically

This will remove:
- The extension DLL and COM registration
- Installation directory
- Cache files from AppData

## Technical Details

### How It Works

1. **COM Shell Extension**: The extension is a COM server that integrates with Windows Explorer
2. **Context Menu Handler**: Implements `IShellExtInit` and `IContextMenu` interfaces
3. **Property System**: Uses Windows Property System for displaying data
4. **Cache Storage**: Stores calculated sizes in JSON format at `%APPDATA%\FolderSizeExtension\`

### Performance Considerations

- Calculations are **manual only** - no automatic calculations
- Large folders may take several minutes to calculate
- Calculations run in background threads with cancellation support
- Cache is loaded once at startup and saved after each calculation

### Limitations

- Only calculates folders (not individual files)
- Requires manual trigger for each calculation
- Column integration requires Windows Property System registration
- Does not update automatically when folder contents change

## Quick Access Toolbar Integration

While true Quick Access Toolbar integration is complex in modern Windows (requires DeskBand implementation which is deprecated), you can achieve similar functionality by:

1. **Pinning to Quick Access**: Create a shortcut that runs the calculation for frequently accessed folders
2. **Keyboard Shortcuts**: Use context menu keyboard navigation (right-click + arrow keys)
3. **Custom Toolbar**: Use third-party Explorer enhancement tools to add custom buttons

## Troubleshooting

### Extension doesn't appear in context menu
- Ensure you ran Install.bat as Administrator
- Try restarting Windows Explorer: `taskkill /f /im explorer.exe && start explorer.exe`
- Check COM registration: `regsvr32 /n /i:user "C:\Program Files\FolderSizeExtension\FolderSizeExtension.dll"`

### Calculations are slow
- This is expected for large folders with many files
- You can cancel calculations at any time
- Consider calculating subfolders individually instead of all at once

### Tooltips don't show
- Ensure you have calculated the folder size first
- Hover for 1-2 seconds for tooltip to appear
- Cache may be cleared - recalculate the folder

### Build errors
- Ensure .NET 6.0 SDK is installed
- Try restoring NuGet packages: `dotnet restore`
- Check that SharpShell package is properly installed

## Development

### Project Structure

```
FolderSizeExtension/
├── FolderSizeExtension/          # Main project
│   ├── Common/                   # Shared utilities
│   ├── Properties/               # Assembly info and resources
│   ├── ShellExtension/           # Shell extension handlers
│   ├── FolderSizeCache.cs        # Caching system
│   ├── FolderSizeCalculator.cs   # Calculation engine
│   ├── FolderSizeCalculatorForm.cs  # Progress UI
│   ├── FolderSizeColumnProvider.cs  # Column integration
│   ├── FolderSizeContextMenu.cs     # Context menu
│   └── FolderSizePropertyHandler.cs # Property handler
├── Build.bat                     # Build script
├── Install.bat                   # Installation script
├── Uninstall.bat                 # Uninstallation script
└── README.md                     # This file
```

### Contributing

Contributions are welcome! Please ensure:
- Code follows existing style
- Changes are tested on Windows 10 and 11
- COM registration still works after changes

## License

This project is open source. See LICENSE file for details.

## Credits

Built using:
- [SharpShell](https://github.com/dwmkerr/sharpshell) - .NET Shell Extensions framework
- .NET Framework 4.8 with Windows Forms
- Windows Shell API

## Version History

### 1.0.0
- Initial release
- Context menu integration
- Folder size calculation
- Persistent caching
- Tooltip display
- Progress tracking
