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

        public static string SteamGuardPath { get; private set; } = DefaultSteamGuardPath;
        public static Manifest Manifest { get; private set; }
        public static SteamGuardAccount[] Accounts { get; private set; }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments(args, OptionsProvider.Types).WithParsed(parsed =>
            {
                var options = (DefaultOptions)(IOptions)parsed;
                if (options.MaFilesPath != null)
                    SteamGuardPath = options.MaFilesPath;

                Manifest = ManifestProvider.GetManifest(true);

                if (Manifest.Encrypted)
                {
                    if (string.IsNullOrEmpty(options.PassKey))
                        throw new DecryptPasswordRequiredException();

                    Accounts = Manifest.GetAllAccounts(options.PassKey);
                }
                else
                {
                    Accounts = Manifest.GetAllAccounts();
                }

                options.GetController().Execute(options);
            });
        }
    }
}
