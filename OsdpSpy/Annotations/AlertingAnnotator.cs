using System.Collections.Concurrent;

namespace OsdpSpy.Annotations
{
    public class AlertingAnnotator<T> : Annotator<T>, IAlertingAnnotator<T>
    {
        protected AlertingAnnotator(IFactory<IAnnotation> factory)
        {
            _queue = new ConcurrentQueue<IAnnotation>();
            _factory = factory;
        }

        private readonly ConcurrentQueue<IAnnotation> _queue;
        private readonly IFactory<IAnnotation> _factory;

        public IAnnotation CreateInformation()
            => _factory.Create();
        
        public IAnnotation CreateAlert(
            string alertName, 
            string alertMessage, 
            string alertHeading = null)
        {
            return _factory.Create()
                .Append($"{alertHeading}: {{{alertName}}}", alertMessage)
                .AppendNewLine();
        }

        public void LogAlert(IAnnotation alert)
            => _queue.Enqueue(alert.AppendNewLine());

        public override void ReportState()
        {
            while (_queue.Count > 0)
            {
                if (!_queue.TryDequeue(out var alert)) return;
                alert.Log();
            }
        }
    }
}