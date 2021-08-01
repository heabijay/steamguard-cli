namespace SteamGuard.Exceptions
{

    [System.Serializable]
    public class ManifestNotEncryptedException : System.Exception
    {
        public ManifestNotEncryptedException() { }
        public ManifestNotEncryptedException(string message) : base(message) { }
        public ManifestNotEncryptedException(string message, System.Exception inner) : base(message, inner) { }
        protected ManifestNotEncryptedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
