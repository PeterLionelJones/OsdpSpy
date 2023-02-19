using McMaster.Extensions.CommandLineUtils;

namespace OsdpSpy.List;

[Command(Name = "list", Description = "List items")]
[Subcommand(typeof(ListKeysCommand))]
[Subcommand(typeof(ListPortsCommand))]
public class ListCommand
{
    // ReSharper disable once UnusedParameter.Local
    // ReSharper disable once UnusedMember.Local
    private int OnExecute(CommandLineApplication app)
    {
        app.ShowHelp();
        return 1;
    }
}