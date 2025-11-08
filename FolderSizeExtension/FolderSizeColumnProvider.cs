using System;
using System.IO;
using System.Runtime.InteropServices;
using SharpShell.Attributes;
using SharpShell.SharpInfoTipHandler;

namespace FolderSizeExtension
{
    /// <summary>
    /// Column provider for displaying folder sizes in Windows Explorer
    /// Note: In modern Windows (Vista+), column providers are replaced by
    /// Property Handlers which use the Windows Property System
    /// This implementation provides info tips that work with the property system
    /// </summary>
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.Directory)]
    public class FolderSizeColumnProvider : SharpInfoTipHandler
    {
        /// <summary>
        /// Gets the info tip text for a folder
        /// This appears when hovering over folders
        /// </summary>
        protected override string GetInfo(RequestedInfoType infoType, bool singleLine)
        {
            try
            {
                // Get the folder path from the selected item
                string folderPath = SelectedItemPath;

                if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
                    return string.Empty;

                // Try to get cached folder size
                if (FolderSizeCache.Instance.TryGetSize(folderPath, out long size) &&
                    FolderSizeCache.Instance.TryGetItemCount(folderPath, out int itemCount))
                {
                    string sizeStr = FolderSizePropertyHandler.FormatSize(size);

                    if (singleLine)
                    {
                        return $"Size: {sizeStr} ({itemCount} items)";
                    }
                    else
                    {
                        return $"Folder Size: {sizeStr}\nTotal Items: {itemCount:N0}\n\nRight-click and select 'Calculate Folder Size' to update.";
                    }
                }
                else
                {
                    if (singleLine)
                    {
                        return "Size: Not calculated";
                    }
                    else
                    {
                        return "Folder size not yet calculated.\n\nRight-click on the folder and select\n'Calculate Folder Size' to compute the size.";
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
