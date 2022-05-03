using OsdpSpy.Annotations;
using OsdpSpy.Osdp;

namespace OsdpSpy.Decoders
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

            var reader = input[offset];
            var toneCode = input[offset+1];
            var on = input[offset + 2];
            var off = input[offset + 3];
            var count = input[offset + 4];
            
            output
                .AppendItem($"ReaderNumber{indexString}", reader)
                .AppendItem($"ToneCode{indexString}", toneCode.ToToneCodeString())
                .AppendItem($"OnTime{indexString}", on / 10.0, "Seconds")
                .AppendItem($"OffTime{indexString}", off / 10.0, "Seconds")
                .AppendItem($"Count{indexString}", count);

            if (on == 0 && toneCode != 1)
            {
                output.AppendItem(
                    "NonCompliance", 
                    "The On Time must be non-zero unless the tone code is 0x01");
            }
        }
    }

    internal static class BuzzerDecoderExtensions
    {
        public static string ToToneCodeString(this byte toneCode)
        {
            return toneCode switch
            {
                0x00 => "No Tone (Off), Deprecated",
                0x01 => "Off",
                0x02 => "Default Tone",
                _ => $"0x{toneCode:X02} - Reserved for Future Use"
            };
        }

        public static int GetEntryCount(this byte[] input, int recordSize)
        {
            if (input.Length < recordSize) return 0;
            if (input.Length % recordSize == 0) return input.Length / recordSize;
            if (input.Length % 16 != 0) return 0;

            for (var count = 1;; ++count)
            {
                var offset = count * recordSize;
                if (offset >= input.Length) return 0;
                if (input[offset] == 0x80) return count;
            }
        }
    }
}