using OsdpSpy.Abstractions;
using OsdpSpy.Annotations;
using OsdpSpy.Decoders;
using OsdpSpy.Osdp;

namespace OsdpSpy.Annotators
{
    public class KeysetAnnotator : AlertingAnnotator<IExchange>
    {
        public KeysetAnnotator(IFactory<IAnnotation> factory) : base(factory) {}

        public override void Annotate(IExchange input, IAnnotation output)
        {
            if (input.Acu.Frame.Command != Command.KEYSET) return;
            if (input.Acu.Payload.Plain == null) return;

            this.CreateOsdpAlert("SCBK Set with osdp_KEYSET")
                .AppendItem("TriggeredBy", input.Sequence)
                .AppendItem("Scbk", input.Acu.Payload.Plain.ToScbk())
                .AndLogTo(this);
        }
    }
}