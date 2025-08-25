using System.Text;
using McMaster.Extensions.CommandLineUtils;
using OsdpSpy.Abstractions;

namespace OsdpSpy.Listen;

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
        
        if (options.BaudRate != null)
            result.Append($"\n  - Rate = {options.BaudRate}");
        
        if (options.SeqUrl != null) 
            result.Append($"\n  - SeqUrl = {options.SeqUrl}");

        if (options.CaptureToOsdpCap)
        {
            result
                .Append($"\n  - Capture to osdpcap File = {options.CaptureToOsdpCap.AsOnOff()}")
                .Append($"\n  - osdpcap File Directory = {options.OsdpCapDirectory}");
        }

        if (options.CaptureOsdpFileTransfer)
        {
            result
                .Append($"\n  - Capture OSDP File Transfer = {options.CaptureOsdpFileTransfer.AsOnOff()}")
                .Append($"\n  - OSDP File Transfer Directory = {options.OsdpFileTransferDirectory}");
        }

        return result.ToString();
    }

    private static string AsOnOff(this bool state) => state ? "On" : "Off";
}