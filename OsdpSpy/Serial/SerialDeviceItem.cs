using System.IO.Ports;

namespace OsdpSpy.Serial;

/// <summary>
///     The SerialDeviceItem class provides the WinForms device implementation for a serial
///     channel.
/// </summary>
internal class SerialDeviceItem : SerialDeviceItem<SerialPort>
{
    /// <summary>
    ///     This constructor instantiates the SerialDeviceItem for access to the .NET Core
    ///     implementation of SerialPort.
    /// </summary>
    /// <param name="name">
    ///     A string containing the name of the serial port as identified by the computer
    ///     system.
    /// </param>
    /// <param name="device">
    ///     The platform specific object used to access the serial hardware.
    /// </param>
    public SerialDeviceItem(string name, SerialPort device) : base(name, device) { }
}

public class SerialDeviceItem<T> : ISerialDeviceItem
{
    /// <summary>
    ///     This constructor sets up the name and the underlying device object for this device
    ///     item.
    /// </summary>
    /// <param name="name">
    ///     The platform-specific name of the port to be instantiated.
    /// </param>
    /// <param name="device">
    ///     The underlying object used to communicate with the platform-specific device.
    /// </param>
    protected SerialDeviceItem(string name, T device)
    {
        PortName = name;
        SerialPort = device;
    }

    /// <summary>
    ///     The PortName property contains the name of the platform-specific name of serial
    ///     communications device.
    /// </summary>
    public string PortName { get; }

    /// <summary>
    ///     The SerialPort is the underlying communications device used to talk to a channel.
    /// </summary>
    public T SerialPort { get; set; }

    /// <summary>
    ///     The Device property returns the underlying device object.
    /// </summary>
    public object Device => SerialPort;

    /// <summary>
    ///     The ToString override returns the port name for this device.
    /// </summary>
    /// <returns>
    ///     The name of this device.
    /// </returns>
    public override string ToString()
    {
        return PortName;
    }
}