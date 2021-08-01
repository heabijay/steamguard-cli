namespace SteamGuard.Exceptions.LinkerFinalize
{

    [System.Serializable]
    public class LinkerFinalizeException : System.Exception
    {
        public LinkerFinalizeException() { }
        public LinkerFinalizeException(string message) : base(message) { }
        public LinkerFinalizeException(string message,  System.Exception inner) : base(message, inner) { }
        protected LinkerFinalizeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
