using SteamGuard.Options;

namespace SteamGuard.Controllers
{
    public class DecryptController : ControllerBase<DecryptOptions>
    {
        public override void Execute(DecryptOptions options)
        {
			if (!Program.Manifest.Encrypted)
			{
				System.Console.WriteLine("Decryption not required.");
				return;
			}

			foreach (var acc in Program.SteamAccounts)
            {
				var success = Program.Manifest.SaveAccount(acc, false);

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
