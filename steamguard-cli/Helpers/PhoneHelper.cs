namespace SteamGuard.Helpers
{
    public static class PhoneHelper
    {
        public static string ExtractPhoneNumber(string phoneNumber) => phoneNumber.Replace("-", "").Replace("(", "").Replace(")", "");

        public static bool IsPhoneNumberOkay(string phoneNumber)
        {
            if (phoneNumber == null || phoneNumber.Length == 0) return false;
            if (phoneNumber[0] != '+') return false;
            return true;
        }
    }
}
