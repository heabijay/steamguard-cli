using CommandLine;

namespace SteamGuard.Options
{
    [Verb(
        name: "generate-code", 
        isDefault: true, 
        aliases: new string[] { "code", "2fa" }, 
        HelpText = "Generate a Steam Guard code for the specified user (if any) and exit. (default)"
        )]
    public class GenerateCodeOptions : DefaultUserOptions
    {
        [Option('c', "copy", HelpText = "Copies codes to clipboard")]
        public bool IsNeedCopyToClipboard { get; set; }
    }
}
