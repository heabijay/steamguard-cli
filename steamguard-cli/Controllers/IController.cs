using SteamGuard.Options;

namespace SteamGuard.Controllers
{
    public interface IController
    {
        public void Execute(IOptions options);

        public bool TryHandleMessage(IOptions options);
    }
}
