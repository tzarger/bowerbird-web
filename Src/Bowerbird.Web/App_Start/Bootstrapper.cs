using System.Reflection;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Web.Mvc;
using NinjectBootstrapper = Ninject.Web.Mvc.Bootstrapper;
using Bowerbird.Web.Config;
using Microsoft.Practices.ServiceLocation;
using CommonServiceLocator.NinjectAdapter;
using SignalR.Infrastructure;
using SignalR.Ninject;
using Bowerbird.Web.App_Start;
using Bowerbird.Core;
using log4net.Config;
using System.Web.Mvc;
using System.Web.Routing;

[assembly: WebActivator.PreApplicationStartMethod(typeof(Bowerbird.Web.App_Start.Bootstrapper), "PreStart")]
[assembly: WebActivator.PostApplicationStartMethod(typeof(Bowerbird.Web.App_Start.Bootstrapper), "PostStart")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(Bowerbird.Web.App_Start.Bootstrapper), "Stop")]

namespace Bowerbird.Web.App_Start
{
    public static class Bootstrapper
    {
        private static readonly NinjectBootstrapper _ninjectBootstrapper = new NinjectBootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void PreStart()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestModule));
            DynamicModuleUtility.RegisterModule(typeof(HttpApplicationInitializationModule));
            _ninjectBootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Sets up the application ready for use
        /// </summary>
        public static void PostStart()
        {
            EventProcessor.ServiceLocator = ServiceLocator.Current;

            XmlConfigurator.Configure();

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            //FluentValidationModelValidatorProvider.Configure(x => x.ValidatorFactory = new NinjectValidatorFactory(Kernel));

            //ModelValidatorProviders.Providers.Add(new ClientDataTypeModelValidatorProvider());

            //ModelValidatorProviders.Providers.Add(new FluentValidationModelValidatorProvider(new NinjectValidatorFactory(Kernel))); 

            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;

            AreaRegistration.RegisterAllAreas();

            RouteRegistrar.RegisterRoutesTo(RouteTable.Routes);

            //IndexCreation.CreateIndexes(typeof(ImageTags_GroupByTagName).Assembly, documentStore);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            _ninjectBootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            RegisterServices(kernel);
            return kernel;
        }

        /// <summary>
        /// Load modules and services
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Load(new BowerbirdNinjectModule());

            ServiceLocator.SetLocatorProvider(() => new NinjectServiceLocator(kernel));
            
            SignalR.Infrastructure.DependencyResolver.SetResolver(new SignalR.Ninject.NinjectDependencyResolver(kernel));
        }
    }
}