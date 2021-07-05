using CommandLine;

namespace SteamGuard.Options
{
    public class DefaultUserOptions : DefaultOptions
    {
        [Option('s', "steam-username", HelpText = "Selected steam username.")]
        public string Username { get; set; }
    }
}
