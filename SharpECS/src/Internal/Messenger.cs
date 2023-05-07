using SharpECS.Internal.Extensions;
using SharpECS.Internal.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpECS.Internal
{
    public delegate void MessageHandler<T>(in T message);

    internal static class Messenger
    {
        public static void Send<T>(ushort registryID, in T message) => Messenger<T>.Send(registryID, message);
    }

    internal static class Messenger<T>
    {
        #region Types

        private readonly struct Subscription : IDisposable
        {
            private readonly ushort RegistryID;
            private readonly MessageHandler<T> Action;

            public Subscription(ushort registryID, MessageHandler<T> action)
            {
                RegistryID = registryID;
                Action = action;
            }

            public void Dispose()
            {
                Actions[RegistryID] -= Action;
            }
        }

        #endregion

        private static MessageHandler<T>[] Actions = new MessageHandler<T>[1];

        #region Constructors

        static Messenger()
        {
            Messenger<RegistryDisposedMessage>.Subscribe(0, OnRegistryDisposed);
        }

        #endregion

        #region General Functions

        public static IDisposable Subscribe(ushort registryID, MessageHandler<T> action)
        {
            ArrayExtension.EnsureLength(ref Actions, registryID);
            Actions[registryID] += action;

            return new Subscription(registryID, action);
        }

        public static void Send(ushort registryID, in T message)
        {
            if (registryID < Actions.Length)
            {
                Actions[registryID]?.Invoke(message);
            }
        }

        #endregion

        #region Callbacks

        private static void OnRegistryDisposed(in RegistryDisposedMessage message)
        {
            if (message.RegistryID < Actions.Length)
                Actions[message.RegistryID] = null;
        }

        #endregion
    }
}
