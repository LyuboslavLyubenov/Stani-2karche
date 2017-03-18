namespace Zenject.Source.Runtime
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Zenject.Source.Internal;
    using Zenject.Source.Usage;

    public class TickableManager
    {
        [Inject(Optional = true, Source = InjectSources.Local)]
        readonly List<ITickable> _tickables = null;

        [Inject(Optional = true, Source = InjectSources.Local)]
        readonly List<IFixedTickable> _fixedTickables = null;

        [Inject(Optional = true, Source = InjectSources.Local)]
        readonly List<ILateTickable> _lateTickables = null;

        [Inject(Optional = true, Source = InjectSources.Local)]
        readonly List<ValuePair<Type, int>> _priorities = null;

        [Inject(Optional = true, Id = "Fixed", Source = InjectSources.Local)]
        readonly List<ValuePair<Type, int>> _fixedPriorities = null;

        [Inject(Optional = true, Id = "Late", Source = InjectSources.Local)]
        readonly List<ValuePair<Type, int>> _latePriorities = null;

        readonly TickablesTaskUpdater _updater = new TickablesTaskUpdater();
        readonly FixedTickablesTaskUpdater _fixedUpdater = new FixedTickablesTaskUpdater();
        readonly LateTickablesTaskUpdater _lateUpdater = new LateTickablesTaskUpdater();

        bool _isPaused;

        public IEnumerable<ITickable> Tickables
        {
            get
            {
                return this._tickables;
            }
        }

        [Inject]
        public void Initialize()
        {
            this.InitTickables();
            this.InitFixedTickables();
            this.InitLateTickables();
        }

        void InitFixedTickables()
        {
            foreach (var type in this._fixedPriorities.Select(x => x.First))
            {
                Assert.That(type.DerivesFrom<IFixedTickable>(),
                    "Expected type '{0}' to drive from IFixedTickable while checking priorities in TickableHandler", type.Name());
            }

            foreach (var tickable in this._fixedTickables)
            {
                // Note that we use zero for unspecified priority
                // This is nice because you can use negative or positive for before/after unspecified
                var matches = this._fixedPriorities.Where(x => tickable.GetType().DerivesFromOrEqual(x.First)).Select(x => x.Second).ToList();
                int priority = matches.IsEmpty() ? 0 : matches.Distinct().Single();

                this._fixedUpdater.AddTask(tickable, priority);
            }
        }

        void InitTickables()
        {
            foreach (var type in this._priorities.Select(x => x.First))
            {
                Assert.That(type.DerivesFrom<ITickable>(),
                    "Expected type '{0}' to drive from ITickable while checking priorities in TickableHandler", type.Name());
            }

            foreach (var tickable in this._tickables)
            {
                // Note that we use zero for unspecified priority
                // This is nice because you can use negative or positive for before/after unspecified
                var matches = this._priorities.Where(x => tickable.GetType().DerivesFromOrEqual(x.First)).Select(x => x.Second).ToList();
                int priority = matches.IsEmpty() ? 0 : matches.Distinct().Single();

                this._updater.AddTask(tickable, priority);
            }
        }

        void InitLateTickables()
        {
            foreach (var type in this._latePriorities.Select(x => x.First))
            {
                Assert.That(type.DerivesFrom<ILateTickable>(),
                    "Expected type '{0}' to drive from ILateTickable while checking priorities in TickableHandler", type.Name());
            }

            foreach (var tickable in this._lateTickables)
            {
                // Note that we use zero for unspecified priority
                // This is nice because you can use negative or positive for before/after unspecified
                var matches = this._latePriorities.Where(x => tickable.GetType().DerivesFromOrEqual(x.First)).Select(x => x.Second).ToList();
                int priority = matches.IsEmpty() ? 0 : matches.Distinct().Single();

                this._lateUpdater.AddTask(tickable, priority);
            }
        }

        public void Add(ITickable tickable, int priority)
        {
            this._updater.AddTask(tickable, priority);
        }

        public void Add(ITickable tickable)
        {
            this.Add(tickable, 0);
        }

        public void AddLate(ILateTickable tickable, int priority)
        {
            this._lateUpdater.AddTask(tickable, priority);
        }

        public void AddLate(ILateTickable tickable)
        {
            this.AddLate(tickable, 0);
        }

        public void AddFixed(IFixedTickable tickable, int priority)
        {
            this._fixedUpdater.AddTask(tickable, priority);
        }

        public void AddFixed(IFixedTickable tickable)
        {
            this._fixedUpdater.AddTask(tickable, 0);
        }

        public void Remove(ITickable tickable)
        {
            this._updater.RemoveTask(tickable);
        }

        public void RemoveLate(ILateTickable tickable)
        {
            this._lateUpdater.RemoveTask(tickable);
        }

        public void RemoveFixed(IFixedTickable tickable)
        {
            this._fixedUpdater.RemoveTask(tickable);
        }

        public void Update()
        {
            if(this._isPaused)
            {
                return;
            }

            this._updater.OnFrameStart();
            this._updater.UpdateAll();
        }

        public void FixedUpdate()
        {
            if(this._isPaused)
            {
                return;
            }

            this._fixedUpdater.OnFrameStart();
            this._fixedUpdater.UpdateAll();
        }

        public void LateUpdate()
        {
            if(this._isPaused)
            {
                return;
            }

            this._lateUpdater.OnFrameStart();
            this._lateUpdater.UpdateAll();
        }

        public void Pause()
        {
            this._isPaused = true;
        }

        public void Resume()
        {
            this._isPaused = false;
        }
    }
}
