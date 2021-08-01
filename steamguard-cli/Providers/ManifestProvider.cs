using Newtonsoft.Json;
using static SteamGuard.Manifest;
using SteamAuth;
using SteamGuard.Controllers;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

namespace SteamGuard.Providers
{
    public static class ManifestProvider
    {
        private static Manifest _manifest;

        public static Manifest GenerateNewManifest(bool scanDir = false)
        {
            Console.WriteLine("Generating new manifest...");

            var newManifest = new Manifest()
            {
                Encrypted = false,
                PeriodicCheckingInterval = 5,
                PeriodicChecking = false,
                AutoConfirmMarketTransactions = false,
                AutoConfirmTrades = false,
                Entries = new List<ManifestEntry>(),
                FirstRun = true
            };

            if (scanDir && Directory.Exists(Program.SteamGuardPath))
            {
                var maFiles = new DirectoryInfo(Program.SteamGuardPath).GetFiles().Where(t => t.Extension == ".maFile");

                foreach (var maFile in maFiles)
                {
                    try
                    {
                        var content = File.ReadAllText(maFile.FullName);
                        SteamGuardAccount account = JsonConvert.DeserializeObject<SteamGuardAccount>(content);
                        var newEntry = new ManifestEntry()
                        {
                            Filename = maFile.Name,
                            SteamID = account.Session.SteamID
                        };
                        newManifest.Entries.Add(newEntry);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("maFile parse exception: {0}", ex.Message);
                    }
                }

                if (newManifest.Entries.Count > 0)
                {
                    newManifest.Save();
                    EncryptController.SetupPassKeyDialog();
                }
            }

            if (newManifest.Save())
                return newManifest;

            return null;
        }


        public static Manifest GetManifest(bool forceLoad = false)
        {
            // Check if already staticly loaded
            if (_manifest != null && !forceLoad)
                return _manifest;

            var manifestFilepath = Path.Combine(Program.SteamGuardPath, "manifest.json");

            // If there's no config dir, create it
            if (!Directory.Exists(Program.SteamGuardPath))
                return _manifest = GenerateNewManifest();

            // If there's no manifest, create it
            if (!File.Exists(manifestFilepath))
            {
                Console.WriteLine("warn: No manifest file found at {0}", manifestFilepath);
                bool isAllowed = Program.SteamGuardPath == Program.DefaultSteamGuardPath;
                if (!isAllowed)
                {
                    Console.Write($"Generate new manifest.json in {Program.SteamGuardPath}? [Y/n]");
                    var answer = Console.ReadLine();
                    isAllowed = !answer.Equals("n");
                }

                if (isAllowed)
                    return _manifest = GenerateNewManifest(true);

                return null;
            }

            try
            {
                string manifestContents = File.ReadAllText(manifestFilepath);
                _manifest = JsonConvert.DeserializeObject<Manifest>(manifestContents);

                if (_manifest.Encrypted && _manifest.Entries.Count == 0)
                {
                    _manifest.Encrypted = false;
                    _manifest.Save();
                }

                _manifest.RecomputeExistingEntries();

                Console.WriteLine($"{_manifest.Entries.Count} accounts loaded.");
                return _manifest;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: Could not open manifest file: {0}", ex.ToString());
                return null;
            }
        }
    }
}
