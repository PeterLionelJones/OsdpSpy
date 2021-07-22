using System;
using System.IO;
using System.Security.Cryptography;

namespace ThirdMillennium.OsdpSpy
{
    public static class CryptoExtensions
    {
        public static byte[] MacT(this byte[] input, byte[] kX)
            => input.CMac(kX).TruncateMac();

        private static byte[] CMac(this byte[] input, byte[] kX, int requiredLength = -1)
        {
            // Initialise const_Rb and the init vector.
            var constRb = new byte[]
            {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x87
            };
            var iv = new byte[16];

            // Calculate K0.
            var k0 = iv.EncryptEcb(kX);

            // Calculate K1
            var k1 = (k0[0] & 0x80) == 0x00
                ? k0.ShiftLeft()
                : k0.ShiftLeft().XOr(constRb);

            // Calculate K2.
            var k2 = (k1[0] & 0x80) == 0x00
                ? k1.ShiftLeft()
                : k1.ShiftLeft().XOr(constRb);
            
            // Pad the input data.
            var d = input.Pad(out var padded, requiredLength);

            // XOR the appropriate sub key into the last block of D.
            var offset = d.Length - 16;
            for (var i = 0; i < 16; ++i)
            {
                d[offset + i] = padded 
                    ? (byte)(d[offset + i] ^ k2[i]) 
                    : (byte)(d[offset + i] ^ k1[i]);
            }

            // Encrypt the block.
            var d2 = d.Encrypt(kX, new byte[16]);

            // Extract the CMAC. It is in the last block.
            var div = new byte[16];
            Buffer.BlockCopy(d2, offset, div, 0, 16);
            return div;
        }
        
        public static byte[] DecryptEcb(this byte[] input, byte[] key)
        {
            // Set up the decryption parameters.
            using var aesAlg = new AesManaged
            {
                Mode = CipherMode.ECB,
                BlockSize = 128,
                KeySize = 128,
                Padding = PaddingMode.None,
                Key = key
            };

            // Decrypt the input data
            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            var output = new byte[16];
            decryptor.TransformBlock(input, 0, 16, output, 0);

            // Done !!
            return output;
        }

        public static byte[] Decrypt(this byte[] input, byte[] key, byte[] iv = null)
        {
            // Set the decryption parameters.
            using var a = Aes.Create();
            a.KeySize = 128;
            a.Key = key;
            a.IV = iv ?? new byte[16];
            a.Padding = PaddingMode.None;

            // Decrypt the memory stream.
            using var ms = new MemoryStream(input);
            using var cs = new CryptoStream(ms, a.CreateDecryptor(), CryptoStreamMode.Read);
            var output = new byte[input.Length];
            cs.Read(output, 0, output.Length);

            // Close the streams.
            cs.Close();
            ms.Close();

            // All done!!
            return output;
        }

        private static byte[] EncryptEcb(this byte[] input, byte[] key)
        {
            // Set up the encryption parameters.
            using var aesAlg = new AesManaged
            {
                Mode = CipherMode.ECB,
                BlockSize = 128,
                KeySize = 128,
                Padding = PaddingMode.None,
                Key = key
            };

            // Encrypt the data.
            var output = new byte[16];
            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            encryptor.TransformBlock(input, 0, 16, output, 0);
            return output;
        }

        public static byte[] Encrypt(this byte[] input, byte[] key, byte[] iv = null)
        {
            // Allocate the output and padded buffers.
            var padded = input.Pad();
            var output = new byte[padded.Length];

            // Encrypt the input buffer to the output buffer.
            using var a = Aes.Create();
            a.KeySize = 128;
            a.Key = key;
            a.IV = iv ?? new byte[16];
            a.Padding = PaddingMode.None;

            // Encrypt the data.
            using var ms = new MemoryStream(padded);
            using var cs = new CryptoStream(ms, a.CreateEncryptor(), CryptoStreamMode.Read);
            cs.Read(output, 0, output.Length);

            // Close the streams.
            cs.Close();
            ms.Close();

            // All done !!
            return output;
        }

        private static byte[] ShiftLeft(this byte[] input)
        {
            var output = new byte[16];
            for (var i = 0; i < 16; ++i)
            {
                output[i] = (byte)(input[i] << 1);
                if (i > 0 && (input[i] & 0x80) == 0x80)
                {
                    output[i - 1] |= 1;
                }
            }
            return output;
        }

        private static byte[] XOr(this byte[] input, byte[] operand)
        {
            var output = new byte[16];
            for (var i = 0; i < 16; ++i)
            {
                output[i] = (byte)(input[i] ^ operand[i]);
            }
            return output;
        }

        private static byte[] Pad(this byte[] input) 
            => input.Pad(out _);

        private static byte[] Pad(this byte[] input, out bool padded, int requiredLength = -1)
        {
            // Determine the size of the padded buffer and copy the input to it.
            padded = false;
            var paddedSize = requiredLength == -1 
                ? 16 * (1 + (input.Length - 1) / 16) 
                : requiredLength;
            var output = new byte[paddedSize];
            Buffer.BlockCopy(input, 0, output, 0, input.Length);

            // Add the pad character, if necessary.
            if (input.Length < output.Length)
            {
                padded = true;
                output[input.Length] = 0x80;
            }

            // Done!!
            return output;
        }

        private static byte[] TruncateMac(this byte[] cmac)
        {
            // Take the odd bytes (starting at 0) to form the 8-byte CMAC for SAM AV3 authentication
            // operations.
            var result = new byte[8];
            for (var i = 0; i < 8; ++i)
            {
                result[i] = cmac[i * 2 + 1];
            }
            return result;
        }

        public static byte[] Diversify(this byte[] key, byte[] input)
        {
            return input
                .ToDiversificationInput()
                .CMac(key, 32);
        }

        private static byte[] ToDiversificationInput(this byte[] input)
        {
            // Build the diversification input block.
            var output = new byte[input.Length + 1];
            output[0] = 0x01;
            Buffer.BlockCopy(input, 0, output, 1, input.Length);
            return output;
        }

        public static byte[] GenerateMac(this byte[] input, byte[] k1, byte[] k2, byte[] iv)
        {
            // Copy the init vector and pad the input data.
            var initVector = new byte[16];
            Buffer.BlockCopy(iv, 0, initVector, 0, 16);
            var paddedInput = input.Pad();
            
            // Calculate the number of blocks for the MAC generation.
            var blocks = paddedInput.Length / 16;

            // Allocate the input block to be used for the MAC generation.
            var iblock = new byte[16];

            // Process each of the blocks apart from the last one.
            for (var i = 0; i < blocks - 1; ++i)
            {
                Buffer.BlockCopy(paddedInput, i * 16, iblock, 0, 16);
                initVector = iblock.Encrypt(k1, initVector);
            }

            // Process the last block.
            Buffer.BlockCopy(paddedInput, (blocks - 1) * 16, iblock, 0, 16);
            return iblock.Encrypt(k2, initVector);
        }

        public static byte[] Invert(this byte[] input)
        {
            var output = new byte[input.Length];
            
            for (var i = 0; i < input.Length; ++i)
            {
                output[i] = (byte) ~input[i];
            }

            return output;
        }
    }
}