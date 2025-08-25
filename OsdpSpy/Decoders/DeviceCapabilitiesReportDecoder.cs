using OsdpSpy.Annotations;
using OsdpSpy.Osdp;

namespace OsdpSpy.Decoders;

public class DeviceCapabilitiesReportDecoder : IReplyDecoder
{
    public Reply Reply => Reply.PDCAP;

    public void Decode(byte[] input, IAnnotation output)
    {
        var capabilities = input.GetCapabilities();

        foreach (var capability in capabilities)
        {
            output.Append(capability);
        }
    }
}

internal enum FunctionCode
{
    ContactStatusMonitoring = 1,
    OutputControl = 2,
    CardDataFormat = 3,
    ReaderLedControl = 4,
    ReaderAudibleOutput = 5,
    ReaderTextOutput = 6,
    TimeKeeping= 7,
    CheckCharacterSupport = 8,
    CommunicationSecurity = 9,
    ReceiveBufferSize = 10,
    LargestCombinedMessageSize = 11,
    SmartCardSupport = 12,
    Readers = 13,
    Biometrics = 14,
    SecurePinEntry = 15,
    OsdpVersion = 16,
    ExtendedId = 32  // TODO:Awaiting designation by OSDP WG.
}

internal class DeviceCapability(FunctionCode function, byte compliance, byte numberOf)
{
    public FunctionCode FunctionCode { get; } = function;
    public byte Compliance { get; } = compliance;
    public byte NumberOf { get; } = numberOf;
}

internal static class DeviceCapabilitiesReportDecoderExtensions
{
    private const int RecordSize = 3;
        
    internal static DeviceCapability[] GetCapabilities(this byte[] input)
    {
        var entries = input.GetEntryCount(RecordSize);
        var capability = new DeviceCapability[entries];
            
        for (var i = 0; i < entries; ++i)
        {
            var offset = i * RecordSize;
                
            capability[i] = new DeviceCapability(
                (FunctionCode) input[offset], 
                input[offset + 1], 
                input[offset + 2]);
        }

        return capability;
    }

    internal static IAnnotation Append(this IAnnotation output, DeviceCapability capability)
    {
        return capability.FunctionCode switch
        {
            FunctionCode.ContactStatusMonitoring => output.AppendContactStatusMonitoring(capability),
            FunctionCode.OutputControl => output.AppendOutputControl(capability),
            FunctionCode.CardDataFormat => output.AppendCardDataFormat(capability),
            FunctionCode.ReaderLedControl => output.AppendReaderLedControl(capability),
            FunctionCode.ReaderAudibleOutput => output.AppendReaderAudibleOutput(capability),
            FunctionCode.ReaderTextOutput => output.AppendReaderTextOutput(capability),
            FunctionCode.TimeKeeping => output.AppendTimeKeeping(capability),
            FunctionCode.CheckCharacterSupport => output.AppendCheckCharacterSupport(capability),
            FunctionCode.CommunicationSecurity => output.AppendCommunicationSecurity(capability),
            FunctionCode.ReceiveBufferSize => output.AppendReceiveBufferSize(capability),
            FunctionCode.LargestCombinedMessageSize => output.AppendLargestCombinedMessageSize(capability),
            FunctionCode.SmartCardSupport => output.AppendSmartCardSupport(capability),
            FunctionCode.Readers => output.AppendReaders(capability),
            FunctionCode.Biometrics => output.AppendBiometrics(capability),
            FunctionCode.SecurePinEntry => output.AppendSecurePinEntry(capability),
            FunctionCode.OsdpVersion => output.AppendOsdpVersion(capability),
            FunctionCode.ExtendedId => output.AppendExtendedId(capability),
            _ => output.AppendUnknownFunctionCode(capability)
        };
    }

    private static IAnnotation AppendExtendedId(
        this IAnnotation output, 
        DeviceCapability capability)
    {
        return output
            .AppendItem(
                "ExtendedId", 
                capability.Compliance.ToExtendedIdString());
    }

    private static string ToExtendedIdString(this byte compliance)
    {
        return compliance switch
        {
            0x00 => "00 - Extended Id Not Supported",
            0x01 => "01 - Supports Extended ID",
            _ => $"{compliance:X02} - Unspecified Compliance Level"
        };
    }

    private static IAnnotation AppendOsdpVersion(
        this IAnnotation output,
        DeviceCapability capability)
    {
        return output.AppendItem(
            "OsdpVersion", 
            capability.Compliance.ToOsdpComplianceString());
    }
        
    private static string ToOsdpComplianceString(this byte compliance)
    {
        return compliance switch
        {
            0x00 => "00 - Unspecified",
            0x01 => "01 - IEC 60839-11-5",
            0x02 => "02 - SIA OSDP 2.2",
            _ => compliance < 0x80 
                ? $"{compliance:X02} - Reserved for Future Use" 
                : $"{compliance:X02} - Reserved for Private Use"
        };
    }

    private static IAnnotation AppendSecurePinEntry(
        this IAnnotation output, 
        DeviceCapability capability)
    {
        return output.AppendItem(
            "SecurePinEntry", 
            capability.Compliance.ToSecurePinEntryString());
    }

    private static string ToSecurePinEntryString(this byte compliance)
    {
        return compliance switch
        {
            0 => "00 - Does Not Support Secure PIN Entry",
            1 => "01 - Supports Secure PIN Entry",
            _ => $"{compliance:X02} - Unspecified Compliance Level"
        };
    }

    private static IAnnotation AppendBiometrics(
        this IAnnotation output, 
        DeviceCapability capability)
    {
        return output.AppendItem(
            "Biometrics", 
            capability.Compliance.ToBiometricString());
    }

    private static string ToBiometricString(this byte compliance)
    {
        return compliance switch
        {
            0 => "00 - No Biometric",
            1 => "01 - Fingerprint, 1 Template",
            2 => "02 - Fingerprint, 2 Templates",
            3 => "03 - Iris, 1 Template",
            _ => $"{compliance:X02} - Unspecified Compliance Level"
        };
    }

    private static IAnnotation AppendReaders(
        this IAnnotation output, 
        DeviceCapability capability)
    {
        output.AppendItem("DownstreamReaderCount", capability.NumberOf);

        if (capability.Compliance != 0)
        {
            output.AppendItem("InvalidReaderCompliance", true);
        }
            
        return output;
    }

    private static IAnnotation AppendSmartCardSupport(
        this IAnnotation output, 
        DeviceCapability capability)
    {
        var supportsTransparentMode = (capability.Compliance & 0x01) == 0x01;
        var supportsExtendedPacketMode = (capability.Compliance & 0x02) == 0x02;

        return output
            .AppendItem("SupportsTransparentMode", supportsTransparentMode)
            .AppendItem("SupportsExtendedPacketMode", supportsExtendedPacketMode);
    }

    private static IAnnotation AppendLargestCombinedMessageSize(
        this IAnnotation output, 
        DeviceCapability capability)
    {
        var size = capability.Compliance | (capability.NumberOf << 8);
        return output.AppendItem("LargestCombinedMessageSize", size);
    }

    private static IAnnotation AppendReceiveBufferSize(
        this IAnnotation output, 
        DeviceCapability capability)
    {
        var size = capability.Compliance | (capability.NumberOf << 8);
        return output.AppendItem("ReceiveBufferSize", size);
    }

    private static IAnnotation AppendCommunicationSecurity(
        this IAnnotation output, 
        DeviceCapability capability)
    {
        var supportsAes128 = (capability.Compliance & 0x01) == 0x01;
        var supportsDefaultScbk = (capability.NumberOf & 0x01) == 0x01;

        var invalidCompliance = 
            (capability.Compliance & 0xFE) + (capability.NumberOf & 0xFE) > 0; 

        output
            .AppendItem("SupportsAes128", supportsAes128)
            .AppendItem("SupportsDefaultScbk", supportsDefaultScbk);

        if (invalidCompliance)
        {
            output.AppendItem("InvalidCommunication", true);
        }
            
        return output;
    }

    private static IAnnotation AppendCheckCharacterSupport(
        this IAnnotation output, 
        DeviceCapability capability)
    {
        return output.AppendItem(
            "CheckCharacterSupport", 
            capability.Compliance.ToCheckCharacterString());
    }

    private static string ToCheckCharacterString(this byte compliance)
    {
        return compliance switch
        {
            0 => "00 - Does Not Support CRC_16, Only Checksum Mode",
            1 => "01 - Supports CRC-16 Mode",
            _ => $"{compliance:X02} - Unspecified Compliance Level"
        };
    }

    private static IAnnotation AppendTimeKeeping(
        this IAnnotation output, 
        DeviceCapability capability)
    {
        return output.AppendItem(
            "TimeKeeping", 
            capability.Compliance.ToTimeKeepingString());
    }

    private static string ToTimeKeepingString(this byte compliance)
    {
        return compliance switch
        {
            0x00 => "00 - Does Not Date/Time Functionality",
            _ => $"{compliance:X02} - Reserved for Future Use"
        };
    }
        
    private static IAnnotation AppendReaderTextOutput(
        this IAnnotation output, 
        DeviceCapability capability)
    {
        return output.AppendItem(
            "TimeKeeping", 
            capability.Compliance.ToReaderTextString());
    }

    private static string ToReaderTextString(this byte compliance)
    {
        return compliance switch
        {
            0x00 => "00 - No Text Display",
            0x01 => "01 - 1 Row of 16 Characters",
            0x02 => "02 - 2 Rows of 16 Characters",
            0x03 => "03 - 4 Rows of 16 Characters",
            _ => compliance < 0x80 
                ? $"{compliance:X02} - Reserved for Future Use" 
                : $"{compliance:X02} - Reserved for Private Use"
        };
    }
        
    private static IAnnotation AppendReaderAudibleOutput(
        this IAnnotation output, 
        DeviceCapability capability)
    {
        return output.AppendItem(
            "ReaderAudibleOutput", 
            capability.Compliance.ToReaderAudibleOutputString());
    }

    private static string ToReaderAudibleOutputString(this byte compliance)
    {
        return compliance switch
        {
            0x01 => "01 - Supports On/Off Commands Only",
            0x02 => "02 - Supports Timed Commands",
            _ => $"{compliance:X02} - Unspecified Compliance Level"
        };
    }
        
    private static IAnnotation AppendReaderLedControl(
        this IAnnotation output, 
        DeviceCapability capability)
    {
        return output
            .AppendItem(
                "ReaderLedControl", 
                capability.Compliance.ToReaderLedControlString())
            .AppendItem(
                "LedCount", 
                capability.NumberOf);
    }

    private static string ToReaderLedControlString(this byte compliance)
    {
        return compliance switch
        {
            0x01 => "01 - Supports On/Off Commands Only",
            0x02 => "02 - Supports Timed Commands",
            0x03 => "03 - Supports Bi-Color LEDs",
            0x04 => "04 - Supports Tri-Color LEDs",
            _ => $"{compliance:X02} - Unspecified Compliance Level"
        };
    }

    private static IAnnotation AppendCardDataFormat(
        this IAnnotation output, 
        DeviceCapability capability)
    {
        return output.AppendItem(
            "CardDataFormat", 
            capability.Compliance.ToCardDataFormatString());
    }

    private static string ToCardDataFormatString(this byte compliance)
    {
        return compliance switch
        {
            0x01 => "01 - Sends Card Data as an Array of Bits, Not Exceeding 1024",
            0x02 => "02 - Sends Card Data as an Array of BCD Characters, Not Exceeding 256",
            0x03 => "03 - Sends Card Data as Either an Array of Bits or BCD Characters",
            _ => $"{compliance:X02} - Unspecified Compliance Level"
        };
    }

    private static IAnnotation AppendOutputControl(
        this IAnnotation output, 
        DeviceCapability capability)
    {
        return output
            .AppendItem(
                "OutputControl", 
                capability.Compliance.ToOutputControlString())
            .AppendItem(
                "OutputCount", 
                capability.NumberOf);
    }

    private static string ToOutputControlString(this byte compliance)
    {
        return compliance switch
        {
            0x01 => "01 - Support Direct Control Only",
            0x02 => "02 - Supports Direct Control With Active State",
            0x03 => "03 - Supports Timed Commands",
            0x04 => "04 - Supports Normal or Inverted Drive, and Timed Commands",
            _ => $"{compliance:X02} - Unspecified Compliance Level"
        };
    }

    private static IAnnotation AppendContactStatusMonitoring(
        this IAnnotation output, 
        DeviceCapability capability)
    {
        return output
            .AppendItem(
                "ContactStatusMonitoring", 
                capability.Compliance.ToContactStatusString())
            .AppendItem(
                "InputCount", 
                capability.NumberOf);
    }

    private static string ToContactStatusString(this byte compliance)
    {
        return compliance switch
        {
            0x01 => "01 - Monitors State Without Supervision",
            0x02 => "02 - Allows Configuration of Active/Inactive States",
            0x03 => "03 - Supports Supervised Monitoring",
            0x04 => "04 - Supports Custom End-Of-Line Settings",
            _ => $"{compliance:X02} - Unspecified Compliance Level"
        };
    }

    private static IAnnotation AppendUnknownFunctionCode(
        this IAnnotation output, 
        DeviceCapability capability)
    {
        return output
            .AppendItem(
                $"UnknownFunction{capability.FunctionCode}", 
                capability.ToUnknownString());
    }

    private static string ToUnknownString(this DeviceCapability capability) 
        => $"Function Code = {capability.FunctionCode}, Compliance = {capability.Compliance:X02}, Count = {capability.NumberOf:X02}";
}