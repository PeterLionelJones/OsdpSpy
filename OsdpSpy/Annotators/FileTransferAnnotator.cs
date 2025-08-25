using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OsdpSpy.Abstractions;
using OsdpSpy.Annotations;
using OsdpSpy.Decoders;
using OsdpSpy.Osdp;

namespace OsdpSpy.Annotators;

public class FileTransferAnnotator(
    IFileTransferOptions options,
    IFactory<IAnnotation> factory) : AlertingAnnotator<IExchange>(factory)
{
    private readonly List<FileTransferReader> _readers = new();

    private FileTransferReader FindReader(int address)
    {
        Debug.WriteLine($"  FindReader({address})");
            
        var reader = _readers.FirstOrDefault(r => r.Address == address);

        if (reader == null)
        {
            Debug.WriteLine($"    New File Transfer Reader for Address {address}");
                
            reader = new FileTransferReader(address);
            _readers.Add(reader);
        }
            
        return reader;
    }

    public override void Annotate(IExchange input, IAnnotation output)
    {
        if (input.Acu.Frame.Command != Command.FILETRANSFER) return;
        if (input.Acu.Payload.Plain == null) return;

        Debug.WriteLine("FileTransferAnnotator.Annotate");
            
        var payload = input.Acu.Payload.Plain;
        var fileSize = payload.GetFileSize();
        var fileOffset = payload.GetFileOffset();
        var fragment = payload.GetFragment();
            
        var reader = FindReader(input.Acu.Frame.Address);

        if (reader.AddFragment(fileSize, fileOffset, fragment, input.Acu.Timestamp))
        {
            Debug.WriteLine("  Logging File Transfer");
                
            var alert = this
                .CreateOsdpAlert("Captured File from osdp_FILETRANSFER Commands")
                .AppendItem("TriggeredBy", input.Sequence)
                .AppendItem("FileSize", fileSize, "Bytes")
                .AppendItem("FileTransferTime", reader.Elapsed)
                .AppendItem("TransferRate", fileSize/reader.Elapsed.TotalSeconds)
                .AppendFile(reader.Data);

            if (options.CaptureOsdpFileTransfer)
            {
                var filename = reader.Data.SaveFile(options.OsdpFileTransferDirectory);
                alert = alert.AppendItem("SavedTo", filename);
            }

            alert.AndLogTo(this);
        }
    }
}