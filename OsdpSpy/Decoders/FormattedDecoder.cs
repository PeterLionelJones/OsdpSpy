using System.Text;
using OsdpSpy.Annotations;
using OsdpSpy.Osdp;

namespace OsdpSpy.Decoders;

public class FormattedDecoder : IReplyDecoder
{
    public Reply Reply => Reply.FMT;

    public void Decode(byte[] input, IAnnotation output)
    {
        output
            .AppendItem("ReaderNumber", input[0])
            .AppendItem("ReadDirection", input[1].ToReadDirectionString())
            .AppendItem("CharacterCount", input[2])
            .AppendItem("Data", input.ToCardDataString());
    }
}

internal static class FormattedDecoderExtensions
{
    internal static string ToReadDirectionString(this byte direction)
    {
        return direction switch
        {
            0 => "0 - Forward Read",
            1 => "1 - Reverse Read",
            _ => $"{direction} - Unspecified"
        };
    }

    internal static string ToCardDataString(this byte[] input)
    {
        var count = input[2];
        var builder = new StringBuilder();
        for (var i = 0; i < count; ++i)
        {
            builder.Append((char) input[3 + i]);
        }
        return builder.ToString();
    }
}