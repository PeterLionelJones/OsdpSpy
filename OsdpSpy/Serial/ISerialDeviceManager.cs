using System;
using System.Collections.Generic;

namespace OsdpSpy.Serial;

/// <summary>
///     The ISerialDeviceManager interface provides an interface to retrieve a list of serial
///     devices connected to the machine and monitor their addition and removal from the system.
/// </summary>
public interface ISerialDeviceManager
{
    /// <summary>
    ///     The DeviceListChangedHandler is used to notify the client object that the device
    ///     list has changed when a device is either plugged in or removed from the system.
    /// </summary>
    EventHandler<EventArgs> DeviceListChangedHandler { get; set; }
        
    /// <summary>
    ///     The DevicesAddedHandler is used to notify the client which devices have been added
    ///     to the system. 
    /// </summary>
    EventHandler<DeviceListChangedEventArgs> DevicesAddedHandler { get; set; }
        
    /// <summary>
    ///     The DevicesRemovedHandler is used to notify the client which devices have been
    ///     removed to the system. 
    /// </summary>
    EventHandler<DeviceListChangedEventArgs> DevicesRemovedHandler { get; set; }

    /// <summary>
    ///     The Devices property contains the list of devices connected to the system. The
    ///     actual object used is implementation dependent.
    /// </summary>
    List<ISerialDeviceItem> Devices { get; }

    /// <summary>
    ///     The FromPortName method returns the ISerialDeviceItem that corresponds to the
    ///     specified port name. 
    /// </summary>
    /// <param name="name">
    ///     A string containing the name of the port to be translated to an active serial device
    ///     in the system.
    /// </param>
    /// <returns>
    ///     Returns the ISerialDeviceItem corresponding to the port name passed to the method.
    /// </returns>
    ISerialDeviceItem FromPortName(string name);

    /// <summary>
    ///     The CreateChannel method creates an instance of the IChannel corresponding to the
    ///     underlying computer hardware managed by this class.
    /// </summary>
    /// <returns>
    ///     Returns a new instance of IChannel for this implementation.
    /// </returns>
    IChannel CreateChannel();
}
    
/// <summary>
///     The DeviceListChangedEventArgs class is used to pass device changes to a client
///     ISerialDeviceManager object. A list of ISerialDeviceItems is passed to the client
///     object.
/// </summary>
public class DeviceListChangedEventArgs : EventArgs
{
    public DeviceListChangedEventArgs(List<ISerialDeviceItem> devices)
    {
        Devices = devices;
    }
    public List<ISerialDeviceItem> Devices { get; }
}