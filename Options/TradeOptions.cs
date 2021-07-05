using CommandLine;

namespace SteamGuard.Options
{
    [Verb(
        name: "trade",
        aliases: new string[] { "setup" },
        HelpText = "Set up Steam Guard for 2 factor authentication."
        )]
    public class TradeOptions : DefaultOptions
    {

    }
}
