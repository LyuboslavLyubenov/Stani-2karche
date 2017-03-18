namespace UITween.ScenesExamplesScripts
{

    using System.Collections;

    using UITween.Scripts;

    using UnityEngine;

    public class ActivateOnEnable : MonoBehaviour {

        public EasyTween EasyTweenStart;

        private IEnumerator Start () 
        {
            yield return new WaitForEndOfFrame();
            this.EasyTweenStart.OpenCloseObjectAnimation();
        }
    }

}
