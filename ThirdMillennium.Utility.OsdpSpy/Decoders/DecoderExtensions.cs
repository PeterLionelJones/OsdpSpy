using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ThirdMillennium.Protocol;

namespace ThirdMillennium.Utility.OSDP
{
    public static class DecoderExtensions
    {
        public static IEnumerable<T> GetImplementationsOf<T>(this IServiceProvider provider)
        {
            return Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(T).IsAssignableFrom(t) && t.IsClass)
                .Select(provider.GetService)
                .Cast<T>();
        }

        public static string ToKeyString(this byte key)
        {
            return key switch
            {
                0x0D => "#",
                0x41 => "F1",
                0x42 => "F2",
                0x43 => "F3",
                0x44 => "F4",
                0x45 => "F1 + F2",
                0x46 => "F2 + F3",
                0x47 => "F3 + F4",
                0x48 => "F1 + F4",
                0x7F => "*",
                _ => $"{(char) key}"
            };
        }

        public static string ToFormatString(this byte format)
        {
            return format switch
            {
                0x00 => "Not Specified, Raw Bit Array",
                0x01 => "P/DATA/P, Wiegand",
                _ => "Unknown"
            };
        }

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

        public static string ToTemporaryControlCodeString(this byte code)
        {
            return code switch
            {
                0x00 => "NOP",
                0x01 => "Cancel",
                0x02 => "Set",
                _ => $"0x{code:X02} - Invalid Control Code"
            };
        }

        public static string ToPermanentControlCodeString(this byte code)
        {
            return code switch
            {
                0x00 => "NOP",
                0x01 => "Cancel",
                _ => $"0x{code:X02} - Invalid Control Code"
            };
        }

        public static string ToLedColorString(this byte color)
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

        public static int ToRawCardLength(this byte[] input)
        {
            return input[2] | (input[3] << 8);
        }

        public static byte[] ToRawCardData(this byte[] input)
        {
            var bits = input.ToRawCardLength();
            var bytes = 1 + (bits - 1) / 8;
            var data = new byte[bytes];
            Buffer.BlockCopy(input, 4, data, 0, bytes);
            return data;
        }

        public static string ToRawCardString(this byte[] input)
        {
            return $"[ {input.ToRawCardLength()} ] {input.ToRawCardData().ToHexString()}";
        }
    }
}