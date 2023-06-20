using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace TradeReportETL.Common.StartupExtensions
{
    public static class SwaggerExtensionsMethods
    {

        public static IServiceCollection AddSwagger(this IServiceCollection services, string endpointName)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = endpointName, Version = "v1" });
            });
            return services;
        }
        public static void UseSwaggerSettings(this IApplicationBuilder app, string endpointName)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("./swagger/v1/swagger.json", endpointName + " v1");
                c.DocumentTitle = endpointName;
                c.RoutePrefix = string.Empty;
            });
        }
    }
}