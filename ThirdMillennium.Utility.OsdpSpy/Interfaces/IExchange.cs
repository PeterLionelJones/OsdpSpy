namespace ThirdMillennium.Utility.OSDP
{
    public interface IExchange
    {
        long Sequence { get; }
        IFrameProduct Acu { get; }
        IFrameProduct Pd { get; }
        void AddReceived(IFrameProduct rx);
    }
}