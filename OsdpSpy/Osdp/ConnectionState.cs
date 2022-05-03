namespace OsdpSpy.Osdp
{
    /// <summary>
    ///     The ConnectionState enumeration lists the states of connection for the circuit are
    ///     signalled to a client object as the circuit progresses through this life cycle.
    /// </summary>
    public enum ConnectionState
    {
        Connecting = 0,
        ConnectFailed = 1,
        Connected = 2,
        Disconnecting = 3,
        Disconnected  = 4
    }

    public static class ConnectionStateExtensions
    {
        /// <summary>
        ///     The AsString method converts the supplied ConnectionType to a string.
        /// </summary>
        /// <param name="state">
        ///     The ConnectionState to be converted.
        /// </param>
        /// <returns>
        ///     A string representation of the ConnectionState.
        /// </returns>
        public static string AsString(this ConnectionState state)
        {
            var text = new string[]
            {
                "Connecting...",
                "Connect Failed",
                "Connected",
                "Disconnecting...",
                "Disconnected"
            };

            return text[(int)state];
        }
    }
}