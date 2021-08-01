namespace SteamGuard.Exceptions.LinkerFinalize
{

    [System.Serializable]
    public class UnableToGenerateCorrectCodesException : LinkerFinalizeException
    {
        public UnableToGenerateCorrectCodesException() : base("Unable to generate the proper codes to finalize this authenticator. The authenticator should not have been linked.") { }
        public UnableToGenerateCorrectCodesException(string message) : base(message) { }
        public UnableToGenerateCorrectCodesException(string message, System.Exception inner) : base(message, inner) { }
        protected UnableToGenerateCorrectCodesException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
