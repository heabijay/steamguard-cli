using SteamGuard.Controllers;
using SteamGuard.Options;
using SteamGuard.Providers;
using System.Linq;

namespace SteamGuard.Extensions
{
    public static class ControllerOptionsExtensions
    {
        /// <summary>
        /// Finds the controller for selected option.
        /// </summary>
        /// <param name="options"></param>
        /// <returns>The controller which could handle <paramref name="options"/>.</returns>
        public static IController GetController(this IOptions options) => ControllersProvider.Instances.First(t => t.TryHandleMessage(options));
    }
}
