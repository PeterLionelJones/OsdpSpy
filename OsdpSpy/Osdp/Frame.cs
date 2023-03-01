using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OsdpSpy.Osdp;

public class Crc
{
	static Crc()
	{
		// Allocate the CRC table.
		Table = new ushort[256];

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
			Table[ii] = ww;
		}
	}
	
	public static ushort[] Table { get; }
}

/// <summary>
///     The Frame class provides the functionality to build OSDP frames, including AES-128
/// 	encryption and decryption of Secure Channel frames.
/// </summary>
public class Frame
{
	//------------------------------------------------------------------------------------------
	//		P R I V A T E   S T A T I C   P R O P E R T I E S
	//------------------------------------------------------------------------------------------

	/// <summary>
	///     The CRC table contains the values used in the CRC calculation for the frame.
	/// </summary>
	private static ushort[] CrcTable = Crc.Table;

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
	/// 	This Frame constructor initialises from a raw frame.
	/// </summary>
	/// <param name="data">
	/// 	A byte array containing the raw frame to be transmitted to the reader under test.
	/// </param>
	public Frame(byte[] data)
	{
		FrameData = data;
		Disassemble();
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
	///     FrameData contains the raw data for the frame.
	/// </summary>
	public byte[] FrameData { get; protected set; }
        
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
	///     The Address property is the address that the frame was addressed to, or received
	/// 	from.
	/// </summary>
	public int Address => (byte)(FrameData[1] & 0x7F);

	/// <summary>
	///     The IsCP property returns true if the frame is from a CP, and false if it is from a
	/// 	PD.
	/// </summary>
	public bool IsAcu => (FrameData[1] & 0x80) == 0x00;

	/// <summary>
	///     The IsPD property returns true if the frame is from a PD, and false if it is from a
	/// 	CP.
	/// </summary>
	public bool IsPd => !IsAcu;

	public bool GoodMac { get; private set; }

	public bool GoodCheck { get; private set; }

	/// <summary>
	///     The FrameLength property is the overall length of the frame, including the check
	/// 	byte(s).
	/// </summary>
	public int FrameLength => GetWord(2);

	/// <summary>
	///     The Sequence property is the sequence number of the frame (0..3).
	/// </summary>
	public byte Sequence => (byte)(FrameData[4] & SeqMask);
	private const byte SeqMask = 0x03;

	/// <summary>
	///     The UseCRC16 property determineS the check method of the frame. If this property is
	/// 	set, or the IsSecure property is set, the frame check method will be CRC16.
	/// 	Otherwise, the Checksum method is used.
	/// </summary>
	public bool UseCrc16 => (FrameData[4] & CrcMask) == CrcMask;
	private const byte CrcMask = 0x04;

	/// <summary>
	///     The IsSecure property is used to indicate whether a security block is present (see
	/// 	SecurityBlock). If a security block is present, the check method for the frame will
	/// 	automatically be CRC16.
	/// </summary>
	public bool IsSecure => (FrameData[4] & SecurityMask) == SecurityMask;
	private const byte SecurityMask = 0x08;

	/// <summary>
	///     The SecurityBlock property contains the security block for the frame.
	/// </summary>
	public byte[] SecurityBlock { get; private set; }

	/// <summary>
	///     The CommandReply property contains the command or reply byte of the frame.
	/// </summary>
	private byte CommandReply { get; set; }

	/// <summary>
	///     The Command property contains the command code for this transmission frame.
	/// </summary>
	public Command Command => (Command) CommandReply;

	/// <summary>
	///     The Reply property contains the reply code for this reception frame.
	/// </summary>
	public Reply Reply => (Reply) CommandReply;

	/// <summary>
	///     The Data property contains the data associated with the command or reply.
	/// </summary>
	public byte[] Data { get; private set; }

	/// <summary>
	///     The SecureSession property contains the current session values for this frame.
	/// </summary>
	private SecureSession Session { get; set; }

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
	///     The VerifyChecksum method verifies the checksum of a received frame.
	/// </summary>
	/// <returns>
	///     Returns true if the frame has a valid checksum. Otherwise, returns false.
	/// </returns>
	private bool VerifyChecksum()
	{
		var cs = CalculateChecksum();
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
		return crc;
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
		var crc = (ushort)(FrameData[FrameLength - 2] | (ushort)(FrameData[FrameLength - 1] << 8));

		// Verify the received CRC.
		return crc == CalculateCrc16();
	}

	/// <summary>
	///     The VerifyCheck method verifies the appropriate check byte(s) in a received frame.
	/// </summary>
	/// <returns>
	///     Returns true if the frame has the appropriate check code. Otherwise, returns false.
	/// </returns>
	private bool VerifyCheck() => UseCrc16 ? VerifyCrc16() : VerifyChecksum();

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
						return false;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				return false;
			}
		}
	}
        
	/// <summary>
	///     The GenerateMAC method calculates the MAC block for the frame.
	/// </summary>
	/// <returns>
	///     Returns a CipherBlock containing the MAC corresponding the data in the frame.
	/// </returns>
	private CipherBlock GenerateMac()
	{
		// Calculate the length of data required for the MAC generation.
		var length = FrameLength - 6;
		var padded = 16 * (1 + ((length - 1) / 16));

		// Allocate the buffer and copy the data for MAC generation.
		var input = new byte[padded];
		for (var i = 0; i < length; ++i)
		{
			input[i] = FrameData[i];
		}
			
		if (padded != length)
		{
			input[length] = 0x80;
			for (var i = length + 1; i < padded; ++i)
			{
				input[i] = 0x00;
			}
		}

		// Calculate the MAC.
		return CipherBlock.GenerateMac(input, Session.Smac1, Session.Smac2, Session.Mac);
	}

	/// <summary>
	/// 	The MAC property retrieves the MAC of the frame.
	/// </summary>
	public IEnumerable<byte> Mac
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
	///     The Disassemble method unpacks a received frame into its component parts.
	/// </summary>
	/// <returns>
	///     Returns true if the frame was disassembled successfully. Otherwise, returns false.
	/// </returns>
	public void Disassemble()
	{
		try
		{
			var offset = 5;
			var dataLength = FrameLength - 5;

			// Is this a secure frame.
			if (IsSecure)
			{
				// Get the security block.
				var length = FrameData[offset];
				SecurityBlock = new byte[length];
				Buffer.BlockCopy(FrameData, offset, SecurityBlock, 0, length);
				offset += length;
				dataLength -= length;
			}

			// Get the command/reply code.
			CommandReply = FrameData[offset++];
			dataLength--;

			//  Determine if there is a MAC present in the frame and adjust the
			//  data length accordingly.
			if (IsSecure)
			{
				// Is a MAC present in the frame?
				if (HasMac)
				{
					dataLength -= 6;
				}
				else
				{
					dataLength -= 2;
				}
			}
			else
			{
				dataLength--;
				if (UseCrc16)
				{
					dataLength--;
				}
			}

			// Build the data frame.
			if (dataLength > 0)
			{
				// Do we need to decrypt the data block?
				if (Session != null && SecurityBlock != null &&
				    (SecurityBlock[1] == (byte)SecurityBlockType.SCS_17 || SecurityBlock[1] == (byte)SecurityBlockType.SCS_18))
				{
					// Get the data to be decrypted.
					Encrypted = new byte[dataLength];
					Buffer.BlockCopy(FrameData, offset, Encrypted, 0, dataLength);

					// Decrypt the data.
					var iv = Session.Mac.Invert();
					Data = CipherBlock.Decrypt(Encrypted, iv, Session.Enc);
				}
				else
				{
					// Allocate the data buffer and copy the data from the frame..
					Data = new byte[dataLength];
					Buffer.BlockCopy(FrameData, offset, Data, 0, dataLength);
				}
			}

			// Verify the checksum or CRC.
			GoodCheck = VerifyCheck();

			// Does this frame have a MAC. Can only check and update the session 
			// MAC if a session is active.
			if (Session != null && HasMac)
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
						GoodMac = false;
						return;
					}
				}

				// Update the session MAC.
				GoodMac = true;
				Session.Mac = mac;
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex.Message);
		}
	}

	/// <summary>
	/// 	The TraceString property presents the Frame in human readable form.
	/// </summary>
	public virtual string TraceString 
		=> IsAcu
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
			.ToString(FrameData)
			.Replace('-', ' ');

		// What kind of check code are we using?
		var check = UseCrc16 ? "CRC" : "Checksum";
		if (!GoodCheck) result += $" <- Bad {check}";

		return result;
	}
}