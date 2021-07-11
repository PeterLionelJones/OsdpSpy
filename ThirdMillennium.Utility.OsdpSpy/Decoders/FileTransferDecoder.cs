using System;
using ThirdMillennium.Annotations;
using ThirdMillennium.Protocol;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.Utility.OSDP
{
    public class FileTransferDecoder : ICommandDecoder
    {
        public Command Command => Command.FILETRANSFER;

        public void Decode(byte[] input, IAnnotation output)
        {
            output
                .AppendItem("FileTransferType", input[0].ToFileTransferTypeString())
                .AppendItem("FileSize", input.ToFileSize())
                .AppendItem("FileOffset", input.ToFileOffset())
                .AppendItem("FragmentSize", input.ToFragmentSize())
                .AppendItem("Fragment", input.ToFragment().ToHexString());
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

        internal static int ToFileSize(this byte[] input)
        {
            return input[1] | (input[2] << 8) | (input[3] << 8) | (input[4] << 8);
        }

        internal static int ToFileOffset(this byte[] input)
        {
            return input[5] | (input[6] << 8) | (input[7] << 8) | (input[8] << 8);
        }

        internal static int ToFragmentSize(this byte[] input)
        {
            return input[9] | (input[10] << 8);
        }

        internal static byte[] ToFragment(this byte[] input)
        {
            var fragment = new byte[input.Length - 11];
            Buffer.BlockCopy(input, 11, fragment, 0, fragment.Length);
            return fragment;
        }
    }
}