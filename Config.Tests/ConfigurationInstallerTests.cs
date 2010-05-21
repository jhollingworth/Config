using System.IO;
using Castle.MicroKernel;
using Castle.Windsor;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Config.Tests
{
    public class ConfigurationInstallerContext
    {
        protected static Mock<FileLoader> FileLoader;
        protected static Mock<IWindsorContainer> Container;
        protected static ConfigurationInstaller Installer;
        
        protected const string ConfigFileName = "foo.config", ConfigFilePath = @"c:\" + ConfigFileName;

        Establish context = () =>
        {
            FileLoader = new Mock<FileLoader>();
            Container = new Mock<IWindsorContainer>();
            Installer = new ConfigurationInstaller(FileLoader.Object);
        };

        protected static void configuration_was_bootstrapped()
        {
            Installer.Install(Container.Object, new Mock<IConfigurationStore>().Object);
        }

        protected static void there_is_a_config_file_which_contains(string data)
        {
            FileLoader.Setup(f => f.GetAllFiles()).Returns(new[] { new FileInfo(ConfigFilePath) });
            FileLoader.Setup(f => f.GetFile(Moq.It.Is<string>(s => s.EndsWith(ConfigFileName)))).Returns(data);
        }
    }

    public class When_there_are_no_config_files_in_the_directory : ConfigurationInstallerContext
    {
        Establish context = () => FileLoader.Setup(f => f.GetAllFiles()).Returns(new[] { new FileInfo("foo.txt"), new FileInfo("bar.xml") });
        Because the = configuration_was_bootstrapped;
        It should_not_install_any_config_files = () => Container.Verify(c => c.Install(Moq.It.IsAny<IWindsorInstaller[]>()), Times.Never());
    }

    public class When_there_is_a_config_file_which_does_not_contain_a_components_section : ConfigurationInstallerContext
    {
        Establish context = () => there_is_a_config_file_which_contains("<config><foo></foo></config>");
        Because the = configuration_was_bootstrapped;
        It should_not_install_any_config_files = () => Container.Verify(c => c.Install(Moq.It.IsAny<IWindsorInstaller[]>()), Times.Never());
    }
    
    public class When_there_is_a_config_file_which_contains_a_components_section : ConfigurationInstallerContext
    {
        Establish context = () => there_is_a_config_file_which_contains("<config><components><component><parameters><Foo>true</Foo></parameters></component></components><config>");
        Because the = configuration_was_bootstrapped;
        It should_install_the_config_file = () => Container.Verify(c => c.Install(Moq.It.IsAny<Castle.Windsor.Installer.ConfigurationInstaller>()));
    }
}