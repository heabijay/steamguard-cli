using CommandLine;

namespace SteamGuard.Options
{
    [Verb(
        name: "add",
        aliases: new string[] { "setup" },
        HelpText = "Set up Steam Guard for 2 factor authentication."
        )]
    public class AddOptions : DefaultUserOptions
    {
    }
}
