namespace Assets.Zenject.Source.Binding.Binders.Factory.FactoryFromBinder.SubContainerBinder
{

    using System;

    using Assets.Zenject.Source.Binding.Binders.GameObject;
    using Assets.Zenject.Source.Binding.BindInfo;
    using Assets.Zenject.Source.Binding.Finalizers;
    using Assets.Zenject.Source.Install;
    using Assets.Zenject.Source.Internal;
    using Assets.Zenject.Source.Providers;
    using Assets.Zenject.Source.Providers.PrefabProviders;
    using Assets.Zenject.Source.Providers.SubContainerCreators;

    public class FactorySubContainerBinderWithParams<TContract> : FactorySubContainerBinderBase<TContract>
    {
        public FactorySubContainerBinderWithParams(
            BindInfo bindInfo, Type factoryType,
            BindFinalizerWrapper finalizerWrapper, object subIdentifier)
            : base(bindInfo, factoryType, finalizerWrapper, subIdentifier)
        {
        }

#if !NOT_UNITY3D

        public GameObjectNameGroupNameBinder ByPrefab<TInstaller>(UnityEngine.Object prefab)
            where TInstaller : IInstaller
        {
            return this.ByPrefab(typeof(TInstaller), prefab);
        }

        public GameObjectNameGroupNameBinder ByPrefab(Type installerType, UnityEngine.Object prefab)
        {
            BindingUtil.AssertIsValidPrefab(prefab);

            Assert.That(installerType.DerivesFrom<MonoInstaller>(),
                "Invalid installer type given during bind command.  Expected type '{0}' to derive from 'MonoInstaller'", installerType.Name());

            var gameObjectInfo = new GameObjectCreationParameters();

            this.SubFinalizer = this.CreateFinalizer(
                (container) => new SubContainerDependencyProvider(
                    this.ContractType, this.SubIdentifier,
                    new SubContainerCreatorByPrefabWithParams(
                        installerType,
                        container,
                        new PrefabProvider(prefab),
                        gameObjectInfo)));

            return new GameObjectNameGroupNameBinder(this.BindInfo, gameObjectInfo);
        }

        public GameObjectNameGroupNameBinder ByPrefabResource<TInstaller>(string resourcePath)
            where TInstaller : IInstaller
        {
            return this.ByPrefabResource(typeof(TInstaller), resourcePath);
        }

        public GameObjectNameGroupNameBinder ByPrefabResource(
            Type installerType, string resourcePath)
        {
            BindingUtil.AssertIsValidResourcePath(resourcePath);

            var gameObjectInfo = new GameObjectCreationParameters();

            this.SubFinalizer = this.CreateFinalizer(
                (container) => new SubContainerDependencyProvider(
                    this.ContractType, this.SubIdentifier,
                    new SubContainerCreatorByPrefabWithParams(
                        installerType,
                        container,
                        new PrefabProviderResource(resourcePath),
                        gameObjectInfo)));

            return new GameObjectNameGroupNameBinder(this.BindInfo, gameObjectInfo);
        }
#endif
    }
}
