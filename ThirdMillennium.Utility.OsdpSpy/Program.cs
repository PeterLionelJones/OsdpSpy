using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using ThirdMillennium.Protocol;
using ThirdMillennium.Protocol.Core;

namespace ThirdMillennium.Utility.OSDP
{
    [HelpOption]
    [Command(Name = "osdpspy", Description = "OSDP Utility Program"),
        Subcommand(typeof(ImportCommand)),
        Subcommand(typeof(ListenCommand)),
        Subcommand(typeof(ListPortsCommand))]
    internal class Program
    {
        private static async Task<int> Main(string[] args)
        {
            try
            {
                // Get any logging options that we may need.
                var seqUrl = args.SeqUrl();
                
                // Configure logging for this command.
                var logger = new LoggerConfiguration().WriteTo.Console();
                if (seqUrl != null) logger = logger.WriteTo.Seq(seqUrl);
                Log.Logger = logger.CreateLogger();

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

        private static void ConfigureServices(
            HostBuilderContext context,
            IServiceCollection services)
        {
            services
                .AddFactories()
                .AddSingleton(PhysicalConsole.Singleton)
                .AddSingleton<ICommandAnnotator, CommandAnnotator>()
                .AddSingleton<IExchangeConsumer, ExchangeLogger>()
                .AddSingleton<IExchangeLoggerOptions, ListenOptions>()
                .AddSingleton<IExchangeProducer, ExchangeProducer>()
                .AddSingleton<IFileFrameProducer, FileFrameProducer>()
                .AddSingleton<IFrameLogger, FrameLogger>()
                .AddSingleton<IKeyStore, KeyStore>()
                .AddSingleton<IRawFrameAnnotator, RawFrameAnnotator>()
                .AddSingleton<IImportOptions, ImportOptions>()
                .AddSingleton<IListenOptions, ListenOptions>()
                //.AddSingleton<IOctetAnnotator, OctetAnnotator>()
                .AddSingleton<IBusFrameProducer, BusFrameProducer>()
                .AddSingleton<IReplyAnnotator, ReplyAnnotator>()
                .AddSingleton<ISecureChannelAnnotator, SecureChannelAnnotator>()
                .AddSingleton<ISerialDeviceManager, SerialDeviceManager>()
                .AddSingleton<ISummariser, ExchangeLogger>();
        }

        // ReSharper disable once UnusedParameter.Local
        // ReSharper disable once UnusedMember.Local
        private static int OnExecute(IConsole console) => -1;
    }
}