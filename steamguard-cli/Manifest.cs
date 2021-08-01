using Newtonsoft.Json;
using SteamAuth;
using SteamGuard.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace SteamGuard
{
    public class Manifest
    {
        [JsonProperty("encrypted")]
        public bool Encrypted { get; set; }

        [JsonProperty("first_run")]
        public bool FirstRun { get; set; } = true;

        [JsonProperty("entries")]
        public List<ManifestEntry> Entries { get; set; }

        [JsonProperty("periodic_checking")]
        public bool PeriodicChecking { get; set; } = false;

        [JsonProperty("periodic_checking_interval")]
        public int PeriodicCheckingInterval { get; set; } = 5;

        [JsonProperty("periodic_checking_checkall")]
        public bool CheckAllAccounts { get; set; } = false;

        [JsonProperty("auto_confirm_market_transactions")]
        public bool AutoConfirmMarketTransactions { get; set; } = false;

        [JsonProperty("auto_confirm_trades")]
        public bool AutoConfirmTrades { get; set; } = false;

        public SteamAuth.SteamGuardAccount[] GetAllAccounts(string passKey = null, int limit = -1)
        {
            if (passKey == null && Encrypted) return new SteamGuardAccount[0];

            var accounts = new List<SteamGuardAccount>();
            foreach (var entry in this.Entries)
            {
                var account = GetAccount(entry, passKey);
                if (account == null) continue;
                accounts.Add(account);

                if (limit != -1 && limit >= accounts.Count)
                    break;
            }

            return accounts.ToArray();
        }

        public SteamGuardAccount GetAccount(ManifestEntry entry, string passKey = null)
        {
            string fileText = "";
            RijndaelManaged aes256;

            string filename = Path.Combine(Program.SteamGuardPath, entry.Filename);
            Stream stream;
            if (Encrypted)
            {
                MemoryStream ms = new MemoryStream(Convert.FromBase64String(File.ReadAllText(filename)));
                byte[] key = CryptographyHelper.GetEncryptionKey(passKey, entry.Salt);

                aes256 = new RijndaelManaged
                {
                    IV = Convert.FromBase64String(entry.IV),
                    Key = key,
                    Padding = PaddingMode.PKCS7,
                    Mode = CipherMode.CBC
                };

                ICryptoTransform decryptor = aes256.CreateDecryptor(aes256.Key, aes256.IV);
                stream = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                Console.WriteLine($"Decrypting {filename}...");
            }
            else
            {
                FileStream fileStream = File.OpenRead(filename);
                stream = fileStream;
            }

            using (StreamReader reader = new StreamReader(stream))
            {
                fileText = reader.ReadToEnd();
            }
            stream.Close();

            return JsonConvert.DeserializeObject<SteamGuardAccount>(fileText);
        }

        public bool VerifyPasskey(string passkey)
        {
            if (!this.Encrypted || Entries.Count == 0) return true;

            var accounts = GetAllAccounts(passkey, 1);
            return accounts != null && accounts.Length == 1;
        }

        public bool RemoveAccount(SteamGuardAccount account, bool deleteMaFile = true)
        {
            ManifestEntry entry = Entries.FirstOrDefault(t => t.SteamID == account.Session.SteamID);
            if (entry == null) return true; // If something never existed, did you do what they asked?

            var filename = Path.Combine(Program.SteamGuardPath, entry.Filename);
            Entries.Remove(entry);

            if (Entries.Count == 0)
            {
                Encrypted = false;
            }

            if (Save() && deleteMaFile)
            {
                try
                {
                    File.Delete(filename);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
        }

        public bool SaveAccount(SteamGuardAccount account, bool encrypt, string passKey = null, string salt = null, string iV = null)
        {
            if (encrypt && (String.IsNullOrEmpty(passKey) || String.IsNullOrEmpty(salt) || String.IsNullOrEmpty(iV))) return false;

            string jsonAccount = JsonConvert.SerializeObject(account);

            string filename = account.Session.SteamID.ToString() + ".maFile";
            Console.WriteLine($"Saving account {account.AccountName} to {filename}...");

            ManifestEntry newEntry = new ManifestEntry()
            {
                SteamID = account.Session.SteamID,
                IV = iV,
                Salt = salt,
                Filename = filename
            };

            bool foundExistingEntry = false;
            for (int i = 0; i < this.Entries.Count; i++)
            {
                if (Entries[i].SteamID == account.Session.SteamID)
                {
                    Entries[i] = newEntry;
                    foundExistingEntry = true;
                    break;
                }
            }

            if (!foundExistingEntry)
            {
                this.Entries.Add(newEntry);
            }

            bool wasEncrypted = this.Encrypted;
            Encrypted = encrypt;

            if (!this.Save())
            {
                this.Encrypted = wasEncrypted;
                return false;
            }

            try
            {
                Stream stream = null;
                MemoryStream ms = null;
                RijndaelManaged aes256;

                if (encrypt)
                {
                    ms = new MemoryStream();
                    byte[] key = CryptographyHelper.GetEncryptionKey(passKey, newEntry.Salt);

                    aes256 = new RijndaelManaged
                    {
                        IV = Convert.FromBase64String(newEntry.IV),
                        Key = key,
                        Padding = PaddingMode.PKCS7,
                        Mode = CipherMode.CBC
                    };

                    ICryptoTransform encryptor = aes256.CreateEncryptor(aes256.Key, aes256.IV);
                    stream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
                }
                else
                {
                    // An unencrypted maFile is shorter than the encrypted version,
                    // so when an unencrypted maFile gets written this way, the file does not get wiped
                    // leaving encrypted text after the final } bracket. Deleting and recreating the file fixes this.
                    File.Delete(Path.Combine(Program.SteamGuardPath, newEntry.Filename));
                    stream = File.OpenWrite(Path.Combine(Program.SteamGuardPath, newEntry.Filename)); // open or create
                }

                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(jsonAccount);
                }

                if (encrypt)
                {
                    File.WriteAllText(Path.Combine(Program.SteamGuardPath, newEntry.Filename), Convert.ToBase64String(ms.ToArray()));
                }

                stream.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: {0}", ex.ToString());
                return false;
            }
        }

        public bool Save()
        {
            string filename = Path.Combine(Program.SteamGuardPath, "manifest.json");
            if (!Directory.Exists(Program.SteamGuardPath))
            {
                try
                {
                    Utils.Verbose("Creating {0}", Program.SteamGuardPath);
                    Directory.CreateDirectory(Program.SteamGuardPath);
                }
                catch (Exception ex)
                {
                    Utils.Verbose($"error: {ex.Message}");
                    return false;
                }
            }

            try
            {
                string contents = JsonConvert.SerializeObject(this);
                File.WriteAllText(filename, contents);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error: {ex.Message}");
                return false;
            }
        }

        public void RecomputeExistingEntries()
        {
            var newEntries = new List<ManifestEntry>();

            foreach (var entry in this.Entries)
            {
                string filename = Path.Combine(Program.SteamGuardPath, entry.Filename);

                if (File.Exists(filename))
                {
                    newEntries.Add(entry);
                }
            }

            this.Entries = newEntries;

            if (Entries.Count == 0)
            {
                Encrypted = false;
            }
        }

        public void MoveEntry(int from, int to)
        {
            if (from < 0 || to < 0 || from > Entries.Count || to > Entries.Count - 1) return;
            ManifestEntry sel = Entries[from];
            Entries.RemoveAt(from);
            Entries.Insert(to, sel);
            Save();
        }

        public class ManifestEntry
        {
            [JsonProperty("encryption_iv")]
            public string IV { get; set; }

            [JsonProperty("encryption_salt")]
            public string Salt { get; set; }

            [JsonProperty("filename")]
            public string Filename { get; set; }

            [JsonProperty("steamid")]
            public ulong SteamID { get; set; }
        }
    }
}
