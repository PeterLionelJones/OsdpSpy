namespace ThirdMillennium.OsdpSpy
{
    public interface IExchangeConsumer
    {
        void Summarise();
        void Subscribe(IExchangeProducer input);
        void Unsubscribe();
    }
}