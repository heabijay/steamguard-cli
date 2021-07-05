using System;

namespace SteamGuard.Exceptions
{

    [Serializable]
    public class AccountNotFoundException : Exception
    {
        public AccountNotFoundException() { }
        public AccountNotFoundException(string message) : base(message) { }
        public AccountNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected AccountNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
