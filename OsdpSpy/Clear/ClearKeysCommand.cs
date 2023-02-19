using McMaster.Extensions.CommandLineUtils;
using OsdpSpy.Abstractions;

namespace OsdpSpy.Clear;

[Command(Name = "keys", Description = "Clears the keys captured from the OSDP bus")]
public class ClearKeysCommand
{
    public ClearKeysCommand(IConsole console, IKeyStore keyStore)
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
        _keyStore.Clear();
        return 1;
    }
}