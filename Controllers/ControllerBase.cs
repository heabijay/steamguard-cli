using SteamGuard.Options;
using System;

namespace SteamGuard.Controllers
{
    public abstract class ControllerBase<T> : IController where T : IOptions
    {
        public virtual void Execute(IOptions options)
        {
            if (!TryHandleMessage(options))
                throw new InvalidCastException($"The type {options.GetType()} couldn't be cast to {typeof(T)}.");

            Execute((T)options);
        }

        public abstract void Execute(T options);

        public virtual bool TryHandleMessage(IOptions options)
        {
            if (options != null && options is T)
                return true;

            return false;
        }
    }
}
