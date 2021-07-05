using CommandLine;
using SteamAuth;
using SteamGuard.Extensions;
using SteamGuard.Options;
using SteamGuard.Providers;
using System;
using System.IO;

namespace SteamGuard
{
    class Program
    {
        public static string DefaultSteamGuardPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "maFiles");

        public static string SteamGuardPath { get; set; } = DefaultSteamGuardPath;
        public static Manifest Manifest { get; set; }
        public static SteamGuardAccount[] SteamAccounts { get; set; }

        static void Main(string[] args)
        {
            //const string overridedArgs = "2fa";
            //args = overridedArgs.Split(' ');

            Parser.Default.ParseArguments(args, OptionsProvider.Types).WithParsed(parsed =>
            {
                var options = (DefaultOptions)(IOptions)parsed;
                if (options.MaFilesPath != null)
                    SteamGuardPath = options.MaFilesPath;

                Manifest = Manifest.GetManifest(true);
                SteamAccounts = Manifest.GetAllAccounts(options.PassKey);

                options.GetController().Execute(options);
            });
        }
    }
}
