namespace OsdpSpy.Osdp
{
    public enum Reply : byte
    {
        ACK = 0x40,
        NAK = 0x41,
        PDID = 0x45,
        PDCAP = 0x46,
        LSTATR = 0x48,
        ISTATR = 0x49,
        OSTATR = 0x4A,
        RSTATR = 0x4B,
        RAW = 0x50,
        FMT = 0x51,
        PRES = 0x52,
        KEYPAD = 0x53,
        COMSET = 0x54,
        SCREP = 0x55,
        SPER = 0x56,
        BIOREADR = 0x57,
        BIOMATCHR = 0x58,
        CCRYPT = 0x76,
        RMAC_I = 0x78,
        BUSY = 0x79,
        FTSTAT = 0x7A,
        PIVDATAR = 0x80,
        GENAUTHR = 0x81,
        CRAUTHR = 0x82,
        MFGSTATR = 0x83,
        MFGERRR = 0x84,
        MFGREP = 0x90,
        XRD = 0xB1
    };
}

