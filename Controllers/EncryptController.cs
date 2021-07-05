using SteamGuard.Options;

namespace SteamGuard.Controllers
{
    public class EncryptController : ControllerBase<EncryptOptions>
    {
        public override void Execute(EncryptOptions message)
        {
            System.Console.WriteLine(GetType().Name);
        }
    }
}
