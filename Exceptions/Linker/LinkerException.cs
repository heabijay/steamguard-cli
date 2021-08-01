using System;

namespace SteamGuard.Exceptions.Linker
{

    [Serializable]
    public class LinkerException : Exception
    {
        public LinkerException() { }
        public LinkerException(string message) : base(message) { }
        public LinkerException(string message, Exception inner) : base(message, inner) { }
        protected LinkerException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
