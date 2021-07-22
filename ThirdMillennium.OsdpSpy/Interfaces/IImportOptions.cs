namespace ThirdMillennium.OsdpSpy
{
    public interface IImportOptions : IFileTransferOptions
    {
        string InputFileName { get; set; }
        bool FilterPollAck { get; set; }
    }
}