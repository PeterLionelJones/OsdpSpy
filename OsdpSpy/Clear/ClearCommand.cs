using McMaster.Extensions.CommandLineUtils;

namespace OsdpSpy.Clear;

[Command(Name = "clear", Description = "Clear items")]
[Subcommand(typeof(ClearKeysCommand))]
public class ClearCommand
{
    // ReSharper disable once UnusedParameter.Local
    // ReSharper disable once UnusedMember.Local
    private int OnExecute(CommandLineApplication app)
    {
        app.ShowHelp();
        return 1;
    }
}