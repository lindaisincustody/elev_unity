using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using LogLevel = PluginsEngine.FileAccess.FileAccessConfig.LoggingLevel;

namespace PluginsEngine.FileAccess
{
    /// <summary>
    /// This is a wrapper around <see cref="IPlugin"/> implementations to make them conform to <see cref="IAsyncPlugin"/> by implementing all async methods synchronously. This should only be used when proper <see cref="IAsyncPlugin"/> implementation for a platform is not available.
    /// </summary>
    public class AsyncPluginWrapper : IAsyncPlugin
    {
        private IPlugin _basePlugin;
        private FileAccessConfig _config;

        public AsyncPluginWrapper(IPlugin basePlugin, FileAccessConfig config)
        {
            _basePlugin = basePlugin;
            _config = config;
        }

        public void DirectoryDelete(string path)
        {
            _basePlugin.DirectoryDelete(path);
        }

        public void DirectoryExists(string path, Action<bool> callback)
        {
            callback(DirectoryExists(path));
        }

        public bool DirectoryExists(string path)
        {
            return _basePlugin.DirectoryExists(path);
        }

        public void Dispose()
        {
            _basePlugin.Dispose();
        }

        public void FileDelete(string path)
        {
            _basePlugin.FileDelete(path);
        }

        public void FileExists(string path, Action<bool> callback)
        {
            callback(FileExists(path));
        }

        public bool FileExists(string path)
        {
            return _basePlugin.FileExists(path);
        }

        public void GetDirectoryFiles(string fullPath, Action<IEnumerable<IFileData>> callback)
        {
            callback(GetDirectoryFiles(fullPath));
        }

        public IEnumerable<IFileData> GetDirectoryFiles(string fullPath)
        {
            return _basePlugin.GetDirectoryFiles(fullPath);
        }

        public void GetFileData(string path, Action<IFileData> callback)
        {
            callback(GetFileData(path));
        }

        public IFileData GetFileData(string path)
        {
            return _basePlugin.GetFileData(path);
        }

        public void Load(string path, Action<byte[]> callback)
        {
            callback(LoadBlocking(path));
        }

        public byte[] LoadBlocking(string path)
        {
            return _basePlugin.LoadBlocking(path);
        }

        public List<string> LoadLinesBlocking(string path)
        {
            return _basePlugin.LoadLinesBlocking(path);
        }

        public void ResolveDirectory(string path)
        {
            _basePlugin.ResolveDirectory(path);
        }

        public void ResolveFile(string path)
        {
            _basePlugin.ResolveFile(path);
        }

        public void Save(string path, byte[] data, Action<bool> callback)
        {
            Debug.Log("Saving...");
            SaveBlocking(path, data);
            callback(true);
        }

        public void SaveBlocking(string path, byte[] data)
        {
            _basePlugin.SaveBlocking(path, data);
        }
    }
}
