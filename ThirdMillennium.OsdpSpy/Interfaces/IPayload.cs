namespace ThirdMillennium.OsdpSpy
{
    public interface IPayload
    {
        public byte[] Cipher { get; set; }
        public byte[] Plain { get; set; }
    }
}