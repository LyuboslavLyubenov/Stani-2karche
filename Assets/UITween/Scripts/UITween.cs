namespace Assets.UITween.Scripts
{

    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class CurrentAnimation
    {
        private AnimationParts animationPart;

        private float counterTween = 2f;

        private enum States
        {
            AWAKE,
            READY,
            START,
            ENDED,
            FINALENDED,
            COUNT}

        ;

        private States AnimationStates = States.AWAKE;

        public CurrentAnimation(AnimationParts animationPart)
        {
            this.animationPart = animationPart;
        }

        public void AnimationFrame(RectTransform rectTransform)
        {
            if (this.AnimationStates == States.AWAKE)
                return;

            if (this.counterTween <= 1f) // Max value is 1f
            {
                this.SetAnimationOnFrame(rectTransform, this.counterTween);
            }
        }

        public void SetAnimationOnFrame(RectTransform rectTransform, float percentage)
        {
            // Position Animation
            if (this.animationPart.PositionPropetiesAnim.IsPositionEnabled())
            {
                this.MoveAnimation(rectTransform, percentage);
            }
			
            // Rotation Animation
            if (this.animationPart.RotationPropetiesAnim.IsRotationEnabled())
            {
                this.RotateAnimation(rectTransform, percentage);
            }
			
            // Scale Animation
            if (this.animationPart.ScalePropetiesAnim.IsScaleEnabled())
            {
                this.ScaleAnimation(rectTransform, percentage);
            }
			
            // Fade Animation
            if (this.animationPart.FadePropetiesAnim.IsFadeEnabled())
            {
                this.SetAlphaValue(rectTransform.transform, percentage);
            }
        }

        public void LateAnimationFrame(RectTransform rectTransform)
        {
            if (this.AnimationStates == States.AWAKE)
                return;

            if (this.counterTween <= 1f) // Max value is 1f
            {
                float deltaTime = (this.animationPart.UnscaledTimeAnimation) ? Time.unscaledDeltaTime : Time.deltaTime;

                this.counterTween += deltaTime / this.animationPart.GetAnimationDuration();
            }
            else
            {
                if (this.AnimationStates != States.FINALENDED)
                {
                    this.SetAnimationOnFrame(rectTransform, 1f);

                    if (this.AnimationStates != States.ENDED
                        && this.AnimationStates != States.FINALENDED
                        && this.animationPart.AtomicAnimation)
                    {
                        this.animationPart.Ended();
                        this.AnimationStates = States.ENDED;
                    }
                    else
                    {
                        this.animationPart.FinalEnd();
                        this.AnimationStates = States.FINALENDED;
                    }
                }
            }

            if (this.counterTween > .9f && !this.animationPart.AtomicAnimation)
            {
                if (this.AnimationStates != States.ENDED
                    && this.AnimationStates != States.FINALENDED)
                {
                    this.animationPart.Ended();
                    this.AnimationStates = States.ENDED;
                }
            }

            this.animationPart.FrameCheck();
        }

        public void PlayOpenAnimations()
        {
            // Open Pos Anim
            if (this.animationPart.PositionPropetiesAnim.IsPositionEnabled())
            {
                this.SetCurrentAnimPos(this.animationPart.PositionPropetiesAnim.TweenCurveEnterPos,
                    this.animationPart.PositionPropetiesAnim.StartPos,
                    this.animationPart.PositionPropetiesAnim.EndPos);
            }

            // Open Rot Anim
            if (this.animationPart.RotationPropetiesAnim.IsRotationEnabled())
            {
                this.SetCurrentAnimRot(this.animationPart.RotationPropetiesAnim.TweenCurveEnterRot,
                    this.animationPart.RotationPropetiesAnim.StartRot,
                    this.animationPart.RotationPropetiesAnim.EndRot);
            }

            // Open scale Anim
            if (this.animationPart.ScalePropetiesAnim.IsScaleEnabled())
            {
                this.SetCurrentAnimScale(this.animationPart.ScalePropetiesAnim.TweenCurveEnterScale,
                    this.animationPart.ScalePropetiesAnim.StartScale,
                    this.animationPart.ScalePropetiesAnim.EndScale);
            }

            // Open Fade Anim
            if (this.animationPart.FadePropetiesAnim.IsFadeEnabled())
            {
                this.SetFadeAnimation(this.animationPart.FadePropetiesAnim.GetStartFadeValue(), 
                    this.animationPart.FadePropetiesAnim.GetEndFadeValue());
            }

            this.counterTween = 0f;

            this.AnimationStates = States.READY;

            this.animationPart.ChangeStatus();
            this.animationPart.CheckCallbackStatus();
        }

        public void SetStatus(bool status)
        {
            this.animationPart.SetStatus(status);
        }

        public void PlayCloseAnimations()
        {
            // Close Pos Anim
            if (this.animationPart.PositionPropetiesAnim.IsPositionEnabled())
            {
                this.SetCurrentAnimPos(this.animationPart.PositionPropetiesAnim.TweenCurveExitPos,
                    this.animationPart.PositionPropetiesAnim.EndPos,
                    this.animationPart.PositionPropetiesAnim.StartPos);
            }

            // Close Rot Anim
            if (this.animationPart.RotationPropetiesAnim.IsRotationEnabled())
            {
                this.SetCurrentAnimRot(this.animationPart.RotationPropetiesAnim.TweenCurveExitRot,
                    this.animationPart.RotationPropetiesAnim.EndRot,
                    this.animationPart.RotationPropetiesAnim.StartRot);
            }

            // Close Scale Anim
            if (this.animationPart.ScalePropetiesAnim.IsScaleEnabled())
            {
                this.SetCurrentAnimScale(this.animationPart.ScalePropetiesAnim.TweenCurveExitScale,
                    this.animationPart.ScalePropetiesAnim.EndScale,
                    this.animationPart.ScalePropetiesAnim.StartScale);
            }

            // Close Fade Anim
            if (this.animationPart.FadePropetiesAnim.IsFadeEnabled())
            {
                this.SetFadeAnimation(this.animationPart.FadePropetiesAnim.GetEndFadeValue(), 
                    this.animationPart.FadePropetiesAnim.GetStartFadeValue());
            }

            this.counterTween = 0f;

            this.AnimationStates = States.READY;

            this.animationPart.ChangeStatus();
            this.animationPart.CheckCallbackStatus();
        }

        public void SetAnimationPos(Vector2 StartAnchoredPos, Vector2 EndAnchoredPos, AnimationCurve EntryTween, AnimationCurve ExitTween, RectTransform rectTransform)
        {
            this.animationPart.PositionPropetiesAnim.SetPositionEnable(true);
            this.animationPart.PositionPropetiesAnim.SetPosStart(StartAnchoredPos, rectTransform);
            this.animationPart.PositionPropetiesAnim.SetPosEnd(EndAnchoredPos, rectTransform.transform);
            this.animationPart.PositionPropetiesAnim.SetAniamtionsCurve(EntryTween, ExitTween);
        }

        public void SetAnimationScale(Vector2 StartAnchoredScale, Vector2 EndAnchoredScale, AnimationCurve EntryTween, AnimationCurve ExitTween)
        {
            this.animationPart.ScalePropetiesAnim.StartScale = StartAnchoredScale;
            this.animationPart.ScalePropetiesAnim.SetScaleEnable(true);
            this.animationPart.ScalePropetiesAnim.EndScale = EndAnchoredScale;
            this.animationPart.ScalePropetiesAnim.SetAniamtionsCurve(EntryTween, ExitTween);
        }

        public void SetAnimationRotation(Vector2 StartAnchoredEulerAng, Vector2 EndAnchoredEulerAng, AnimationCurve EntryTween, AnimationCurve ExitTween)
        {
            this.animationPart.RotationPropetiesAnim.SetRotationEnable(true);
            this.animationPart.RotationPropetiesAnim.StartRot = StartAnchoredEulerAng;
            this.animationPart.RotationPropetiesAnim.EndRot = EndAnchoredEulerAng;
            this.animationPart.RotationPropetiesAnim.SetAniamtionsCurve(EntryTween, ExitTween);
        }

        public void SetFade(bool OverrideFade)
        {
            this.animationPart.FadePropetiesAnim.SetFadeEnable(true);
            this.animationPart.FadePropetiesAnim.SetFadeOverride(OverrideFade);
        }

        public void SetFadeValuesStartEnd(float startAlphaValue, float endAlphaValue)
        {
            this.animationPart.FadePropetiesAnim.SetFadeValues(startAlphaValue, endAlphaValue);
        }

        public bool IsObjectOpened()
        {
            return this.animationPart.IsObjectOpened();
        }

        public void SetAniamtioDuration(float duration)
        {
            this.animationPart.SetAniamtioDuration(duration);
        }

        public float GetAnimationDuration()
        {
            return this.animationPart.GetAnimationDuration();
        }

        #region PositionAnim

        private AnimationCurve currentAnimationCurvePos;
        private Vector3 currentStartPos;
        private Vector3 currentEndPos;

        public void SetCurrentAnimPos(AnimationCurve currentAnimationCurvePos, Vector3 currentStartPos, Vector3 currentEndPos)
        {
            this.currentAnimationCurvePos = currentAnimationCurvePos;
            this.currentStartPos = currentStartPos;
            this.currentEndPos = currentEndPos;
        }

        public void MoveAnimation(RectTransform _rectTransform, float _counterTween)
        {
            float evaluatedValue = this.currentAnimationCurvePos.Evaluate(_counterTween);
            Vector3 valueAdded = (this.currentEndPos - this.currentStartPos) * evaluatedValue;

            _rectTransform.anchoredPosition = (Vector2)(this.currentStartPos + valueAdded);
        }

        #endregion

        #region ScaleAnim

        private AnimationCurve currentAnimationCurveScale;
        private Vector3 currentStartScale;
        private Vector3 currentEndScale;

        public void SetCurrentAnimScale(AnimationCurve currentAnimationCurveScale, Vector3 currentStartScale, Vector3 currentEndScale)
        {
            this.currentAnimationCurveScale = currentAnimationCurveScale;
            this.currentStartScale = currentStartScale;
            this.currentEndScale = currentEndScale;
        }

        public void ScaleAnimation(RectTransform _rectTransform, float _counterTween)
        {
            float evaluatedValue = this.currentAnimationCurveScale.Evaluate(_counterTween);
            Vector3 valueAdded = (this.currentEndScale - this.currentStartScale) * evaluatedValue;

            _rectTransform.localScale = this.currentStartScale + valueAdded;
        }

        #endregion

        #region RotationAnim

        private AnimationCurve currentAnimationCurveRot;
        private Vector3 currentStartRot;
        private Vector3 currentEndRot;

        public void SetCurrentAnimRot(AnimationCurve currentAnimationCurveRot, Vector3 currentStartRot, Vector3 currentEndRot)
        {
            this.currentAnimationCurveRot = currentAnimationCurveRot;
            this.currentStartRot = currentStartRot;
            this.currentEndRot = currentEndRot;
        }

        public void RotateAnimation(RectTransform _rectTransform, float _counterTween)
        {
            float evaluatedValue = this.currentAnimationCurveRot.Evaluate(_counterTween);
            Vector3 valueAdded = (this.currentEndRot - this.currentStartRot) * evaluatedValue;

            _rectTransform.localEulerAngles = this.currentStartRot + valueAdded;
        }

        #endregion

        #region FadeAnim

        private float startAlphaValue;
        private float endAlphaValue;

        public void SetFadeAnimation(float startAlphaValue, float endAlphaValue)
        {
            this.startAlphaValue = startAlphaValue;
            this.endAlphaValue = endAlphaValue;
        }

        public void SetAlphaValue(Transform _objectToSetAlpha, float _counterTween)
        {
            if (_objectToSetAlpha.GetComponent<MaskableGraphic>())
            {
                MaskableGraphic GraphicElement = _objectToSetAlpha.GetComponent<MaskableGraphic>();

                Color objectColor = GraphicElement.color;

                _counterTween = Mathf.Clamp(_counterTween, 0f, 1f);

                objectColor.a = Mathf.Abs(this.startAlphaValue + (this.endAlphaValue - this.startAlphaValue) * _counterTween);
                GraphicElement.color = objectColor;
            }

            if (_objectToSetAlpha.childCount > 0)
            {
                for (int i = 0; i < _objectToSetAlpha.childCount; i++)
                {
                    Transform childNumber = _objectToSetAlpha.GetChild(i);

                    if (childNumber.gameObject.activeSelf &&
                        (!childNumber.GetComponent<ReferencedFrom>() || this.animationPart.FadePropetiesAnim.IsFadeOverrideEnabled()))
                    {
                        this.SetAlphaValue(childNumber, _counterTween);
                    }
                }
            }
        }

        #endregion
    }

    [System.Serializable]
    public class PositionPropetiesAnim
    {
        #region PositionEditor

        [SerializeField]
        [HideInInspector]
        private bool positionEnabled;

        public void SetPositionEnable(bool enabled)
        {
            this.positionEnabled = enabled;
        }

        public bool IsPositionEnabled()
        {
            return this.positionEnabled;
        }

        [HideInInspector]
        public AnimationCurve TweenCurveEnterPos;
        [HideInInspector]
        public AnimationCurve TweenCurveExitPos;
        [HideInInspector]
        public Vector3 StartPos;
        [HideInInspector]
        public Vector3 EndPos;
        #if UNITY_EDITOR
        [SerializeField] [HideInInspector]
        public Vector3 StartWorldPos;
        [SerializeField] [HideInInspector]
        public Vector3 EndWorldPos;
        #endif

        public void SetPosStart(Vector3 StartPos, RectTransform rectTr)
        {
            this.StartPos = StartPos;
#if UNITY_EDITOR
            float xMes = (rectTr.anchorMin.x + rectTr.anchorMax.x) / 2f;
            float yMes = (rectTr.anchorMin.y + rectTr.anchorMax.y) / 2f;
			
            Transform rootObject = rectTr.root;
			
            Rect rectangleScreen = rootObject.GetComponent<RectTransform>().rect;
			
            this.StartWorldPos.x = (xMes * rectangleScreen.width + StartPos.x) * rootObject.localScale.x;
            this.StartWorldPos.y = (yMes * rectangleScreen.height + StartPos.y) * rootObject.localScale.y;
#endif
        }

        public void SetPosEnd(Vector3 EndPos, Transform rectTr)
        {
            this.EndPos = EndPos;
#if UNITY_EDITOR
            this.EndWorldPos.x = this.StartWorldPos.x + (EndPos.x - this.StartPos.x) * rectTr.root.localScale.x;
            this.EndWorldPos.y = this.StartWorldPos.y + (EndPos.y - this.StartPos.y) * rectTr.root.localScale.y;
#endif
        }

        public void SetAniamtionsCurve(AnimationCurve EntryTween, AnimationCurve ExitTween)
        {
            this.TweenCurveEnterPos = EntryTween;
            this.TweenCurveExitPos = ExitTween;
        }

        #endregion
    }

    [System.Serializable]
    public class ScalePropetiesAnim
    {
        #region ScaleEditor

        [SerializeField]
        [HideInInspector]
        private bool scaleEnabled;

        public void SetScaleEnable(bool enabled)
        {
            this.scaleEnabled = enabled;
        }

        public bool IsScaleEnabled()
        {
            return this.scaleEnabled;
        }

        [HideInInspector]
        public AnimationCurve TweenCurveEnterScale;
        [HideInInspector]
        public AnimationCurve TweenCurveExitScale;
        [HideInInspector]
        public Vector3 StartScale;
        [HideInInspector]
        public Vector3 EndScale;

        public void SetAniamtionsCurve(AnimationCurve EntryTween, AnimationCurve ExitTween)
        {
            this.TweenCurveEnterScale = EntryTween;
            this.TweenCurveExitScale = ExitTween;
        }

        #endregion
    }

    [System.Serializable]
    public class RotationPropetiesAnim
    {
        #region RotationEditor

        [SerializeField]
        [HideInInspector]
        private bool rotationEnabled;

        public void SetRotationEnable(bool enabled)
        {
            this.rotationEnabled = enabled;
        }

        public bool IsRotationEnabled()
        {
            return this.rotationEnabled;
        }

        [HideInInspector]
        public AnimationCurve TweenCurveEnterRot;
        [HideInInspector]
        public AnimationCurve TweenCurveExitRot;
        [HideInInspector]
        public Vector3 StartRot;
        [HideInInspector]
        public Vector3 EndRot;

        public void SetAniamtionsCurve(AnimationCurve EntryTween, AnimationCurve ExitTween)
        {
            this.TweenCurveEnterRot = EntryTween;
            this.TweenCurveExitRot = ExitTween;
        }

        #endregion
    }

    [System.Serializable]
    public class FadePropetiesAnim
    {
        #region FadeEditor

        [SerializeField]
        [HideInInspector]
        private bool fadeInOutEnabled;

        [SerializeField]
        [HideInInspector]
        private bool fadeOverride;

        [SerializeField]
        [HideInInspector]
        private float startFade = 0f;

        [SerializeField]
        [HideInInspector]
        private float endFade = 1f;

        public void SetFadeEnable(bool enabled)
        {
            this.fadeInOutEnabled = enabled;
        }

        public void SetFadeValues(float startFade, float endFade)
        {
            if (endFade < startFade)
            {
                Debug.LogError("End Value should be greater than the start value, values not changed");
                return;
            } 

            this.startFade = startFade;
            this.endFade = endFade;
        }

        public float GetStartFadeValue()
        {
            return this.startFade;
        }

        public float GetEndFadeValue()
        {
            return this.endFade;
        }

        public bool IsFadeEnabled()
        {
            return this.fadeInOutEnabled;
        }

        public void SetFadeOverride(bool enabled)
        {
            this.fadeOverride = enabled;
        }

        public bool IsFadeOverrideEnabled()
        {
            return this.fadeOverride;
        }

        #endregion
    }

    public interface IAniamtionPartProxy
    {
        bool IsObjectOpened();

        void ChangeStatus();

        void SetAniamtioDuration(float duration);

        float GetAnimationDuration();
    }

    [System.Serializable]
    public class AnimationParts : IAniamtionPartProxy
    {
        public delegate void DisableOrDestroy(bool disable,AnimationParts part);

        public static event DisableOrDestroy OnDisableOrDestroy;

        #region PositionEditor

        [HideInInspector]
        public PositionPropetiesAnim PositionPropetiesAnim = new PositionPropetiesAnim();

        #endregion

        #region ScaleEditor

        [HideInInspector]
        public ScalePropetiesAnim ScalePropetiesAnim = new ScalePropetiesAnim();

        #endregion

        #region RotationEditor

        [HideInInspector]
        public RotationPropetiesAnim RotationPropetiesAnim = new RotationPropetiesAnim();

        #endregion

        #region FadeEditor

        [HideInInspector]
        public FadePropetiesAnim FadePropetiesAnim = new FadePropetiesAnim();

        #endregion

        #region PUBLIC_Var

        public void SetAniamtioDuration(float duration)
        {
            if (duration > 0f)
                this.animationDuration = duration;
            else
                duration = 0.01f;
        }

        public float GetAnimationDuration()
        {
            return this.animationDuration;
        }

        public bool UnscaledTimeAnimation = false;
        public bool SaveState = false;
        public bool AtomicAnimation = false;

        public enum State
        {
            OPEN,
            CLOSE}

        ;

        public State ObjectState = State.CLOSE;


        public enum EndTweenClose
        {
            DEACTIVATE,
            DESTROY,
            NOTHING}

        ;

        public EndTweenClose EndState = EndTweenClose.DEACTIVATE;

        public enum CallbackCall
        {
            END_OF_INTRO_ANIM,
            END_OF_EXIT_ANIM,
            END_OF_INTRO_AND_END_OF_EXIT_ANIM,
            START_INTRO_ANIM,
            START_INTRO_END_OF_EXIT_ANIM,
            START_INTRO_END_OF_INTRO_ANIM,
            START_INTRO_END_OF_INTRO_AND_END_OF_EXIT_ANIM,
            START_EXIT_ANIM,
            START_EXIT_START_INTRO_ANIM,
            START_EXIT_END_OF_EXIT_ANIM,
            START_EXIT_END_OF_INTRO_ANIM,
            START_EXIT_END_OF_INTRO_AND_END_OF_EXIT_ANIM,
            START_INTRO_AND_START_EXIT_END_OF_EXIT_ANIM,
            START_INTRO_AND_START_EXIT_END_OF_INTRO_ANIM,
            START_INTRO_AND_START_EXIT_END_OF_INTRO_AND_END_OF_EXIT_ANIM,
            NOTHING}

        ;

        public CallbackCall CallCallback = CallbackCall.END_OF_INTRO_ANIM;

        public UnityEvent IntroEvents = new UnityEvent();
        public UnityEvent ExitEvents = new UnityEvent();
        private UnityEvent CallBackObject;

        #endregion

        #region PRIVATE_Var

        private bool CheckNextFrame = false;
        private bool CallOnThisFrame = false;

        [SerializeField]
        [HideInInspector]
        private float animationDuration = 1f;

        #endregion

        #region PUBLIC_Methods

        public AnimationParts(State ObjectState, bool UnscaledTimeAnimation, bool SaveState, bool AtomicAnim, EndTweenClose EndState, CallbackCall CallCallback, UnityEvent IntroEvents, UnityEvent ExitEvents)
        {
            this.ObjectState = ObjectState;
            this.UnscaledTimeAnimation = UnscaledTimeAnimation;
            this.SaveState = SaveState;
            this.AtomicAnimation = AtomicAnim;
            this.EndState = EndState;
            this.CallCallback = CallCallback;
            this.IntroEvents = IntroEvents;
            this.ExitEvents = ExitEvents;
        }

        public void CheckCallbackStatus()
        {
            if (this.CallCallback != CallbackCall.NOTHING)
            {
                if ((this.CallCallback == CallbackCall.START_INTRO_END_OF_EXIT_ANIM
                    || this.CallCallback == CallbackCall.START_INTRO_ANIM
                    || this.CallCallback == CallbackCall.START_INTRO_END_OF_INTRO_ANIM
                    || this.CallCallback == CallbackCall.START_INTRO_END_OF_INTRO_AND_END_OF_EXIT_ANIM
                    || this.CallCallback == CallbackCall.START_INTRO_AND_START_EXIT_END_OF_EXIT_ANIM
                    || this.CallCallback == CallbackCall.START_INTRO_AND_START_EXIT_END_OF_INTRO_ANIM
                    || this.CallCallback == CallbackCall.START_INTRO_AND_START_EXIT_END_OF_INTRO_AND_END_OF_EXIT_ANIM
                    || this.CallCallback == CallbackCall.START_EXIT_START_INTRO_ANIM) && this.ObjectState == State.OPEN)
                {
                    this.CheckCallBack(this.IntroEvents);
                }
                else if ((this.CallCallback == CallbackCall.START_EXIT_END_OF_EXIT_ANIM
                         || this.CallCallback == CallbackCall.START_EXIT_ANIM
                         || this.CallCallback == CallbackCall.START_EXIT_END_OF_INTRO_ANIM
                         || this.CallCallback == CallbackCall.START_EXIT_END_OF_INTRO_AND_END_OF_EXIT_ANIM
                         || this.CallCallback == CallbackCall.START_INTRO_AND_START_EXIT_END_OF_EXIT_ANIM
                         || this.CallCallback == CallbackCall.START_INTRO_AND_START_EXIT_END_OF_INTRO_ANIM
                         || this.CallCallback == CallbackCall.START_INTRO_AND_START_EXIT_END_OF_INTRO_AND_END_OF_EXIT_ANIM
                         || this.CallCallback == CallbackCall.START_EXIT_START_INTRO_ANIM) && this.ObjectState == State.CLOSE)
                {
                    this.CheckCallBack(this.ExitEvents);
                }
            }
        }

        public void FinalEnd()
        {
            if (this.ObjectState == State.CLOSE)
            {
                if (this.EndState == EndTweenClose.DEACTIVATE)
                {
                    if (OnDisableOrDestroy != null)
                    {
                        OnDisableOrDestroy(true, this);
                    }
                }
                else if (this.EndState == EndTweenClose.DESTROY)
                {
                    if (OnDisableOrDestroy != null)
                    {
                        OnDisableOrDestroy(false, this);
                    }
                }
            }

            if (this.SaveState)
            {
                this.ObjectState = (this.ObjectState == State.OPEN) ? State.CLOSE : State.OPEN;
            }		
        }

        public void Ended()
        {
            if (this.CallCallback != CallbackCall.NOTHING)
            {
                if (this.ObjectState == State.CLOSE)
                {
                    if (this.CallCallback == CallbackCall.END_OF_EXIT_ANIM
                        || this.CallCallback == CallbackCall.END_OF_INTRO_AND_END_OF_EXIT_ANIM
                        || this.CallCallback == CallbackCall.START_INTRO_END_OF_EXIT_ANIM
                        || this.CallCallback == CallbackCall.START_INTRO_END_OF_INTRO_AND_END_OF_EXIT_ANIM
                        || this.CallCallback == CallbackCall.START_EXIT_END_OF_EXIT_ANIM
                        || this.CallCallback == CallbackCall.START_EXIT_END_OF_INTRO_AND_END_OF_EXIT_ANIM
                        || this.CallCallback == CallbackCall.START_INTRO_AND_START_EXIT_END_OF_EXIT_ANIM
                        || this.CallCallback == CallbackCall.START_INTRO_AND_START_EXIT_END_OF_INTRO_AND_END_OF_EXIT_ANIM)
                    {
                        this.CheckCallBack(this.ExitEvents);
                    }                
                }

                if ((this.CallCallback == CallbackCall.END_OF_INTRO_ANIM
                    || this.CallCallback == CallbackCall.END_OF_INTRO_AND_END_OF_EXIT_ANIM
                    || this.CallCallback == CallbackCall.START_INTRO_END_OF_INTRO_ANIM
                    || this.CallCallback == CallbackCall.START_INTRO_END_OF_INTRO_AND_END_OF_EXIT_ANIM
                    || this.CallCallback == CallbackCall.START_EXIT_END_OF_INTRO_ANIM
                    || this.CallCallback == CallbackCall.START_EXIT_END_OF_INTRO_AND_END_OF_EXIT_ANIM
                    || this.CallCallback == CallbackCall.START_INTRO_AND_START_EXIT_END_OF_INTRO_ANIM
                    || this.CallCallback == CallbackCall.START_INTRO_AND_START_EXIT_END_OF_INTRO_AND_END_OF_EXIT_ANIM) && this.ObjectState == State.OPEN)
                {
                    this.CheckCallBack(this.IntroEvents);
                }       
            }
        }

        public void FrameCheck()
        {
            if (this.CheckNextFrame)
            {
                if (this.CallOnThisFrame)
                {
                    this.CallCallbackObjects();
                }

                this.CallOnThisFrame = !this.CallOnThisFrame;
            }
        }

        public bool IsObjectOpened()
        {
            if (this.ObjectState == State.CLOSE)
            {
                return false;
            }

            return true;
        }

        public void ChangeStatus()
        {
            if (this.ObjectState == State.CLOSE)
            {
                this.ObjectState = State.OPEN;
            }
            else
            {
                this.ObjectState = State.CLOSE;
            }
        }

        public void SetStatus(bool open)
        {
            if (open)
            {
                this.ObjectState = State.OPEN;
            }
            else
            {
                this.ObjectState = State.CLOSE;
            }
        }

        #endregion

        #region PRIVATE_Methods

        private void CheckCallBack(UnityEvent CallbackObject)
        {
            this.CallBackObject = CallbackObject;
            this.CheckNextFrame = !this.CheckNextFrame;        
        }

        private void CallCallbackObjects()
        {           
            this.CheckNextFrame = !this.CheckNextFrame;

            this.CallBackObject.Invoke();     
        }

        #endregion
    }
}