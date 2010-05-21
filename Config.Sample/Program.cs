using System;
using Castle.Windsor;

namespace Config.Sample
{
    class Program
    {
        static void Main()
        {
            var container = new WindsorContainer();

            container.Install(new ConfigurationInstaller());
            container.RegisterConfigurationEntity<Example>();

            var example = container.Resolve<Example>();

            if(example.IsAwesome)
                Console.WriteLine("This is {0} awesome. Its level {1} awesome", example.HowAwesome, example.LevelOfAwesome);

            Console.ReadKey();
        }
    }
}
