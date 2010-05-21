using System;
using System.ComponentModel;
using Castle.Components.Validator;
using Castle.Core.Configuration;
using Castle.Windsor;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Config.Tests
{
    public class ValidationContext
    {
        protected static TestConfig Config;
        protected static Exception Exception;
        protected static Mock<IWindsorContainer> Container;

        Establish context = () =>
        {
            Exception = null;
            Config = new TestConfig();
            Container = new Mock<IWindsorContainer>();
            Container.Setup(c => c.Resolve<TestConfig>()).Returns(Config);
            Container.Setup(c => c.Kernel.ConfigurationStore.GetComponentConfiguration(typeof (TestConfig).Name)).
                Returns(new Mock<IConfiguration>().Object);
        };

        protected static void config_was_registerd()
        {
            Exception = Catch.Exception(() => Container.Object.RegisterConfigurationEntity<TestConfig>());
        }
    }

    public class TestConfig
    {
        [ValidateEmail]
        public string Email { get; set; }
        [ValidateLength(0, 2)]
        public string NotLongerThan2 { get; set; }
    }

    public class When_a_configuration_entity_has_validation_errors : ValidationContext
    {
        Establish context = () =>
        {
            Config.Email = "foo";
            Config.NotLongerThan2 = "abc";
        };

        Because the = config_was_registerd;

        It should_throw_a_validation_exception_exception = () => Exception.ShouldBeOfType<ValidationException>();
    }
}