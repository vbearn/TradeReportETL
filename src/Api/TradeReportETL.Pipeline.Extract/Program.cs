using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Debugging;
using NServiceBus;
using TradeReportETL.Common.DryIoc;
using TradeReportETL.Shared.Messages;
using FluentStorage;

namespace TradeReportETL.Pipeline.Extract
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
                StorageFactory.Modules.UseFtpStorage();

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
                    var endpointConfiguration = new EndpointConfiguration(ExtractCsvFile.EndpointName);

                    endpointConfiguration.EnableInstallers();

                    var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
                    transport.UseConventionalRoutingTopology(QueueType.Quorum);
                    transport.ConnectionString(context.Configuration["RabbitMQ:ConnectionStrings"]);

                    transport.Routing().RouteToEndpoint(typeof(TransformTransactions), TransformTransactions.EndpointName);

                    return endpointConfiguration;

                });
        }
    }
}