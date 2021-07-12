using System;
using System.Diagnostics;

namespace ThirdMillennium.Utility.OSDP
{
    public class FileTransferReader
    {
        public FileTransferReader(int address)
            => Address = address;

        private int _offset;
        private int _remaining;
        
        public int Address { get; }
        
        public byte[] Data { get; private set; }

        private bool FileTransferError()
        {
            Data = null;
            return false;
        }
        
        public bool AddFragment(int size, int offset, byte[] fragment)
        {
            try
            {
                // Start of transfer?
                if (offset == 0 || Data == null)
                {
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