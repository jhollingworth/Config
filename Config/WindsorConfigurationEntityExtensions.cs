using System.Configuration;
using Castle.Components.Validator;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using log4net;

namespace Config
{
    public static class WindsorConfigurationEntityExtensions
    {
        private static readonly CachedValidationRegistry _registry = new CachedValidationRegistry();
        private static readonly ValidatorRunner _runner = new ValidatorRunner(_registry);

        private static readonly ILog Log = LogManager.GetLogger(typeof (WindsorConfigurationEntityExtensions));

        public static void RegisterConfigurationEntity<TEntity>(this IWindsorContainer container)
        {
            var name = typeof(TEntity).Name;

            if(null == container.Kernel.ConfigurationStore.GetComponentConfiguration(name))
            {
                var message = string.Format("Could not find the xml configuration for the configuration entity {0}", name);
                Log.Warn(message);
                throw new ConfigurationException(message);
            }
            
            container.Register(Component.For<TEntity>().Named(name));
            var entity = container.Resolve<TEntity>();

            if(false == _runner.IsValid(entity))
            {
                throw new ValidationException(
                    string.Format("The configuration entity {0} had invalid registration data", name), 
                    _runner.GetErrorSummary(entity).ErrorMessages);
            }
            
            Log.DebugFormat("Registered the configuration entity {0}", name);
        }
    }
}