namespace ThirdMillennium.Utility.OSDP
{
    public interface IFrameConsumer
    {
        void Subscribe(IFrameProducer input);
        void Unsubscribe();
    }
}