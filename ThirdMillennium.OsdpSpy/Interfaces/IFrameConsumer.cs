namespace ThirdMillennium.OsdpSpy
{
    public interface IFrameConsumer
    {
        void Subscribe(IFrameProducer input);
        void Unsubscribe();
    }
}