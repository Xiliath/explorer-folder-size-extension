namespace FolderSizeExtension
{
    /// <summary>
    /// Utility methods for formatting file sizes
    /// </summary>
    public static class FormatUtils
    {
        /// <summary>
        /// Formats bytes to human-readable format
        /// </summary>
        public static string FormatSize(long bytes)
        {
            if (bytes == 0) return "0 B";

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
