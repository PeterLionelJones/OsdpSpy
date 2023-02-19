using McMaster.Extensions.CommandLineUtils;
using OsdpSpy.Abstractions;

namespace OsdpSpy.List;

[Command(Name = "keys", Description = "Lists the keys captured from the OSDP bus")]
public class ListKeysCommand
{
    public ListKeysCommand(IConsole console, IKeyStore keyStore)
    {
        _console = console;
        _keyStore = keyStore;
    }

    private readonly IConsole _console;
    private readonly IKeyStore _keyStore;
        
    // ReSharper disable once UnusedParameter.Local
    // ReSharper disable once UnusedMember.Local
    private int OnExecute(IConsole console)
    {
        _keyStore.List();
        return 1;
    }
}