using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivObservable
{
    /// <summary>
    /// The static class provides methods to raise a fixed event.
    /// </summary>
    public static class FixedEventReceiver
    {
        /// <summary>
        /// Raises a fixed event with the specific root of hierarchy, in the direction of forward DFS.
        /// </summary>
        /// <typeparam name="T">The type of receiver.</typeparam>
        /// <param name="root">The root of fixed event hierarchy.</param>
        /// <param name="action">The action to be called with receiver.</param>
        public static void RaiseDownForward<T>(IFixedEventReceiver<T> root, Action<T> action)
            where T : class
        {
            if (root != null)
            {
                if (root.Receiver != null)
                    action(root.Receiver);

                if (root.Children != null)
                {
                    foreach (var child in root.Children)
                    {
                        RaiseDownForward(child, action);
                    }
                }
            }
        }

        /// <summary>
        /// Raises a fixed event with the specific root of hierarchy, in the direction of backward DFS.
        /// </summary>
        /// <typeparam name="T">The type of receiver.</typeparam>
        /// <param name="root">The root of fixed event hierarchy.</param>
        /// <param name="action">The action to be called with receiver.</param>
        public static void RaiseDownBackward<T>(IFixedEventReceiver<T> root, Action<T> action)
            where T : class
        {
            if (root != null)
            {
                if (root.Children != null)
                {
                    foreach (var child in root.Children.Reverse())
                    {
                        RaiseDownBackward(child, action);
                    }
                }

                if (root.Receiver != null)
                    action(root.Receiver);
            }
        }
    }
}
