using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("OsdpSpy.Tests.Annotations")]

namespace OsdpSpy.Annotations;

public class AlertingAnnotator<T> : Annotator<T>, IAlertingAnnotator<T>
{
    protected AlertingAnnotator(IFactory<IAnnotation> factory)
    {
        _queue = new ConcurrentQueue<IAnnotation>();
        _factory = factory;
    }

    private readonly ConcurrentQueue<IAnnotation> _queue;
    private readonly IFactory<IAnnotation> _factory;

    internal ConcurrentQueue<IAnnotation> InternalQueue => _queue;

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
        do
        {
            if (_queue.TryDequeue(out var alert))
            {
                alert.Log();
            }
        } while (!_queue.IsEmpty);
    }
}