namespace ThirdMillennium.Utility.OSDP
{
    public class Payload : IPayload
    {
        public byte[] Cipher { get; set; }
        public byte[] Plain { get; set; }
    }
}