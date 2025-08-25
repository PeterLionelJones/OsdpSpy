using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Timers;

namespace OsdpSpy.Serial;

/// <summary>
///     The SerialDeviceManager class implements the ISerialDeviceManager interface and provides
///     and manages the list of available serial devices to the system.
/// </summary>
public class SerialDeviceManager : ISerialDeviceManager
{
    /// <summary>
    ///     The SerialDeviceManager constructor sets up a timer to periodically scan the COM
    ///     ports in the system and notify client objects of any changes.
    /// </summary>
    public SerialDeviceManager()
    {
        // Initialise the device list.
        Devices = new List<ISerialDeviceItem>();

        // Create the timer.
        Timer = new Timer();
        Timer.Elapsed += (o, e) => ScanDevices();
        Timer.Interval = 200;

        // Scan the devices for the first time.
        ScanDevices();
    }

    /// <summary>
    ///     The SerialDeviceManager destructor stops the timer and disposes of it.
    /// </summary>
    ~SerialDeviceManager()
    {
        Timer.Stop();
        Timer.Dispose();
        Timer = null;
    }
        
    /// <summary>
    ///     The ScanDevices method is called periodically to find out if any serial ports have
    ///     been added to or removed from the system.
    /// </summary>
    private void ScanDevices()
    {
        // Stop the timer.
        Timer.Stop();

        // Get the current list of COM ports.
        var ports = SerialPort.GetPortNames();

        var changed = ProcessAdded(ports);

        changed = changed || ProcessRemoved(ports);

        // Have we modified the device list?
        if (changed)
        {
            // Fire the handler.
            DeviceListChangedHandler?.Invoke(this, EventArgs.Empty);
        }

        // Restart the timer.
        Timer.Start();
    }

    /// <summary>
    ///     The ProcessAdded method determines from the latest port list if any ports have been
    ///     added. 
    /// </summary>
    /// <param name="ports">
    ///     The latest list of serial ports connected to the system.
    /// </param>
    private bool ProcessAdded(string[] ports)
    {
        // Assume the worst.
        var result = false;

        // Scan the current port list.
        foreach(var p in ports)
        {
            // Filter out any "rubbish" port names on Unix systems. We only support USB serial
            // ports on MacOS and Linux.
            if (  p.StartsWith("/dev/") && 
                  !p.StartsWith("/dev/tty.usbserial") && 
                  !p.StartsWith("/dev/ttyUSB")) continue;
                
            // The found flag is set when the specified device is found in the 
            // list.
            var found = false;

            // Look for the current port in the device list.
            foreach (var d in Devices)
            {
                // Check the next device.
                var dev = d as SerialDeviceItem;
                if (p == dev.PortName)
                {
                    // This port is in the list!
                    found = true;
                    break;
                }
            }

            // Do we have a new device?
            if (!found)
            {
                // Yes, so add the device to the list and let the caller know we 
                // detected a change.
                var dev = new SerialDeviceItem(p, new SerialPort(p));
                Devices.Add(dev);
                result = true;
            }
        }

        // Did we detect a change?
        return result;
    }

    /// <summary>
    ///     The ProcessRemoved method determines from the latest port list if any ports have
    ///     been added.
    /// </summary>
    /// <param name="ports">
    ///     The latest list of serial ports connected to the system.
    /// </param>
    /// <returns>
    ///     Returns true if a device has been removed. Otherwise, returns false.
    /// </returns>
    private bool ProcessRemoved(string[] ports)
    {
        // Scan the device list.
        foreach (var d in Devices)
        {
            // Get the device object.
            var dev = d as SerialDeviceItem;

            // The found flag is set when the specified device is found in the 
            // list.

            // Scan the port list to see if this device is still present.
            var found = ports.Any(p => dev != null && p == dev.PortName);

            // Was the device found in the port list?
            if (!found)
            {
                // No, so remove this port from the device list and let the caller 
                // know we detected a change.
                Devices.Remove(dev);
                return true;
            }
        }

        // Let the caller know we didn't detect a change.
        return false;
    }

    /// <summary>
    ///     The Timer property contains the timer used to periodically check the system for
    ///     ports that have been added and removed.
    /// </summary>
    private Timer Timer { get; set; }

    /// <summary>
    ///     The DevicesListChangedHandler is used to notify the client object that the device
    ///     list has changed when a device is either plugged in or removed from the system.
    /// </summary>
    public EventHandler<EventArgs> DeviceListChangedHandler { get; set; }

    /// <summary>
    ///     The DevicesAddedHandler is used to notify the client which devices have been added
    ///     to the system. 
    /// </summary>
    public EventHandler<DeviceListChangedEventArgs> DevicesAddedHandler { get; set; }
        
    /// <summary>
    ///     The DevicesRemovedHandler is used to notify the client which devices have been
    ///     removed to the system. 
    /// </summary>
    public EventHandler<DeviceListChangedEventArgs> DevicesRemovedHandler { get; set; }

    /// <summary>
    ///     The Devices property contains the list of devices connected to the system. The
    ///     actual object used is an implementation of ISerialDeviceItem.
    /// </summary>
    public List<ISerialDeviceItem> Devices { get; }

    /// <summary>
    ///     The FromPortName method takes the name of a serial port device and returns the
    ///     corresponding ISerialDeviceItem, if it exists in the system.
    /// </summary>
    /// <param name="name">
    ///     The name of the serial port device (e.g. "COM1", "tty.usbserial-FT1GRIGC") for which
    ///     the ISerialDeviceItem should be returned.
    /// </param>
    /// <returns>
    ///     If found, returns the ISerialDeviceItem corresponding to the port name. Otherwise,
    ///     returns null.
    /// </returns>
    public ISerialDeviceItem FromPortName(string name)
    {
        // Scan the device table for a matching entry.
        foreach (var dev in Devices)
        {
            if (dev.PortName == name) return dev;
        }
            
        // Could not find a matching entry.
        return null;
    }

    /// <summary>
    ///     The CreateChannel method creates an instance of the IChannel corresponding to the
    ///     underlying computer hardware managed by this class.
    /// </summary>
    /// <returns>
    ///     Returns a new instance of IChannel for this implementation.
    /// </returns>
    public IChannel CreateChannel() => new SerialChannel();
}