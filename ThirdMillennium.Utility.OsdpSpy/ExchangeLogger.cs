using ThirdMillennium.Annotations;

namespace ThirdMillennium.Utility.OSDP
{
    public class ExchangeLogger : IExchangeConsumer, ISummariser
    {
        public ExchangeLogger(
            IExchangeLoggerOptions options,
            IFactory<IAnnotation> annotations,
            IRawFrameAnnotator rawFrameAnnotator,
            //IOctetAnnotator octetAnnotator,
            ISecureChannelAnnotator secureChannelAnnotator,
            ICommandAnnotator commandAnnotator,
            IReplyAnnotator replyAnnotator)
        {
            _options = options;
            _annotations = annotations;
            
            _annotators = new IAnnotator<IExchange>[]
            {
                rawFrameAnnotator,
                //octetAnnotator,
                secureChannelAnnotator,
                commandAnnotator,
                replyAnnotator
            };
            
            _summarisers = new ISummariser[]
            {
                //octetAnnotator
            };
        }

        private readonly IExchangeLoggerOptions _options;
        private readonly IFactory<IAnnotation> _annotations;
        private readonly IAnnotator<IExchange>[] _annotators;
        private readonly ISummariser[] _summarisers;
        private IExchangeProducer _input;
        
        private void OnExchange(object sender, IExchange input)
        {
            // Annotate the exchange.
            var annotation = _annotations.Create();
            foreach (var annotator in _annotators)
            {
                annotator.Annotate(input, annotation);
            }
                
            // Filter out the Poll/Ack pairs?
            if (input.IsPollAckPair() && _options.FilterPollAck) return;
            
            // Log the exchange.
            annotation.Log();
        }
        
        public void Subscribe(IExchangeProducer input)
        {
            if (_input != null) Unsubscribe();
            
            _input = input;
            _input.ExchangeHandler += OnExchange;
        }

        public void Unsubscribe()
        {
            if (_input == null) return;

            _input.ExchangeHandler -= OnExchange;
            _input = null;
        }

        public void Summarise()
        {
            foreach (var summariser in _summarisers)
            {
                summariser.Summarise();
            }
        }
    }
}