using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PluginsEngine.FileAccess
{
    public class FileAccessConfig
    {
        [Flags]
        public enum LoggingLevel
        {
            None = 0, // Invalid value
            Init = 1 << 0,
            Base = 1 << 1,
            Verbose = 1 << 2 | Base,
            VerboseUltra = 1 << 3 | Verbose,
            PlatformSpecific = 1 << 4,
        }

        public LoggingLevel LogLevel = LoggingLevel.Verbose | LoggingLevel.PlatformSpecific | LoggingLevel.Init;

        private static readonly char[] sep = new char[] { ' ', ';', '-' };
        public string GameName = string.Join("_", Application.productName.Split(sep, StringSplitOptions.RemoveEmptyEntries));

        public string SavesLocation = "My Saves";

        public bool AtLogLevel(LoggingLevel level)
        {
            return (LogLevel & level) != LoggingLevel.None;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"Configuration: {typeof(FileAccessConfig)}");
            sb.AppendLine($"logLevel: {string.Join(" | ", GetLogLevelFlags().Select(x => x.ToString()))}");
            sb.AppendLine($"GameName: {GameName}");
            return sb.ToString();
        }

        protected IEnumerable<LoggingLevel> GetLogLevelFlags()
        {
            var allPossible = System.Enum.GetValues(typeof(LoggingLevel)).Cast<LoggingLevel>();
            foreach (var test in allPossible)
            {
                if ((test & LogLevel) != LoggingLevel.None)
                {
                    yield return test;
                }
            }
        }
    }
}
