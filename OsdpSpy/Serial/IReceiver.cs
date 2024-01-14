namespace OsdpSpy.Serial;

/// <summary>
///     The IReceiver interface is used to build a received from from a channel. Please see
///     ThirdMillennium.Protocol.ByteReceiver and ThirdMillennium.Protocol.OSDP.ResponseFrame
///     for example implementations.
/// </summary>
public interface IReceiver
{
    /// <summary>
    ///     The AddByte method adds the input character to the reception frame and determines
    ///     if the frame is now complete.
    /// </summary>
    /// <param name="inch">
    ///     The input character to be added to the buffer.
    /// </param>
    /// <returns>
    ///     Returns true if the frame has been built successfully. Otherwise, returns false.
    /// </returns>
    bool AddByte(byte inch);

    /// <summary>
    ///     The Reset method resets reception into the receiver buffer.
    /// </summary>
    void Reset();

    /// <summary>
    ///     The MaximumLength property contains the maximum length of data that may be received
    ///     in a single frame.
    /// </summary>
    int MaximumLength { get; }

    /// <summary>
    ///    The Elapsed property contains the number of milliseconds it took to receive the
    ///     frame. This can be a useful diagnostic. The value is -1 if a frame was not received.
    /// </summary>
    int Elapsed { get; }
}