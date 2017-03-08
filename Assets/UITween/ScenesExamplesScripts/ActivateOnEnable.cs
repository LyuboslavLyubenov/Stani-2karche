using System.Collections;

using UnityEngine;

namespace Assets.UITween.ScenesExamplesScripts
{

    using Assets.UITween.Scripts;

    public class ActivateOnEnable : MonoBehaviour {

        public EasyTween EasyTweenStart;

        private IEnumerator Start () 
        {
            yield return new WaitForEndOfFrame();
            this.EasyTweenStart.OpenCloseObjectAnimation();
        }
    }

}
