using SteamAuth;
using SteamGuard.Providers;
using System;
using System.Linq;

namespace SteamGuard
{
    class Program
    {
        public const string defaultSteamGuardPath = "~/maFiles";

        public static string SteamGuardPath { get; set; } = defaultSteamGuardPath;
        public static Manifest Manifest { get; set; }
        public static SteamGuardAccount[] SteamGuardAccounts { get; set; }
        public static bool Verbose { get; set; } = false;

        static void Main(string[] args)
        {
            args = "".Split(' ');

            Console.WriteLine(string.Join("\n", ControllersProvider.Types.Select(t => t.FullName)));
            Console.WriteLine(string.Join("\n", OptionsProvider.Types.Select(t => t.FullName)));

            //var parsed = Parser.Default.ParseArguments(args, filtered);
        }
    }
}
