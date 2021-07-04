using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace ThirdMillennium.Utility.OSDP
{
    public class FileFrameProducer : IFileFrameProducer
    {
        private void ProcessRecord(string record)
        {
            var raw = JsonConvert.DeserializeObject<RawTrace>(record);
            var product = raw.ToFrameProduct();
            if (product != null)
            {
                FrameHandler?.Invoke(this, product);
            }
        }
        
        public EventHandler<IFrameProduct> FrameHandler { get; set; }
        
        public bool Process(string filename)
        {
            try
            {
                // Make sure the file exists.
                if (!File.Exists(filename)) return false;

                // Process each line of the file.
                var sr = new StreamReader(filename);
                while (sr.Peek() >= 0)
                {
                    // Process the record.
                    ProcessRecord(sr.ReadLine());
                }

                // All of the records have been processed.
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }
    }
}