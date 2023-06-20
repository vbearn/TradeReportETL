using System;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace TradeReportETL.Common.DryIoc
{
    public class DryIocServiceProviderFactory<TBootstrap> : IServiceProviderFactory<IContainer> where TBootstrap : DryIoCBootstrap
    {

        public IContainer CreateBuilder(IServiceCollection services)
        {
            return new Container()
                .WithDependencyInjectionAdapter(services);
        }

        public IServiceProvider CreateServiceProvider(IContainer containerBuilder)
        {
            return containerBuilder.ConfigureServiceProvider<TBootstrap>();
        }

    }
}