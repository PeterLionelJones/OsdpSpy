namespace ThirdMillennium.Utility.OSDP
{
    public class ImportOptions : IImportOptions
    {
        public string InputFileName { get; set; }
        public bool FilterPollAck { get; set; }
    }
}