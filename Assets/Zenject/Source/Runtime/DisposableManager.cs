namespace Assets.Zenject.Source.Runtime
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Assets.Zenject.Source.Internal;
    using Assets.Zenject.Source.Usage;

    public class DisposableManager : IDisposable
    {
        readonly List<DisposableInfo> _disposables = new List<DisposableInfo>();
        bool _disposed;

        public DisposableManager(
            [Inject(Optional = true, Source = InjectSources.Local)]
            List<IDisposable> disposables,
            [Inject(Optional = true, Source = InjectSources.Local)]
            List<ValuePair<Type, int>> priorities)
        {
            foreach (var disposable in disposables)
            {
                // Note that we use zero for unspecified priority
                // This is nice because you can use negative or positive for before/after unspecified
                var matches = priorities.Where(x => disposable.GetType().DerivesFromOrEqual(x.First)).Select(x => x.Second).ToList();
                int priority = matches.IsEmpty() ? 0 : matches.Distinct().Single();

                this._disposables.Add(new DisposableInfo(disposable, priority));
            }

            Log.Debug("Loaded {0} IDisposables to DisposablesHandler", this._disposables.Count());
        }

        public void Add(IDisposable disposable)
        {
            this.Add(disposable, 0);
        }

        public void Add(IDisposable disposable, int priority)
        {
            this._disposables.Add(
                new DisposableInfo(disposable, priority));
        }

        public void Remove(IDisposable disposable)
        {
            this._disposables.RemoveWithConfirm(
                this._disposables.Where(x => x.Disposable == disposable).Single());
        }

        public void Dispose()
        {
            Assert.That(!this._disposed, "Tried to dispose DisposableManager twice!");
            this._disposed = true;

            // Dispose in the reverse order that they are initialized in
            var disposablesOrdered = this._disposables.OrderBy(x => x.Priority).Reverse().ToList();

            foreach (var disposable in disposablesOrdered.Select(x => x.Disposable).GetDuplicates())
            {
                Assert.That(false, "Found duplicate IDisposable with type '{0}'".Fmt(disposable.GetType()));
            }

            foreach (var disposable in disposablesOrdered)
            {
                Log.Debug("Disposing '" + disposable.Disposable.GetType() + "'");

                try
                {
                    disposable.Disposable.Dispose();
                }
                catch (Exception e)
                {
                    throw Assert.CreateException(
                        e, "Error occurred while disposing IDisposable with type '{0}'", disposable.Disposable.GetType().Name());
                }
            }

            Log.Debug("Disposed of {0} disposables in DisposablesHandler", disposablesOrdered.Count());
        }

        class DisposableInfo
        {
            public IDisposable Disposable;
            public int Priority;

            public DisposableInfo(IDisposable disposable, int priority)
            {
                this.Disposable = disposable;
                this.Priority = priority;
            }
        }
    }
}
