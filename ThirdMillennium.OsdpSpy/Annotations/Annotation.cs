using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace OsdpSpy.Annotations
{
    public class Annotation : IAnnotation
    {
        public Annotation(ILogger<Annotation> logger)
            => _logger = logger;

        private readonly StringBuilder _message = new StringBuilder();
        private readonly List<object> _parameters = new List<object>();
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
            _logger.LogInformation(_message.ToString(), _parameters.ToArray());
        }
    }
}