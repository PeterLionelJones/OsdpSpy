using OsdpSpy.Abstractions;
using OsdpSpy.Annotations;
using OsdpSpy.Decoders;
using OsdpSpy.Extensions;

namespace OsdpSpy.Annotators
{
    public class CommandAnnotator : Annotator<IExchange>
    {
        public CommandAnnotator(ICommandDecoderCollection decoder)
        {
            _decoder = decoder;
        }

        private readonly ICommandDecoderCollection _decoder;
        
        public override void Annotate(IExchange input, IAnnotation output)
        {
            output.AppendNewLine();
            
            output.AppendItem("AcuCommand", input.Acu.Frame.Command.ToString());

            if (input.Acu.Payload.Cipher != null)
            {
                output.AppendItem("AcuCipher", input.Acu.Payload.Cipher.ToHexString());
            }
            
            if (input.Acu.Payload.Plain != null)
            {
                output.AppendItem("AcuPlain",input.Acu.Payload.Plain.ToHexString());
                _decoder.Decode(input.Acu.Frame.Command, input.Acu.Payload.Plain, output);
            }
        }
    }
}