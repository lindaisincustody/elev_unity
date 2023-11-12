using System;
using System.Collections.Generic;

namespace PluginsEngine.FileAccess
{
    public interface IAsyncPlugin : IPlugin
    {
        void Load(string path, Action<byte[]> callback);
        void Save(string path, byte[] data, Action<bool> callback);

        void FileExists(string path, Action<bool> callback);

        void DirectoryExists(string path, Action<bool> callback);

        void GetFileData(string path, Action<IFileData> callback);
        void GetDirectoryFiles(string fullPath, Action<IEnumerable<IFileData>> callback);
    }
}
