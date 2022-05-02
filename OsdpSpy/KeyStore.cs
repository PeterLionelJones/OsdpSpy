using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using McMaster.Extensions.CommandLineUtils;

namespace OsdpSpy
{
    public class KeyStore : IKeyStore
    {
        public KeyStore(IConsole console)
        {
            _console = console;
            
            Load();            
        }

        private readonly IConsole _console;
        private List<KeyItem> _keys = new List<KeyItem>();
        
        public byte[] DefaultBaseKey => new byte[]
        {
            0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37,
            0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F
        };
        
        private string SettingsFileName 
            => Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData), 
                "osdpspyKeyStore.json");

        private void Load()
        {
            if (File.Exists(SettingsFileName))
            {
                var json = File.ReadAllText(SettingsFileName);
                _keys = JsonSerializer.Deserialize<List<KeyItem>>(json);
            }
        }

        private void Save()
        {
            var json = JsonSerializer.Serialize(_keys);
            File.WriteAllText(SettingsFileName, json);
        }
        
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

            Save();
        }

        public byte[] Find(byte[] uid)
        {
            var item = _keys.FirstOrDefault(x => x.Uid.SequenceEqual(uid));
            return item?.Key;
        }

        public void List()
        {
            _console.WriteLine("Available Secure Channel Base Keys:");
            foreach (var key in _keys)
            {
                var uidString = BitConverter.ToString(key.Uid).Replace('-', ' ');
                var keyString = BitConverter.ToString(key.Key).Replace('-', ' ');
                _console.WriteLine($"  {uidString} -> {keyString}");
            }
            _console.WriteLine();
        }
    }
}