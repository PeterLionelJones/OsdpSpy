using System.Security.Cryptography;
using ThirdMillennium.Protocol;

namespace OsdpSpy.Osdp
{
    /// <summary>
    ///     The RandomNumber class is used to generate an 8-byte random number for
    ///     use in OSDP secure channel operation.
    /// </summary>
    public class RandomNumber
    {
        /// <summary>
        ///     The RandomNumber constructor generates an 8-byte random number.
        /// </summary>
        public RandomNumber()
        {
            Data = new byte[8];
            new RNGCryptoServiceProvider().GetBytes(Data);
        }

        /// <summary>
        ///     The Data property contains the 8-byte random number.
        /// </summary>
        public byte[] Data { get; }

        /// <summary>
        ///     The ToString method returns a text representation of the random 
        ///     number.
        /// </summary>
        /// <returns>
        ///     The text representation of the random number.
        /// </returns>
        public override string ToString() => Data.ToHexString();
    }
}

