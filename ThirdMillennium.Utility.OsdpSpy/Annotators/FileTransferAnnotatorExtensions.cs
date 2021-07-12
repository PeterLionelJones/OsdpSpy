using System;
using System.Collections.Generic;
using System.IO;
using ThirdMillennium.Annotations;
using ThirdMillennium.Protocol;

namespace ThirdMillennium.Utility.OSDP
{
    internal static class FileTransferAnnotatorExtensions
    {
        internal static IAnnotation AppendFile(this IAnnotation output, byte[] file)
            => output.AppendFile(file.Divide());

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

        private static List<byte[]> Divide(this byte[] data)
        {
            const int BlockSize = 32;
            var segmentList = new List<byte[]>();
            var blocks = 1 + (data.Length - 1) / BlockSize;
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
    }
}