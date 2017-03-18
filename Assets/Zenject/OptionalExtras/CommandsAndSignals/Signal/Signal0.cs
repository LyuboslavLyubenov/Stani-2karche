namespace Zenject.OptionalExtras.CommandsAndSignals.Signal
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Zenject.Source.Internal;

    // Zero Parameters
    public abstract class Signal<TDerived> : ISignal
        where TDerived : Signal<TDerived>
    {
        readonly List<Action> _listeners = new List<Action>();

        bool _hasDisposed;

        static string MethodToString(Action action)
        {
#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
            return action.ToString();
#else
            return "{0}.{1}".Fmt(action.Method.DeclaringType.Name(), action.Method.Name);
#endif
        }

        public void Listen(Action listener)
        {
            Assert.That(!this._listeners.Contains(listener),
                () => "Tried to add method '{0}' to signal '{1}' but it has already been added"
                .Fmt(MethodToString(listener), this.GetType().Name));
            this._listeners.Add(listener);
        }

        public void Unlisten(Action listener)
        {
            bool success = this._listeners.Remove(listener);

            Assert.That(success,
                () => "Tried to remove method '{0}' from signal '{1}' without adding it first"
                .Fmt(MethodToString(listener), this.GetType().Name));
        }

        void IDisposable.Dispose()
        {
            Assert.That(!this._hasDisposed, "Tried to dispose signal '{0}' twice", this.GetType().Name);
            this._hasDisposed = true;

            // If you don't want to verify that all event handlers have been removed feel free to comment out this assert or remove
            Assert.That(this._listeners.IsEmpty(),
                () => "Found {0} methods still added to signal '{1}'.  Methods: {2}"
                .Fmt(this._listeners.Count, this.GetType().Name, this._listeners.Select(x => MethodToString(x)).Join(", ")));
        }

        public static TDerived operator + (Signal<TDerived> signal, Action listener)
        {
            signal.Listen(listener);
            return (TDerived)signal;
        }

        public static TDerived operator - (Signal<TDerived> signal, Action listener)
        {
            signal.Unlisten(listener);
            return (TDerived)signal;
        }

        public void Fire()
        {
            // Use ToArray in case they remove in the handler
            foreach (var listener in this._listeners.ToArray())
            {
                listener();
            }
        }
    }
}
