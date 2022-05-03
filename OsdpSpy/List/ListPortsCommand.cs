using McMaster.Extensions.CommandLineUtils;
using OsdpSpy.Serial;

namespace OsdpSpy.List
{
    [Command(Name = "list", Description = "Lists the serial ports installed on this machine")]
    public class ListPortsCommand
    {
        public ListPortsCommand(IConsole console, ISerialDeviceManager mgr)
        {
            _console = console;
            _mgr = mgr;
        }

        private readonly IConsole _console;
        private readonly ISerialDeviceManager _mgr;
        
        // ReSharper disable once UnusedParameter.Local
        // ReSharper disable once UnusedMember.Local
        private int OnExecute(IConsole console)
        {
            var ports = _mgr.Devices;
            foreach (var port in ports)
            {
                _console.WriteLine($"{port.PortName}");
            }
            return 1;
        }
    }
}