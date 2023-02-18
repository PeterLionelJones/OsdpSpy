using System;
using System.Linq;

using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("OsdpSpy.Tests")]

namespace OsdpSpy;

internal static class ArgsExtensions
{
    private static string GetOption(this string[] args, string[] verbs, string[] switches)
    {
        try
        {
            var validVerb = verbs.Any(x => x.ToLower() == args[0].ToLower());
            if (!validVerb) return null;

            for (var i = 1; i < args.Length - 1; ++i)
            {
                var validSwitch = switches.Any(x => x == args[i]);
                if (validSwitch) return args[i + 1];
            }

            return null;
        }
        catch (IndexOutOfRangeException)
        {
            return null;
        }
    }

    public static string SeqUrl(this string[] args)
        => args.GetOption(new[] {"listen"}, new[] {"-s", "--seq"});

    public static string ElasticsearchUrl(this string[] args)
        => args.GetOption(new[] {"listen"}, new[] {"-e", "--elasticsearch"});
}