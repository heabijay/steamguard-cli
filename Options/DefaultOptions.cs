using CommandLine;

namespace SteamGuard.Options
{
    public class DefaultOptions : IOptions
    {
        [Option('m', "mafiles-path", HelpText = "Input file to read.")]
        public string InputFile { get; set; }


        [Option('p', "pass-key", HelpText = "Specify your encryption passkey.")]
        public string PassKey { get; set; }
    }
}
