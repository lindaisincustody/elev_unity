using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PluginsEngine.FileAccess.Standalone
{
    public class FileAccessPlugin : IPlugin
    {
        private FileAccessConfig _config;

        private string rootPath;

        private string SanitizePath(string path)
        {
            if (path.StartsWith(rootPath))
            {
                return path;
            }

            return Path.Combine(rootPath, path);
        }

        public void SaveBlocking(string path, byte[] data)
        {
            path = SanitizePath(path);

            var dir = Path.GetDirectoryName(path);
            ResolveDirectory(dir);
            ResolveFile(path);
            File.WriteAllBytes(path, new byte[0]);

            using (var stream = new FileStream(path,
                FileMode.OpenOrCreate,
                System.IO.FileAccess.ReadWrite,
                FileShare.None))
            {

                stream.Write(data, 0, data.Length);
            }
        }

        public byte[] LoadBlocking(string path)
        {
            path = SanitizePath(path);

            UnityEngine.Debug.Assert(File.Exists(path),
                $"Save file at path \"{path}\" doesnt exist");
            var data = File.ReadAllBytes(path);

            return data;
        }

        public List<string> LoadLinesBlocking(string path)
        {
            path = SanitizePath(path);

            return new List<string>(File.ReadAllLines(path));
        }

        public bool FileExists(string path)
        {
            path = SanitizePath(path);

            return File.Exists(path);
        }

        public void ResolveFile(string path)
        {
            path = SanitizePath(path);
            if (!File.Exists(path)) File.WriteAllText(path, "");
        }

        public bool DirectoryExists(string path)
        {
            path = SanitizePath(path);

            return Directory.Exists(path);
        }

        public void ResolveDirectory(string path)
        {
            path = SanitizePath(path);

            var dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
                dirInfo.Create();
        }

        public void FileDelete(string path)
        {
            path = SanitizePath(path);

            File.Delete(path);
        }

        public void DirectoryDelete(string path)
        {
            path = SanitizePath(path);

            Directory.Delete(path);
        }

        public IFileData GetFileData(string path)
        {
            path = SanitizePath(path);

            return new FileData(new FileInfo(path));
        }

        public IEnumerable<IFileData> GetDirectoryFiles(string fullpath)
        {
            fullpath = SanitizePath(fullpath);

            var dir = new DirectoryInfo(fullpath);
            if (!dir.Exists) return new List<FileData>();
            return dir.GetFiles().Select(x => new FileData(x));
        }


        #region Init / Dispose

        public FileAccessPlugin(FileAccessConfig config)
        {
            _config = config;
            CreateRootPath();
        }

        private void CreateRootPath()
        {
            rootPath = Path.Combine(_config.SavesLocation, _config.GameName);      }

        #endregion // Init / Dispose


        #region Logging
        public string Caller([CallerMemberName] string caller = "")
        {
            return caller;
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public string Class
        {
            get => nameof(FileAccessPlugin);
        }
        public string Platform
        {
            get => "Standalone";
        }
        #endregion // Logging
    }
}
