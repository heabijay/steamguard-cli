using SteamGuard.Options;

namespace SteamGuard.Controllers
{
    public class TradeController : ControllerBase<TradeOptions>
    {
        public override void Execute(TradeOptions message)
        {
            System.Console.WriteLine(GetType().Name);
        }
    }
}
