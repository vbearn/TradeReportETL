using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using TradeReportETL.Common.Http;
using TradeReportETL.Common.StartupExtensions;

namespace TradeReportETL.TransactionCost
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