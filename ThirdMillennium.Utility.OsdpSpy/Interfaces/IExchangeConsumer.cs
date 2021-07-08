using System.Runtime.InteropServices.ComTypes;

namespace ThirdMillennium.Utility.OSDP
{
    public interface IExchangeConsumer
    {
        void Summarise();
        void Subscribe(IExchangeProducer input);
        void Unsubscribe();
    }
}