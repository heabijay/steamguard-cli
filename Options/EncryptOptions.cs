using CommandLine;

namespace SteamGuard.Options
{
    [Verb(
        name: "encrypt",
        HelpText = "Encrypt your maFiles or change your encryption passkey."
        )]
    public class EncryptOptions : DefaultOptions
    {
    }
}
