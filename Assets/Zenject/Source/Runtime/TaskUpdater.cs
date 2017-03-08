namespace Assets.Zenject.Source.Runtime
{

    using System.Collections.Generic;
    using System.Linq;

    using Assets.Zenject.Source.Internal;
    using Assets.Zenject.Source.Usage;

    // Update tasks once per frame based on a priority
    [System.Diagnostics.DebuggerStepThrough]
    public abstract class TaskUpdater<TTask>
    {
        readonly LinkedList<TaskInfo> _tasks = new LinkedList<TaskInfo>();
        readonly List<TaskInfo> _queuedTasks = new List<TaskInfo>();

        IEnumerable<TaskInfo> AllTasks
        {
            get
            {
                return this.ActiveTasks.Concat(this._queuedTasks);
            }
        }

        IEnumerable<TaskInfo> ActiveTasks
        {
            get
            {
                return this._tasks;
            }
        }

        public void AddTask(TTask task, int priority)
        {
            this.AddTaskInternal(task, priority);
        }

        void AddTaskInternal(TTask task, int priority)
        {
            Assert.That(!this.AllTasks.Select(x => x.Task).ContainsItem(task),
                "Duplicate task added to DependencyRoot with name '" + task.GetType().FullName + "'");

            // Wait until next frame to add the task, otherwise whether it gets updated
            // on the current frame depends on where in the update order it was added
            // from, so you might get off by one frame issues
            this._queuedTasks.Add(new TaskInfo(task, priority));
        }

        public void RemoveTask(TTask task)
        {
            var info = this.AllTasks.Where(x => ReferenceEquals(x.Task, task)).Single();

            Assert.That(!info.IsRemoved, "Tried to remove task twice, task = " + task.GetType().Name);
            info.IsRemoved = true;
        }

        public void OnFrameStart()
        {
            // See above comment
            this.AddQueuedTasks();
        }

        public void UpdateAll()
        {
            this.UpdateRange(int.MinValue, int.MaxValue);
        }

        public void UpdateRange(int minPriority, int maxPriority)
        {
            var node = this._tasks.First;

            while (node != null)
            {
                var next = node.Next;
                var taskInfo = node.Value;

                // Make sure that tasks with priority of int.MaxValue are updated when maxPriority is int.MaxValue
                if (!taskInfo.IsRemoved && taskInfo.Priority >= minPriority
                    && (maxPriority == int.MaxValue || taskInfo.Priority < maxPriority))
                {
                    this.UpdateItem(taskInfo.Task);
                }

                node = next;
            }

            this.ClearRemovedTasks(this._tasks);
        }

        void ClearRemovedTasks(LinkedList<TaskInfo> tasks)
        {
            var node = tasks.First;

            while (node != null)
            {
                var next = node.Next;
                var info = node.Value;

                if (info.IsRemoved)
                {
                    //Log.Debug("Removed task '" + info.Task.GetType().ToString() + "'");
                    tasks.Remove(node);
                }

                node = next;
            }
        }

        void AddQueuedTasks()
        {
            for (int i = 0; i < this._queuedTasks.Count; i++)
            {
                var task = this._queuedTasks[i];

                if (!task.IsRemoved)
                {
                    this.InsertTaskSorted(task);
                }
            }
            this._queuedTasks.Clear();
        }

        void InsertTaskSorted(TaskInfo task)
        {
            for (var current = this._tasks.First; current != null; current = current.Next)
            {
                if (current.Value.Priority > task.Priority)
                {
                    this._tasks.AddBefore(current, task);
                    return;
                }
            }

            this._tasks.AddLast(task);
        }

        protected abstract void UpdateItem(TTask task);

        class TaskInfo
        {
            public TTask Task;
            public int Priority;
            public bool IsRemoved;

            public TaskInfo(TTask task, int priority)
            {
                this.Task = task;
                this.Priority = priority;
            }
        }
    }

    public class TickablesTaskUpdater : TaskUpdater<ITickable>
    {
        protected override void UpdateItem(ITickable task)
        {
#if PROFILING_ENABLED
            using (ProfileBlock.Start("{0}.Tick()".Fmt(task.GetType().Name())))
#endif
            {
                task.Tick();
            }
        }
    }

    public class LateTickablesTaskUpdater : TaskUpdater<ILateTickable>
    {
        protected override void UpdateItem(ILateTickable task)
        {
#if PROFILING_ENABLED
            using (ProfileBlock.Start("{0}.LateTick()".Fmt(task.GetType().Name())))
#endif
            {
                task.LateTick();
            }
        }
    }

    public class FixedTickablesTaskUpdater : TaskUpdater<IFixedTickable>
    {
        protected override void UpdateItem(IFixedTickable task)
        {
#if PROFILING_ENABLED
            using (ProfileBlock.Start("{0}.FixedTick()".Fmt(task.GetType().Name())))
#endif
            {
                task.FixedTick();
            }
        }
    }
}
