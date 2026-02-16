using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using OsdpSpy.Serial;

[assembly: InternalsVisibleTo("OsdpSpy.Osdp.Tests")]

namespace OsdpSpy.Osdp;

/// <summary>
///     The ResponseFrame class provides a mechanism by which an OSDP frame may be built byte by
///     byte.
/// </summary>
public class ResponseFrame : Frame, IReceiver
{
    //------------------------------------------------------------------------------------------
    //        C O N S T R U C T O R
    //------------------------------------------------------------------------------------------

    /// <summary>
    ///     The RXFrame constructor initialises the frame for reception.
    /// </summary>
    public ResponseFrame() => Reset();

        
    //------------------------------------------------------------------------------------------
    //        P R I V A T E   P R O P E R T I E S
    //------------------------------------------------------------------------------------------
    private readonly Stopwatch _stopwatch = new Stopwatch();
    private RxState _state  = RxState.WaitingStart;
    private int _index;
    private int _length;
    private int _remaining;

        
    //------------------------------------------------------------------------------------------
    //        P U B L I C   P R O P E R T I E S
    //------------------------------------------------------------------------------------------

    /// <summary>
    ///    The Elapsed property contains the number of milliseconds it took to receive the
    ///    frame. This can be a useful diagnostic. The value is -1 if a frame was not received.
    /// </summary>
    public int Elapsed { get; private set; }
        
    /// <summary>
    ///    The ElapsedWithPrecision property contains the number of milliseconds it took to
    ///     receive the frame as a double. This can be a useful diagnostic.
    /// </summary>
    public double ElapsedWithPrecision { get; private set; }

    /// <summary>
    ///     The TraceString property returns the received frame and its response time in a form
    /// that can be logged to a console. 
    /// </summary>
    public override string TraceString 
        => IsAcu 
            ? base.TraceString 
            : $"{base.TraceString}\nResponse Time = {ElapsedWithPrecision:F}mS";

        
    //------------------------------------------------------------------------------------------
    //        P U B L I C   M E T H O D S
    //------------------------------------------------------------------------------------------

    /// <summary>
    ///     The Reset method initialises the frame for reception of a new OSDP frame.
    /// </summary>
    public void Reset()
    {
        _state = RxState.WaitingStart;
        _stopwatch.Reset();
        _stopwatch.Start();
        Elapsed = -1;
    }

    /// <summary>
    ///     The RXState enumeration lists each of the states for the OSDP frame reception state
    ///     machine.
    /// </summary>
    private enum RxState : byte
    {
        WaitingStart,
        WaitingAddress,
        WaitingLenLsb,
        WaitingLenMsb,
        WaitingData,
        Complete
    }

    /// <summary>
    ///     The AddByte method adds the input character to the reception frame and determines if
    ///     the frame is now complete.
    /// </summary>
    /// <param name="inch">
    ///     The input character to be added to the buffer.
    /// </param>
    /// <returns>
    ///     Returns true if the frame has been built successfully. Otherwise,
    ///     returns false.
    /// </returns>
    public bool AddByte(byte inch)
    {
        try
        {
            // Assume the worst.
            var result = false;

            switch (_state)
            {
                // Waiting for the start character (0x53).
                case RxState.WaitingStart:
                    if (inch == 0x53)
                    {
                        FrameData = new byte[MaximumLength];
                        FrameData[0] = 0x53;
                        _state = RxState.WaitingAddress;
                    }
                    break;

                // Waiting for the address byte.
                case RxState.WaitingAddress:
                    FrameData[1] = inch;
                    _state = RxState.WaitingLenLsb;
                    break;

                // Waiting for the LSB of the length.
                case RxState.WaitingLenLsb:
                    FrameData[2] = inch;
                    _state = RxState.WaitingLenMsb;
                    _length = inch;
                    break;

                // Waiting for the MSB of the length.
                case RxState.WaitingLenMsb:
                    FrameData[3] = inch;
                    _state = RxState.WaitingData;
                    _length += (inch << 8);
                    if (_length > MaximumLength)
                    {
                        _state = RxState.WaitingStart;
                    }
                    else
                    {
                        _remaining = _length - 4;
                        _index = 4;
                    }
                    break;

                // Loading in the remainder of the frame.
                case RxState.WaitingData:
                    // Add the character to the buffer.
                    FrameData[_index++] = inch;
                        
                    // Was that the last character?
                    if (--_remaining == 0)
                    {
                        // Truncate the frame data.
                        byte[] f = new byte[_index];
                        Buffer.BlockCopy(FrameData, 0, f, 0, _index);
                        FrameData = f;

                        // Mark the frame as complete.
                        _state = RxState.Complete;
                            
                        // Record the elapsed time.
                        _stopwatch.Stop();
                        Elapsed = (int) _stopwatch.ElapsedMilliseconds;
                        ElapsedWithPrecision = _stopwatch.Elapsed.TotalMilliseconds;

                        // Return the frame.
                        result = true;
                    }
                    break;

                // Frame is already complete.
                case RxState.Complete:
                    break;
            }

            // Frame is not complete!
            return result;
        }
        catch (Exception)
        {
            // An exception occurred! So, reset frame reception.
            _state = RxState.WaitingStart;
            return false;
        }
    }

    internal bool IsComplete => _state == RxState.Complete;

    internal void KillFrame() => FrameData = new byte[1];
}