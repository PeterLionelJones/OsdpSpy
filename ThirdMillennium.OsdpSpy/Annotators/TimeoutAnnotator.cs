using System;
using ThirdMillennium.Annotations;

namespace ThirdMillennium.OsdpSpy
{
    public class TimeoutAnnotator : AlertingAnnotator<IExchange>
    {
        public TimeoutAnnotator(IFactory<IAnnotation> factory) : base(factory) {}

        private readonly DateTime _start = DateTime.UtcNow;
        private DateTime _lastReport = DateTime.UtcNow;
        private long _total;
        private long _timeouts;
        
        public double TimeoutRate => _total == 0 ? 0 : _timeouts * 100.00 / _total;
        public long TimeoutsPerHour => (long) (_timeouts / (DateTime.UtcNow - _start).TotalHours);

        public override void Annotate(IExchange input, IAnnotation output)
        {
            ++_total;

            if (input.Pd == null)
            {
                ++_timeouts;
                TimeoutAlert();
            }

            if (DateTime.UtcNow - _lastReport > TimeSpan.FromMinutes(15))
            {
                _lastReport = DateTime.UtcNow;
                TimeoutAlert();
            }
        }

        private void TimeoutAlert()
        {
            this.CreateOsdpAlert("Timeout Update")
                .AppendItem("Timeouts", _timeouts)
                .AppendItem("TotalExchanges", _total)
                .AppendItem("TimeoutRate", TimeoutRate, "%")
                .AppendItem("TimeoutsPerHour", TimeoutsPerHour)
                .AndLogTo(this);
        }
    }
}