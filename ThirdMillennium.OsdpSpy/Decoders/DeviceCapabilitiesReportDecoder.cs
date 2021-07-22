using ThirdMillennium.Annotations;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.OsdpSpy
{
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

    internal class DeviceCapability
    {
        public DeviceCapability(byte function, byte compliance, byte numberOf)
        {
            FunctionCode = function;
            Compliance = compliance;
            NumberOf = numberOf;
        }
        
        public byte FunctionCode { get; }
        public byte Compliance { get; }
        public byte NumberOf { get; }
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
                    input[offset], 
                    input[offset + 1], 
                    input[offset + 2]);
            }

            return capability;
        }

        internal static IAnnotation Append(this IAnnotation output, DeviceCapability capability)
        {
            return capability.FunctionCode switch
            {
                1 => output.AppendContactStatusMonitoring(capability),
                2 => output.AppendOutputControl(capability),
                3 => output.AppendCardDataFormat(capability),
                4 => output.AppendReaderLedControl(capability),
                5 => output.AppendReaderAudibleOutput(capability),
                6 => output.AppendReaderTextOutput(capability),
                7 => output.AppendTimeKeeping(capability),
                8 => output.AppendCheckCharacterSupport(capability),
                9 => output.AppendCommunicationSecurity(capability),
                10 => output.AppendReceiveBufferSize(capability),
                11 => output.AppendLargestCombinedMessageSize(capability),
                12 => output.AppendSmartCardSupport(capability),
                13 => output.AppendReaders(capability),
                14 => output.AppendBiometrics(capability),
                15 => output.AppendSecurePinEntry(capability),
                16 => output.AppendOsdpVersion(capability),
                _ => output.AppendUnknownFunctionCode(capability)
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
                    $"Function{capability.FunctionCode:X02}", 
                    "Unspecified Function Code")
                .AppendItem(
                    $"Compliance{capability.FunctionCode:X02}", 
                    $"{capability.Compliance:X02}")
                .AppendItem(
                    $"NumberOf{capability.FunctionCode:X02}", 
                    $"{capability.NumberOf:X02}");
        }
    }
}