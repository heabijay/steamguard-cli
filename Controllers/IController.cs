using SteamGuard.Options;

namespace SteamGuard.Controllers
{
    public interface IController
    {
        public void Execute(IOptions message);

        public bool TryHandleMessage(IOptions message);
    }
}
