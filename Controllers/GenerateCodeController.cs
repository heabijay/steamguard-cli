using SteamGuard.Options;

namespace SteamGuard.Controllers
{
    public class GenerateCodeController : ControllerBase<GenerateCodeOptions>
    {
        public override void Execute(GenerateCodeOptions message)
        {
            System.Console.WriteLine(GetType().Name);
        }
    }
}
