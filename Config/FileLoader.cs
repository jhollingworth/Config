using System;
using System.IO;
using System.Linq;
using log4net;

namespace Config
{
    public class FileLoader 
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (FileLoader));
        private const string ConfigDirectory = "Configuration";

        public virtual FileInfo[] GetAllFiles()
        {
            var domain = AppDomain.CurrentDomain;
            var path = domain.BaseDirectory ?? domain.RelativeSearchPath;

            var configDir = Path.Combine(path, ConfigDirectory);

            if(false == Directory.Exists(configDir))
            {
                Log.DebugFormat("Could not find the configuration directory {0}", configDir);
                return new FileInfo[0];
            }

            var files = Directory.GetFiles(configDir)
                .Select(p => new FileInfo(p))
                .ToArray();

            Log.DebugFormat("Loaded {0} files from the configuration directory {1}", files.Length, configDir);

            return files;
        }

        public virtual string GetFile(string filePath)
        {
            return File.ReadAllText(filePath);
        }
    }
}