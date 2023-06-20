using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using DryIoc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TradeReportETL.Common.DryIoc
{
    public class DryIoCBootstrap
    {

        public DryIoCBootstrap(IContainer container, ILogger<DryIoCBootstrap> logger) : this(container, logger, AppDomain.CurrentDomain.GetAssemblies()) { }

        protected DryIoCBootstrap(IContainer container, ILogger<DryIoCBootstrap> iLogger, Assembly[] assemblies)
        {

            var logger = EnsureLoggerExist(iLogger);

            container.RegisterMany(assemblies,
                                   serviceTypeCondition: type => type.IsAssignableTo(typeof(IDependencyInjectionModule)),
                                   reuse: Reuse.Singleton);

            container.Register<DependencyInjectionModuleBootstrapper>(reuse: Reuse.Singleton);

            BootstrapModules(container, logger);

            var containerErrors = container.Validate();

            if (containerErrors != null && containerErrors.Any())
            {
                foreach (var errors in containerErrors)
                {
                    logger.LogWarning($"DryIoC resolution error for type {errors.Key.ServiceType} : {errors.Value.Message} ({errors.Value.StackTrace})");
                }
            }
            else
            {
                logger.LogInformation("DryIoC resolutions are OK.");
            }
        }

        private static ILogger<DryIoCBootstrap> EnsureLoggerExist(ILogger<DryIoCBootstrap> logger)
        {
            return logger ?? LoggerFactory.Create(builder => builder.AddDebug()).CreateLogger<DryIoCBootstrap>();
        }

        private static void BootstrapModules(IContainer container, ILogger<DryIoCBootstrap> logger)
        {

            try
            {
                RegisterModules(container);
            }
            catch (Exception e)
            {
                logger.LogError(e, "failed on bootstrapping the modules");
                throw;
            }
        }

        private static void RegisterModules(IContainer container)
        {
            var modules = container.Resolve<DependencyInjectionModuleBootstrapper>();
            modules.RegisterModules();
        }



    }

}