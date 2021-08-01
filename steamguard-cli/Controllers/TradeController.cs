using SteamAuth;
using SteamGuard.Enums;
using SteamGuard.Extensions;
using SteamGuard.Options;
using System;

namespace SteamGuard.Controllers
{
    public class TradeController : ControllerBase<TradeOptions>
    {
        public override void Execute(TradeOptions options)
        {
            if (options.Username == null)
            {
                Console.WriteLine("Account doesn't selected!");
                return;
            }

            ProcessConfirmationsDialog(options.GetAccount());
        }

        private static bool NeedRefreshSessionDialog(SteamGuardAccount account)
        {
            Console.WriteLine("Your Steam credentials have expired. For trade and market confirmations to work properly, please login again.");
            var username = account.AccountName;
            Console.WriteLine($"Username: {username}");
            Console.Write("Password: ");
            var password = Console.ReadLine();

            UserLogin login = new UserLogin(username, password);
            Console.Write($"Logging in {username}... ");
            LoginResult loginResult = login.DoLogin();
            if (loginResult == LoginResult.Need2FA && !string.IsNullOrEmpty(account.SharedSecret))
            {
                // if we need a 2fa code, and we can generate it, generate a 2fa code and log in.
                Utils.Verbose(loginResult);
                TimeAligner.AlignTime();
                login.TwoFactorCode = account.GenerateSteamGuardCode();
                Utils.Verbose($"Logging in {username}... ");
                loginResult = login.DoLogin();
            }
            Console.WriteLine(loginResult);
            if (loginResult == LoginResult.LoginOkay)
            {
                account.Session = login.Session;
            }

            if (account.RefreshSession())
            {
                Utils.Verbose("Session refreshed");
                Program.Manifest.SaveAccount(account, Program.Manifest.Encrypted);
                return true;
            }
            else
            {
                return false;
            }
        }

        internal static void ProcessConfirmationsDialog(SteamGuardAccount account)
        {
            Utils.Verbose("Refeshing Session...");
            if (account.RefreshSession())
            {
                Utils.Verbose("Session refreshed");
                Program.Manifest.SaveAccount(account, Program.Manifest.Encrypted);
            }
            else
            {
                Utils.Verbose("Failed to refresh session, prompting user...");
                if (!NeedRefreshSessionDialog(account))
                {
                    Console.WriteLine("Failed to refresh session, aborting...");
                }
            }
            Console.WriteLine("Retrieving trade confirmations...");
            var tradesTask = account.FetchConfirmationsAsync();
            tradesTask.Wait();
            var trades = tradesTask.Result;
            var tradeActions = new TradeAction[trades.Length];
            for (var i = 0; i < tradeActions.Length; i++)
            {
                tradeActions[i] = TradeAction.Ignore;
            }
            if (trades.Length == 0)
            {
                Console.WriteLine($"No trade confirmations for {account.AccountName}.");
                return;
            }
            var selected = 0;
            var colorAccept = ConsoleColor.Green;
            var colorDeny = ConsoleColor.Red;
            var colorIgnore = ConsoleColor.Gray;
            var colorSelected = ConsoleColor.Yellow;
            var confirm = false;

            do
            {
                Console.Clear();
                if (selected >= trades.Length)
                    selected = trades.Length - 1;
                else if (selected < 0)
                    selected = 0;
                Console.ResetColor();
                Console.WriteLine($"Trade confirmations for {account.AccountName}...");
                Console.WriteLine("No action will be made without your confirmation.");
                Console.WriteLine("[a]ccept   [d]eny   [i]gnore  [enter] Confirm  [q]uit"); // accept = 1, deny = 0, ignore = -1
                Console.WriteLine();

                for (var t = 0; t < trades.Length; t++)
                {
                    ConsoleColor itemColor;
                    switch (tradeActions[t])
                    {
                        case TradeAction.Accept:
                            itemColor = colorAccept;
                            break;
                        case TradeAction.Deny:
                            itemColor = colorDeny;
                            break;
                        case TradeAction.Ignore:
                            itemColor = colorIgnore;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    Console.ForegroundColor = t == selected ? colorSelected : itemColor;

                    Console.WriteLine($"  [{t}] [{tradeActions[t]}] {trades[t].ConfType} {trades[t].Creator}");
                }
                var key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        selected--;
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        selected++;
                        break;
                    case ConsoleKey.A:
                        tradeActions[selected] = TradeAction.Accept;
                        break;
                    case ConsoleKey.D:
                        tradeActions[selected] = TradeAction.Deny;
                        break;
                    case ConsoleKey.I:
                        tradeActions[selected] = TradeAction.Ignore;
                        break;
                    case ConsoleKey.Enter:
                        confirm = true;
                        break;
                    case ConsoleKey.Escape:
                    case ConsoleKey.Q:
                        Console.ResetColor();
                        Console.WriteLine("Quitting...");
                        return;
                    default:
                        break;
                }
            } while (!confirm);
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Processing...");
            for (var t = 0; t < trades.Length; t++)
            {
                bool success = false;
                switch (tradeActions[t])
                {
                    case TradeAction.Accept:
                        Console.Write($"Accepting {trades[t].ConfType} {trades[t].Creator}...");
                        success = account.AcceptConfirmation(trades[t]);
                        break;
                    case TradeAction.Deny:
                        Console.Write($"Denying {trades[t].ConfType} {trades[t].Creator}...");
                        success = account.DenyConfirmation(trades[t]);
                        break;
                    case TradeAction.Ignore:
                        Console.Write($"Ignoring {trades[t].ConfType} {trades[t].Creator}...");
                        success = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                Utils.Verbose(success);
            }
            Console.WriteLine("Done.");
        }
    }
}
