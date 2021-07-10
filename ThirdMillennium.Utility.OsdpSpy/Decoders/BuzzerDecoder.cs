using ThirdMillennium.Annotations;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.Utility.OSDP
{
    public class BuzzerDecoder : ICommandDecoder
    {
        public Command Command => Command.BUZ;
        
        private const int RecordSize = 5;

        public void Decode(byte[] input, IAnnotation output)
        {
            var entries = input.GetEntryCount(RecordSize);

            if (entries == 0)
            {
                output.AppendItem(
                    "NonCompliance", 
                    "Invalid payload for osdp_BUZ command");
            }

            for (var i = 0; i < entries; ++i)
            {
                DecodeEntry(i, input, output);
            }
        }

        private static void DecodeEntry(int index, byte[] input, IAnnotation output)
        {
            var indexString = index == 0 ? "" : $"{index + 1}";
            var offset = index * RecordSize;
            
            output.AppendItem($"ReaderNumber{indexString}", input[offset]);
            output.AppendItem($"ToneCode{indexString}", input[offset + 1].ToToneCodeString());
            output.AppendItem($"OnTime{indexString}", input[offset + 2] / 10.0, "Seconds");
            output.AppendItem($"OffTime{indexString}", input[offset + 3] / 10.0, "Seconds");
            output.AppendItem($"Count{indexString}", input[offset + 4]);
        }
    }
}