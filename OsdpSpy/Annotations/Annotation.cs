using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.Logging;

[assembly:InternalsVisibleTo("OsdpSpy.Tests.Annotations")]

namespace OsdpSpy.Annotations;

public class Annotation : IAnnotation
{
    public Annotation(ILogger<Annotation> logger)
        => _logger = logger;

    private readonly StringBuilder _message = new();
    private readonly List<object> _parameters = new();
    private readonly ILogger<Annotation> _logger;
        
    public IAnnotation Append(string message, object[] parameters)
    {
        _message.Append(message);
            
        if (parameters != null)
        {
            _parameters.AddRange(parameters);
        }
            
        return this;
    }

    public bool Contains(string tag)
    {
        return _message.ToString().Contains($"{{{tag}}}");
    }

    public void Log()
    {
        // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
#pragma warning disable CA2254
        _logger.LogInformation(Message, Parameters);
#pragma warning restore CA2254
    }

    internal string Message => _message.ToString();

    internal object[] Parameters => _parameters.ToArray();
}