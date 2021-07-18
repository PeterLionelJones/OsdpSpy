using ThirdMillennium.Annotations;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.Utility.OSDP
{
    public class LedDecoder : ICommandDecoder
    {
        public Command Command => Command.LED;

        private const int RecordSize = 14;
        
        public void Decode(byte[] input, IAnnotation output)
        {
            var entries = input.GetEntryCount(RecordSize);

            if (entries == 0)
            {
                output.AppendItem(
                    "NonCompliance", 
                    "Invalid payload for osdp_LED command");
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
            output.AppendItem($"LedNumber{indexString}", input[offset + 1]);

            output.AppendItem(
                $"TemporaryControlCode{indexString}",
                input[offset + 2].ToTemporaryControlCodeString());
            
            output.AppendItem(
                $"TemporaryOnTime{indexString}", 
                input[offset + 3] / 10.0, "Seconds");
            
            output.AppendItem(
                $"TemporaryOffTime{indexString}", 
                input[offset + 4] / 10.0, "Seconds");
            
            output.AppendItem(
                $"TemporaryOnColor{indexString}", 
                input[offset + 5].ToLedColorString());
            
            output.AppendItem(
                $"TemporaryOffColor{indexString}", 
                input[offset + 6].ToLedColorString());
            
            output.AppendItem(
                $"Timer{indexString}", 
                (input[offset + 7] | (input[offset + 8] << 8)) / 10.0, "Seconds");
            
            output.AppendItem(
                $"PermanentControlCode{indexString}", 
                input[offset + 9].ToPermanentControlCodeString());
            
            output.AppendItem(
                $"PermanentOnTime{indexString}", 
                input[offset + 10] / 10.0, "Seconds");
            
            output.AppendItem(
                $"PermanentOffTime{indexString}", 
                input[offset + 11] / 10.0, "Seconds");
            
            output.AppendItem(
                $"PermanentOnColor{indexString}", 
                input[offset + 12].ToLedColorString());
            
            output.AppendItem(
                $"PermanentOffColor{indexString}", 
                input[offset + 13].ToLedColorString());
        }
    }

    internal static class LedDecoderExtensions
    {
        internal static string ToTemporaryControlCodeString(this byte code)
        {
            return code switch
            {
                0x00 => "NOP",
                0x01 => "Cancel",
                0x02 => "Set",
                _ => $"0x{code:X02} - Invalid Control Code"
            };
        }

        internal static string ToPermanentControlCodeString(this byte code)
        {
            return code switch
            {
                0x00 => "NOP",
                0x01 => "Set",
                _ => $"0x{code:X02} - Invalid Control Code"
            };
        }

        internal static string ToLedColorString(this byte color)
        {
            return color switch
            {
                0x00 => "Black or Off",
                0x01 => "Red",
                0x02 => "Green",
                0x03 => "Amber",
                0x04 => "Blue",
                0x05 => "Magenta",
                0x06 => "Cyan",
                0x07 => "White",
                _ => $"0x{color:X02} - Reserved for Future Use"
            };
        }
    }
}