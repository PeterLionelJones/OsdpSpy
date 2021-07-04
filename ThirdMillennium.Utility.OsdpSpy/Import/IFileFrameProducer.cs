namespace ThirdMillennium.Utility.OSDP
{
    public interface IFileFrameProducer : IFrameProducer
    {
        bool Process(string filename);
    }
}