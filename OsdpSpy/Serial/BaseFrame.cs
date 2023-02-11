namespace OsdpSpy.Serial
{
    /// <summary>
    ///     The BaseFrame class provides the primitives to be used for a variety or frame based
    ///     protocol employed at Third Millennium.
    /// </summary>
    public abstract class BaseFrame
    {
        /// <summary>
        ///     The TracePrefix property is used to identify which protocol is being traced.
        /// </summary>
        public string TracePrefix { get; protected set; }

        /// <summary>
        ///     FrameData contains the raw data for the frame.
        /// </summary>
        public byte[] FrameData { get; protected set; }

        /// <summary>
        ///     The Assemble method is used in a derived class to assemble the frame data from
        ///     constituent parts.
        /// </summary>
        public virtual void Assemble() { }

        /// <summary>
        ///     The Disassemble method is used in a derived class to break the frame data into the
        ///     constituent parts of the data frame.
        /// </summary>
        /// <returns>
        ///     Returns true if the data frame is valid. Otherwise, returns false.
        /// </returns>
        public virtual bool Disassemble() { return true; }
    }
}
