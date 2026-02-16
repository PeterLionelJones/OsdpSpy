namespace OsdpSpy.Serial;

/// <summary>
///     The ISerialDeviceItem interface is used to abstract a platform-specific serial device.
///     This may be used to open a channel, for examples. You would use the corresponding
///     ISerialDeviceManager implementation to obtain a list of serial ports on a system. 
/// </summary>
public interface ISerialDeviceItem
{
    /// <summary>
    ///     The PortName property contains the platform specific name of the serial
    ///     communications port.
    /// </summary>
    string PortName { get; }
        
    /// <summary>
    ///     The Device property holds the underlying object used to communicate with the serial
    ///     device. 
    /// </summary>
    object Device { get; }
}