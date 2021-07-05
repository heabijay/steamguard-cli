using SteamAuth;
using SteamGuard.Extensions;
using SteamGuard.Options;
using System;

namespace SteamGuard.Controllers
{
    public class GenerateCodeController : ControllerBase<GenerateCodeOptions>
    {
        public override void Execute(GenerateCodeOptions options)
        {
			TimeAligner.AlignTime();
			if (options.Username == null)
            {
                int i = 0;
				foreach (var acc in Program.SteamAccounts)
                {
                    Console.WriteLine($"#{++i} {acc.AccountName}: {acc.GenerateSteamGuardCode()}");
                }
            } 
			else
            {
                Console.WriteLine(options.GetAccount().GenerateSteamGuardCode());
            }
		}
    }
}
