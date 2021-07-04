namespace ThirdMillennium.Utility.OSDP
{
    public interface IPayload
    {
        public byte[] Cipher { get; set; }
        public byte[] Plain { get; set; }
    }
}