using McMaster.Extensions.CommandLineUtils;
using OsdpSpy.Abstractions;

namespace OsdpSpy.Clear;

[Command(Name = "keys", Description = "Clears the keys captured from the OSDP bus")]
public class ClearKeysCommand(IConsole console, IKeyStore keyStore)
{
    private readonly IConsole _console = console;

    // ReSharper disable once UnusedParameter.Local
    // ReSharper disable once UnusedMember.Local
    private int OnExecute(IConsole console)
    {
        keyStore.Clear();
        return 1;
    }
}