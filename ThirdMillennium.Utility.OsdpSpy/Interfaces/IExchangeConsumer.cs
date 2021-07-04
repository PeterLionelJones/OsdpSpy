namespace ThirdMillennium.Utility.OSDP
{
    public interface IExchangeConsumer
    {
        void Subscribe(IExchangeProducer input);
        void Unsubscribe();
    }
}