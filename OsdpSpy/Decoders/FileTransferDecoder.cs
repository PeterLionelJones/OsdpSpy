using System;
using OsdpSpy.Annotations;
using OsdpSpy.Extensions;
using OsdpSpy.Osdp;

namespace OsdpSpy.Decoders
{
    public class FileTransferDecoder : ICommandDecoder
    {
        public Command Command => Command.FILETRANSFER;

        public void Decode(byte[] input, IAnnotation output)
        {
            output
                .AppendItem("FileTransferType", input[0].ToFileTransferTypeString())
                .AppendItem("FileSize", input.GetFileSize())
                .AppendItem("FileOffset", input.GetFileOffset())
                .AppendItem("FragmentSize", input.GetFragmentSize())
                .AppendItem("Fragment", input.GetFragment().ToHexString());
        }
    }

    internal static class FileTransferDecoderExtensions
    {
        internal static string ToFileTransferTypeString(this byte fileType)
        {
            return fileType switch
            {
                1 => "1 = Opaque File Contents",
                >=2 and <128 => $"{fileType} - Reserved For Future Use",
                >=128 => $"{fileType} - Reserved For Future Use",
                _ => $"{fileType} - Unspecified File Type"
            };
        }

        internal static int GetFileSize(this byte[] input)
        {
            return input[1] | (input[2] << 8) | (input[3] << 16) | (input[4] << 24);
        }

        internal static int GetFileOffset(this byte[] input)
        {
            return input[5] | (input[6] << 8) | (input[7] << 16) | (input[8] << 24);
        }

        internal static int GetFragmentSize(this byte[] input)
        {
            return input[9] | (input[10] << 8);
        }

        internal static byte[] GetFragment(this byte[] input)
        {
            var fragment = new byte[input.GetFragmentSize()];
            Buffer.BlockCopy(input, 11, fragment, 0, fragment.Length);
            return fragment;
        }
    }
}