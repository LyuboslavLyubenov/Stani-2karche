namespace CielaSpike.Thread_Ninja.Example
{

    using System;
    using System.Collections;
    using System.Threading;

    using UnityEngine;

    public class ExampleScript : MonoBehaviour {
        private void Start()
        {
            this.StartCoroutine(this.StartExamples());
        }

        private void Update()
        {
            // rotate cube to see if main thread has been blocked;
            this.transform.Rotate(Vector3.up, Time.deltaTime * 180);
        }

        private IEnumerator StartExamples()
        {
            Task task;
            this.LogExample("Blocking Thread");
            this.StartCoroutineAsync(this.Blocking(), out task);
            yield return this.StartCoroutine(task.Wait());
            this.LogState(task);

            this.LogExample("Cancellation");
            this.StartCoroutineAsync(this.Cancellation(), out task);
            yield return new WaitForSeconds(2.0f);
            task.Cancel();
            this.LogState(task);

            this.LogExample("Error Handling");
            yield return this.StartCoroutineAsync(this.ErrorHandling(), out task);
            this.LogState(task);
        }

        private IEnumerator Blocking()
        {
            this.LogAsync("Thread.Sleep(5000); -> See if cube rotates.");
            Thread.Sleep(5000);
            this.LogAsync("Jump to main thread.");
            yield return Ninja.JumpToUnity;
            this.LogSync("Thread.Sleep(5000); -> See if cube rotates.");
            yield return new WaitForSeconds(0.1f);
            Thread.Sleep(5000);
            this.LogSync("Jump to background.");
            yield return Ninja.JumpBack;
            this.LogAsync("Yield WaitForSeconds on background.");
            yield return new WaitForSeconds(3.0f);
        }

        private IEnumerator Cancellation()
        {
            this.LogAsync("Running heavy task...");
            for (int i = 0; i < int.MaxValue; i++)
            {
                // do some heavy ops;
                // ...
            }

            yield break;
        }

        private IEnumerator ErrorHandling()
        {
            this.LogAsync("Running heavy task...");
            for (int i = 0; i < int.MaxValue; i++)
            {
                if (i > int.MaxValue / 2)
                    throw new Exception("Some error from background thread...");
            }

            yield break;
        }

        private void LogAsync(string msg)
        {
            Debug.Log("[Async]" + msg);
        }

        private void LogState(Task task)
        {
            Debug.Log("[State]" + task.State);
        }

        private void LogSync(string msg)
        {
            Debug.Log("[Sync]" + msg);
        }

        private void LogExample(string msg)
        {
            Debug.Log("[Example]" + msg);
        }
    }

}
