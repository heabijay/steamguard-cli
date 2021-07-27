using SteamAuth;
using SteamGuard.Helpers;
using SteamGuard.Options;
using System;

namespace SteamGuard.Controllers
{
    public class AddController : ControllerBase<AddOptions>
    {
        public override void Execute(AddOptions options)
        {
			if (string.IsNullOrWhiteSpace(options.Username))
			{
				Console.Write("Username: ");
				options.Username = Console.ReadLine();
			}
			Console.Write("Password: ");
			var password = ConsoleHelper.SecureReadLine();

			UserLogin login = new UserLogin(options.Username, password);
			LoginResult loginResult;
			do
			{
				Console.Write($"Logging in {options.Username}... ");
				loginResult = login.DoLogin();
				Console.WriteLine(loginResult);
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
					case LoginResult.BadCredentials:
						Console.WriteLine("error: Bad Credentials");
						return;
					case LoginResult.TooManyFailedLogins:
						Console.WriteLine("error: Too many failed logins. Wait a bit before trying again.");
						return;
					case LoginResult.LoginOkay:
						break;
					default:
						Console.WriteLine($"Unknown login result: {loginResult}");
						break;
				}
			} while (loginResult != LoginResult.LoginOkay);

			AuthenticatorLinker linker = new AuthenticatorLinker(login.Session);
			AuthenticatorLinker.LinkResult linkResult = AuthenticatorLinker.LinkResult.GeneralFailure;

			do
			{
				linkResult = linker.AddAuthenticator();
				Console.WriteLine($"Link result: {linkResult}");
				switch (linkResult)
				{
					case AuthenticatorLinker.LinkResult.MustProvidePhoneNumber:
						var phonenumber = "";
						do
						{
							Console.WriteLine("Enter your mobile phone number in the following format: +{cC} phoneNumber. EG, +1 123-456-7890");
							phonenumber = Console.ReadLine();
							phonenumber = PhoneHelper.ExtractPhoneNumber(phonenumber);
							linker.PhoneNumber = phonenumber;
						} while (!PhoneHelper.IsPhoneNumberOkay(phonenumber));
						break;
					//case AuthenticatorLinker.LinkResult.MustConfirmEmail:
					//	Console.WriteLine("Check your email. Before continuing, click the link in the email to confirm your phone number. Press enter to continue...");
					//	Console.ReadLine();
					//	break;
					case AuthenticatorLinker.LinkResult.MustRemovePhoneNumber:
						linker.PhoneNumber = null;
						break;
					case AuthenticatorLinker.LinkResult.AwaitingFinalization:
						break;
					case AuthenticatorLinker.LinkResult.GeneralFailure:
						Console.WriteLine("error: Unable to add your phone number. Steam returned GeneralFailure");
						return;
					case AuthenticatorLinker.LinkResult.AuthenticatorPresent:
						Console.WriteLine("An authenticator is already present.");
						Console.WriteLine("If you have the revocation code (Rxxxxx), this program can remove it for you.");
						Console.Write("Would you like to remove the current authenticator using your revocation code? (y/n) ");
						var answer = Console.ReadLine();
						if (answer != "y")
							continue;
						Console.Write("Revocation code (Rxxxxx): ");
						var revocationCode = Console.ReadLine();
						var account = new SteamGuardAccount();
						account.Session = login.Session;
						account.RevocationCode = revocationCode;
						if (account.DeactivateAuthenticator())
							Console.WriteLine("Successfully deactivated the current authenticator.");
						else
							Console.WriteLine("Deactivating the current authenticator was unsuccessful.");
						continue;
					default:
						Console.WriteLine($"error: Unexpected linker result: {linkResult}");
						return;
				}
			} while (linkResult != AuthenticatorLinker.LinkResult.AwaitingFinalization);

			string passKey = null;
			if (Program.Manifest.Entries.Count == 0)
			{
				Console.WriteLine("Looks like we are setting up your first account.");
				passKey = Program.Manifest.PromptSetupPassKey(true);
			}
			else if (Program.Manifest.Entries.Count > 0 && Program.Manifest.Encrypted)
			{
				if (string.IsNullOrEmpty(options.PassKey))
				{
					options.PassKey = Program.Manifest.PromptForPassKey();
				}
			}

			//Save the file immediately; losing this would be bad.
			if (!Program.Manifest.SaveAccount(linker.LinkedAccount, passKey != null, passKey))
			{
				Program.Manifest.RemoveAccount(linker.LinkedAccount);
				Console.WriteLine("Unable to save mobile authenticator file. The mobile authenticator has not been linked.");
				return;
			}

			Console.WriteLine(
				$"The Mobile Authenticator has not yet been linked. Before finalizing the authenticator, please write down your revocation code: {linker.LinkedAccount.RevocationCode}");

			AuthenticatorLinker.FinalizeResult finalizeResponse = AuthenticatorLinker.FinalizeResult.GeneralFailure;
			do
			{
				Console.Write("Please input the SMS message sent to your phone number: ");
				string smsCode = Console.ReadLine();

				finalizeResponse = linker.FinalizeAddAuthenticator(smsCode);
				Utils.Verbose(finalizeResponse);

				switch (finalizeResponse)
				{
					case AuthenticatorLinker.FinalizeResult.BadSMSCode:
						continue;

					case AuthenticatorLinker.FinalizeResult.UnableToGenerateCorrectCodes:
						Console.WriteLine(
							"Unable to generate the proper codes to finalize this authenticator. The authenticator should not have been linked.");
						Console.WriteLine(
							$"In the off-chance it was, please write down your revocation code, as this is the last chance to see it: {linker.LinkedAccount.RevocationCode}");
						Program.Manifest.RemoveAccount(linker.LinkedAccount);
						return;

					case AuthenticatorLinker.FinalizeResult.GeneralFailure:
						Console.WriteLine("Unable to finalize this authenticator. The authenticator should not have been linked.");
						Console.WriteLine(
							$"In the off-chance it was, please write down your revocation code, as this is the last chance to see it: {linker.LinkedAccount.RevocationCode}");
						Program.Manifest.RemoveAccount(linker.LinkedAccount);
						return;
				}
			} while (finalizeResponse != AuthenticatorLinker.FinalizeResult.Success);

			//Linked, finally. Re-save with FullyEnrolled property.
			Program.Manifest.SaveAccount(linker.LinkedAccount, passKey != null, passKey);
			Console.WriteLine(
				$"Mobile authenticator successfully linked. Please actually write down your revocation code: {linker.LinkedAccount.RevocationCode}");
		}
	}
}
