using System.Linq;
using OsdpSpy.Annotations;
using ThirdMillennium.Protocol;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.OsdpSpy
{
    public class SequenceAnnotator : Annotator<IExchange>
    {
        // Allow the configuration address.
        private const int ReaderCount = 128;

        private readonly IExchange[] _lastExchange = new IExchange[ReaderCount];
        
        public override void Annotate(IExchange input, IAnnotation output)
        {
            var address = input.Acu.Frame.Address;
            var previous = _lastExchange[address];

            // Do we have a previous frame to compare with?
            if (previous == null)
            {
                _lastExchange[address] = input;
                return;
            }

            if (!IsValidSequence(previous, input))
            {
                output
                    .AppendItem("NonCompliance", "Incorrect ACU Frame Sequence Number")
                    .AppendItem("PreviousSeqNo", previous.Acu.Frame.Sequence)
                    .AppendItem("PreviousSequence", previous.Sequence);
            }

            if (!IsValidResponseSequence(input))
            {
                output
                    .AppendItem(
                        "NonCompliance", "Incorrect PD Frame Sequence Number")
                    .AppendItem("PdFrameSeqNo", previous.Acu.Frame.Sequence);
            }
            
            _lastExchange[address] = input;

            if (input.Acu.Frame.Command == Command.COMSET &&
                input.Acu.Frame.Address == Frame.ConfigurationAddress)
            {
                var newAddress = input.Pd.Payload.Plain.ToNewAddress();
                _lastExchange[newAddress] = input;
            }
        }

        private static bool IsValidSequence(IExchange previous, IExchange current)
        {
            var isRetry = IsRetry(previous.Acu.Frame, current.Acu.Frame);
            
            var isValidSequence = IsValidSequence(
                previous.Acu.Frame.Sequence, 
                current.Acu.Frame.Sequence);
            
            return isRetry || isValidSequence;
        }

        private static bool IsValidResponseSequence(IExchange input)
        {
            return
                input.Pd?.Frame == null ||
                input.Acu.Frame.Sequence == input.Pd.Frame.Sequence ||
                input.Pd.Frame.Reply == Reply.NAK && input.Pd.Frame.Sequence == 0;
        }

        private static bool IsValidSequence(int previous, int current)
        {
            var nextSeq = new byte[] { 1, 2, 3, 1 };
            return current < nextSeq.Length && (current == 0 || current == nextSeq[previous]);
        }

        private static bool IsRetry(IExchange previous, IExchange current)
        {
            return 
                current.Pd?.Frame != null && 
                previous.Pd?.Frame != null && 
                IsRetry(previous.Acu.Frame, current.Acu.Frame);
        }
        
        private static bool IsRetry(BaseFrame previous, BaseFrame current)
            => current.FrameData.SequenceEqual(previous.FrameData);
    }
}