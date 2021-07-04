using System;
using System.Linq;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.Utility.OSDP
{
    public class Session
    {
        private byte[] Scbk { get; } = new byte[16];
        private byte[] RndA { get; } = new byte[8];
        private byte[] RndB { get; } = new byte[8];
        public byte[] ClientCryptogram { get; private set; }
        public byte[] ServerCryptogram { get; private set; }
        private byte[] SEnc { get; set; }
        public byte[] SMac1 { get; private set; }
        public byte[] SMac2 { get; private set; }
        private byte[] Mac { get; set; }

        private byte[] GenerateMac(Frame frame)
        {
            // Extract the data from the frame for the MAC.
            var macInput = new byte[frame.FrameLength - 6];
            Buffer.BlockCopy(
                frame.FrameData, 0, macInput, 0, macInput.Length);
            
            // Generate the MAC.
            return macInput.GenerateMac(SMac1, SMac2, Mac);
        }

        public void Generate(byte[] scbk, byte[] rndA, byte[] rndB)
        {
            // Set the base key and random numbers for this session.
            Buffer.BlockCopy(scbk, 0, Scbk, 0, 16);
            Buffer.BlockCopy(rndA, 0, RndA, 0, 8);
            Buffer.BlockCopy(rndB, 0, RndB, 0, 8);
            
            // Generate the S-ENC.
            var enc = new byte[16];
            enc[0] = 0x01;
            enc[1] = 0x82;
            Buffer.BlockCopy(rndA, 0, enc, 2, 6);
            SEnc = enc.Encrypt(scbk);

            // Generate the S-MAC1.
            var mac1 = new byte[16];
            mac1[0] = 0x01;
            mac1[1] = 0x01;
            Buffer.BlockCopy(rndA, 0, mac1, 2, 6);
            SMac1 = mac1.Encrypt(scbk);

            // Generate the S-MAC2.
            var mac2 = new byte[16];
            mac2[0] = 0x01;
            mac2[1] = 0x02;
            Buffer.BlockCopy(rndA, 0, mac2, 2, 6);
            SMac2 = mac2.Encrypt(scbk);
            
            // Don't set the RMAC until the second step of the authentication has been completed
            // successfully.
            Mac = null;

            // Generate the client cryptogram.
            var ccryptI = new byte[16];
            Buffer.BlockCopy(RndA, 0, ccryptI, 0, 8);
            Buffer.BlockCopy(RndB, 0, ccryptI, 8, 8);
            ClientCryptogram = ccryptI.Encrypt(SEnc);

            // Generate the server cryptogram.
            var scryptI = new byte[16];
            Buffer.BlockCopy(RndB, 0, scryptI, 0, 8);
            Buffer.BlockCopy(RndA, 0, scryptI, 8, 8);
            ServerCryptogram = scryptI.Encrypt(SEnc);
        }

        public void SetInitialRMac(byte[] rmac)
        {
            Mac = rmac;
        }

        public bool ProcessFrame(IFrameProduct product)
        {
            // Does the frame have a MAC?
            if (!product.Frame.HasMac) return true;
            
            // What is the correct mac for this frame?
            var calculatedMac = GenerateMac(product.Frame);
            if (calculatedMac == null) return false;
            
            // Do we have a correct MAC?
            var calculatedFrameMac = new byte[4];
            Buffer.BlockCopy(calculatedMac, 0, calculatedFrameMac, 0, 4);
            var macMatched = product.Frame.Mac.SequenceEqual(calculatedFrameMac);
            if (!macMatched)
                return false;
            
            // Do we have data to decrypt?
            if (product.Payload.Cipher != null)
            {
                // Decrypt the data.
                var initVector = Mac.Invert();
                product.Payload.Plain = product.Payload.Cipher.Decrypt(SEnc, initVector);
            }
            
            // Update the MAC.
            Mac = calculatedMac;
            
            return true;
        }
    }
}