using System;

namespace SteamGuard.Exceptions.Login
{

    [Serializable]
    public class TooManyFailedLoginsException : LoginException
    {
        public TooManyFailedLoginsException() : base("Too many failed logins. Wait a bit before trying again.") { }
        public TooManyFailedLoginsException(string message) : base(message) { }
        public TooManyFailedLoginsException(string message, Exception inner) : base(message, inner) { }
        protected TooManyFailedLoginsException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
