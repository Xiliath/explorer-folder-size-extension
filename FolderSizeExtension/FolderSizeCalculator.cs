using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FolderSizeExtension
{
    /// <summary>
    /// Calculates folder sizes recursively with cancellation support
    /// </summary>
    public class FolderSizeCalculator
    {
        public class CalculationResult
        {
            public long TotalSize { get; set; }
            public int FileCount { get; set; }
            public int FolderCount { get; set; }
            public int TotalItemCount => FileCount + FolderCount;
            public bool WasCompleted { get; set; }
            public Exception? Error { get; set; }
        }

        /// <summary>
        /// Calculate the size of a folder and all its contents
        /// </summary>
        public static async Task<CalculationResult> CalculateAsync(string folderPath, CancellationToken cancellationToken = default, IProgress<string>? progress = null)
        {
            var result = new CalculationResult();

            try
            {
                await Task.Run(() => CalculateRecursive(folderPath, result, cancellationToken, progress), cancellationToken);
                result.WasCompleted = !cancellationToken.IsCancellationRequested;
            }
            catch (OperationCanceledException)
            {
                result.WasCompleted = false;
            }
            catch (Exception ex)
            {
                result.Error = ex;
                result.WasCompleted = false;
            }

            return result;
        }

        /// <summary>
        /// Synchronous calculation for a single folder
        /// </summary>
        public static CalculationResult Calculate(string folderPath)
        {
            var result = new CalculationResult();

            try
            {
                CalculateRecursive(folderPath, result, CancellationToken.None, null);
                result.WasCompleted = true;
            }
            catch (Exception ex)
            {
                result.Error = ex;
                result.WasCompleted = false;
            }

            return result;
        }

        private static void CalculateRecursive(string folderPath, CalculationResult result, CancellationToken cancellationToken, IProgress<string>? progress)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                progress?.Report(folderPath);

                // Get all files in current directory
                var files = Directory.EnumerateFiles(folderPath);
                foreach (var file in files)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        var fileInfo = new FileInfo(file);
                        result.TotalSize += fileInfo.Length;
                        result.FileCount++;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Skip files we don't have access to
                    }
                    catch (IOException)
                    {
                        // Skip files that are in use or inaccessible
                    }
                }

                // Recursively process subdirectories
                var directories = Directory.EnumerateDirectories(folderPath);
                foreach (var directory in directories)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        result.FolderCount++;
                        CalculateRecursive(directory, result, cancellationToken, progress);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Skip folders we don't have access to
                    }
                    catch (IOException)
                    {
                        // Skip folders that are inaccessible
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Can't access this folder
            }
            catch (IOException)
            {
                // Folder access error
            }
        }

        /// <summary>
        /// Calculate sizes for all immediate subfolders
        /// </summary>
        public static async Task CalculateSubfoldersAsync(string parentFolder, IProgress<(string folder, CalculationResult result)>? progress = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var subfolders = Directory.GetDirectories(parentFolder);

                foreach (var subfolder in subfolders)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    var result = await CalculateAsync(subfolder, cancellationToken);

                    // Cache the result
                    if (result.WasCompleted)
                    {
                        FolderSizeCache.Instance.SetFolderInfo(subfolder, result.TotalSize, result.TotalItemCount);
                    }

                    progress?.Report((subfolder, result));
                }
            }
            catch (Exception)
            {
                // Error accessing parent folder
            }
        }
    }
}
