using SteamGuard.Options;

namespace SteamGuard.Controllers
{
    public class DecryptController : ControllerBase<DecryptOptions>
    {
        public override void Execute(DecryptOptions message)
        {
            System.Console.WriteLine(GetType().Name);
        }
    }
}
