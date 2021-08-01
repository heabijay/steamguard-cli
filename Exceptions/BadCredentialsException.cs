namespace SteamGuard.Exceptions
{

    [System.Serializable]
    public class BadCredentialsException : System.Exception
    {
        public BadCredentialsException() { }
        public BadCredentialsException(string message) : base(message) { }
        public BadCredentialsException(string message, System.Exception inner) : base(message, inner) { }
        protected BadCredentialsException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
