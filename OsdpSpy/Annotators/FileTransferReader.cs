using System;
using System.Diagnostics;

namespace OsdpSpy.Annotators
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
            Debug.WriteLine("  FileTransferError()");
            Data = null;
            return false;
        }
        
        public bool AddFragment(int size, int offset, byte[] fragment, DateTime timestamp)
        {
            try
            {
                Debug.WriteLine($"  AddFragment(size:{size}, offset:{offset}, ... , timestamp:{timestamp}");
                
                // Start of transfer?
                if (offset == 0 || Data == null)
                {
                    _start = timestamp;
                    Data = new byte[size];
                    _offset = 0;
                    _remaining = size;
                }

                if (offset != _offset) return FileTransferError();
                
                Debug.WriteLine($"  Before: _offset = {_offset}, _remaining = {_remaining}, fragment.Length = {fragment.Length}");

                // The fragment may contain padding from cryptographic operations!
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
                
                Debug.WriteLine($"    After: _remaining = {_remaining}");

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