using System;

namespace PluginsEngine.FileAccess
{
    public interface IFileData
    {
        string Name { get; }
        string FullName { get; }
        bool Exists { get; }
        DateTime LastWriteTime { get; }
    }
}
