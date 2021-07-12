using System;
using System.Collections.Generic;
using System.IO;
using ThirdMillennium.Annotations;
using ThirdMillennium.Protocol;

namespace ThirdMillennium.Utility.OSDP
{
    internal static class FileTransferAnnotatorExtensions
    {
        internal static IAnnotation AppendFile(
            this IAnnotation output, 
            byte[] file,
            bool truncate = true, 
            int truncatedLength = 1024)
        {
            return output.AppendFile(file.Divide(true, truncatedLength));
        }

        private static IAnnotation AppendFile(
            this IAnnotation output, 
            IReadOnlyList<byte[]> segments)
        {
            for (var i = 0; i < segments.Count; ++i)
            {
                output.AppendItem(
                    $"Offset_{i * 32:D06}", 
                    segments[i].ToHexString());
            }

            return output;
        }
        
        internal static string SaveFile(this byte[] data, string path = null)
        {
            // Name the file.
            var timeStamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var sep = Path.DirectorySeparatorChar;
            var trimmed = Path.TrimEndingDirectorySeparator(path ?? ".");
            var fileName = $"{trimmed}{sep}{timeStamp}.osdp";

            try
            {
                // Save the file.
                File.WriteAllBytes(fileName, data);
                return fileName;
            }
            catch (Exception e)
            {
                // Something wrong with the file path.
                Console.WriteLine(e);
                return null;
            }
        }

        private static List<byte[]> Divide(this byte[] input, bool truncate = true, int truncatedLength = 1024)
        {
            var data = truncate ? input.Truncate(truncatedLength) : input;
            
            const int BlockSize = 32;
            var segmentList = new List<byte[]>();
            var blocks = (data.Length + BlockSize - 1) / BlockSize;
            var remaining = data.Length;

            for (var i = 0; i < blocks; ++i)
            {
                var length = remaining < BlockSize ? remaining : BlockSize;
                var block = new byte[length];
                Buffer.BlockCopy(data, i * BlockSize, block, 0, length);
                segmentList.Add(block);
                remaining -= length;
            }

            return segmentList;
        }

        private static byte[] Truncate(this byte[] input, int truncatedLength = 1024)
        {
            var output = input;
            
            if (input.Length > truncatedLength)
            {
                output = new byte[truncatedLength];
                Buffer.BlockCopy(input, 0, output, 0, truncatedLength);
            }

            return output;
        }
    }
}