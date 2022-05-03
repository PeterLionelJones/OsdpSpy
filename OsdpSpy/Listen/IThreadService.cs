using System.Threading;

namespace OsdpSpy.Listen
{
    public interface IThreadService
    {
        bool IsRunning { get; }
        void Start(CancellationToken token);
    }
}