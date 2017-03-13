namespace Assets.Zenject.Source.Editor.Testing
{

    using System;
    using System.Linq;

    using Assets.Zenject.Source.Install.Contexts;
    using Assets.Zenject.Source.Main;
    using Assets.Zenject.Source.Runtime.Kernels;

    using NUnit.Framework;

    using UnityEditor.SceneManagement;

    using UnityEngine;

    using Assert = Assets.Zenject.Source.Internal.Assert;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ValidateOnlyAttribute : Attribute
    {
    }

    public abstract class ZenjectIntegrationTestFixture
    {
        SceneContext _sceneContext;

        bool _hasStarted;
        bool _isValidating;

        protected DiContainer Container
        {
            get
            {
                return this._sceneContext.Container;
            }
        }

        [SetUp]
        public void SetUp()
        {
            this.ClearScene();
            this._hasStarted = false;
            this._isValidating = this.CurrentTestHasAttribute<ValidateOnlyAttribute>();

            ProjectContext.ValidateOnNextRun = this._isValidating;

            this._sceneContext = new GameObject("SceneContext").AddComponent<SceneContext>();
            this._sceneContext.ParentNewObjectsUnderRoot = true;
            // This creates the container but does not resolve the roots yet
            this._sceneContext.Install();
        }

        public void Initialize()
        {
            Assert.That(!this._hasStarted);
            this._hasStarted = true;

            this._sceneContext.Resolve();

            // This allows them to make very common bindings fields for use in any of the tests
            this.Container.Inject(this);

            if (this._isValidating)
            {
                this.Container.ValidateIValidatables();
            }
            else
            {
                this._sceneContext.gameObject.GetComponent<SceneKernel>().Start();
            }
        }

        [TearDown]
        public void TearDown()
        {
            if (TestContext.CurrentContext.Result.Status == TestStatus.Passed)
            {
                // If we expected an exception then initialize would normally not be called
                // Unless the initialize method itself is what caused the exception
                if (!this.CurrentTestHasAttribute<ExpectedExceptionAttribute>())
                {
                    Assert.That(this._hasStarted, "ZenjectIntegrationTestFixture.Initialize was not called by current test");
                }
            }

            this.ClearScene();
        }

        bool CurrentTestHasAttribute<T>()
            where T : Attribute
        {
            var fullMethodName = TestContext.CurrentContext.Test.FullName;
            var name = fullMethodName.Substring(fullMethodName.LastIndexOf(".")+1);

            return this.GetType().GetMethod(name).GetCustomAttributes(true)
                .Cast<Attribute>().OfType<T>().Any();
        }

        void ClearScene()
        {
            var scene = EditorSceneManager.GetActiveScene();

            // This is the temp scene that unity creates for EditorTestRunner
            Assert.IsEqual(scene.name, "");

            // This will include ProjectContext which is what we want, to ensure no test
            // affects any other test
            foreach (var gameObject in scene.GetRootGameObjects())
            {
                GameObject.DestroyImmediate(gameObject);
            }
        }
    }
}
