using System.Text;
using TradeReportETL.Common.StartupExtensions;
using TradeReportETL.Common.Cache;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using TradeReportETL.Pipeline.Modules.Transform.Models;
using TradeReportETL.Common.Http;
using System.Reflection;
using TradeReportETL.Pipeline.Modules.Transform.Services.ApiClient;

namespace TradeReportETL.Pipeline.Transform
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddOptions()
                .AddSwagger(Assembly.GetExecutingAssembly().GetName().Name)
                .AddCorsSettings(Configuration)
                .AddDistributedCache(Configuration)
                .AddHttpClient()
                .AddHttpContextAccessor()
                .AddApiClients(Configuration)
                .AddControllers();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwaggerSettings(Assembly.GetExecutingAssembly().GetName().Name);

            app.UseHttpsRedirection();

            app.UseCorsSettings();
            app.UseMiddleware<HttpStatusExceptionMiddleware>();

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}