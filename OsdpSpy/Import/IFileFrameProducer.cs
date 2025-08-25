using OsdpSpy.Abstractions;

namespace OsdpSpy.Import;

public interface IFileFrameProducer : IFrameProducer
{
    bool Process(string filename);
}