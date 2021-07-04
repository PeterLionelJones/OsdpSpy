using ThirdMillennium.Annotations;
using ThirdMillennium.Protocol;

namespace ThirdMillennium.Utility.OSDP
{
    public static class AnnotationsExtensions
    {
        public static IAnnotation Annotate(this IAnnotation a, string m, object p1)
            => a.Append(m, new[] { p1 });

        private static IAnnotation Annotate(this IAnnotation a, string m, object p1, object p2)
            => a.Append(m, new[] { p1, p2 });

        public static IAnnotation Annotate(
            this IAnnotation a, string m, object p1, object p2, object p3)
            => a.Append(m, new[] { p1, p2, p3 });

        private static IAnnotation Annotate(
            this IAnnotation a, string m, object p1, object p2, object p3, object p4)
            => a.Append(m, new[] { p1, p2, p3, p4 });

        public static IAnnotation Annotate(
            this IAnnotation a, string m, object p1, object p2, object p3, object p4, object p5)
            => a.Append(m, new[] { p1, p2, p3, p4, p5 });

        private const string Space = " ";
        private static readonly string Pad = $"{Space,29}";

        public static IAnnotation AnnotateAcuFrame(this IAnnotation a, IFrameProduct product)
        {
            var result = a.Annotate(
                Pad + "Addr:{AcuAddress} Len:{AcuLength} Seq:{AcuSequence} Check:{AcuCheck}",
                product.Frame.Address,
                product.Frame.FrameLength,
                product.Frame.Sequence,
                product.CheckMethod());

            if (product.HasSecurityControlBlock())
            {
                result = result.Annotate(
                    " Security:{AcuSecurityBlock}", 
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
                    Pad + "Addr:{PdAddress} Len:{PdLength} Seq:{PdSequence} Check:{AcuCheck}", 
                    product.Frame.Address,
                    product.Frame.FrameLength,
                    product.Frame.Sequence,
                    product.CheckMethod());

            if (product.HasSecurityControlBlock())
            {
                result = result.Annotate(
                    " Security:{PDSecurityBlock}", 
                    product.SecurityControlBlock());
            }

            return result.Append("\n", null);
        }
    }
}