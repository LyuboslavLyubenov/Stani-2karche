namespace Assets.Zenject.Source.Binding.Binders
{

    using System;

    using Assets.Zenject.Source.Binding.Binders.GameObject;
    using Assets.Zenject.Source.Binding.BindInfo;
    using Assets.Zenject.Source.Binding.Finalizers;
    using Assets.Zenject.Source.Install;
    using Assets.Zenject.Source.Internal;
    using Assets.Zenject.Source.Main;

    public class SubContainerBinder
    {
        readonly BindInfo _bindInfo;
        readonly BindFinalizerWrapper _finalizerWrapper;
        readonly object _subIdentifier;

        public SubContainerBinder(
            BindInfo bindInfo,
            BindFinalizerWrapper finalizerWrapper,
            object subIdentifier)
        {
            this._bindInfo = bindInfo;
            this._finalizerWrapper = finalizerWrapper;
            this._subIdentifier = subIdentifier;

            // Reset in case the user ends the binding here
            finalizerWrapper.SubFinalizer = null;
        }

        protected IBindingFinalizer SubFinalizer
        {
            set
            {
                this._finalizerWrapper.SubFinalizer = value;
            }
        }

        public ScopeBinder ByInstaller<TInstaller>()
            where TInstaller : InstallerBase
        {
            return this.ByInstaller(typeof(TInstaller));
        }

        public ScopeBinder ByInstaller(Type installerType)
        {
            Assert.That(installerType.DerivesFrom<InstallerBase>(),
                "Invalid installer type given during bind command.  Expected type '{0}' to derive from 'Installer<>'", installerType.Name());

            this.SubFinalizer = new SubContainerInstallerBindingFinalizer(
                this._bindInfo, installerType, this._subIdentifier);

            return new ScopeBinder(this._bindInfo);
        }

        public ScopeBinder ByMethod(Action<DiContainer> installerMethod)
        {
            this.SubFinalizer = new SubContainerMethodBindingFinalizer(
                this._bindInfo, installerMethod, this._subIdentifier);

            return new ScopeBinder(this._bindInfo);
        }

#if !NOT_UNITY3D

        public GameObjectNameGroupNameScopeBinder ByPrefab(UnityEngine.Object prefab)
        {
            BindingUtil.AssertIsValidPrefab(prefab);

            var gameObjectInfo = new GameObjectCreationParameters();

            this.SubFinalizer = new SubContainerPrefabBindingFinalizer(
                this._bindInfo, gameObjectInfo, prefab, this._subIdentifier);

            return new GameObjectNameGroupNameScopeBinder(this._bindInfo, gameObjectInfo);
        }

        public GameObjectNameGroupNameScopeBinder ByPrefabResource(string resourcePath)
        {
            BindingUtil.AssertIsValidResourcePath(resourcePath);

            var gameObjectInfo = new GameObjectCreationParameters();

            this.SubFinalizer = new SubContainerPrefabResourceBindingFinalizer(
                this._bindInfo, gameObjectInfo, resourcePath, this._subIdentifier);

            return new GameObjectNameGroupNameScopeBinder(this._bindInfo, gameObjectInfo);
        }
#endif
    }
}
