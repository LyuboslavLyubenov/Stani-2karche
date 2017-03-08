#if !NOT_UNITY3D

namespace Assets.Zenject.Source.Util
{

    using System;

    using Assets.Zenject.Source.Install.Contexts;
    using Assets.Zenject.Source.Internal;
    using Assets.Zenject.Source.Main;

    using UnityEngine;
    using UnityEngine.SceneManagement;

    public enum LoadSceneRelationship
    {
        // This will use the ProjectContext container as parent for the new scene
        // This is similar to just running the new scene normally
        None,
        // This will use current scene as parent for the new scene
        // This will allow the new scene to refer to dependencies in the current scene
        Child,
        // This will use the parent of the current scene as the parent for the next scene
        // In most cases this will be the same as None
        Sibling,
    }

    public class ZenjectSceneLoader
    {
        readonly DiContainer _sceneContainer;

        public ZenjectSceneLoader(SceneContext sceneRoot)
        {
            this._sceneContainer = sceneRoot.Container;
        }

        public void LoadScene(string sceneName)
        {
            this.LoadScene(sceneName, LoadSceneMode.Single);
        }

        public void LoadScene(string sceneName, LoadSceneMode loadMode)
        {
            this.LoadScene(sceneName, loadMode, null);
        }

        public void LoadScene(
            string sceneName, LoadSceneMode loadMode, Action<DiContainer> extraBindings)
        {
            this.LoadScene(sceneName, loadMode, extraBindings, LoadSceneRelationship.None);
        }

        public void LoadScene(
            string sceneName,
            LoadSceneMode loadMode,
            Action<DiContainer> extraBindings,
            LoadSceneRelationship containerMode)
        {
            this.PrepareForLoadScene(loadMode, extraBindings, containerMode);

            Assert.That(Application.CanStreamedLevelBeLoaded(sceneName),
                "Unable to load scene '{0}'", sceneName);

            SceneManager.LoadScene(sceneName, loadMode);

            // It would be nice here to actually verify that the new scene has a SceneContext
            // if we have extra binding hooks, or LoadSceneRelationship != None, but
            // we can't do that in this case since the scene isn't loaded until the next frame
        }

        public AsyncOperation LoadSceneAsync(string sceneName)
        {
            return this.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        }

        public AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode loadMode)
        {
            return this.LoadSceneAsync(sceneName, loadMode, null);
        }

        public AsyncOperation LoadSceneAsync(
            string sceneName, LoadSceneMode loadMode, Action<DiContainer> extraBindings)
        {
            return this.LoadSceneAsync(sceneName, loadMode, extraBindings, LoadSceneRelationship.None);
        }

        public AsyncOperation LoadSceneAsync(
            string sceneName,
            LoadSceneMode loadMode,
            Action<DiContainer> extraBindings,
            LoadSceneRelationship containerMode)
        {
            this.PrepareForLoadScene(loadMode, extraBindings, containerMode);

            Assert.That(Application.CanStreamedLevelBeLoaded(sceneName),
                "Unable to load scene '{0}'", sceneName);

            return SceneManager.LoadSceneAsync(sceneName, loadMode);
        }

        void PrepareForLoadScene(
            LoadSceneMode loadMode,
            Action<DiContainer> extraBindings,
            LoadSceneRelationship containerMode)
        {
            if (loadMode == LoadSceneMode.Single)
            {
                Assert.IsEqual(containerMode, LoadSceneRelationship.None);
            }

            if (containerMode == LoadSceneRelationship.None)
            {
                SceneContext.ParentContainer = null;
            }
            else if (containerMode == LoadSceneRelationship.Child)
            {
                SceneContext.ParentContainer = this._sceneContainer;
            }
            else
            {
                Assert.IsEqual(containerMode, LoadSceneRelationship.Sibling);
                SceneContext.ParentContainer = this._sceneContainer.ParentContainer;
            }

            SceneContext.ExtraBindingsInstallMethod = extraBindings;
        }
    }
}

#endif
