namespace Assets.UnityTestTools.Assertions
{

    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Linq;

    using Assets.UnityTestTools.Assertions.Comparers;

    using UnityEngine;

    using Debug = UnityEngine.Debug;
    using Object = UnityEngine.Object;

    [Serializable]
    public class AssertionComponent : MonoBehaviour, IAssertionComponentConfigurator
    {
        [SerializeField] public float checkAfterTime = 1f;
        [SerializeField] public bool repeatCheckTime = true;
        [SerializeField] public float repeatEveryTime = 1f;
        [SerializeField] public int checkAfterFrames = 1;
        [SerializeField] public bool repeatCheckFrame = true;
        [SerializeField] public int repeatEveryFrame = 1;
        [SerializeField] public bool hasFailed;

        [SerializeField] public CheckMethod checkMethods = CheckMethod.Start;
        [SerializeField] private ActionBase m_ActionBase;

        [SerializeField] public int checksPerformed = 0;

        private int m_CheckOnFrame;

        private string m_CreatedInFilePath = "";
        private int m_CreatedInFileLine = -1;

        public ActionBase Action
        {
            get { return this.m_ActionBase; }
            set
            {
                this.m_ActionBase = value;
                this.m_ActionBase.go = this.gameObject;
            }
        }

        public Object GetFailureReferenceObject()
        {
            #if UNITY_EDITOR
            if (!string.IsNullOrEmpty(this.m_CreatedInFilePath))
            {
                return UnityEditor.AssetDatabase.LoadAssetAtPath(this.m_CreatedInFilePath, typeof(Object));
            }
            #endif
            return this;
        }

        public string GetCreationLocation()
        {
            if (!string.IsNullOrEmpty(this.m_CreatedInFilePath))
            {
                var idx = this.m_CreatedInFilePath.LastIndexOf("\\") + 1;
                return string.Format("{0}, line {1} ({2})", this.m_CreatedInFilePath.Substring(idx), this.m_CreatedInFileLine, this.m_CreatedInFilePath);
            }
            return "";
        }

        public void Awake()
        {
            if (!Debug.isDebugBuild)
                Destroy(this);
            this.OnComponentCopy();
        }

        public void OnValidate()
        {
            if (Application.isEditor)
                this.OnComponentCopy();
        }

        private void OnComponentCopy()
        {
            if (this.m_ActionBase == null) return;
            var oldActionList = Resources.FindObjectsOfTypeAll(typeof(AssertionComponent)).Where(o => ((AssertionComponent)o).m_ActionBase == this.m_ActionBase && o != this);

            // if it's not a copy but a new component don't do anything
            if (!oldActionList.Any()) return;
            if (oldActionList.Count() > 1)
                Debug.LogWarning("More than one refence to comparer found. This shouldn't happen");

            var oldAction = oldActionList.First() as AssertionComponent;
            this.m_ActionBase = oldAction.m_ActionBase.CreateCopy(oldAction.gameObject, this.gameObject);
        }

        public void Start()
        {
            this.CheckAssertionFor(CheckMethod.Start);

            if (this.IsCheckMethodSelected(CheckMethod.AfterPeriodOfTime))
            {
                this.StartCoroutine("CheckPeriodically");
            }
            if (this.IsCheckMethodSelected(CheckMethod.Update))
            {
                this.m_CheckOnFrame = Time.frameCount + this.checkAfterFrames;
            }
        }

        public IEnumerator CheckPeriodically()
        {
            yield return new WaitForSeconds(this.checkAfterTime);
            this.CheckAssertionFor(CheckMethod.AfterPeriodOfTime);
            while (this.repeatCheckTime)
            {
                yield return new WaitForSeconds(this.repeatEveryTime);
                this.CheckAssertionFor(CheckMethod.AfterPeriodOfTime);
            }
        }

        public bool ShouldCheckOnFrame()
        {
            if (Time.frameCount > this.m_CheckOnFrame)
            {
                if (this.repeatCheckFrame)
                    this.m_CheckOnFrame += this.repeatEveryFrame;
                else
                    this.m_CheckOnFrame = Int32.MaxValue;
                return true;
            }
            return false;
        }

        public void OnDisable()
        {
            this.CheckAssertionFor(CheckMethod.OnDisable);
        }

        public void OnEnable()
        {
            this.CheckAssertionFor(CheckMethod.OnEnable);
        }

        public void OnDestroy()
        {
            this.CheckAssertionFor(CheckMethod.OnDestroy);
        }

        public void Update()
        {
            if (this.IsCheckMethodSelected(CheckMethod.Update) && this.ShouldCheckOnFrame())
            {
                this.CheckAssertionFor(CheckMethod.Update);
            }
        }

        public void FixedUpdate()
        {
            this.CheckAssertionFor(CheckMethod.FixedUpdate);
        }

        public void LateUpdate()
        {
            this.CheckAssertionFor(CheckMethod.LateUpdate);
        }

        public void OnControllerColliderHit()
        {
            this.CheckAssertionFor(CheckMethod.OnControllerColliderHit);
        }

        public void OnParticleCollision()
        {
            this.CheckAssertionFor(CheckMethod.OnParticleCollision);
        }

        public void OnJointBreak()
        {
            this.CheckAssertionFor(CheckMethod.OnJointBreak);
        }

        public void OnBecameInvisible()
        {
            this.CheckAssertionFor(CheckMethod.OnBecameInvisible);
        }

        public void OnBecameVisible()
        {
            this.CheckAssertionFor(CheckMethod.OnBecameVisible);
        }

        public void OnTriggerEnter()
        {
            this.CheckAssertionFor(CheckMethod.OnTriggerEnter);
        }

        public void OnTriggerExit()
        {
            this.CheckAssertionFor(CheckMethod.OnTriggerExit);
        }

        public void OnTriggerStay()
        {
            this.CheckAssertionFor(CheckMethod.OnTriggerStay);
        }

        public void OnCollisionEnter()
        {
            this.CheckAssertionFor(CheckMethod.OnCollisionEnter);
        }

        public void OnCollisionExit()
        {
            this.CheckAssertionFor(CheckMethod.OnCollisionExit);
        }

        public void OnCollisionStay()
        {
            this.CheckAssertionFor(CheckMethod.OnCollisionStay);
        }

        public void OnTriggerEnter2D()
        {
            this.CheckAssertionFor(CheckMethod.OnTriggerEnter2D);
        }

        public void OnTriggerExit2D()
        {
            this.CheckAssertionFor(CheckMethod.OnTriggerExit2D);
        }

        public void OnTriggerStay2D()
        {
            this.CheckAssertionFor(CheckMethod.OnTriggerStay2D);
        }

        public void OnCollisionEnter2D()
        {
            this.CheckAssertionFor(CheckMethod.OnCollisionEnter2D);
        }

        public void OnCollisionExit2D()
        {
            this.CheckAssertionFor(CheckMethod.OnCollisionExit2D);
        }

        public void OnCollisionStay2D()
        {
            this.CheckAssertionFor(CheckMethod.OnCollisionStay2D);
        }

        private void CheckAssertionFor(CheckMethod checkMethod)
        {
            if (this.IsCheckMethodSelected(checkMethod))
            {
                Assertions.CheckAssertions(this);
            }
        }

        public bool IsCheckMethodSelected(CheckMethod method)
        {
            return method == (this.checkMethods & method);
        }


        #region Assertion Component create methods

        public static T Create<T>(CheckMethod checkOnMethods, GameObject gameObject, string propertyPath) where T : ActionBase
        {
            IAssertionComponentConfigurator configurator;
            return Create<T>(out configurator, checkOnMethods, gameObject, propertyPath);
        }

        public static T Create<T>(out IAssertionComponentConfigurator configurator, CheckMethod checkOnMethods, GameObject gameObject, string propertyPath) where T : ActionBase
        {
            return CreateAssertionComponent<T>(out configurator, checkOnMethods, gameObject, propertyPath);
        }

        public static T Create<T>(CheckMethod checkOnMethods, GameObject gameObject, string propertyPath, GameObject gameObject2, string propertyPath2) where T : ComparerBase
        {
            IAssertionComponentConfigurator configurator;
            return Create<T>(out configurator, checkOnMethods, gameObject, propertyPath, gameObject2, propertyPath2);
        }

        public static T Create<T>(out IAssertionComponentConfigurator configurator, CheckMethod checkOnMethods, GameObject gameObject, string propertyPath, GameObject gameObject2, string propertyPath2) where T : ComparerBase
        {
            var comparer = CreateAssertionComponent<T>(out configurator, checkOnMethods, gameObject, propertyPath);
            comparer.compareToType = ComparerBase.CompareToType.CompareToObject;
            comparer.other = gameObject2;
            comparer.otherPropertyPath = propertyPath2;
            return comparer;
        }

        public static T Create<T>(CheckMethod checkOnMethods, GameObject gameObject, string propertyPath, object constValue) where T : ComparerBase
        {
            IAssertionComponentConfigurator configurator;
            return Create<T>(out configurator, checkOnMethods, gameObject, propertyPath, constValue);
        }

        public static T Create<T>(out IAssertionComponentConfigurator configurator, CheckMethod checkOnMethods, GameObject gameObject, string propertyPath, object constValue) where T : ComparerBase
        {
            var comparer = CreateAssertionComponent<T>(out configurator, checkOnMethods, gameObject, propertyPath);
            if (constValue == null)
            {
                comparer.compareToType = ComparerBase.CompareToType.CompareToNull;
                return comparer;
            }
            comparer.compareToType = ComparerBase.CompareToType.CompareToConstantValue;
            comparer.ConstValue = constValue;
            return comparer;
        }

        private static T CreateAssertionComponent<T>(out IAssertionComponentConfigurator configurator, CheckMethod checkOnMethods, GameObject gameObject, string propertyPath) where T : ActionBase
        {
            var ac = gameObject.AddComponent<AssertionComponent>();
            ac.checkMethods = checkOnMethods;
            var comparer = ScriptableObject.CreateInstance<T>();
            ac.Action = comparer;
            ac.Action.go = gameObject;
            ac.Action.thisPropertyPath = propertyPath;
            configurator = ac;

#if !UNITY_METRO
            var stackTrace = new StackTrace(true);
            var thisFileName = stackTrace.GetFrame(0).GetFileName();
            for (int i = 1; i < stackTrace.FrameCount; i++)
            {
                var stackFrame = stackTrace.GetFrame(i);
                if (stackFrame.GetFileName() != thisFileName)
                {
                    string filePath = stackFrame.GetFileName().Substring(Application.dataPath.Length - "Assets".Length);
                    ac.m_CreatedInFilePath = filePath;
                    ac.m_CreatedInFileLine = stackFrame.GetFileLineNumber();
                    break;
                }
            }
#endif  // if !UNITY_METRO
            return comparer;
        }

        #endregion

        #region AssertionComponentConfigurator
        public int UpdateCheckStartOnFrame { set { this.checkAfterFrames = value; } }
        public int UpdateCheckRepeatFrequency { set { this.repeatEveryFrame = value; } }
        public bool UpdateCheckRepeat { set { this.repeatCheckFrame = value; } }
        public float TimeCheckStartAfter { set { this.checkAfterTime = value; } }
        public float TimeCheckRepeatFrequency { set { this.repeatEveryTime = value; } }
        public bool TimeCheckRepeat { set { this.repeatCheckTime = value; } }
        public AssertionComponent Component { get { return this; } }
        #endregion
    }

    public interface IAssertionComponentConfigurator
    {
        /// <summary>
        /// If the assertion is evaluated in Update, after how many frame should the evaluation start. Deafult is 1 (first frame)
        /// </summary>
        int UpdateCheckStartOnFrame { set; }
        /// <summary>
        /// If the assertion is evaluated in Update and UpdateCheckRepeat is true, how many frame should pass between evaluations
        /// </summary>
        int UpdateCheckRepeatFrequency { set; }
        /// <summary>
        /// If the assertion is evaluated in Update, should the evaluation be repeated after UpdateCheckRepeatFrequency frames
        /// </summary>
        bool UpdateCheckRepeat { set; }

        /// <summary>
        /// If the assertion is evaluated after a period of time, after how many seconds the first evaluation should be done
        /// </summary>
        float TimeCheckStartAfter { set; }
        /// <summary>
        /// If the assertion is evaluated after a period of time and TimeCheckRepeat is true, after how many seconds should the next evaluation happen
        /// </summary>
        float TimeCheckRepeatFrequency { set; }
        /// <summary>
        /// If the assertion is evaluated after a period, should the evaluation happen again after TimeCheckRepeatFrequency seconds
        /// </summary>
        bool TimeCheckRepeat { set; }

        AssertionComponent Component { get; }
    }
}
