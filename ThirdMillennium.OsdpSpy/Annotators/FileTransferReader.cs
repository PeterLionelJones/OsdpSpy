using System;
using System.Diagnostics;

namespace ThirdMillennium.OsdpSpy
{
    public class FileTransferReader
    {
        public FileTransferReader(int address)
            => Address = address;

        private int _offset;
        private int _remaining;
        private DateTime _start;
        
        public int Address { get; }
        public TimeSpan Elapsed { get; private set; }
        
        public byte[] Data { get; private set; }

        private bool FileTransferError()
        {
            Data = null;
            return false;
        }
        
        public bool AddFragment(int size, int offset, byte[] fragment, DateTime timestamp)
        {
            try
            {
                // Start of transfer?
                if (offset == 0 || Data == null)
                {
                    _start = timestamp;
                    Data = new byte[size];
                    _offset = 0;
                    _remaining = size;
                }

                if (offset != _offset) 
                    return FileTransferError();
                
                Debug.WriteLine($"_offset = {_offset}, _remaining = {_remaining}, fragment.Length = {fragment.Length}");

                // The may contain padding from cryptographic operations!
                var correctedLength = _remaining < fragment.Length 
                    ? _remaining 
                    : fragment.Length;
            
                Buffer.BlockCopy(
                    fragment, 0, 
                    Data, _offset, 
                    correctedLength);
                
                _offset += fragment.Length;
                _remaining -= correctedLength;
                Elapsed = timestamp - _start;

                return _remaining switch
                {
                    0 => true,
                    <0 => FileTransferError(),
                    _ => false
                };
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return FileTransferError();
            }
        }
    }
}