using SteamGuard.Options;
using System;

namespace SteamGuard.Controllers
{
    public class DecryptController : ControllerBase<DecryptOptions>
    {
        public override void Execute(DecryptOptions options)
        {
            if (!Program.Manifest.Encrypted)
            {
                Console.WriteLine("Decryption not required.");
                return;
            }

            foreach (var acc in Program.Accounts)
            {
                var success = Program.Manifest.SaveAccount(acc, false);

                if (!success)
                {
                    Console.WriteLine("Unsuccess :(");
                    return;
                }
            }

            Console.WriteLine("Success!");
        }
    }
}
