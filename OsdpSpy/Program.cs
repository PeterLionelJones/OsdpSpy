using System;
using System.Reflection;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OsdpSpy.Abstractions;
using OsdpSpy.Annotators;
using OsdpSpy.Import;
using OsdpSpy.List;
using OsdpSpy.Listen;
using OsdpSpy.Models;
using OsdpSpy.Serial;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace OsdpSpy
{
    [HelpOption]
    [Command(Name = "osdpspy", Description = "\nosdpspy Protocol Analysis Tool")]
    [Subcommand(typeof(ImportCommand))]
    [Subcommand(typeof(ListenCommand))]
    [Subcommand(typeof(ListPortsCommand))]
    internal class Program
    {
        private static async Task<int> Main(string[] args)
        {
            try
            {
                // Create the logger for this application.
                Log.Logger = CreateLogger(args);

                // Configure dependency injection, logging and start the command.
                return await new HostBuilder()
                    .ConfigureLogging((_, b) => b.AddSerilog(dispose: true))
                    .ConfigureServices(ConfigureServices)
                    .RunCommandLineApplicationAsync<Program>(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }
        }

        [Option("-v|--version")]
        private static bool ReportVersion { get; }

        private static string GetVersion()
        {
            var version = Assembly.GetEntryAssembly().GetName().Version;
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }

        private static string Version => GetVersion(); 

        private static ILogger CreateLogger(string[] args)
        {
            // Get any logging options that we may need.
            var seqUrl = args.SeqUrl();
            var elasticsearchUrl = args.ElasticsearchUrl();
                
            // Configure logging for this command.
            var logger = new LoggerConfiguration().WriteTo.Console();

            if (seqUrl != null)
            {
                logger = logger.WriteTo.Seq(seqUrl);
            }

            if (elasticsearchUrl != null)
            {
                logger = logger.WriteTo.Elasticsearch(
                    new ElasticsearchSinkOptions(new Uri(elasticsearchUrl) ){
                        AutoRegisterTemplate = true,
                        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                        IndexFormat = $"osdpspy-{DateTime.UtcNow:yyyy-MM}"
                    });
            }
            
            return logger.CreateLogger();
        }

        private static void ConfigureServices(
            HostBuilderContext context,
            IServiceCollection services)
        {
            services
                .AddAnnotators()
                .AddFactories()
                .AddSingleton(PhysicalConsole.Singleton)
                .AddSingleton<IBusFrameProducer, BusFrameProducer>()
                .AddSingleton<IExchangeConsumer, ExchangeLogger>()
                .AddSingleton<IExchangeLoggerOptions, ListenOptions>()
                .AddSingleton<IExchangeProducer, ExchangeProducer>()
                .AddSingleton<IFileFrameProducer, FileFrameProducer>()
                .AddSingleton<IFrameLogger, FrameLogger>()
                .AddSingleton<IFrameNotifier, FrameNotifier>()
                .AddSingleton<IFrameQueue, FrameQueue>()
                .AddSingleton<IFrameReceiver, FrameReceiver>()
                .AddSingleton<IKeyStore, KeyStore>()
                .AddSingleton<IFileTransferOptions, FileTransferOptions>()
                .AddSingleton<IFrameLoggerOptions, FrameLoggerOptions>()
                .AddSingleton<IImportOptions, ImportOptions>()
                .AddSingleton<IListenOptions, ListenOptions>()
                .AddSingleton<ISerialDeviceManager, SerialDeviceManager>();
        }

        // ReSharper disable once UnusedParameter.Local
        // ReSharper disable once UnusedMember.Local
        private static int OnExecute(CommandLineApplication app)
        {
            if (ReportVersion)
            {
                var console = app.GetRequiredService<IConsole>();
                console.WriteLine(Version);
                return 0;
            }
            
            app.ShowHelp();
            return 1;
        }
    }
}