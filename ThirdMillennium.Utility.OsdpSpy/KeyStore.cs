using System.Collections.Generic;
using System.Linq;

namespace ThirdMillennium.Utility.OSDP
{
    public class KeyStore : IKeyStore
    {
        private readonly List<KeyItem> _keys = new List<KeyItem>();
        
        public byte[] DefaultBaseKey => new byte[]
        {
            0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37,
            0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F
        };
        
        public void Store(byte[] uid, byte[] key)
        {
            var item = _keys.FirstOrDefault(x => x.Uid.SequenceEqual(uid));

            if (item == null)
            {
                _keys.Add(new KeyItem {Uid = uid, Key = key});
            }
            else
            {
                item.Key = key;
            }
        }

        public byte[] Find(byte[] uid)
        {
            var item = _keys.FirstOrDefault(x => x.Uid.SequenceEqual(uid));
            return item?.Key;
        }
    }
}