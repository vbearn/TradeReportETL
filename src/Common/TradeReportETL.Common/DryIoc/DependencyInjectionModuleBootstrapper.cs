using System.Collections.Generic;
using DryIoc;
using Microsoft.Extensions.Configuration;

namespace TradeReportETL.Common.DryIoc
{

    internal class DependencyInjectionModuleBootstrapper
    {

        private readonly IConfiguration _configuration;
        private readonly IEnumerable<IDependencyInjectionModule> _modules;

        private readonly IResolver _resolver;
        private readonly IRegistrator _service;

        public DependencyInjectionModuleBootstrapper(IEnumerable<IDependencyInjectionModule> modules, IRegistrator service, IConfiguration configuration, IResolver resolver)
        {
            _modules = modules;
            _service = service;
            _configuration = configuration;
            _resolver = resolver;
        }

        public void RegisterModules()
        {
            foreach (var m in _modules)
            {
                m.ConfigureServices(_service, _resolver, _configuration);
            }
        }

    }

}