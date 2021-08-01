using SteamAuth;
using SteamGuard.Exceptions;
using SteamGuard.Exceptions.Linker;
using SteamGuard.Exceptions.LinkerFinalize;
using SteamGuard.Exceptions.Login;
using SteamGuard.Helpers;
using SteamGuard.Options;
using System;
using static SteamAuth.AuthenticatorLinker;

namespace SteamGuard.Controllers
{
    public class AddController : ControllerBase<AddOptions>
    {
        public override void Execute(AddOptions options)
        {
            var login = ProcessLoginDialog();
            var linker = ProccessLinkerDialog(login.Session);

            var passKey = EnsureEncryptionNeededDialog(options);

            //Save the file immediately; losing this would be bad.
            if (!Program.Manifest.SaveAccount(linker.LinkedAccount, passKey != null, passKey))
            {
                Program.Manifest.RemoveAccount(linker.LinkedAccount);
                throw new UnableToSaveAuthenticationException("Unable to save mobile authenticator file. The mobile authenticator has not been linked.");
            }

            Console.WriteLine(
                $"The Mobile Authenticator has not yet been linked. " +
                $"Before finalizing the authenticator, please write down your revocation code: " +
                $"{linker.LinkedAccount.RevocationCode}");

            ProcessFinalizeDialog(linker);

            Program.Manifest.SaveAccount(linker.LinkedAccount, passKey != null, passKey);
            Console.WriteLine(
                $"Mobile authenticator successfully linked. " +
                $"Please actually write down your revocation code: " +
                $"{linker.LinkedAccount.RevocationCode}");
        }

        public static UserLogin ProcessLoginDialog()
        {
            Console.Write("Username: ");
            var username = Console.ReadLine();
            Console.Write("Password: ");
            var password = ConsoleHelper.SecureReadLine();

            UserLogin login = new(username, password);
            LoginResult loginResult;
            do
            {
                Console.Write($"Logging in {username}... ");
                loginResult = login.DoLogin();
                Console.WriteLine($"Login result: {loginResult}");
                switch (loginResult)
                {
                    case LoginResult.NeedEmail:
                        Console.Write("Email code: ");
                        login.EmailCode = Console.ReadLine();
                        break;
                    case LoginResult.Need2FA:
                        Console.Write("2FA code: ");
                        login.TwoFactorCode = Console.ReadLine();
                        break;
                    case LoginResult.NeedCaptcha:
                        Console.WriteLine($"Please open: https://steamcommunity.com/public/captcha.php?gid={login.CaptchaGID}");
                        Console.Write("Captcha text: ");
                        login.CaptchaText = Console.ReadLine();
                        break;
                    case LoginResult.BadCredentials:        throw new BadCredentialsException();
                    case LoginResult.TooManyFailedLogins:   throw new TooManyFailedLoginsException();
                    case LoginResult.LoginOkay:             break;
                    default:                                Console.WriteLine($"Unknown login result: {loginResult}"); break;
                }
            } while (loginResult != LoginResult.LoginOkay);

            return login;
        }

        public static string EnsureEncryptionNeededDialog(DefaultOptions options)
        {
            string passKey = null;
            if (Program.Manifest.Entries.Count == 0)
            {
                Console.Write(
                    "Looks like we are setting up your first account.\n" +
                    "Would you like to use encryption? [Y/n] ");
                var doEncryptAnswer = Console.ReadLine();

                if (doEncryptAnswer.Equals("n", StringComparison.OrdinalIgnoreCase))
                    Console.WriteLine(
                        "WARNING: You chose to not encrypt your files. Doing so imposes a security risk for yourself.\n" +
                        "If an attacker were to gain access to your computer, they could completely lock you out of your account and steal all your items.\n" +
                        "You may add encryption later using the --encrypt argument.");
                else
                    passKey = EncryptController.SetupPassKeyDialog();
            }
            else if (Program.Manifest.Entries.Count > 0 && Program.Manifest.Encrypted && string.IsNullOrEmpty(options.PassKey))
                options.PassKey = PassKeyInputDialog(Program.Manifest);

            return passKey;
        }

        public static AuthenticatorLinker ProccessLinkerDialog(SessionData session)
        {
            var linker = new AuthenticatorLinker(session);
            LinkResult linkResult;

            do
            {
                linkResult = linker.AddAuthenticator();
                Console.WriteLine($"Link result: {linkResult}");
                switch (linkResult)
                {
                    case LinkResult.MustProvidePhoneNumber:
                        do
                        {
                            Console.WriteLine("Enter your mobile phone number in the following format: +{cC} phoneNumber. EG, +1 123-456-7890");
                            linker.PhoneNumber = PhoneHelper.ExtractPhoneNumber(Console.ReadLine());
                        } while (!PhoneHelper.IsPhoneNumberOkay(linker.PhoneNumber));
                        break;
                    case LinkResult.MustConfirmEmail:
                        Console.WriteLine("Check your email. Before continuing, click the link in the email to confirm your phone number. Press enter to continue...");
                        Console.ReadLine();
                        break;
                    case LinkResult.AuthenticatorPresent:
                        Console.Write(
                            "An authenticator is already present.\n" +
                            "If you have the revocation code (Rxxxxx), this program can remove it for you.\n" +
                            "Would you like to remove the current authenticator using your revocation code? (y/n) ");
                        var answer = Console.ReadLine();
                        if (answer.Equals("y", StringComparison.OrdinalIgnoreCase))
                        {
                            var isSuccess = DeleteAuthenticatorDialog(session);
                            if (isSuccess)
                                Console.WriteLine("Successfully deactivated the current authenticator.");
                            else
                                Console.WriteLine("Deactivating the current authenticator was unsuccessful.");
                        }
                        continue;
                    case LinkResult.MustRemovePhoneNumber:  linker.PhoneNumber = null; break;
                    case LinkResult.AwaitingFinalization:   break;
                    case LinkResult.GeneralFailure:         throw new Exceptions.Linker.GeneralFailureException("Unable to add your phone number. Steam returned GeneralFailure.");
                    default:                                throw new LinkerException($"Unexpected linker result: {linkResult}");
                }
            } while (linkResult != LinkResult.AwaitingFinalization);

            return linker;
        }

        public static void ProcessFinalizeDialog(AuthenticatorLinker linker)
        {
            var finalizeResponse = FinalizeResult.GeneralFailure;
            do
            {
                Console.Write("Please input the SMS message sent to your phone number: ");
                string smsCode = Console.ReadLine();

                finalizeResponse = linker.FinalizeAddAuthenticator(smsCode);
                Console.WriteLine($"Finalize result: {finalizeResponse}");

                switch (finalizeResponse)
                {
                    case FinalizeResult.BadSMSCode: continue;

                    case FinalizeResult.UnableToGenerateCorrectCodes:
                        Program.Manifest.RemoveAccount(linker.LinkedAccount);
                        throw new UnableToGenerateCorrectCodesException(
                            $"Unable to generate the proper codes to finalize this authenticator. The authenticator should not have been linked. " +
                            $"In the off-chance it was, please write down your revocation code, as this is the last chance to see it: {linker.LinkedAccount.RevocationCode}"); 

                    case FinalizeResult.GeneralFailure:
                        Program.Manifest.RemoveAccount(linker.LinkedAccount);
                        throw new Exceptions.LinkerFinalize.GeneralFailureException(
                            $"Unable to finalize this authenticator. The authenticator should not have been linked. " +
                            $"In the off-chance it was, please write down your revocation code, as this is the last chance to see it: {linker.LinkedAccount.RevocationCode}");
                }
            } while (finalizeResponse != FinalizeResult.Success);
        }

        public static string PassKeyInputDialog(Manifest manifest)
        {
            if (!manifest.Encrypted)
                throw new ManifestNotEncryptedException();

            string passKey;
            do
            {
                Console.Write("Please enter encryption password: ");
                passKey = ConsoleHelper.SecureReadLine();
                if (!string.IsNullOrEmpty(passKey) && manifest.VerifyPasskey(passKey))
                    break;

                Console.WriteLine("Incorrect.");
            }
            while (true);

            return passKey;
        }

        public static bool DeleteAuthenticatorDialog(SessionData session)
        {
            Console.Write("Revocation code (Rxxxxx): ");
            var revocationCode = Console.ReadLine();
            var account = new SteamGuardAccount
            {
                Session = session,
                RevocationCode = revocationCode
            };

            return account.DeactivateAuthenticator();
        }
    }
}
