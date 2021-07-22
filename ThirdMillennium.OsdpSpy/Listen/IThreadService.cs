using System.Threading;

namespace ThirdMillennium.OsdpSpy
{
    public interface IThreadService
    {
        bool IsRunning { get; }
        void Start(CancellationToken token);
    }
}