using SteamAuth;
using SteamGuard.Extensions;
using SteamGuard.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using TextCopy;

namespace SteamGuard.Controllers
{
    public class GenerateCodeController : ControllerBase<GenerateCodeOptions>
    {
        public override void Execute(GenerateCodeOptions options)
        {
            Console.Write("Aligning time...");
            TimeAligner.AlignTime();
            Console.WriteLine(" Success!");

            Dictionary<string, string> requestedCodes;
            if (options.Username != null)
            {
                var acc = options.GetAccount();

                requestedCodes = new()
                {
                    { acc.AccountName, acc.GenerateSteamGuardCode() }
                };
            }
            else requestedCodes = Program.Accounts.ToDictionary(t => t.AccountName, t => t.GenerateSteamGuardCode());

            Console.WriteLine($"Generated codes ({requestedCodes.Count}): ");

            var usernameMaxLength = requestedCodes?.Keys?.Max(t => t.Length);
            foreach (var code in requestedCodes)
                Console.WriteLine("{0,-" + usernameMaxLength + "}: {1}", code.Key, code.Value);

            if (options.IsNeedCopyToClipboard && requestedCodes?.Count > 0)
                ClipboardService.SetText(string.Join(',', requestedCodes.Select(t => t.Value)));
        }
    }
}
