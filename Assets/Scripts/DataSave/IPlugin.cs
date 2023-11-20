using System;
using System.Collections.Generic;

namespace PluginsEngine.FileAccess
{
    public interface IPlugin : IDisposable
    {
        void SaveBlocking(string path, byte[] data);
        byte[] LoadBlocking(string path);
        List<string> LoadLinesBlocking(string path);

        bool FileExists(string path);
        void ResolveFile(string path);

        bool DirectoryExists(string path);
        void ResolveDirectory(string path);

        void FileDelete(string path);
        void DirectoryDelete(string path);

        IFileData GetFileData(string path);
        IEnumerable<IFileData> GetDirectoryFiles(string fullPath);
    }
}
