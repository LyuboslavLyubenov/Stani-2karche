namespace Assets.Scripts.Utils.Unity
{
    using System;
    using System.Collections;

    using UnityEngine;

    public class CoroutineUtils
    {
        private MonoBehaviour instance;
        
        public CoroutineUtils(MonoBehaviour instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            this.instance = instance;
        }

        public void WaitForSeconds(float seconds, Action callback)
        {
            this.instance.StartCoroutine(WaitForSecondsCoroutine(seconds, callback));
        }

        public void WaitForFrames(int frames, Action callback)
        {
            this.instance.StartCoroutine(WaitForFramesCoroutine(frames, callback));
        }

        public void WaitForRenderGUIFrames(int frames, Action callback)
        {
            this.instance.StartCoroutine(this.WaitForGUIRenderFrameCoroutine(frames, callback));
        }

        public void WaitUntil(Func<bool> condition, Action callback)
        {
            this.instance.StartCoroutine(this.WaitUntilCoroutine(condition, callback));
        }

        public void RepeatEverySeconds(float seconds, Action callback)
        {
            this.instance.StartCoroutine(this.RepeatEverySecondsCoroutine(seconds, callback));
        }

        private IEnumerator RepeatEverySecondsCoroutine(float seconds, Action callback)
        {
            while (true)
            {
                yield return new WaitForSeconds(seconds);
                callback();
            }
        }

        private IEnumerator WaitUntilCoroutine(Func<bool> condition, Action callback)
        {
            yield return new WaitUntil(condition);
            callback();
        }

        private IEnumerator WaitForGUIRenderFrameCoroutine(int frames, Action callback)
        {
            for (int i = 0; i < frames; i++)
            {
                yield return new WaitForEndOfFrame();    
            }

            callback();
        }

        private IEnumerator WaitForFramesCoroutine(int frames, Action callback)
        {
            for (int i = 0; i < frames; i++)
            {
                yield return null;    
            }

            callback();
        }

        private IEnumerator WaitForSecondsCoroutine(float seconds, Action callback)
        {
            yield return new WaitForSeconds(seconds);
            callback();
        }
    }

}

