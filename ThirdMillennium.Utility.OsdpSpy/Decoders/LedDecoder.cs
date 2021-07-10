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
}