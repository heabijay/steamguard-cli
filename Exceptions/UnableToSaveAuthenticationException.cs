namespace SteamGuard.Exceptions
{

    [System.Serializable]
    public class UnableToSaveAuthenticationException : System.Exception
    {
        public UnableToSaveAuthenticationException() { }
        public UnableToSaveAuthenticationException(string message) : base(message) { }
        public UnableToSaveAuthenticationException(string message, System.Exception inner) : base(message, inner) { }
        protected UnableToSaveAuthenticationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
