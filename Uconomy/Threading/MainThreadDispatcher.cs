using System.Threading;
using Rocket.Core.Utils;
using SDG.Unturned;
using Action = System.Action;

namespace fr34kyn01535.Uconomy.Threading
{
    /// <summary>
    /// Provides a utility for dispatching actions to the main game thread.
    /// If already on the main thread, the action executes immediately.
    /// </summary>
    public static class MainThreadDispatcher
    {
        /// <summary>
        /// Executes the specified action on the main game thread.
        /// If already on the main thread, the action runs immediately;
        /// otherwise, it is queued for execution on the next main thread tick.
        /// </summary>
        /// <param name="action">The action to execute on the main thread.</param>
        public static void Run(Action action)
        {
            if (Thread.CurrentThread == ThreadUtil.gameThread)
            {
                action();
                return;
            }
            TaskDispatcher.QueueOnMainThread(action);
        }
    }
}