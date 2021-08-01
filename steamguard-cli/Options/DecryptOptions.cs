using CommandLine;

namespace SteamGuard.Options
{
    [Verb(
        name: "decrypt",
        HelpText = "Remove encryption from your maFiles."
        )]
    public class DecryptOptions : DefaultOptions
    {
    }
}
