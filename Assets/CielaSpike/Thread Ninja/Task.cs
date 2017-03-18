namespace CielaSpike.Thread_Ninja
{

    using System.Collections;
    using System.Threading;

    using UnityEngine;

    /// <summary>
    /// Represents an async task.
    /// </summary>
    public class Task : IEnumerator
    {
        // implements IEnumerator to make it usable by StartCoroutine;
        #region IEnumerator Interface
        /// <summary>
        /// The current iterator yield return value.
        /// </summary>
        public object Current { get; private set; }

        /// <summary>
        /// Runs next iteration.
        /// </summary>
        /// <returns><code>true</code> for continue, otherwise <code>false</code>.</returns>
        public bool MoveNext()
        {
            return this.OnMoveNext();
        }

        public void Reset()
        {
            // Reset method not supported by iterator;
            throw new System.NotSupportedException(
                "Not support calling Reset() on iterator.");
        }
        #endregion

        // inner running state used by state machine;
        private enum RunningState
        {
            Init,
            RunningAsync,
            PendingYield,
            ToBackground,
            RunningSync,
            CancellationRequested,
            Done,
            Error
        }

        // routine user want to run;
        private readonly IEnumerator _innerRoutine;

        // current running state;
        private RunningState _state;
        // last running state;
        private RunningState _previousState;
        // temporary stores current yield return value
        // until we think Unity coroutine engine is OK to get it;
        private object _pendingCurrent;

        /// <summary>
        /// Gets state of the task.
        /// </summary>
        public TaskState State
        {
            get
            {
                switch (this._state)
                {
                    case RunningState.CancellationRequested:
                        return TaskState.Cancelled;
                    case RunningState.Done:
                        return TaskState.Done;
                    case RunningState.Error:
                        return TaskState.Error;
                    case RunningState.Init:
                        return TaskState.Init;
                    default:
                        return TaskState.Running;
                }
            }
        }

        /// <summary>
        /// Gets exception during running.
        /// </summary>
        public System.Exception Exception { get; private set; }

        public Task(IEnumerator routine)
        {
            this._innerRoutine = routine;
            // runs into background first;
            this._state = RunningState.Init;
        }

        /// <summary>
        /// Cancel the task till next iteration;
        /// </summary>
        public void Cancel()
        {
            if (this.State == TaskState.Running)
            {
                this.GotoState(RunningState.CancellationRequested);
            }
        }

        /// <summary>
        /// A co-routine that waits the task.
        /// </summary>
        public IEnumerator Wait()
        {
            while (this.State == TaskState.Running)
                yield return null;
        }

        // thread safely switch running state;
        private void GotoState(RunningState state)
        {
            if (this._state == state) return;

            lock (this)
            {
                // maintainance the previous state;
                this._previousState = this._state;
                this._state = state;
            }
        }

        // thread safely save yield returned value;
        private void SetPendingCurrentObject(object current)
        {
            lock (this)
            {
                this._pendingCurrent = current;
            }
        }

        // actual MoveNext method, controls running state;
        private bool OnMoveNext()
        {
            // no running for null;
            if (this._innerRoutine == null)
                return false;

            // set current to null so that Unity not get same yield value twice;
            this.Current = null;

            // loops until the inner routine yield something to Unity;
            while (true)
            {
                // a simple state machine;
                switch (this._state)
                {
                    // first, goto background;
                    case RunningState.Init:
                        this.GotoState(RunningState.ToBackground);
                        break;
                    // running in background, wait a frame;
                    case RunningState.RunningAsync:
                        return true;

                    // runs on main thread;
                    case RunningState.RunningSync:
                        this.MoveNextUnity();
                        break;

                    // need switch to background;
                    case RunningState.ToBackground:
                        this.GotoState(RunningState.RunningAsync);
                        // call the thread launcher;
                        this.MoveNextAsync();
                        return true;

                    // something was yield returned;
                    case RunningState.PendingYield:
                        if (this._pendingCurrent == Ninja.JumpBack)
                        {
                            // do not break the loop, switch to background;
                            this.GotoState(RunningState.ToBackground);
                        }
                        else if (this._pendingCurrent == Ninja.JumpToUnity)
                        {
                            // do not break the loop, switch to main thread;
                            this.GotoState(RunningState.RunningSync);
                        }
                        else
                        {
                            // not from the Ninja, then Unity should get noticed,
                            // Set to Current property to achieve this;
                            this.Current = this._pendingCurrent;

                            // yield from background thread, or main thread?
                            if (this._previousState == RunningState.RunningAsync)
                            {
                                // if from background thread, 
                                // go back into background in the next loop;
                                this._pendingCurrent = Ninja.JumpBack;
                            }
                            else
                            {
                                // otherwise go back to main thread the next loop;
                                this._pendingCurrent = Ninja.JumpToUnity;
                            }

                            // end this iteration and Unity get noticed;
                            return true;
                        }
                        break;

                    // done running, pass false to Unity;
                    case RunningState.Done:
                    case RunningState.CancellationRequested:
                    default:
                        return false;
                }
            }
        }

        // background thread launcher;
        private void MoveNextAsync()
        {
            ThreadPool.QueueUserWorkItem(
                new WaitCallback(this.BackgroundRunner));
        }

        // background thread function;
        private void BackgroundRunner(object state)
        {
            // just run the sync version on background thread;
            this.MoveNextUnity();
        }

        // run next iteration on main thread;
        private void MoveNextUnity()
        {
            try
            {
                // run next part of the user routine;
                var result = this._innerRoutine.MoveNext();

                if (result)
                {
                    // something has been yield returned, handle it;
                    this.SetPendingCurrentObject(this._innerRoutine.Current);
                    this.GotoState(RunningState.PendingYield);
                }
                else
                {
                    // user routine simple done;
                    this.GotoState(RunningState.Done);
                }
            }
            catch (System.Exception ex)
            {
                // exception handling, save & log it;
                this.Exception = ex;
                Debug.LogError(string.Format("{0}\n{1}", ex.Message, ex.StackTrace));
                // then terminates the task;
                this.GotoState(RunningState.Error);
            }
        }
    }
}