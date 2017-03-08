using UnityEngine;
using UnityEngine.Events;

/**** * 
 * Animation Initialized Object
 * 
 * new AnimationParts(AnimationParts.State.CLOSE, 
	                  false, 
	                  AnimationParts.EndTweenClose.DEACTIVATE, 
	                  AnimationParts.CallbackCall.END_OF_INTRO_ANIM, 
					  new UnityEvent(),
	                  new UnityEvent());
 * 
 * 
 ****/
namespace Assets.UITween.Scripts
{

    [System.Serializable]
    public class EasyTween : MonoBehaviour
    {

        public RectTransform rectTransform;
        public AnimationParts animationParts = new AnimationParts(AnimationParts.State.CLOSE,
            false,
            false,
            false,
            AnimationParts.EndTweenClose.DEACTIVATE,
            AnimationParts.CallbackCall.NOTHING,
            new UnityEvent(),
            new UnityEvent());

        private CurrentAnimation currentAnimationGoing;

        #region Public_Methods

        public EasyTween()
        {
            this.CheckIfCurrenAnimationGoingExits();
        }

        public void OpenCloseObjectAnimation()
        {
            this.rectTransform.gameObject.SetActive(true);

            this.TriggerOpenClose();
        }

        public bool IsObjectOpened()
        {
            return this.currentAnimationGoing.IsObjectOpened();
        }

        public void SetUnscaledTimeAnimation(bool UnscaledTimeAnimation)
        {
            this.animationParts.UnscaledTimeAnimation = UnscaledTimeAnimation;
        }

        public void SetAnimatioDuration(float duration)
        {
            if (duration > 0f)
                this.currentAnimationGoing.SetAniamtioDuration(duration);
            else
                this.currentAnimationGoing.SetAniamtioDuration(.01f);
        }

        public float GetAnimationDuration()
        {
            return this.currentAnimationGoing.GetAnimationDuration();
        }

        public void SetAnimationPosition(Vector2 StartAnchoredPos, Vector2 EndAnchoredPos, AnimationCurve EntryTween, AnimationCurve ExitTween)
        {
            this.currentAnimationGoing.SetAnimationPos(StartAnchoredPos, EndAnchoredPos, EntryTween, ExitTween, this.rectTransform);
        }

        public void SetAnimationScale(Vector3 StartAnchoredScale, Vector3 EndAnchoredScale, AnimationCurve EntryTween, AnimationCurve ExitTween)
        {
            this.currentAnimationGoing.SetAnimationScale(StartAnchoredScale, EndAnchoredScale, EntryTween, ExitTween);
        }

        public void SetAnimationRotation(Vector3 StartAnchoredEulerAng, Vector3 EndAnchoredEulerAng, AnimationCurve EntryTween, AnimationCurve ExitTween)
        {
            this.currentAnimationGoing.SetAnimationRotation(StartAnchoredEulerAng, EndAnchoredEulerAng, EntryTween, ExitTween);
        }

        public void SetFade(bool OverrideFade)
        {
            this.currentAnimationGoing.SetFade(OverrideFade);
        }

        public void SetFade()
        {
            this.currentAnimationGoing.SetFade(false);
        }

        public void SetFadeStartEndValues(float startValua, float endValue)
        {
            this.currentAnimationGoing.SetFadeValuesStartEnd(startValua, endValue);
        }

        public void SetAnimationProperties(AnimationParts animationParts)
        {
            this.animationParts = animationParts;
            this.currentAnimationGoing = new CurrentAnimation(animationParts);
        }

        public void ChangeSetState(bool opened)
        {
            this.currentAnimationGoing.SetStatus(opened);
        }

        #endregion

        #region Private_Methods

        private void Start()
        {
            AnimationParts.OnDisableOrDestroy += this.CheckTriggerEndState;
        }

        private void OnDestroy()
        {
            AnimationParts.OnDisableOrDestroy -= this.CheckTriggerEndState;
        }

        private void Update()
        {
            this.currentAnimationGoing.AnimationFrame(this.rectTransform);
        }

        private void LateUpdate()
        {
            this.currentAnimationGoing.LateAnimationFrame(this.rectTransform);
        }

        private void TriggerOpenClose()
        {
            if (!this.currentAnimationGoing.IsObjectOpened())
            {
                this.currentAnimationGoing.PlayOpenAnimations();
            }
            else
            {			
                this.currentAnimationGoing.PlayCloseAnimations();
            }
        }

        private void CheckTriggerEndState(bool disable, AnimationParts part)
        {
            if (part != this.animationParts)
                return;

            if (disable)
            {
                this.rectTransform.gameObject.SetActive(false);
            }
            else
            {
                if (this.gameObject && !this.rectTransform.gameObject == this.gameObject)
                {
                    Destroy(this.gameObject);
                }
			
                DestroyImmediate(this.rectTransform.gameObject);
            }
        }

        private void CheckIfCurrenAnimationGoingExits()
        {
            if (this.currentAnimationGoing == null)
            {
                this.SetAnimationProperties(this.animationParts);
            }
        }

        #endregion
    }

}