namespace OsdpSpy.Abstractions
{
    public interface IFrameConsumer
    {
        void Subscribe(IFrameProducer input);
        void Unsubscribe();
    }
}