using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace ThirdMillennium.Utility.OSDP
{
    public class DeferredLogger : IDeferredLogger
    {
        public DeferredLogger(ILogger<DeferredLogger> logger)
        {
            _logger = logger;
            _queue = new ConcurrentQueue<DeferredLogItem>();
        }

        private readonly ILogger<DeferredLogger> _logger;
        private readonly ConcurrentQueue<DeferredLogItem> _queue;

        public void LogInformation(string msg, object[] information)
        {
            _queue.Enqueue(new DeferredLogItem(msg, information));
        }

        public void Flush()
        {
            if (_queue.IsEmpty) return;

            while (_queue.TryDequeue(out var item))
            {
                _logger.Log(item);
            }
        }
    }

    internal class DeferredLogItem
    {
        public DeferredLogItem(string message, object[] information)
        {
            Message = message;
            Information = information;
        }
        internal string Message { get; }
        internal object[] Information { get; }
    }

    public interface IDeferredLogger
    {
        void LogInformation(string msg, object[] data);
        void Flush();
    }

    internal static class DeferredLoggerExtensions
    {
        internal static void Log(this ILogger logger, DeferredLogItem item)
        {
            logger.LogInformation(item.Message, item.Information);
        }

        public static void LogInformation(this IDeferredLogger l, string m, object p1 = null)
        {
            l.LogInformation(m, new object[] { p1 });
        }
    }
}