using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using ThirdMillennium.Annotations;

namespace ThirdMillennium.Utility.OSDP
{
    public class OctetAnnotator : IOctetAnnotator
    {
        public OctetAnnotator(ILogger<OctetAnnotator> logger)
            => _logger = logger;

        private readonly ILogger<OctetAnnotator> _logger;
        private readonly bool[] _octets = new bool[256];

        private void ProcessFrame(IFrameProduct frame)
        {
            var octets = frame?.Frame?.FrameData;
            
            if (octets != null)
            {
                foreach (var octet in octets)
                {
                    _octets[octet] = true;
                }
            }
        }
        
        public void Annotate(IExchange input, IAnnotation output)
        {
            if (input != null)
            {
                ProcessFrame(input.Acu);
                ProcessFrame(input.Pd);
            }
        }

        public void Summarise()
        {
            var covered = _octets.Count(x => x);
            var coverage = 100 * (double) covered / 256;

            var lines = new[]
            {
                new StringBuilder(), 
                new StringBuilder(), 
                new StringBuilder(), 
                new StringBuilder()
            };

            for (var i = 0; i < 64; ++i)
            {
                lines[0].Append(_octets[i] ? "1" : "0");
                lines[1].Append(_octets[64 + i] ? "1" : "0");
                lines[2].Append(_octets[128 + i] ? "1" : "0");
                lines[3].Append(_octets[192 + i] ? "1" : "0");
            }

            _logger.LogInformation(
                "Octet Coverage: {Covered}/256 [{Coverage:F2}%]\n\n" +
                    "     0..63:    {OctetMap1}\n" +
                    "   64..127:    {OctetMap2}\n" +
                    "  128..191:    {OctetMap3}\n" +
                    "  192..255:    {OctetMad4}\n",
                covered, 
                coverage,
                lines[0].ToString(),
                lines[1].ToString(),
                lines[2].ToString(),
                lines[3].ToString());
        }
    }
}