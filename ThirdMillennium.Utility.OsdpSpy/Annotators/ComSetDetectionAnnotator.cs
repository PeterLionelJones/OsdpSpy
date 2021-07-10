using ThirdMillennium.Annotations;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.Utility.OSDP
{
    public class ComSetDetectionAnnotator : ExchangeAnnotator
    {
        public ComSetDetectionAnnotator(
            IBusFrameProducer frames, 
            IDeferredLogger logger)
        {
            _frames = frames;
            _logger = logger;
        }

        private readonly IBusFrameProducer _frames;
        private readonly IDeferredLogger _logger;
        
        public override void Annotate(IExchange input, IAnnotation output)
        {
            // Looking for a COMSET command with a plaintext payload.
            if (input.Acu.Frame.Command != Command.COMSET) return;
            if (input.Acu.Payload.Plain == null) return;

            // Make sure we have a valid baud rate.
            var payload = input.Acu.Payload.Plain;
            var rate = input.Acu.Payload.Plain.ToBaudRate();
            if (!rate.IsValidBaudRate()) return;
            
            // Notify the event sink.
            _logger.LogInformation("Switching to {BaudRate} Baud\n", rate);
            
            // Switch the rate.
            _frames.SetRate(rate);
        }

        public override void ReportState()
        {
            _logger.Flush();
        }
    }

    internal static class ComSetPayloadExtensions
    {
        internal static int ToBaudRate(this byte[] payload)
        {
            return payload[1] | payload[2] << 8 | payload[3] << 16 | payload[4] << 24;
        }

        internal static bool IsValidBaudRate(this int rate)
        {
            return rate switch
            {
                9600 => true,
                19200 => true,
                38400 => true,
                57600 => true,
                115200 => true,
                230400 => true,
                _ => false
            };
        }
    }
}