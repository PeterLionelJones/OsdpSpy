using System;
using System.Linq;
using OsdpSpy.Extensions;
using ThirdMillennium.Protocol;

namespace OsdpSpy.Osdp
{
    /// <summary>
    ///     The CipherBlock class implements the basic unit of cryptography for the OSDP secure
    ///     channel.
    /// </summary>
    public class CipherBlock
    {
        /// <summary>
        ///     This CipherBlock constructor initialises the data content from a specified buffer
        ///     and offset within the buffer.
        /// </summary>
        /// <param name="data">
        ///     The byte buffer from which to initialise this CipherBlock.
        /// </param>
        /// <param name="offset">
        ///     The offset from with in the byte buffer from which to initialise this 
        ///     CipherBlock.
        /// </param>
        public CipherBlock(byte[] data = null, int offset = 0)
        {
            Initialise(data, offset);
        }

        /// <summary>
        ///     This CipherBlock constructor initialises this CipherBlock from another CipherBlock.
        /// </summary>
        /// <param name="cb">
        ///     The CipherBlock from which to initialise this CipherBlock.
        /// </param>
        private CipherBlock(CipherBlock cb)
        {
            // Clear this cipher block.
            Initialise();

            // Copy the data in from the specified cipher block.
            Read(cb);
        }

        /// <summary>
        ///     This Initialise method copies the data in from the specified buffer.
        /// </summary>
        /// <param name="data">
        ///     The byte buffer from which to copy the data.
        /// </param>
        /// <param name="offset">
        ///     The offset within the byte buffer from which to copy the data.
        /// </param>
        private void Initialise(byte[] data = null, int offset = 0)
        {
            // Do the basic initialisation.
            Data = new byte[]
            {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };

            // Copy the data into the block.
            if (data != null) Read(data, offset);
        }

        /// <summary>
        ///     The Data property contains the 16-byte raw cipher block data.
        /// </summary>
        public byte[] Data { get; private set; }

        /// <summary>
        ///     The Write method copies the cipher block to a memory buffer.
        /// </summary>
        /// <param name="output">
        ///     The buffer to which the cipher block should be written.
        /// </param>
        /// <param name="offset">
        ///     The offset within the buffer to which the cipher block should be written.
        /// </param>
        private void Write(byte[] output, int offset = 0)
        {
            Buffer.BlockCopy(Data, 0, output, offset, 16);
        }

        /// <summary>
        ///     This Read method loads the cipher block from the specified memory buffer.
        /// </summary>
        /// <param name="output">
        ///     The buffer from which to load the cipher block.
        /// </param>
        /// <param name="offset">
        ///     The offset within the buffer from which to load the cipher block.
        /// </param>
        private void Read(byte[] output, int offset = 0)
        {
            Buffer.BlockCopy(output, offset, Data, 0, 16);
        }

        /// <summary>
        ///     This Read method loads the cipher block from another cipher block.
        /// </summary>
        /// <param name="cb">
        ///     The cipher block from which this cipher block should be loaded.
        /// </param>
        private void Read(CipherBlock cb)
        {
            Read(cb.Data, 0);
        }

        /// <summary>
        ///     The Invert method returns a new CipherBlock that contains the inverse of the data in
        ///     this cipher block.
        /// </summary>
        /// <returns>
        ///     Returns a CipherBlock containing the inverse of this CipherBlock.
        /// </returns>
        public CipherBlock Invert()
        {
            // Copy this cipher block into the result.
            var result = new CipherBlock(Data);

            // Invert each byte of the cipher block.
            for (var i = 0; i < 16; ++i)
            {
                result.Data[i] = (byte)~result.Data[i];
            }

            // Pass the inverted cipher block back to the caller.
            return result;
        }

        /// <summary>
        ///     The Compare method compares two cipher blocks.
        /// </summary>
        /// <param name="c1">
        ///     The first cipher block to be compared.
        /// </param>
        /// <param name="c2">
        ///     The second cipher block to be compared.
        /// </param>
        /// <returns>
        ///     Returns true if the cipher blocks are the same. Otherwise, returns 
        ///     false.
        /// </returns>
        public static bool Compare(CipherBlock c1, CipherBlock c2)
            => c1.Data.SequenceEqual(c2.Data);

        /// <summary>
        ///     The GenerateSessionKey generates a session key from the supplied information.
        /// </summary>
        /// <param name="n">
        ///     The type of session key (0x82 = S_ENC, 0x01 = S_MAC1, 0x02 = S_MAC2).
        /// </param>
        /// <param name="r">
        ///     The random number used in building the session key.
        /// </param>
        /// <param name="k">
        ///     The key used to generate the session key.
        /// </param>
        /// <returns>
        ///     Returns the CipherBlock containing the session key.
        /// </returns>
        private static CipherBlock GenerateSessionKey(byte n, RandomNumber r, CipherBlock k)
        {
            // Instantiate the session key we are about to generate.
            var s = new CipherBlock();
            s.Data[0] = 0x01;
            s.Data[1] = n;
            for (var i = 0; i < 6; ++i)
            {
                s.Data[2 + i] = r.Data[i];
            }

            // Generate the session key.
            s = Encrypt(s, k);

            // Pass the session key back to the caller.
            return s;

        }

        /// <summary>
        ///     The GenerateENC method generates the S_ENC session key.
        /// </summary>
        /// <param name="r">
        ///     The random number used in building the session key.
        /// </param>
        /// <param name="k">
        ///     The key used to generate the session key.
        /// </param>
        /// <returns>
        ///     Returns the CipherBlock containing the session key.
        /// </returns>
        public static CipherBlock GenerateEnc(RandomNumber r, CipherBlock k)
        {
            return GenerateSessionKey(0x82, r, k);
        }

        /// <summary>
        ///     The GenerateMAC1 method generates the S_MAC1 session key.
        /// </summary>
        /// <param name="r">
        ///     The random number used in building the session key.
        /// </param>
        /// <param name="k">
        ///     The key used to generate the session key.
        /// </param>
        /// <returns>
        ///     Returns the CipherBlock containing the session key.
        /// </returns>
        public static CipherBlock GenerateMac1(RandomNumber r, CipherBlock k)
        {
            return GenerateSessionKey(0x01, r, k);
        }

        /// <summary>
        ///     The GenerateMAC1 method generates the S_MAC1 session key.
        /// </summary>
        /// <param name="r">
        ///     The random number used in building the session key.
        /// </param>
        /// <param name="k">
        ///     The key used to generate the session key.
        /// </param>
        /// <returns>
        ///     Returns the CipherBlock containing the session key.
        /// </returns>
        public static CipherBlock GenerateMac2(RandomNumber r, CipherBlock k)
        {
            return GenerateSessionKey(0x02, r, k);
        }

        /// <summary>
        ///     The XOR operator returns the XOR of two cipher blocks.
        /// </summary>
        /// <param name="c1">
        ///     The first cipher block to be XORred.
        /// </param>
        /// <param name="c2">
        ///     The second cipher block to be XORred.
        /// </param>
        /// <returns>
        ///     A cipher block containing the XOR of the two passed cipher blocks.
        /// </returns>
        public static CipherBlock operator ^(CipherBlock c1, CipherBlock c2)
        {
            var r = new CipherBlock();
            for (var i = 0; i < 16; ++i)
            {
                r.Data[i] = (byte)(c1.Data[i] ^ c2.Data[i]);
            }
            return r;
        }

        /// <summary>
        ///     The ChainOperation method executes a chain operation on a given cipher block using
        ///     an initialisation vector and a key. The chain operation may be an encryption or
        ///     decryption operation.
        /// </summary>
        /// <param name="encrypt">
        ///     A boolean parameter indication if this is an encryption operation if true, a
        ///     decryption operation if false.
        /// </param>
        /// <param name="input">
        ///     The cipher block on which the chained operation should be performed.
        /// </param>
        /// <param name="iv">
        ///     The initial value for the chained operation.
        /// </param>
        /// <param name="key">
        ///     The AES128 key to be used for the chained operation.
        /// </param>
        /// <returns></returns>
        private static CipherBlock ChainOperation(bool encrypt, CipherBlock input, CipherBlock iv, CipherBlock key)
        {
            // Initialise the output cipher block.
            CipherBlock output;

            // Is this an encryption operation?
            if (encrypt)
            {
                // Chain the encryption.
                iv ^= input;
                output = Encrypt(iv, key);
                iv.Read(output);
            }
            else
            {
                // Chain the decryption operation?
                output = Decrypt(input, key);
                output ^= iv;
                iv.Read(input);
            }

            // Return the chained cipher block.
            return output;
        }

        /// <summary>
        ///     The GenerateMAC method generates a MAC for a data buffer based on two AES128 keys
        ///     and an initial value.
        /// </summary>
        /// <param name="input">
        ///     The data buffer for which the MAC should be generated.
        /// </param>
        /// <param name="k1">
        ///     The first AES128 key that is used as part of the MAC generation.
        /// </param>
        /// <param name="k2">
        ///     The second AES128 key that is used as part of the MAC generation.
        /// </param>
        /// <param name="iv">
        ///     The initial value that is to be used for the MAC generation.
        /// </param>
        /// <returns>
        ///     Returns a cipher block containing the mach generated for the data 
        ///     block.
        /// </returns>
        public static CipherBlock GenerateMac(byte[] input, CipherBlock k1, CipherBlock k2, CipherBlock iv)
        {
            // Calculate the number of blocks for the MAC generation.
            var blocks = input.Length / 16;

            // Allocate the input block to be used for the MAC generation.
            var iblock = new CipherBlock();

            // Process each of the blocks apart from the last one.
            for (var i = 0; i < blocks - 1; ++i)
            {
                iblock.Read(input, i * 16);
                iv = ChainOperation(true, iblock, iv, k1);
            }

            // Process the last block.
            iblock.Read(input, (blocks - 1) * 16);
            return ChainOperation(true, iblock, iv, k2);
        }

        /// <summary>
        ///     This Encrypt method encrypts the data block based on an AES128 key and an initial
        ///     value.
        /// </summary>
        /// <param name="data">
        ///     The data to be encrypted/
        /// </param>
        /// <param name="iv">
        ///     The initial value for the encryption.
        /// </param>
        /// <param name="k">    
        ///     The AES128 key o be used for the encryption.
        /// </param>
        /// <returns>
        ///     Returns a byte buffer containing the encrypted data.
        /// </returns>
        public static byte[] Encrypt(byte[] data, CipherBlock iv, CipherBlock k)
        {
            // Pad the input data.
            var blocks = 1 + ((data.Length - 1) / 16);
            var length = blocks * 16;
            var input = new byte[length];
            for (var i = 0; i < data.Length; ++i)
            {
                input[i] = data[i];
            }
            if (data.Length < length)
            {
                input[data.Length] = 0x80;
            }
            for (var i = data.Length + 1; i < length; ++i)
            {
                input[i] = 0;
            }

            // Create the output buffer.
            var output = new byte[length];

            // Copy the init vector.
            var init = new CipherBlock(iv);

            // Encrypt the data block.
            for (var i = 0; i < blocks; ++i)
            {
                var ip = new CipherBlock(input, 16 * i);
                init = ChainOperation(true, ip, init, k);
                init.Write(output, 16 * i);
            }

            // Pass back the encrypted data block the the caller.
            return output;
        }

        /// <summary>
        ///     The Encrypt method performs an ECB encryption of the input cipher block using the
        ///     supplied key.
        /// </summary>
        /// <param name="input">
        ///     The cipher block to be encrypted.
        /// </param>
        /// <param name="key">
        ///     The cipher block containing the AES128 key to be used for the 
        ///     encryption.
        /// </param>
        /// <returns>
        ///     A cipher block containing the encrypted data.
        /// </returns>
        public static CipherBlock Encrypt(CipherBlock input, CipherBlock key)
        {
            // var i = new Ecb(input.Data);
            // var k = new Aes128(key.Data);
            // var o = Crypto.Encrypt(i, k);
            // return new CipherBlock(o.Value);

            return new CipherBlock(input.Data.EncryptEcb(key.Data));
        }

        /// <summary>
        ///     The Decrypt method performs an ECB decryption of the input cipher block using the
        ///     supplied key.
        /// </summary>
        /// <param name="input">
        ///     The cipher block to be decrypted.
        /// </param>
        /// <param name="key">
        ///     The CipherBlock containing the AES128 key to be used for the 
        ///     decryption.
        /// </param>
        /// <returns>
        ///     A cipher block containing the decrypted data.
        /// </returns>
        private static CipherBlock Decrypt(CipherBlock input, CipherBlock key)
        {
            // var i = new Ecb(input.Data);
            // var k = new Aes128(key.Data);
            // var o = Crypto.Decrypt(i,k);
            // return new CipherBlock(o.Value);

            return new CipherBlock(input.Data.DecryptEcb(key.Data));
        }

        /// <summary>
        ///     This Decrypt method decrypts the data block based on an AES128 key and an initial
        ///     value.
        /// </summary>
        /// <param name="input">
        ///     The data to be decrypted.
        /// </param>
        /// <param name="iv">
        ///     The initial value for the decryption.
        /// </param>
        /// <param name="k">    
        ///     The AES128 key o be used for the decryption.
        /// </param>
        /// <returns>
        ///     Returns a byte buffer containing the decrypted data.
        /// </returns>
        public static byte[] Decrypt(byte[] input, CipherBlock iv, CipherBlock k)
        {
            // Create the output buffer.
            var output = new byte[input.Length];

            // How many block operations are required?
            var blocks = 1 + ((input.Length - 1) / 16);

            // Copy the init vector.
            var init = new CipherBlock(iv);

            // Decrypt the data block.
            for (var i = 0; i < blocks; ++i)
            {
                var ip = new CipherBlock(input, 16 * i);
                var op = ChainOperation(false, ip, iv, k);
                op.Write(output, 16 * i);
                iv = ip;
            }

            // Pass back the encrypted data block the the caller.
            return output;
        }

        /// <summary>
        ///     The ToString method returns a text representation of this cipher block.
        /// </summary>
        /// <returns>
        ///     A string containing a text representation of this cipher block.
        /// </returns>
        public override string ToString()
        {
            return Data.ToHexString();
        }
    }
}

