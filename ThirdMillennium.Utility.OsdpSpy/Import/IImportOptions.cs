namespace ThirdMillennium.Utility.OSDP
{
    public interface IImportOptions
    {
        string InputFileName { get; set; }
        bool FilterPollAck { get; set; }
    }
}