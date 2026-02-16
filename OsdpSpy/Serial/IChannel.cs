using System.Threading.Tasks;

namespace OsdpSpy.Serial;

//----------------------------------------------------------------------------------------------
//        D E L E G A T E S
//----------------------------------------------------------------------------------------------

/// <summary>
///     The AsyncChannelStartDelegate allows IChannel implementations to verify that a channel
///     is the correct type of device when it is opened an start any background tasks. This
///     delegate is called immediately after the channel has been opened successfully.
/// </summary>
/// <param name="ch">
///     The IChannel attempting to open the device.
/// </param>
/// <returns>
///     Returns true if the channel was started successfully.
/// </returns>
public delegate Task<bool> AsyncChannelStartDelegate(IChannel ch);

/// <summary>
///     The ChannelStartDelegate allows IChannel implementations to verify that a channel is the
///     correct type of device when it is opened and start any background tasks. This delegate
///     is called immediately after the channel has been opened successfully.
/// </summary>
/// <param name="ch">
///     The IChannel attempting to open the device.
/// </param>
/// <returns>
///     Returns true if the channel was started successfully.
/// </returns>
public delegate bool ChannelStartDelegate(IChannel ch);

/// <summary>
///     The ChannelStopDelegate allows IChannel implementations to release any resources
///     allocated when the channel was opened. This delegate is called immediately before the
///     channel is closed.
/// </summary>
/// <param name="ch">
///     The IChannel attempting to open the device.
/// </param>
public delegate void ChannelStopDelegate(IChannel ch);

/// <summary>
///     The ChannelFilterDelegate is used to filter out serial devices that should not be
///     opened. This is particularly useful for UWP applications where the USB name provides
///     additional clues about the device connected to the system. This delegate is called
///     immediately before an attempt is made to open the device.
/// </summary>
/// <param name="dev">
///     The serial device to be checked for suitability to be opened.
/// </param>
/// <returns>
///     Returns true if the channel should attempt to open this device. Otherwise, returns
///     false.
/// </returns>
public delegate bool ChannelFilterDelegate(object dev);

//----------------------------------------------------------------------------------------------
//        I C H A N N E L   I N T E R F A C E
//----------------------------------------------------------------------------------------------

/// <summary>
///     The IChannel interface provides an interface to control the opening and closing of a
///     serial channel. Both asynchronous and synchronous interfaces are provided although it is
///     only necessary to implement what is required by a given implementation. 
/// </summary>
public interface IChannel
{
    //------------------------------------------------------------------------------------------
    //        P R O P E R T I E S
    //------------------------------------------------------------------------------------------
    
    /// <summary>
    ///     The Device property is ISerialDeviceItem that is cast to the underlying
    ///     implementation object.
    /// </summary>
    ISerialDeviceItem Device { get; set; }

    /// <summary>
    ///     The IsOpen property indicates if the serial channel is open for business.
    /// </summary>
    bool IsOpen { get; }

    /// <summary>
    ///     The BaudRate property holds the speed at which communication should occur.
    /// </summary>
    int BaudRate { get; set; }
        
    /// <summary>
    ///     The BytesToRead property returns the number of bytes available in the serial channel
    ///     buffer.
    /// </summary>
    int BytesToRead { get; }

    /// <summary>
    ///     The AsyncStartDelegate property provides a function to verify that the device that
    ///     has just been opened is the correct type of device.
    /// </summary>
    AsyncChannelStartDelegate AsyncStartDelegate { get; set; }

    /// <summary>
    ///     The StartDelegate property provides an asynchronous function to verify that the
    ///     device that has just been opened is the correct type of device.
    /// </summary>
    ChannelStartDelegate StartDelegate { get; set; }

    /// <summary>
    ///     The StopDelegate property provides a function to release any resources allocated by
    ///     one of the start delegates.
    /// </summary>
    ChannelStopDelegate StopDelegate { get; set; }

    /// <summary>
    ///     The FilterDelegate property provides a function to check if the device is about to
    ///     be opened is a valid device for the channel implementation.
    /// </summary>
    ChannelFilterDelegate FilterDelegate { get; set; }

    //------------------------------------------------------------------------------------------
    //        M E T H O D S
    //------------------------------------------------------------------------------------------
    
    /// <summary>
    ///     The Open method opens the specified device synchronously.
    /// </summary>
    /// <param name="dev">
    ///     The device to be opened.
    /// </param>
    /// <param name="rate">
    ///     The baud rate to be used on the open channel.
    /// </param>
    /// <returns>
    ///     Returns true of the port was opened successfully. Otherwise, returns false.
    /// </returns>
    bool Open(ISerialDeviceItem dev, int rate = 9600);

    /// <summary>
    ///     The Close method closes the device synchronously.
    /// </summary>
    void Close();

    /// <summary>
    ///     The OpenAsync method opens the specified device asynchronously.
    /// </summary>
    /// <param name="dev">
    ///     The device to be opened.
    /// </param>
    /// <param name="rate">
    ///     The baud rate to be used on the open channel.
    /// </param>
    /// <returns>
    ///     After the asynchronous operation completes, returns true of the port was opened
    ///     successfully. Otherwise, returns false.
    /// </returns>
    Task<bool> OpenAsync(ISerialDeviceItem dev, int rate = 9600);

    /// <summary>
    ///     The CloseAsync method closes the device asynchronously.
    /// </summary>
    /// <returns>
    ///     Returns void after the asynchronous operation has completed.
    /// </returns>
    Task CloseAsync();

    /// <summary>
    ///     The AutoOpen method attempts to open the specified device. If this fails, the method
    ///     scans for an available device until one is found.
    /// </summary>
    /// <param name="mgr">
    ///     The ISerialDeviceManager interface to a device manager that contains the list of
    ///     available devices.
    /// </param>
    /// <param name="dev">
    ///     The preferred device to be opens.
    /// </param>
    /// <param name="rate">
    ///     The baud rate to be used for the device.
    /// </param>
    /// <returns>
    ///     Returns true when a suitable device has been found. Cannot return false.
    /// </returns>
    bool AutoOpen(ISerialDeviceManager mgr, ISerialDeviceItem dev = null, int rate = 9600);

    /// <summary>
    ///     The AutoOpenAsync method attempts to open the specified device. If this fails, the
    ///     method scans for an available device until one is found.
    /// </summary>
    /// <param name="mgr">
    ///     The ISerialDeviceManager interface to a device manager that contains the list of
    ///     available devices.
    /// </param>
    /// <param name="dev">
    ///     The preferred device to be opens.
    /// </param>
    /// <param name="rate">
    ///     The baud rate to be used for the device.
    /// </param>
    /// <returns>
    ///     After the asynchronous operation has completed, returns true when a suitable device
    ///     has been found. Cannot return false!
    /// </returns>
    Task<bool> AutoOpenAsync(ISerialDeviceManager mgr, ISerialDeviceItem dev = null, int rate = 9600);

    /// <summary>
    ///     The WriteAsync method sends a string to the specified serial channel.
    /// </summary>
    /// <param name="dev">
    ///     The device through which the string should be sent.
    /// </param>
    /// <param name="data">
    ///     The string to be sent.
    /// </param>
    /// <returns>
    ///     Returns void after the asynchronous operation has completed.
    /// </returns>
    Task WriteAsync(string s);

    /// <summary>
    ///     The WriteAsync method sends a byte array to the specified serial channel.
    /// </summary>
    /// <param name="dev">
    ///     The device through which the data should be sent.
    /// </param>
    /// <param name="data">
    ///     The byte array to be sent.
    /// </param>
    /// <returns>
    ///     Returns void after the asynchronous operation has completed.
    /// </returns>
    Task WriteAsync(byte[] data);

    /// <summary>
    ///     The Write method sends a string to the specified serial channel.
    /// </summary>
    /// <param name="dev">
    ///     The device through which the string should be sent.
    /// </param>
    /// <param name="data">
    ///     The string to be sent.
    /// </param>
    void Write(string s);

    /// <summary>
    ///     The Write method sends a byte array to the specified serial channel.
    /// </summary>
    /// <param name="dev">
    ///     The device through which the data should be sent.
    /// </param>
    /// <param name="data">
    ///     The byte array to be sent.
    /// </param>
    void Write(byte[] data);

    /// <summary>
    ///     The ReadAsync method attempts to receive a frame of data into an IReceiver object.
    /// </summary>
    /// <param name="dev">
    ///     The serial device from which we are trying to receive data.
    /// </param>
    /// <param name="rx">
    ///     The IReceiver that will receive the data that we receive from the 
    ///     serial channel.
    /// </param>
    /// <param name="timeout">
    ///     The amount of time allowed for the frame of data to be received.
    /// </param>
    /// <returns>
    ///     After the asynchronous operation has been performed, returns true if the frame was
    ///     received successfully. Otherwise, returns false.
    /// </returns>
    Task<bool> ReadAsync(IReceiver rx, int timeout);

    /// <summary>
    ///     The Read method attempts to receive a frame of data into an IReceiver object.
    /// </summary>
    /// <param name="dev">
    ///     The serial device from which we are trying to receive data.
    /// </param>
    /// <param name="rx">
    ///     The IReceiver that will receive the data that we receive from the serial channel.
    /// </param>
    /// <param name="timeout">
    ///     The amount of time allowed for the frame of data to be received.
    /// </param>
    bool Read(IReceiver rx, int timeout);
        
    /// <summary>
    ///     This Read method attempts to read a specified number of bytes from the serial
    ///     channel.
    /// </summary>
    /// <param name="buffer">
    ///     The buffer to store the bytes read from the serial channel.
    /// </param>
    /// <param name="offset">
    ///     The offset in the buffer where the first byte read should be stored.
    /// </param>
    /// <param name="length">
    ///     The number of bytes to read from the serial channel into the buffer.
    /// </param>
    /// <returns>
    ///     Returns the buffer that has just been filled with the data read from the serial
    ///     channel.
    /// </returns>
    byte[] Read(byte[] buffer, int offset, int length);
}