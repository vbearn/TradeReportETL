using System;
using System.IO;
using TradeReportETL.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Debugging;
using NServiceBus;
using TradeReportETL.Common.DryIoc;
using TradeReportETL.Pipeline.Transform;
using TradeReportETL.Shared.Messages;

namespace TradeReportETL.Pipeline
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SelfLog.Enable(Console.Error);
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            try
            {
                Log.Information("Configuring web host...");
                var host = CreateHostBuilder(args).Build();

                Log.Information("Starting web host...");
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new DryIocServiceProviderFactory<DryIoCBootstrap>())
                .UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .WriteTo.Seq(context.Configuration["Serilog:SeqServerUrl"])
                )
                .ConfigureWebHostDefaults(webHostBuilder =>
                {
                    webHostBuilder
                        .UseContentRoot(Directory.GetCurrentDirectory())
                        .UseIISIntegration()
                        .ConfigureKestrel((context, options) =>
                        {
                        })
                        .UseStartup<Startup>();
                }).UseNServiceBus(context =>
                {
                    Log.Information("Start configuring NServiceBus...");
                    var endpointConfiguration = new EndpointConfiguration(TransformTransactions.EndpointName);

                    endpointConfiguration.EnableInstallers();

                    var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
                    transport.UseConventionalRoutingTopology(QueueType.Quorum);
                    transport.ConnectionString(context.Configuration["RabbitMQ:ConnectionStrings"]);

                    transport.Routing().RouteToEndpoint(typeof(LoadTransaction), LoadTransaction.EndpointName);

                    Log.Information("Finished configuring NServiceBus.");

                    return endpointConfiguration;

                });
        }
    }
}