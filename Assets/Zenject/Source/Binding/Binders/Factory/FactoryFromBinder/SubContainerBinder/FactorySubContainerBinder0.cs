namespace Zenject.Source.Binding.Binders.Factory.FactoryFromBinder.SubContainerBinder
{

    using System;

    using Zenject.Source.Binding.Binders.GameObject;
    using Zenject.Source.Binding.BindInfo;
    using Zenject.Source.Binding.Finalizers;
    using Zenject.Source.Main;
    using Zenject.Source.Providers;
    using Zenject.Source.Providers.PrefabProviders;
    using Zenject.Source.Providers.SubContainerCreators;

    public class FactorySubContainerBinder<TContract>
        : FactorySubContainerBinderBase<TContract>
    {
        public FactorySubContainerBinder(
            BindInfo bindInfo, Type factoryType,
            BindFinalizerWrapper finalizerWrapper, object subIdentifier)
            : base(bindInfo, factoryType, finalizerWrapper, subIdentifier)
        {
        }

        public ConditionBinder ByMethod(Action<DiContainer> installerMethod)
        {
            this.SubFinalizer = this.CreateFinalizer(
                (container) => new SubContainerDependencyProvider(
                    this.ContractType, this.SubIdentifier,
                    new SubContainerCreatorByMethod(
                        container, installerMethod)));

            return new ConditionBinder(this.BindInfo);
        }

#if !NOT_UNITY3D

        public GameObjectNameGroupNameBinder ByPrefab(UnityEngine.Object prefab)
        {
            BindingUtil.AssertIsValidPrefab(prefab);

            var gameObjectInfo = new GameObjectCreationParameters();

            this.SubFinalizer = this.CreateFinalizer(
                (container) => new SubContainerDependencyProvider(
                    this.ContractType, this.SubIdentifier,
                    new SubContainerCreatorByPrefab(
                        container,
                        new PrefabProvider(prefab),
                        gameObjectInfo)));

            return new GameObjectNameGroupNameBinder(this.BindInfo, gameObjectInfo);
        }

        public GameObjectNameGroupNameBinder ByPrefabResource(string resourcePath)
        {
            BindingUtil.AssertIsValidResourcePath(resourcePath);

            var gameObjectInfo = new GameObjectCreationParameters();

            this.SubFinalizer = this.CreateFinalizer(
                (container) => new SubContainerDependencyProvider(
                    this.ContractType, this.SubIdentifier,
                    new SubContainerCreatorByPrefab(
                        container,
                        new PrefabProviderResource(resourcePath),
                        gameObjectInfo)));

            return new GameObjectNameGroupNameBinder(this.BindInfo, gameObjectInfo);
        }
#endif
    }
}
