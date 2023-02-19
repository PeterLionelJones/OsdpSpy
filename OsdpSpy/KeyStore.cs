using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using McMaster.Extensions.CommandLineUtils;
using OsdpSpy.Abstractions;

[assembly:InternalsVisibleTo("OsdpSpy.Tests")]

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
        private List<KeyItem> _keyItems = new List<KeyItem>();
        
        public byte[] DefaultBaseKey => new byte[]
        {
            0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37,
            0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F
        };
        
        private string SettingsFileName 
            => Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData), 
                "osdpspyKeyStore.json");

        internal List<KeyItem> KeyItemList => _keyItems;
        public void Clear()
        {
            _keyItems = new List<KeyItem>();
            Save();
        }

        private void Load()
        {
            if (File.Exists(SettingsFileName))
            {
                var json = File.ReadAllText(SettingsFileName);
                _keyItems = JsonSerializer.Deserialize<List<KeyItem>>(json);
            }
        }

        private void Save()
        {
            var json = JsonSerializer.Serialize(_keyItems);
            File.WriteAllText(SettingsFileName, json);
        }
        
        public void Store(byte[] uid, byte[] key)
        {
            var item = _keyItems.FirstOrDefault(x => x.Uid.SequenceEqual(uid));

            if (item == null)
            {
                _keyItems.Add(new KeyItem {Uid = uid, Key = key});
            }
            else
            {
                item.Key = key;
            }

            Save();
        }

        public byte[] Find(byte[] uid)
        {
            var item = _keyItems.FirstOrDefault(x => x.Uid.SequenceEqual(uid));
            return item?.Key;
        }

        public void List()
        {
            _console.WriteLine("Available Secure Channel Base Keys:");
            foreach (var keyItem in _keyItems)
            {
                _console.WriteLine($"  {keyItem}");
            }
            _console.WriteLine();
        }
    }
}