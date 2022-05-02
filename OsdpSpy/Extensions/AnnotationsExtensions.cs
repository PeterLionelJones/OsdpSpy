using OsdpSpy.Abstractions;
using OsdpSpy.Annotations;
using ThirdMillennium.Protocol;

namespace OsdpSpy.Extensions
{
    public static class AnnotationsExtensions
    {
        private const string Space = " ";
        private static readonly string Pad = $"{Space,29}";

        public static IAnnotation AnnotateAcuFrame(this IAnnotation a, IFrameProduct product)
        {
            var result = a.Annotate(
                Pad + "Address: {AcuAddress} Len: {AcuLength} Seq: {AcuSequence} Check: {AcuCheck}",
                product.Frame.Address,
                product.Frame.FrameLength,
                product.Frame.Sequence,
                product.CheckMethod());

            if (product.HasSecurityControlBlock())
            {
                result = result.Annotate(
                    " Security: {AcuSecurityBlock}", 
                    product.SecurityControlBlock());
            }
                
            result = result.Annotate(
                "\n  {Transmitted:dd/MM HH:mm:ss.ffffff}  TX: {AcuCommandFrame}\n",
                product.Timestamp, 
                product.Frame.FrameData.ToHexString());

            return result;
        }

        public static IAnnotation AnnotatePdFrame(this IAnnotation a, IFrameProduct product)
        {
            var result = a
                .Annotate(
                    "  {Received:dd/MM HH:mm:ss.ffffff}  RX: {PdResponseFrame}\n",
                    product.Timestamp,
                    product.Frame.FrameData.ToHexString())
                .Annotate(
                    Pad + "Address: {PdAddress} Len: {PdLength} Seq: {PdSequence} Check: {AcuCheck}", 
                    product.Frame.Address,
                    product.Frame.FrameLength,
                    product.Frame.Sequence,
                    product.CheckMethod());

            if (product.HasSecurityControlBlock())
            {
                result = result.Annotate(
                    " Security: {PdSecurityBlock}", 
                    product.SecurityControlBlock());
            }

            return result;
        }
    }
}