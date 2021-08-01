using SteamAuth;
using SteamGuard.Exceptions;
using SteamGuard.Options;
using System.Linq;

namespace SteamGuard.Extensions
{
    public static class OptionsAccountExtensions
    {
        public static SteamGuardAccount GetAccount(this DefaultUserOptions userOptions)
        {
            var acc = Program.Accounts.FirstOrDefault(
                t => t.AccountName.Equals(userOptions.Username, System.StringComparison.OrdinalIgnoreCase
                    )
                );

            if (acc == null)
                throw new AccountNotFoundException(userOptions.Username);

            return acc;
        }
    }
}
