using System;
using System.IO;

namespace PluginsEngine.FileAccess.Standalone
{
    public class FileData : IFileData
    {
        private FileInfo _fileInfo;
        private DateTime? _externalTime;

        public FileData() { }

        public FileData(FileInfo info)
        {
            _fileInfo = info;
        }

        public FileData(FileInfo info, DateTime trackerTime) : this(info)
        {
            _externalTime = trackerTime;
        }

        public string Name => _fileInfo?.Name;

        public string FullName => _fileInfo?.FullName;

        public bool Exists => _fileInfo?.Exists == true;

        public DateTime LastWriteTime => _externalTime ?? _fileInfo.LastWriteTime;
    }
}
