namespace SteamGuard.Exceptions
{
    [System.Serializable]
    public class IncorrectPassKeyException : System.Exception
    {
        public IncorrectPassKeyException() { }
        public IncorrectPassKeyException(string message) : base(message) { }
        public IncorrectPassKeyException(string message, System.Exception inner) : base(message, inner) { }
        protected IncorrectPassKeyException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
