using SteamGuard.Options;
using System;

namespace SteamGuard.Controllers
{
    public class EncryptController : ControllerBase<EncryptOptions>
    {
        public override void Execute(EncryptOptions options)
        {
            string newKey = Program.Manifest.PromptSetupPassKey();

            foreach (var acc in Program.SteamAccounts)
            {
                var salt = Manifest.GetRandomSalt();
                var iv = Manifest.GetInitializationVector();
                bool isSuccess = Program.Manifest.SaveAccount(acc, true, newKey, salt, iv);

                if (!isSuccess)
                {
                    Console.WriteLine("Unsuccess :(");
                    return;
                }
            }

            Console.WriteLine("Success!");
        }
    }
}
