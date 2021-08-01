namespace SteamGuard.Exceptions
{

    [System.Serializable]
    public class DecryptPasswordRequiredException : System.Exception
    {
        public DecryptPasswordRequiredException() { }
        public DecryptPasswordRequiredException(string message) : base(message) { }
        public DecryptPasswordRequiredException(string message, System.Exception inner) : base(message, inner) { }
        protected DecryptPasswordRequiredException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
