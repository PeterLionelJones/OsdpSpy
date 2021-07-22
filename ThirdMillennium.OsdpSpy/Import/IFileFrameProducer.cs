namespace ThirdMillennium.OsdpSpy
{
    public interface IFileFrameProducer : IFrameProducer
    {
        bool Process(string filename);
    }
}