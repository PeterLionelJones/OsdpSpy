using ThirdMillennium.Annotations;

namespace ThirdMillennium.Utility.OSDP
{
    public class RawFrameAnnotator : BaseAnnotator<IExchange>
    {
        public RawFrameAnnotator(IExchangeLoggerOptions options)
        {
            _options = options;
        }

        private readonly IExchangeLoggerOptions _options;

        public override bool IncludeInput(IExchange input)
            => !input.IsPollAckPair() || !_options.FilterPollAck;

        public override void Annotate(IExchange input, IAnnotation output)
        {
            output.Append("Sequence: {Sequence}, ", new object[] {input.Sequence});
            
            // Add the command/response summary.
            if (input.Pd == null)
            {
                // Append the command only as the frame timed out.
                output
                    .Annotate(
                        "osdp_{AcuCommand} -> <NO RESPONSE>\n\n",
                        input.Acu.Frame.Command.ToString())
                    .AnnotateAcuFrame(input.Acu);
            }
            else
            {
                // How long between the command and the reply?
                var elapsed = (input.Pd.Timestamp - input.Acu.Timestamp).TotalMilliseconds;

                // Append the command and the reply.
                output
                    .Annotate(
                        "osdp_{AcuCommand} -> osdp_{PdReply} in {ResponseTime:F3}ms\n\n",
                        input.Acu.Frame.Command.ToString(),
                        input.Pd?.Frame.Reply.ToString(),
                        elapsed)
                    .AnnotateAcuFrame(input.Acu)
                    .AnnotatePdFrame(input.Pd);
            }

            // Tag a Poll/Ack pair for easy filtering later.
            if (input.IsPollAckPair())
            {
                output.AppendItem("PollAckPair", input.IsPollAckPair());
            }
            
            // Tag a timeout for easy filtering later.
            if (input.IsNoResponse())
            {
                output.AppendItem("NoResponse",input.IsNoResponse());
            }

            // Set the payloads for the command and the reply.
            input.Acu.SetInitialPayload();
            input.Pd?.SetInitialPayload();
        }
    }
}