namespace ThirdMillennium.Utility.OSDP
{
    public interface IImportOptions : IFileTransferOptions
    {
        string InputFileName { get; set; }
        bool FilterPollAck { get; set; }
    }
}