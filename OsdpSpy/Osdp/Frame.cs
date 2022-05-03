using System;
using System.Diagnostics;
using OsdpSpy.Serial;

namespace OsdpSpy.Osdp
{
    /// <summary>
    ///     The Frame class provides the functionality to build OSDP frames, including AES-128
    /// 	encryption and decryption of Secure Channel frames.
    /// </summary>
    public class Frame : BaseFrame
	{
		//------------------------------------------------------------------------------------------
		//		S T A T I C   C O N S T R U C T O R
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// 	The static Frame constructor initialises the CRCTable used to calculate the CRC
		/// 	value for a frame.
		/// </summary>
		static Frame()
		{
			// Allocate the CRC table.
			CrcTable = new ushort[256];

			// Initialise the CRC table.
			for (var ii = 0; ii < 256; ii++)
			{
				// Calculate the next table value.
				var ww = (ushort)(ii << 8);
				for (var jj = 0; jj < 8; jj++)
				{
					if ((ww & 0x8000) == 0x8000)
					{
						ww = (ushort)((ww << 1) ^ 0x1021);
					}
					else
					{
						ww = (ushort)(ww << 1);
					}
				}

				// Save the value to the table.
				CrcTable[ii] = ww;
			}
		}

		
		//------------------------------------------------------------------------------------------
		//		P R I V A T E   S T A T I C   P R O P E R T I E S
		//------------------------------------------------------------------------------------------

		/// <summary>
		///     The CRC table contains the values used in the CRC calculation for the frame.
		/// </summary>
		private static ushort[] CrcTable { get; set; } = null;

		
		//------------------------------------------------------------------------------------------
		//		C O N S T R U C T O R S
		//------------------------------------------------------------------------------------------

		/// <summary>
		///     This Frame constructor allocates and initialises the frame variables.
		/// </summary>
		public Frame()
		{
			// Allocate and initialise the frame memory.
			FrameData = new byte[MaximumLength];
			FrameData[0] = 0x53;

			// Initialise the security, data and encrypted data blocks.
			SecurityBlock = null;
			Data = null;
			Encrypted = null;
			Session = null;
		}

		/// <summary>
		/// 	This Frame constructor initialises a raw frame to be transmitted using the Circuit's
		/// 	OSDPTransceiveRaw method.
		/// </summary>
		/// <param name="data">
		/// 	A byte array containing the raw frame to be transmitted to the reader under test.
		/// </param>
		public Frame(byte[] data)
		{
			// Copy the data into the frame.
			FrameData = data;

			// Create a security block if this is supposed to be a secure frame.
			if (IsSecure)
			{
				SecurityBlock = new byte[FrameData[5]];
				Buffer.BlockCopy(FrameData, 5, SecurityBlock, 0, SecurityBlock.Length);
			}
	        
			// Set the command based on the data passed.
			CommandReply = FrameData[IsSecure ? 5 + SecurityBlock.Length : 5]; 

			// Add the appropriate check code.
			AddCheck();
		}

        /// <summary>
        ///     The MaximumLength property contains the maximum length of the raw frame data.
        /// </summary>
        public int MaximumLength => 256;

        /// <summary>
        ///     The ConfigurationAddress constant contains the address used to communicate with a
        /// 	single reader on the bus.
        /// </summary>
        public const byte ConfigurationAddress = 0x7F;

        /// <summary>
        ///     The Encrypted property contains the encrypted portion of the data frame.
        /// </summary>
        private byte[] Encrypted { get; set; }
        
        /// <summary>
        ///     The GetWord method takes the two adjacent bytes starting at index and turns them
        /// 	into a word. The data array is in little endian format.
        /// </summary>
        /// <param name="index">
        ///     The index within the raw data frame containing the word value to be returned.
        /// </param>
        /// <returns>
        ///     The little endian word stored at teh specified offset.
        /// </returns>
        private int GetWord(int index)
		{
			int result = FrameData[index + 1];
			result <<= 8;
			result |= FrameData[index];
			return (result);
		}

        /// <summary>
        ///     The SetWord method sets the two adjacent bytes starting at index from the word value
        /// 	in little endian format.
        /// </summary>
        /// <param name="index">
        ///     The index within the raw data frame where the word value should be stored.
        /// </param>
        /// <param name="value">
        ///     The value to be stored in the raw data frame.
        /// </param>
        private void SetWord(int index, int value)
		{
            FrameData[index] = ((byte)(value & 0x00FF));
            FrameData[index + 1] = ((byte)(value >> 8));
		}

        /// <summary>
        ///     The Address property is the address that the frame was addressed to, or received
        /// 	from.
        /// </summary>
        public int Address
		{
			get => (byte)(FrameData[1] & 0x7F);
			set
            {
                byte addr = (byte)(value & 0x7F);
                byte pd = (byte)(IsPd ? 0x80 : 0x00);
                FrameData[1] = (byte)(pd | addr);
            }
		}

        /// <summary>
        ///     The IsCP property returns true if the frame is from a CP, and false if it is from a
        /// 	PD.
        /// </summary>
        public bool IsCp => (FrameData[1] & 0x80) == 0x00;

        /// <summary>
        ///     The IsPD property returns true if the frame is from a PD, and false if it is from a
        /// 	CP.
        /// </summary>
        public bool IsPd
        {
            get => !IsCp;
            set => FrameData[1] = (byte)(value ? FrameData[1] | 0x80 : FrameData[1] & 0x7F);
        }

        public bool GoodMac { get; set; }

        public bool GoodCheck { get; set; }

        /// <summary>
        ///     The FrameLength property is the overall length of the frame, including the check
        /// 	byte(s).
        /// </summary>
        public int FrameLength
		{
			get => GetWord(2);
			set => SetWord(2, value);
		}

        /// <summary>
        ///     The Sequence property is the sequence number of the frame (0..3).
        /// </summary>
        public byte Sequence
		{
			get => (byte)(FrameData[4] & SeqMask);
			set
			{
                FrameData[4] &= InvSeqMask;
                FrameData[4] |= (byte)(value & SeqMask);
			}
		}
        private const byte SeqMask = 0x03;
        private const byte InvSeqMask = 0xFC;

        /// <summary>
        ///     The UseCRC16 property determineS the check method of the frame. If this property is
        /// 	set, or the IsSecure property is set, the frame check method will be CRC16.
        /// 	Otherwise, the Checksum method is used.
        /// </summary>
        public bool UseCRC16
		{
			get => (FrameData[4] & CrcMask) == CrcMask;
			
			set
			{
                FrameData[4] &= InvCrcMask;
				if (value) FrameData[4] |= CrcMask;
			}
		}
        private const byte CrcMask = 0x04;
        private const byte InvCrcMask = 0xFB;

        /// <summary>
        ///     The IsSecure property is used to indicate whether a security block is present (see
        /// 	SecurityBlock). If a security block is present, the check method for the frame will
        /// 	automatically be CRC16.
        /// </summary>
        public bool IsSecure
		{
			get => (FrameData[4] & SecurityMask) == SecurityMask;
			
			set
			{
                FrameData[4] &= InvSecurityMask;
				if (value) FrameData[4] |= SecurityMask;
			}
		}
        private const byte SecurityMask = 0x08;
        private const byte InvSecurityMask = 0xF7;

        /// <summary>
        ///     The SecurityBlock property contains the security block for the frame.
        /// </summary>
        public byte[] SecurityBlock
		{
			get => _security;
			
			set
			{
                _security = value;
                IsSecure = _security != null;
			}
		}
        private byte[] _security;

        /// <summary>
        ///     The CommandReply property contains the command or reply byte of the frame.
        /// </summary>
        public byte CommandReply { get; set; }

        /// <summary>
        ///     The Command property contains the command code for this transmission frame.
        /// </summary>
        public Command Command
        {
            get => (Command) CommandReply;
            set => CommandReply = (byte) value;
        }

        /// <summary>
        ///     The Reply property contains the reply code for this reception frame.
        /// </summary>
        public Reply Reply
        {
            get => (Reply)CommandReply;
            set => CommandReply = (byte) value;
        }

        /// <summary>
        ///     The Data property contains the data associated with the command or reply.
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        ///     The SecureSession property contains the current session values for this frame.
        /// </summary>
        public SecureSession Session { get; set; }

        /// <summary>
        ///     The CalculateChecksum method calculates the expected checksum 8-bit for the frame.
        /// </summary>
        /// <returns>
        ///     The 8-bit checksum that has been calculated for the frame.
        /// </returns>
        private byte CalculateChecksum()
		{
			sbyte sum = 0;
			for (short i = 0; i < FrameLength - 1; ++i)
				sum += (sbyte)FrameData[i];
			return ((byte)-sum);
		}

        /// <summary>
        ///     The AddChecksum method adds an 8-bit checksum to a frame for transmission.
        /// </summary>
        private void AddChecksum()
		{
            FrameData[FrameLength - 1] = CalculateChecksum();
		}

        /// <summary>
        ///     The VerifyChecksum method verifies the checksum of a received frame.
        /// </summary>
        /// <returns>
        ///     Returns true if the frame has a valid checksum. Otherwise, returns false.
        /// </returns>
        private bool VerifyChecksum()
		{
			byte cs = CalculateChecksum();
			return cs == FrameData[FrameLength - 1];
		}

        /// <summary>
        ///     The CalculateCRC16 method calculates the expected 16-bit CRC for a frame.
        /// </summary>
        /// <returns>
        ///     The 16-bit CRC that has been calculated for the frame.
        /// </returns>
        private ushort CalculateCrc16()
		{
			ushort crc = 0x1D0F;
			for (short i = 0; i < FrameLength - 2; i++)
			{
				crc = (ushort)((ushort)(crc << 8) ^ CrcTable[((crc >> 8) ^ FrameData[i]) & 0xFF]);
			}
			return (crc);
		}

        /// <summary>
        ///     The AddCRC16 method adds a 16-bit CRC to a frame for transmission.
        /// </summary>
        private void AddCrc16()
		{
			ushort crc = CalculateCrc16();
            FrameData[FrameLength - 2] = (byte)(crc & 0xFF);
            FrameData[FrameLength - 1] = (byte)(crc >> 8);
		}

        /// <summary>
        ///     The VerifyCRC16 method verifies the 16-bit CRC in a received frame.
        /// </summary>
        /// <returns>
        ///     Returns true if the received frame has a valid 16-bit CRC.
        /// </returns>
        private bool VerifyCrc16()
		{
			// Get the CRC from the frame.
			ushort crc =
				(ushort)(FrameData[FrameLength - 2] |
				(ushort)(FrameData[FrameLength - 1] << 8));

			// Verify the received CRC.
			return crc == CalculateCrc16();
		}

        /// <summary>
        ///     The AddCheck method adds the appropriate check byte(s) to a frame for transmission.
        /// </summary>
        private void AddCheck()
		{
            // Do we need a 16-bit CRC?
			if (UseCRC16)
			{
                // Adda 16-bit CRC.
				AddCrc16();
			}
			else
			{
                // Add an 8-bit checksum.
				AddChecksum();
			}
		}

        /// <summary>
        ///     The VerifyCheck method verifies the appropriate check byte(s) in a received frame.
        /// </summary>
        /// <returns>
        ///     Returns true if the frame has the appropriate check code. Otherwise, returns false.
        /// </returns>
        private bool VerifyCheck() => UseCRC16 ? VerifyCrc16() : VerifyChecksum();

        /// <summary>
        ///     The HasMAC read-only property returns true if a 4-byte MAC is present 
        ///     in the frame.
        /// </summary>
        public bool HasMac
		{
            get
            {
                try
                {
                    // No MAC required if no security block!
                    if (SecurityBlock == null)
                    {
                        return false;
                    }

                    //  Determine if a MAC is required based on the security 
                    //  block type.
                    switch ((SecurityBlockType)SecurityBlock[1])
                    {
                        case SecurityBlockType.SCS_15:
                        case SecurityBlockType.SCS_16:
                        case SecurityBlockType.SCS_17:
                        case SecurityBlockType.SCS_18:
                            return true;

                        default:
                            break;
                    }

                    // No MAC required for this frame.
                    return false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return false;
                }
            }
        }
        
        /// <summary>
        /// 	The InjectBadMAC property is set by a unit test when the test requires a bad MAC to
        /// 	be sent to a reader in secure channel.
        /// </summary>
        public bool InjectBadMac { get; set; }

        /// <summary>
        ///     The GenerateMAC method calculates the MAC block for the frame.
        /// </summary>
        /// <returns>
        ///     Returns a CipherBlock containing the MAC corresponding the data in the frame.
        /// </returns>
        private CipherBlock GenerateMac()
		{
			// Calculate the length of data required for the MAC generation.
			int length = FrameLength - 6;
			int padded = 16 * (1 + ((length - 1) / 16));

			// Allocate the buffer and copy the data for MAC generation.
			byte[] input = new byte[padded];
			for (int i = 0; i < length; ++i)
			{
				input[i] = FrameData[i];
			}
			if (padded != length)
			{
				input[length] = 0x80;
				for (int i = length + 1; i < padded; ++i)
				{
					input[i] = 0x00;
				}
			}

			// Calculate the MAC.
			var result = CipherBlock.GenerateMac(input, Session.Smac1, Session.Smac2, Session.Mac);

			// Has bad MAC injection been requested?
			if (InjectBadMac)
			{
				// Invert the correct MAC to create a bad MAC for this frame.
				InjectBadMac = false;
				result = result.Invert();
			}

			// Done!!
			return result;
		}

        /// <summary>
        /// 	The MAC property retrieves the MAC of the frame.
        /// </summary>
        public byte[] Mac
        {
            get
            {
                if (!HasMac) return null;
                var mac = new byte[4];
                Buffer.BlockCopy(FrameData, FrameLength - 6, mac, 0, 4);
                return mac;
            }
        }

        /// <summary>
        ///     The Assemble method builds a frame for transmission.
        /// </summary>
        public override void Assemble()
		{
            // Add the start of message character.
            FrameData[0] = 0x53;

			// Set the initial length.
			FrameLength = 5;

			// Should the frame have a security block?
			if ((SecurityBlock == null) && (Session != null))
			{
                SecurityBlock = new byte[2]
                {
                    2,
                    (byte)((Data == null) 
                        ? (IsCp ? SecurityBlockType.SCS_15 : SecurityBlockType.SCS_16) 
                        : (IsCp ? SecurityBlockType.SCS_17 : SecurityBlockType.SCS_18))
                };
			}

			// Does this frame have a security block?
			if (SecurityBlock != null)
			{
				for (int i = 0; i < SecurityBlock.Length; ++i)
				{
                    FrameData[FrameLength + i] = SecurityBlock[i];
				}
				FrameLength += SecurityBlock.Length;
				IsSecure = true;
				UseCRC16 = true;
			}

            // Add the command byte.
            FrameData[FrameLength++] = CommandReply;

			// Add the data frame, updating the length.
			if (Data != null)
			{
				// Do we need to encrypt the data block?
				if (SecurityBlock != null && SecurityBlock[1] == (byte)SecurityBlockType.SCS_17)
				{
					// Encrypt the data block and load it into the frame.
					var iv = Session.Mac.Invert();
                    Encrypted = CipherBlock.Encrypt(Data, iv, Session.Enc);

                    // Load the encrypted data in to the frame.
                    for (var i = 0; i < Encrypted.Length; ++i)
					{
                        FrameData[FrameLength + i] = Encrypted[i];
					}
					FrameLength += Encrypted.Length;
				}
				else
				{
					// Load the plain text into the frame.
					for (var i = 0; i < Data.Length; ++i)
					{
                        FrameData[FrameLength + i] = Data[i];
					}
					FrameLength += Data.Length;
				}
			}

			// Add the MAC?
			if (HasMac)
			{
				// Update the frame length.
				FrameLength += 6;

				// Generate the MAC and add it to the frame.
				var mac = GenerateMac();
				for (int i = 0; i < 4; ++i)
				{
                    FrameData[FrameLength - 6 + i] = mac.Data[i];
				}

				// Update the session MAC.
				Session.Mac = mac;
			}
			else
			{
				// Update the frame length.
				++FrameLength;
				if (UseCRC16)
				{
					++FrameLength;
				}
			}

            // Set the check character.
            AddCheck();

            // Truncate the frame.
            byte[] tx = new byte[FrameLength];
            Buffer.BlockCopy(FrameData, 0, tx, 0, FrameLength);
            FrameData = tx;
		}

        /// <summary>
        ///     The Disassemble method unpacks a received frame into its component parts.
        /// </summary>
        /// <returns>
        ///     Returns true if the frame was disassembled successfully. Otherwise, returns false.
        /// </returns>
        public override bool Disassemble()
		{
            try
            {
                var offset = 5;
                var dlen = FrameLength - 5;

                // Is this a secure frame.
                if (IsSecure)
                {
                    // Get the security block.
                    int length = FrameData[offset];
                    SecurityBlock = new byte[length];
                    Buffer.BlockCopy(FrameData, offset, SecurityBlock, 0, length);
                    offset += length;
                    dlen -= length;
                }

                // Get the command/reply code.
                CommandReply = FrameData[offset++];
                dlen--;

                //  Determine if there is a MAC present in the frame and adjust the
                //  data length accordingly.
                if (IsSecure)
                {
                    // Is a MAC present in the frame?
                    if (HasMac)
                    {
                        dlen -= 6;
                    }
                    else
                    {
                        dlen -= 2;
                    }
                }
                else
                {
                    dlen--;
                    if (UseCRC16)
                    {
                        dlen--;
                    }
                }

                // Build the data frame.
                if (dlen > 0)
                {
                    // Do we need to decrypt the data block?
                    if ((Session != null) && (SecurityBlock != null) &&
                            ((SecurityBlock[1] == (byte)SecurityBlockType.SCS_17) || (SecurityBlock[1] == (byte)SecurityBlockType.SCS_18)))
                    {
                        // Get the data to be decrypted.
                        Encrypted = new byte[dlen];
                        Buffer.BlockCopy(FrameData, offset, Encrypted, 0, dlen);

                        // Decrypt the data.
                        var iv = Session.Mac.Invert();
                        Data = CipherBlock.Decrypt(Encrypted, iv, Session.Enc);
                    }
                    else
                    {
                        // Allocate the data buffer and copy the data from the frame..
                        Data = new byte[dlen];
                        Buffer.BlockCopy(FrameData, offset, Data, 0, dlen);
                    }
                }

                // Verify the checksum or CRC.
                GoodCheck = VerifyCheck();

                // Does this frame have a MAC. Can only check and update the session 
                // MAC if a session is active.
                if ((Session != null) && HasMac)
                {
                    // Generate the MAC for this frame based on the current 
                    // session data.
                    var mac = GenerateMac();

                    // Check the MAC.
                    for (var i = 0; i < 4; ++i)
                    {
                        if (FrameData[FrameLength - 6 + i] != mac.Data[i])
                        {
                            // The MAC does not match . . .                          
                            return GoodMac = false;
                        }
                    }

                    // Update the session MAC.
                    GoodMac = true;
                    Session.Mac = mac;
                }

                // Verify the checksum/CRC and we have successfully disassembled the 
                // frame.
                return GoodCheck;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
		}

        /// <summary>
        /// 	The TraceString property presents the Frame in human readable form.
        /// </summary>
        public virtual string TraceString 
	        => IsCp
		        ? $"\nosdp_{Command}\nTX: {this}"
		        : $"RX: {this}";

        /// <summary>
        ///     Translates the frame to a string representation.
        /// </summary>
        /// <returns>
        ///     A string representation of the frame.
        /// </returns>
        public override string ToString() 
        { 
            // Get the basic trace to dump.
            var result = BitConverter
	            .ToString(FrameData, FrameLength)
	            .Replace('-', ' ');

            // If this is a transmission frame we are done.
            if (IsCp) return result;

            // What kind of check code are we using?
            var check = UseCRC16 ? "CRC" : "Checksum";

            if (!HasMac) GoodMac = true;
            if (!GoodMac || !GoodCheck) result += " <- ";
            if (!GoodMac) result += "Bad MAC";
            if (!GoodMac && !GoodCheck) result += ", ";
            if (!GoodCheck) result += $"Bad {check}";

            return result;
        }
	}
}
