using System.Collections.Generic;
using System.Linq;
using ThirdMillennium.Annotations;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.Utility.OSDP
{
    public class FileTransferAnnotator : AlertingAnnotator<IExchange>
    {
        public FileTransferAnnotator(
            IFileTransferOptions options,
            IFactory<IAnnotation> factory) : base(factory)
        {
            _options = options;
            _readers = new List<FileTransferReader>();
        }

        private readonly IFileTransferOptions _options;
        private readonly List<FileTransferReader> _readers;

        private FileTransferReader FindReader(int address)
        {
            var reader = _readers.FirstOrDefault(r => r.Address == address);

            if (reader == null)
            {
                reader = new FileTransferReader(address);
                _readers.Add(reader);
            }
            
            return reader;
        }

        public override void Annotate(IExchange input, IAnnotation output)
        {
            if (input.Acu.Frame.Command != Command.FILETRANSFER) return;
            if (input.Acu.Payload.Plain == null) return;

            var payload = input.Acu.Payload.Plain;
            var fileSize = payload.GetFileSize();
            var fileOffset = payload.GetFileOffset();
            var fragment = payload.GetFragment();

            var reader = FindReader(input.Acu.Frame.Address);

            if (reader.AddFragment(fileSize, fileOffset, fragment, input.Acu.Timestamp))
            {
                var alert = this
                    .CreateOsdpAlert("Captured File from osdp_FILETRANSFER Commands")
                    .AppendItem("FileSize", fileSize, "Bytes")
                    .AppendItem("FileTransferTime", reader.Elapsed)
                    .AppendFile(reader.Data);

                if (_options.CaptureOsdpFileTransfer)
                {
                    var filename = reader.Data.SaveFile(_options.OsdpFileTransferDirectory);
                    alert = alert.AppendItem("SavedTo", filename);
                }

                alert.AppendNewLine().Log();
            }
        }
    }
}