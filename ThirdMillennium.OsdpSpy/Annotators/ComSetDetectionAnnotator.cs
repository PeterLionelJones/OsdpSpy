using System.Linq;
using OsdpSpy.Annotations;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.OsdpSpy
{
    public class ComSetDetectionAnnotator : AlertingAnnotator<IExchange>
    {
        public ComSetDetectionAnnotator(
            IBusFrameProducer frames, 
            IFactory<IAnnotation> factory) : base(factory)
        {
            _frames = frames;
        }

        private readonly IBusFrameProducer _frames;
        
        public override void Annotate(IExchange input, IAnnotation output)
        {
            // Looking for a COMSET command with a plaintext payload.
            if (input.Acu.Frame.Command != Command.COMSET) return;
            if (input.Acu.Payload.Plain == null) return;

            // Make sure we have a valid baud rate.
            var payload = input.Acu.Payload.Plain;
            var rate = input.Acu.Payload.Plain.ToBaudRate();
            if (!rate.IsValidBaudRate()) return;
            
            // Raise an alert.
            this.CreateOsdpAlert("Switching to New Baud Rate")
                .AppendItem("TriggeredBy", input.Sequence)
                .AppendItem("NewBaudRate", rate)
                .AndLogTo(this);
            
            // Switch the rate.
            _frames.SetRate(rate);
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
            var rates = new[] {9600, 19200, 38400, 57600, 115200, 230400};
            return rates.Any(x => x == rate);
        }
    }
}