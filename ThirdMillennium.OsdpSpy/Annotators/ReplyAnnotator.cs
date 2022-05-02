using OsdpSpy.Annotations;
using ThirdMillennium.Protocol;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.OsdpSpy
{
    public class ReplyAnnotator : Annotator<IExchange>
    {
        public ReplyAnnotator(IReplyDecoderCollection decoder)
        {
            _decoder = decoder;
        }

        private readonly IReplyDecoderCollection _decoder;
        
        public override void Annotate(IExchange input, IAnnotation output)
        {
            if (input.Pd?.Payload == null) return;

            if (input.Pd.Frame.Reply == Reply.NAK && input.Pd.Payload.Cipher?.Length == 1)
            {
                input.Pd.Payload.Plain = input.Pd.Payload.Cipher;
                input.Pd.Payload.Cipher = null;
            }

            output.AppendNewLine();
            
            output.AppendItem("PdReply",input.Pd.Frame.Reply.ToString());

            if (input.Pd.Payload.Cipher != null)
            {
                output.AppendItem("PdCipher",input.Pd.Payload.Cipher.ToHexString());
            }
            
            if (input.Pd.Payload.Plain != null)
            {
                output.AppendItem("PdPlain",input.Pd.Payload.Plain.ToHexString());
                _decoder.Decode(input.Pd.Frame.Reply, input.Pd.Payload.Plain, output);
            }
        }
    }
}