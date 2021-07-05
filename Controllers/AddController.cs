using SteamGuard.Options;

namespace SteamGuard.Controllers
{
    public class AddController : ControllerBase<AddOptions>
    {
        public override void Execute(AddOptions message)
        {
            System.Console.WriteLine(GetType().Name);
        }
    }
}
