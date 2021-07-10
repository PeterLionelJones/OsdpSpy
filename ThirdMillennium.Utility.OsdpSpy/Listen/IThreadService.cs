using System.Threading;

namespace ThirdMillennium.Utility.OSDP
{
    public interface IThreadService
    {
        bool IsRunning { get; }
        void Start(CancellationToken token);
    }
}