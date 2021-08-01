using System;

namespace SteamGuard.Exceptions.Linker
{

    [Serializable]
    public class GeneralFailureException : LinkerException
    {
        public GeneralFailureException() { }
        public GeneralFailureException(string message) : base(message) { }
        public GeneralFailureException(string message, Exception inner) : base(message, inner) { }
        protected GeneralFailureException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
