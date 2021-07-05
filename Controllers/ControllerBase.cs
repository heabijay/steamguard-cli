using SteamGuard.Options;
using System;

namespace SteamGuard.Controllers
{
    public abstract class ControllerBase<T>: IController where T: IOptions
    {
        public virtual void Execute(IOptions message)
        {
            if (!TryHandleMessage(message))
                throw new InvalidCastException($"The type {message.GetType()} couldn't be cast to {typeof(T)}.");

            Execute((T)message);
        }

        public abstract void Execute(T message);

        public virtual bool TryHandleMessage(IOptions message)
        {
            if (message != null && message is T)
                return true;

            return false;
        }
    }
}
