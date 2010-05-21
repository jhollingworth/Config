using System;
using System.Configuration;
using Castle.Core.Configuration;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Machine.Specifications;
using Moq;
using Moq.Language.Flow;
using It = Machine.Specifications.It;

namespace Config.Tests
{
    public class WindsorConfigurationEntityExtensionsContext
    {
        protected static Exception Exception;
        protected static Mock<IWindsorContainer> Container;
        protected static ISetup<IWindsorContainer, IConfiguration> ConfigConfiguration;

        protected class Config { }

        Establish context = () =>
        {
            Exception = null;
            Container = new Mock<IWindsorContainer>();
            
            ConfigConfiguration = Container.Setup(c => c.Kernel.ConfigurationStore.GetComponentConfiguration(typeof(Config).Name));
            ConfigConfiguration.Returns((IConfiguration)null);
        };

        protected static void config_entity_was_registered()
        {
            Exception = Catch.Exception(() => Container.Object.RegisterConfigurationEntity<Config>());
        }
    }

    public class When_the_xml_config_for_the_entity_has_not_been_loaded : WindsorConfigurationEntityExtensionsContext
    {
        Because the = config_entity_was_registered;
        It should_throw_a_configuration_exception = () => Exception.ShouldBeOfType<ConfigurationException>();                                               
    }
    
    public class When_the_xml_config_for_the_entity_has_been_loaded : WindsorConfigurationEntityExtensionsContext
    {
        Establish the_configuration_has_been_loaded = () => ConfigConfiguration.Returns(new Mock<IConfiguration>().Object);
        Because the = config_entity_was_registered;
        It should_register_the_entity = () => Container.Verify(c => c.Register(Moq.It.IsAny<ComponentRegistration<Config>>()));
        It should_register_the_entity_with_its_class_name = () => Container.Verify(c => c.Register(Moq.It.Is<ComponentRegistration<Config>>(r => r.Name.Equals(typeof(Config).Name))));
    }
}