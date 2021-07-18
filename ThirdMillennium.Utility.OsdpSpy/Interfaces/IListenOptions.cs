namespace ThirdMillennium.Utility.OSDP
{
    public interface IListenOptions : IFileTransferOptions, IFrameLoggerOptions
    {
        string ElasticSearchUrl { get; set; }
        bool FilterPollAck { get; set; }
        string PortName { get; set; }
        string BaudRate { get; set; }
        string SeqUrl { get; set; }
    }

    public static class ListenOptionsExtensions
    {
        private const string AutoOption = "auto";

        public static int ToBaudRate(this IListenOptions options)
            => options.BaudRate.ToBaudRate();

        private static int ToBaudRate(this string rateString)
        {
            return rateString?.ToLower() switch
            {
                null or "auto" => 9600,     
                _ => int.TryParse(rateString, out var baudRate) ? baudRate : 9600
            };
        }

        public static bool CanScanBaudRates(this IListenOptions options)
            => options.BaudRate.CanScanBaudRates();

        private static bool CanScanBaudRates(this string rateString)
            => rateString?.ToLower() == "auto";
    }

    public interface IFrameLoggerOptions
    {
        bool CaptureToOsdpCap { get; set; }
        string OsdpCapDirectory { get; set; }
    }
}