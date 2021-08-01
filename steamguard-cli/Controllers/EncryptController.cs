using SteamGuard.Helpers;
using SteamGuard.Options;
using System;

namespace SteamGuard.Controllers
{
    public class EncryptController : ControllerBase<EncryptOptions>
    {
        public override void Execute(EncryptOptions options)
        {
            string newKey = SetupPassKeyDialog();

            foreach (var acc in Program.Accounts)
            {
                var salt = CryptographyHelper.GetRandomSalt();
                var iv = CryptographyHelper.GetInitializationVector();
                bool isSuccess = Program.Manifest.SaveAccount(acc, true, newKey, salt, iv);

                if (!isSuccess)
                {
                    Console.WriteLine("Unsuccess :(");
                    return;
                }
            }

            Console.WriteLine("Success!");
        }

        public static string SetupPassKeyDialog()
        {
            string passKey;
            do
            {
                Console.Write("Enter passkey: ");
                passKey = ConsoleHelper.SecureReadLine();
                Console.Write("Confirm passkey: ");
                var confirmPassKey = ConsoleHelper.SecureReadLine();

                if (passKey.Equals(confirmPassKey) && !string.IsNullOrEmpty(passKey))
                    break;

                Console.WriteLine("Passkeys do not match.");
            } while (true);

            return passKey;
        }
    }
}
