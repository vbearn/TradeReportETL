using TradeReportETL.ImportExport.Services.FileUpload;
using DryIoc;
using Microsoft.Extensions.Configuration;
using TradeReportETL.Common.DryIoc;
using TradeReportETL.Pipeline.Modules.Load.Services;
using TradeReportETL.Shared.Services;

namespace TradeReportETL.ImportExport
{
    public class ImportExportServicesModule : IDependencyInjectionModule
    {
        public void ConfigureServices(IRegistrator service, IResolver resolver, IConfiguration configuration)
        {
            service.Register<IFileUploader, FileUploader>(Reuse.Scoped);
            service.Register<RedisLockService>(Reuse.Singleton);
            service.Register<ITradeReportLoadService, TradeReportLoadService>(Reuse.Scoped);
        }
    }
}