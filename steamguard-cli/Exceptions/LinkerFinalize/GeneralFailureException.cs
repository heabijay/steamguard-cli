namespace SteamGuard.Exceptions.LinkerFinalize
{
    [System.Serializable]
    public class GeneralFailureException : LinkerFinalizeException
    {
        public GeneralFailureException() : base("Unable to finalize this authenticator. The authenticator should not have been linked.") { }
        public GeneralFailureException(string message) : base(message) { }
        public GeneralFailureException(string message, System.Exception inner) : base(message, inner) { }
        protected GeneralFailureException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
