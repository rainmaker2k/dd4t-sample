using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc3;
using Microsoft.Practices.Unity.Configuration;
using System.Configuration;

namespace Sample.Website
{
    public static class Bootstrapper
    {
        public static void Initialise()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();
            var section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            section.Configure(container, "main");

            //DependencyResolver.SetResolver(new UnityDependencyResolver(container));
            return container;
        }
    }
}