using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace OsdpSpy.Serial
{
    /// <summary>
    ///     The SerialChannel class implements a general purpose serial device for the .NET Core
    ///     platform.
    /// </summary>
    public class SerialChannel : IChannel
    {
        /// <summary>
        ///     The default baud rate for the channel.
        /// </summary>
        public int DefaultBaudRate { get; set; } = 9600;

        /// <summary>
        ///     The Device property is a plain object that is cast to the underlying implementation
        ///     object.
        /// </summary>
        public ISerialDeviceItem Device { get; set; }

        /// <summary>
        ///     The SerialPort property contains a reference to the platform-specific implementation
        ///     of the channel.
        /// </summary>
        public SerialPort SerialPort
        {
            get => ((SerialDeviceItem)Device).SerialPort;
            set => ((SerialDeviceItem)Device).SerialPort = value;
        }

        /// <summary>
        ///     The BytesToRead property returns the number of bytes available in the serial channel
        ///     buffer.
        /// </summary>
        public int BytesToRead => SerialPort.BytesToRead;

        /// <summary>
        ///     The IsBatteryPowered property indicates if the channel is in a battery powered
        ///     device.
        /// </summary>
        /// <returns>
        ///     Returns true if the channel is in a battery powered device. Otherwise, returns
        ///     false.
        /// </returns>
        public virtual bool IsBatteryPowered => false;

        /// <summary>
        ///     The CanPowerOff property indicates if the channel  can be powered down by an
        ///     application.
        /// </summary>
        public bool CanPowerOff { get => false; set => _ = value; }
        
        /// <summary>
        ///     The IsOpen property indicates if the serial channel is open for business.
        /// </summary>
        public bool IsOpen { get; private set; }

        /// <summary>
        ///     The BaudRate property holds the speed at which communication should occur.
        /// </summary>
        public int BaudRate { get; set; }

        /// <summary>
        ///     The AsyncStartDelegate property provides an asynchronous function to verify that the
        ///     device that has just been opened is the correct type of device.
        /// </summary>
        public AsyncChannelStartDelegate AsyncStartDelegate { get; set; }

        /// <summary>
        ///     The StartDelegate property provides a function to verify that the device that has
        ///     just been opened is the correct type of device.
        /// </summary>
        public ChannelStartDelegate StartDelegate { get; set; }

        /// <summary>
        ///     The StopDelegate property provides a function to release any resources allocated by
        ///     one of the start delegates.
        /// </summary>
        public ChannelStopDelegate StopDelegate { get; set; }

        /// <summary>
        ///     The FilterDelegate property provides a function to check if the device is about to
        ///     be opened is a valid device for the channel implementation.
        /// </summary>
        public ChannelFilterDelegate FilterDelegate { get; set; }

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
        public bool Open(ISerialDeviceItem dev, int rate = 9600)
        {
            try
            {
                // Do we have a filter for the devices?
                if (FilterDelegate != null && !FilterDelegate(dev)) return false;

                // Attempt to open the channel.
                Device = dev;
                BaudRate = rate;

                // Do we have a SerialPort yet?
                if (SerialPort == null)
                {
                    // Create the new SerialPort.
                    SerialPort = new SerialPort
                    {
                        PortName = Device.PortName,
                        BaudRate = rate,
                        DataBits = 8,
                        StopBits = StopBits.One,
                        Parity = Parity.None
                    };
                }
                else
                {
                    // Make sure the port is closed.
                    SerialPort.Close();
                    while (SerialPort.IsOpen) Thread.Sleep(10);
                }

                // Setup the serial port.
                SerialPort.PortName = Device.PortName;
                SerialPort.BaudRate = rate;
                SerialPort.Open();
                IsOpen = SerialPort.IsOpen;

                // Do we have a delegate to invoke?
                if (IsOpen && StartDelegate != null && !StartDelegate(this))
                {
                    // No, so close the channel.
                    Close();
                }

                // Return the result of the operation.
                return IsOpen;
            }
            catch (Exception)
            {
                return IsOpen = false;
            }
        }

        /// <summary>
        ///     The OpenAsync method open the specified device asynchronously.
        /// </summary>
        /// <param name="dev">
        ///     The device to be opened.
        /// </param>
        /// <param name="rate">
        ///     The baud rate to be used on the open channel. This parameter is ignored and
        ///     overridden by the default baud rate.
        /// </param>
        /// <returns>
        ///     After the asynchronous operation completes, returns true of the port was opened
        ///     successfully. Otherwise, returns false.
        /// </returns>
        public async Task<bool> OpenAsync(ISerialDeviceItem dev, int rate = 9600)
        {
            // Is this a suitable device?
            if (FilterDelegate != null && !FilterDelegate(dev)) return false;

            // Attempt to open the channel.
            Device = dev;
            BaudRate = rate;

            // Assume the worst . . 
            var result = false;

            // Attempt to open the port on a background task.
            await Task.Run(() => { result = Open(dev, rate); });

            // Pass back the result.
            IsOpen = result;
            
            // Do we have a delegate to invoke?
            if (IsOpen && AsyncStartDelegate != null && !await AsyncStartDelegate(this))
            {
                // No, so close the channel.
                await CloseAsync();
            }

            // Return the result of the operation.
            return IsOpen;
        }

        /// <summary>
        ///     The Close method closes the device synchronously.
        /// </summary>
        public void Close()
        {
            // Are we already closed?
            if (!IsOpen) return;

            // Invoke the stop delegate for this channel if we have one.
            StopDelegate?.Invoke(this);

            // Close the channel.
            SerialPort.Close();

            // Mark the port as closed.
            IsOpen = false;
        }

        /// <summary>
        ///     The CloseAsync method closes the device asynchronously.
        /// </summary>
        /// <returns>
        ///     Returns void after the asynchronous operation has completed.
        /// </returns>
        public async Task CloseAsync()
        {
            await Task.Run(Close);
        }

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
        public bool AutoOpen(   ISerialDeviceManager mgr, 
                                        ISerialDeviceItem dev = null, 
                                        int rate = 9600    )
        {
            // Do we have a saved device we can attempt to open?
            if (dev != null && Open(dev, rate)) return true;

            // Keep going until we find a serial device we can use.
            while (true)
            {
                try
                {
                    // Scan through the available devices.
                    foreach (var d in mgr.Devices)
                    {
                        // Attempt to open the device.
                        if (Open(d, rate)) return true;
                    }
                }
                catch (InvalidOperationException)
                {
                    // Ignore InvalidOperationException. The channel is closing down.
                }

                // Let's wait a little while.
                Thread.Sleep(100);
            }
        }

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
        public async Task<bool> AutoOpenAsync(  ISerialDeviceManager mgr, 
                                                        ISerialDeviceItem dev = null, 
                                                        int rate = 9600   )
        {
            // Do we have a saved device we can attempt to open?
            if (dev != null && await OpenAsync(dev, rate)) return true;

            // Keep going until we find a serial device we can use.
            while (true)
            {
                // Scan through the available devices.
                foreach (var d in mgr.Devices)
                {
                    // Attempt to open the device.
                    if (await OpenAsync(d, rate)) return true;

                    // Let's wait a little while.
                    Thread.Sleep(100);
                }
            }
        }

        /// <summary>
        ///     The WriteAsync method sends a string to the specified serial channel.
        /// </summary>
        /// <param name="data">
        ///     The string to be sent.
        /// </param>
        /// <returns>
        ///     Returns void after the asynchronous operation has completed.
        /// </returns>
        public async Task WriteAsync(string s)
        {
            await Task.Run(() => { Write(s); });
        }

        /// <summary>
        ///     The WriteAsync method sends a byte array to the specified serial channel.
        /// </summary>
        /// <param name="data">
        ///     The byte array to be sent.
        /// </param>
        /// <returns>
        ///     Returns void after the asynchronous operation has completed.
        /// </returns>
        public async Task WriteAsync(byte[] data)
        {
            await Task.Run(() => { Write(data); });
        }

        /// <summary>
        ///     The Write method sends a string to the specified serial channel.
        /// </summary>
        /// <param name="data">
        ///     The string to be sent.
        /// </param>
        public void Write(string s)
        {
            SerialPort.DiscardInBuffer();
            SerialPort.Write(s);
            while (SerialPort.BytesToWrite > 0) Thread.Sleep(1);
        }

        /// <summary>
        ///     The Write method sends a byte array to the specified serial channel.
        /// </summary>
        /// <param name="data">
        ///     The byte array to be sent.
        /// </param>
        public void Write(byte[] data)
        {
            try
            {
                // Wait a little while.
                Thread.Sleep(2);

                // Clear the receive buffer.
                SerialPort.DiscardInBuffer();

                // Transmit the data frame.
                SerialPort.WriteTimeout = 100;
                SerialPort.Write(data, 0, data.Length);
            }
            catch (TimeoutException)
            {
                // The call to Write timed out.
            }
            catch (InvalidOperationException)
            {
                // The port is closed!
            }
        }

        /// <summary>
        ///     The ReadAsync method attempts to receive a frame of data into an IReceiver object.
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
        /// <returns>
        ///     After the asynchronous operation has been performed, returns true if the frame was
        ///     received successfully. Otherwise, returns false.
        /// </returns>
        public async Task<bool> ReadAsync(IReceiver rx, int timeout)
        {
            // Assume the worst.
            var complete = false;

            // Attempt to read the response from the reader.
            await Task.Run(() => { complete = Read(rx, timeout); });

            // Pass back the result of the read.
            return complete;
        }

        /// <summary>
        ///     The Read method attempts to receive a frame of data into an IReceiver object.
        /// </summary>
        /// <param name="rx">
        ///     The IReceiver that will receive the data that we receive from the serial channel.
        /// </param>
        /// <param name="timeout">
        ///     The amount of time allowed for the frame of data to be received.
        /// </param>
        public bool Read(IReceiver rx, int timeout)
        {
            try
            {
                // Mark the start of our time waiting for the response.
                var start = Environment.TickCount;

                SerialPort.ReadTimeout = 5;

                // Attempt to receive data from the serial mPort.
                for (var timedOut = false; !timedOut;)
                {
                    // Are there any bytes to be read from the serial mPort.
                    while (SerialPort.BytesToRead > 0)
                    {
                        // Read the available bytes into a buffer.
                        var b = new byte[SerialPort.BytesToRead];
                        SerialPort.Read(b, 0, b.Length);

                        // Add each of the received bytes into the frame.
                        foreach (var inch in b)
                        {
                            // Is this the end of the data packet.
                            if (rx.AddByte(inch))
                            {
                                // The frame is complete.
                                return true;
                            }
                        }
                    }

                    // Have we run out of time?
                    timedOut = Environment.TickCount - start > timeout;
                }

                // Frame timed out.
                return false;
            }
            catch (TimeoutException)
            {
                return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

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
        public byte[] Read(byte[] buffer, int offset, int length)
        {
            SerialPort.Read(buffer, offset, length);
            return buffer;
        }

        /// <summary>
        ///     The PowerOff method is used to turn off power to the channel in a battery-powered
        ///     device.
        /// </summary>
        public void PowerOff()
        {
            // The default implementation assumes that the channel is always powered so does
            // nothing.
        }

        /// <summary>
        ///     The PowerOff method is used to turn on power to the channel in a battery-powered
        ///     device.
        /// </summary>
        public void PowerOn()
        {
            // The default implementation assumes that the channel is always powered so does
            // nothing.
        }
    }
}
