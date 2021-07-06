using SteamGuard.Options;

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
				bool success = Program.Manifest.SaveAccount(acc, true, newKey, salt, iv);

				if (!success)
                {
                    System.Console.WriteLine("Unsuccess :(");
                    return;
                }
			}

            System.Console.WriteLine("Success!");
        }
    }
}
