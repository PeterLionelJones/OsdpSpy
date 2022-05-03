namespace OsdpSpy.Osdp
{
    /// <summary>
    ///     The SecureSession class contains the session keys for an OSDP secure channel session and
    ///     the rolling MAC.
    /// </summary>
    public class SecureSession
    {
        /// <summary>
        ///     The SecureSession constructor initialises the session keys for this OSDP secure
        ///     session.
        /// </summary>
        public SecureSession()
        {
            Smac1 = new CipherBlock();
            Smac2 = new CipherBlock();
            Enc = new CipherBlock();
            Mac = new CipherBlock();
        }

        /// <summary>
        ///     The Enc property contains the S_ENC session  key.
        /// </summary>
        public CipherBlock Enc { get; set; }

        /// <summary>
        ///     The Smac1 property contains the S_MAC1 session key.
        /// </summary>
        public CipherBlock Smac1 { get; set; }

        /// <summary>
        ///     The Smac2 property contains the S_MAC2 session key.
        /// </summary>
        public CipherBlock Smac2 { get; set; }

        /// <summary>
        ///     The MAC property contains the rolling MAC for the OSDP secure channel 
        ///     session.
        /// </summary>
        public CipherBlock Mac { get; set; }
    }
}

