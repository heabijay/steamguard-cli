using CommandLine;
using SteamAuth;
using SteamGuard.Exceptions;
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
            Parser.Default.ParseArguments(args, OptionsProvider.Types).WithParsed(parsed =>
            {
                var options = (DefaultOptions)(IOptions)parsed;
                if (options.MaFilesPath != null)
                    SteamGuardPath = options.MaFilesPath;

                Manifest = Manifest.GetManifest(true);

                if (Manifest.Encrypted)
                {
                    if (string.IsNullOrEmpty(options.PassKey))
                        throw new DecryptPasswordRequiredException();

                    SteamAccounts = Manifest.GetAllAccounts(options.PassKey);
                }
                else
                {
                    SteamAccounts = Manifest.GetAllAccounts();
                }

                options.GetController().Execute(options);
            });
        }
    }
}
