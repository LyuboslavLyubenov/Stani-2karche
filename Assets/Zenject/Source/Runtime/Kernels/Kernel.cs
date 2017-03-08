namespace Assets.Zenject.Source.Runtime.Kernels
{

    using System;

    using Assets.Zenject.Source.Internal;
    using Assets.Zenject.Source.Usage;

    [System.Diagnostics.DebuggerStepThrough]
    public class Kernel : IInitializable, IDisposable, ITickable, ILateTickable, IFixedTickable
    {
        [InjectLocal]
        TickableManager _tickableManager = null;

        [InjectLocal]
        InitializableManager _initializableManager = null;

        [InjectLocal]
        DisposableManager _disposablesManager = null;

        public virtual void Initialize()
        {
            Log.Debug("Zenject: Initializing IInitializable's");

            this._initializableManager.Initialize();
        }

        public virtual void Dispose()
        {
            Log.Debug("Zenject: Disposing IDisposable's");

            this._disposablesManager.Dispose();
        }

        public virtual void Tick()
        {
            this._tickableManager.Update();
        }

        public virtual void LateTick()
        {
            this._tickableManager.LateUpdate();
        }

        public virtual void FixedTick()
        {
            this._tickableManager.FixedUpdate();
        }
    }
}
