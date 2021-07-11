using ThirdMillennium.Annotations;
using ThirdMillennium.Protocol;

namespace ThirdMillennium.Utility.OSDP
{
    public class ReplyAnnotator : BaseAnnotator<IExchange>
    {
        public ReplyAnnotator(IReplyDecoderCollection decoder)
        {
            _decoder = decoder;
        }

        private readonly IReplyDecoderCollection _decoder;
        
        public override void Annotate(IExchange input, IAnnotation output)
        {
            if (input.Pd?.Payload == null) return;

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