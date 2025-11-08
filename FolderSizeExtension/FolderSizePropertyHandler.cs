using System;
using System.IO;
using System.Runtime.InteropServices;
using SharpShell.Attributes;
using SharpShell.SharpPropertySheet;
using Microsoft.Win32;

namespace FolderSizeExtension
{
    /// <summary>
    /// Property handler that adds a "Folder Size" column to Windows Explorer
    /// This integrates directly with the Windows Property System
    /// </summary>
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.Directory)]
    public class FolderSizePropertyHandler : SharpPropertySheet
    {
        // Custom property key for folder size
        // Format: {GUID}.PropertyID
        private static readonly Guid PROPERTY_GUID = new Guid("A9B3C5D7-E1F2-4A5B-8C9D-0E1F2A3B4C5D");
        private const int PROPERTY_ID_SIZE = 2;
        private const int PROPERTY_ID_ITEM_COUNT = 3;

        private string? folderPath;

        protected override void OnPropertyPageInitialized(IntPtr hwndDlg)
        {
            base.OnPropertyPageInitialized(hwndDlg);
        }

        /// <summary>
        /// Gets the folder size from cache or calculates it
        /// </summary>
        public static long GetFolderSize(string path)
        {
            try
            {
                // First, try to get cached value
                var cache = FolderSizeCache.Instance;
                if (cache.TryGetSize(path, out long cachedSize))
                {
                    return cachedSize;
                }

                // If not in cache and auto-calculate is disabled, return 0
                // This prevents performance issues when browsing folders
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the number of items in a folder
        /// </summary>
        public static int GetItemCount(string path)
        {
            try
            {
                var cache = FolderSizeCache.Instance;
                if (cache.TryGetItemCount(path, out int count))
                {
                    return count;
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Formats bytes to human-readable format
        /// </summary>
        public static string FormatSize(long bytes)
        {
            if (bytes == 0) return "";

            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double size = bytes;

            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }

            return $"{size:0.##} {sizes[order]}";
        }
    }
}
