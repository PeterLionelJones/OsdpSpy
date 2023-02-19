using OsdpSpy.Extensions;

namespace OsdpSpy
{
    internal class KeyItem
    {
        public byte[] Uid { get; init; }
        public byte[] Key { get; set; }

        public override string ToString()
            => $"{Uid.ToHexString()} -> {Key.ToHexString()}";
    }
}