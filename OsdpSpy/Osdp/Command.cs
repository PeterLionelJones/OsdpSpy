﻿namespace OsdpSpy.Osdp
{
    public enum Command : byte
    {
        POLL = 0x60,
        ID = 0x61,
        CAP = 0x62,
        DIAG = 0x63,
        LSTAT = 0x64,
        ISTAT = 0x65,
        OSTAT = 0x66,
        RSTAT = 0x67,
        OUT = 0x68,
        LED = 0x69,
        BUZ = 0x6A,
        TEXT = 0x6B,
        COMSET = 0x6E,
        DATA = 0x6F,
        PROMPT = 0x71,
        BIOREAD = 0x73,
        BIOMATCH = 0x74,
        KEYSET = 0x75,
        CHLNG = 0x76,
        SCRYPT = 0x77,
        ACURXSIZE = 0x7B,
        FILETRANSFER = 0x7C,
        MFG = 0x80,
        XWR = 0xA1,
        ABORT = 0xA2,
        PIVDATA = 0xA3,
        GENAUTH = 0xA4,
        CRAUTH = 0xA5,
        MFGSTAT = 0xA6,
        KEEPACTIVE = 0xA7
    };
}

