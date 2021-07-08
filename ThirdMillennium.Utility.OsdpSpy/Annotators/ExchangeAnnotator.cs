using ThirdMillennium.Annotations;

namespace ThirdMillennium.Utility.OSDP
{
    public class ExchangeAnnotator : BaseAnnotator<IExchange>
    {
        protected ExchangeAnnotator(int priority = 99)
        {
            Priority = priority;
        }
        
        public int Priority { get; }
    }
}