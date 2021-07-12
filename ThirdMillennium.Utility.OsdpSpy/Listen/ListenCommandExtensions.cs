using System.Text;
using McMaster.Extensions.CommandLineUtils;

namespace ThirdMillennium.Utility.OSDP
{
    public static class ListenCommandExtensions
    {
        public static void LogParameters(
            this IConsole console, IListenOptions options, string prefix = null)
        {
            var message = prefix ?? "Listen Command Parameters";
            console.WriteLine($"\n{message}\n{options.AsString()}\n");
        }

        private static string AsString(this IListenOptions options)
        {
            var result = new StringBuilder();
            
            if (options.ElasticSearchUrl != null) 
                result.Append($"\n  - ElasticSearchUrl = {options.ElasticSearchUrl}");

            if (options.FilterPollAck)
                result.Append($"\n  - Filter = {options.FilterPollAck.AsOnOff()}");

            if (options.PortName != null)
                result.Append($"\n  - PortName = {options.PortName}");
        
            if (options.BaudRate != -1)
                result.Append($"\n  - Rate = {options.BaudRate}");
        
            if (options.SeqUrl != null) 
                result.Append($"\n  - SeqUrl = {options.SeqUrl}");

            if (options.CaptureOsdpFiles)
            {
                result.Append($"\n  - Capture OSDP Files = {options.CaptureOsdpFiles.AsOnOff()}");
                result.Append($"\n  - OSDP File Directory = {options.OsdpFileCaptureDirectory}");
            }

            return result.ToString();
        }

        private static string AsOnOff(this bool state) => state ? "On" : "Off";
    }
}