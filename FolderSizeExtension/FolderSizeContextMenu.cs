using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SharpShell.Attributes;
using SharpShell.SharpContextMenu;

namespace FolderSizeExtension
{
    /// <summary>
    /// Context menu extension for calculating folder sizes
    /// Right-click on a folder to trigger size calculation
    /// </summary>
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.Directory)]
    public class FolderSizeContextMenu : SharpContextMenu
    {
        /// <summary>
        /// Determines whether the menu item should be shown
        /// </summary>
        protected override bool CanShowMenu()
        {
            // Show menu for folders only
            return SelectedItemPaths.Any() && SelectedItemPaths.All(path => Directory.Exists(path));
        }

        /// <summary>
        /// Creates the context menu
        /// </summary>
        protected override ContextMenuStrip CreateMenu()
        {
            var menu = new ContextMenuStrip();

            // Main menu item
            var calculateItem = new ToolStripMenuItem
            {
                Text = "Calculate Folder Size",
                Image = Properties.Resources.FolderSizeIcon // We'll create this resource
            };

            // Calculate this folder
            var calculateThisItem = new ToolStripMenuItem
            {
                Text = "Calculate Size (This Folder)"
            };
            calculateThisItem.Click += (sender, args) => CalculateSizes(false);

            // Calculate all subfolders
            var calculateAllItem = new ToolStripMenuItem
            {
                Text = "Calculate Size (All Subfolders)"
            };
            calculateAllItem.Click += (sender, args) => CalculateSizes(true);

            // Clear cached sizes
            var clearCacheItem = new ToolStripMenuItem
            {
                Text = "Clear Cached Sizes"
            };
            clearCacheItem.Click += (sender, args) => ClearCache();

            calculateItem.DropDownItems.Add(calculateThisItem);
            calculateItem.DropDownItems.Add(calculateAllItem);
            calculateItem.DropDownItems.Add(new ToolStripSeparator());
            calculateItem.DropDownItems.Add(clearCacheItem);

            menu.Items.Add(calculateItem);

            return menu;
        }

        /// <summary>
        /// Calculate folder sizes
        /// </summary>
        private void CalculateSizes(bool includeSubfolders)
        {
            foreach (var folderPath in SelectedItemPaths)
            {
                if (!Directory.Exists(folderPath))
                    continue;

                // Launch the calculation UI
                var calculatorForm = new FolderSizeCalculatorForm(folderPath, includeSubfolders);
                calculatorForm.Show();
            }
        }

        /// <summary>
        /// Clear cached folder sizes
        /// </summary>
        private void ClearCache()
        {
            foreach (var folderPath in SelectedItemPaths)
            {
                FolderSizeCache.Instance.Clear(folderPath);
            }

            MessageBox.Show(
                "Cached folder sizes have been cleared. Windows Explorer will refresh.",
                "Folder Size Extension",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            // Refresh Explorer view
            RefreshExplorerView();
        }

        /// <summary>
        /// Refresh Windows Explorer to show updated values
        /// </summary>
        private void RefreshExplorerView()
        {
            try
            {
                // Send F5 to refresh
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = "/select," + SelectedItemPaths.FirstOrDefault(),
                    UseShellExecute = true
                });
            }
            catch
            {
                // Refresh failed, no big deal
            }
        }
    }
}
