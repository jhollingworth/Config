using System;
using System.Linq;
using System.Text.RegularExpressions;
using Castle.MicroKernel;
using Castle.Windsor;
using log4net;

namespace Config
{
    public class ConfigurationInstaller : IWindsorInstaller
    {
        private readonly FileLoader _fileLoader;
        private static readonly ILog Log = LogManager.GetLogger(typeof (ConfigurationInstaller));

        public ConfigurationInstaller()
            :this(new FileLoader())
        {   
        }

        internal ConfigurationInstaller(FileLoader fileLoader)
        {
            _fileLoader = fileLoader;
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            Log.DebugFormat("Starting configuration bootstrapping");

            var containsComponents =
                new Predicate<string>(path => Regex.IsMatch(_fileLoader.GetFile(path), "<components>", RegexOptions.Compiled));

            var files = from f in _fileLoader.GetAllFiles()
                        where string.Compare(f.Extension, ".config", true) == 0 && containsComponents(f.FullName)
                        select f.FullName;

            foreach (var file in files)
            {
                Log.DebugFormat("Found configuration file {0}", file);
                container.Install(Castle.Windsor.Installer.Configuration.FromXmlFile(file));
            }
        }
    }
}