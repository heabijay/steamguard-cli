using System;
using System.Diagnostics;

namespace SteamGuard
{
	public static class Utils
	{
		public static string ReadLineSecure()
		{
			var pass = string.Empty;
			ConsoleKey key;
			do
			{
				var keyInfo = Console.ReadKey(true);
				key = keyInfo.Key;

				if (key == ConsoleKey.Backspace && pass.Length > 0)
				{
					Console.Write("\b \b");
					pass = pass[0..^1];
				}
				else if (!char.IsControl(keyInfo.KeyChar))
				{
					Console.Write("*");
					pass += keyInfo.KeyChar;
				}
			} while (key != ConsoleKey.Enter);

			return pass;
		}

		public static void Verbose(object obj)
		{
			Verbose(obj.ToString(), null);
		}

		public static void Verbose(string format, params object[] arg)
		{
			
		}
	}
}
