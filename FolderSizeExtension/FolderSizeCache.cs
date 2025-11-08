using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text.Json;

namespace FolderSizeExtension
{
    /// <summary>
    /// Thread-safe cache for folder size information
    /// Persists to disk to maintain state across sessions
    /// </summary>
    public class FolderSizeCache
    {
        private static readonly Lazy<FolderSizeCache> _instance = new(() => new FolderSizeCache());
        public static FolderSizeCache Instance => _instance.Value;

        private readonly ConcurrentDictionary<string, FolderInfo> _cache;
        private readonly string _cacheFilePath;
        private readonly object _saveLock = new();

        public class FolderInfo
        {
            public long Size { get; set; }
            public int ItemCount { get; set; }
            public DateTime LastCalculated { get; set; }
        }

        private FolderSizeCache()
        {
            _cache = new ConcurrentDictionary<string, FolderInfo>(StringComparer.OrdinalIgnoreCase);

            // Store cache in AppData
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string cacheDir = Path.Combine(appData, "FolderSizeExtension");
            Directory.CreateDirectory(cacheDir);
            _cacheFilePath = Path.Combine(cacheDir, "folder_sizes.json");

            LoadCache();
        }

        /// <summary>
        /// Try to get cached folder size
        /// </summary>
        public bool TryGetSize(string path, out long size)
        {
            if (_cache.TryGetValue(path, out var info))
            {
                size = info.Size;
                return true;
            }

            size = 0;
            return false;
        }

        /// <summary>
        /// Try to get cached item count
        /// </summary>
        public bool TryGetItemCount(string path, out int count)
        {
            if (_cache.TryGetValue(path, out var info))
            {
                count = info.ItemCount;
                return true;
            }

            count = 0;
            return false;
        }

        /// <summary>
        /// Update or add folder size to cache
        /// </summary>
        public void SetFolderInfo(string path, long size, int itemCount)
        {
            var info = new FolderInfo
            {
                Size = size,
                ItemCount = itemCount,
                LastCalculated = DateTime.UtcNow
            };

            _cache.AddOrUpdate(path, info, (key, oldValue) => info);
            SaveCache();
        }

        /// <summary>
        /// Clear cache entry for a specific folder
        /// </summary>
        public void Clear(string path)
        {
            _cache.TryRemove(path, out _);
            SaveCache();
        }

        /// <summary>
        /// Clear all cache entries
        /// </summary>
        public void ClearAll()
        {
            _cache.Clear();
            SaveCache();
        }

        /// <summary>
        /// Load cache from disk
        /// </summary>
        private void LoadCache()
        {
            try
            {
                if (File.Exists(_cacheFilePath))
                {
                    string json = File.ReadAllText(_cacheFilePath);
                    var loadedCache = JsonSerializer.Deserialize<ConcurrentDictionary<string, FolderInfo>>(json);

                    if (loadedCache != null)
                    {
                        foreach (var kvp in loadedCache)
                        {
                            _cache.TryAdd(kvp.Key, kvp.Value);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // If cache file is corrupted, start fresh
                _cache.Clear();
            }
        }

        /// <summary>
        /// Save cache to disk
        /// </summary>
        private void SaveCache()
        {
            try
            {
                lock (_saveLock)
                {
                    string json = JsonSerializer.Serialize(_cache, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });
                    File.WriteAllText(_cacheFilePath, json);
                }
            }
            catch (Exception)
            {
                // Fail silently - cache is optional
            }
        }
    }
}
