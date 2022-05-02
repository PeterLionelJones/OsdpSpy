using OsdpSpy.Annotations;

namespace ThirdMillennium.OsdpSpy
{
    public class ExchangeLogger : IExchangeConsumer
    {
        public ExchangeLogger(IAnnotatorCollection<IExchange> annotators)
        {
            _annotators = annotators;
        }

        private readonly IAnnotatorCollection<IExchange> _annotators;
        
        private IExchangeProducer _input;
        
        private void OnExchange(object sender, IExchange input)
        {
            _annotators.Annotate(input);
            _annotators.ReportState();
        }

        public void Summarise()
        {
            _annotators.Summarise();
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
    }
}