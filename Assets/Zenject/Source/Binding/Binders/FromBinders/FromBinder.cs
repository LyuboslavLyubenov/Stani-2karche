#if !NOT_UNITY3D
#endif

namespace Zenject.Source.Binding.Binders.FromBinders
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using UnityEngine;

    using Zenject.Source.Binding.Binders.GameObject;
    using Zenject.Source.Binding.BindInfo;
    using Zenject.Source.Binding.Finalizers;
    using Zenject.Source.Factories;
    using Zenject.Source.Injection;
    using Zenject.Source.Internal;
    using Zenject.Source.Providers;
    using Zenject.Source.Providers.ComponentProviders.AddToGameObjectComponentProviders;
    using Zenject.Source.Providers.GameObjectProviders;
    using Zenject.Source.Providers.Singleton;
    using Zenject.Source.Util;

    public abstract class FromBinder : ScopeArgBinder
    {
        public FromBinder(
            BindInfo bindInfo,
            BindFinalizerWrapper finalizerWrapper)
            : base(bindInfo)
        {
            this.FinalizerWrapper = finalizerWrapper;
        }

        protected BindFinalizerWrapper FinalizerWrapper
        {
            get;
            private set;
        }

        protected IBindingFinalizer SubFinalizer
        {
            set
            {
                this.FinalizerWrapper.SubFinalizer = value;
            }
        }

        protected IEnumerable<Type> AllParentTypes
        {
            get
            {
                return this.BindInfo.ContractTypes.Concat(this.BindInfo.ToTypes);
            }
        }

        protected IEnumerable<Type> ConcreteTypes
        {
            get
            {
                if (this.BindInfo.ToChoice == ToChoices.Self)
                {
                    return this.BindInfo.ContractTypes;
                }

                Assert.IsNotEmpty(this.BindInfo.ToTypes);
                return this.BindInfo.ToTypes;
            }
        }

        // This is the default if nothing else is called
        public ScopeArgBinder FromNew()
        {
            BindingUtil.AssertTypesAreNotComponents(this.ConcreteTypes);
            BindingUtil.AssertTypesAreNotAbstract(this.ConcreteTypes);

            return this;
        }

        public ScopeBinder FromResolve()
        {
            return this.FromResolve(null);
        }

        public ScopeBinder FromResolve(object subIdentifier)
        {
            this.SubFinalizer = new ScopableBindingFinalizer(
                this.BindInfo,
                SingletonTypes.ToResolve, subIdentifier,
                (container, type) => new ResolveProvider(type, container, subIdentifier, false));

            return new ScopeBinder(this.BindInfo);
        }

        public SubContainerBinder FromSubContainerResolve()
        {
            return this.FromSubContainerResolve(null);
        }

        public SubContainerBinder FromSubContainerResolve(object subIdentifier)
        {
            return new SubContainerBinder(
                this.BindInfo, this.FinalizerWrapper, subIdentifier);
        }

        public ScopeArgBinder FromFactory(Type factoryType)
        {
            Assert.That(factoryType.DerivesFrom<IFactory>());

            this.SubFinalizer = new ScopableBindingFinalizer(
                this.BindInfo,
                SingletonTypes.ToFactory, factoryType,
                (container, type) => new UntypedFactoryProvider(
                    factoryType, container, this.BindInfo.Arguments));

            return new ScopeArgBinder(this.BindInfo);
        }

#if !NOT_UNITY3D

        public ScopeArgBinder FromComponent(GameObject gameObject)
        {
            BindingUtil.AssertIsValidGameObject(gameObject);
            BindingUtil.AssertIsComponent(this.ConcreteTypes);
            BindingUtil.AssertTypesAreNotAbstract(this.ConcreteTypes);

            this.SubFinalizer = new ScopableBindingFinalizer(
                this.BindInfo, SingletonTypes.ToComponentGameObject, gameObject,
                (container, type) => new AddToExistingGameObjectComponentProvider(
                    gameObject, container, type, this.BindInfo.ConcreteIdentifier, this.BindInfo.Arguments));

            return new ScopeArgBinder(this.BindInfo);
        }

        public ArgumentsBinder FromSiblingComponent()
        {
            BindingUtil.AssertIsComponent(this.ConcreteTypes);
            BindingUtil.AssertTypesAreNotAbstract(this.ConcreteTypes);

            this.SubFinalizer = new SingleProviderBindingFinalizer(
                this.BindInfo, (container, type) => new AddToCurrentGameObjectComponentProvider(
                    container, type, this.BindInfo.ConcreteIdentifier, this.BindInfo.Arguments));

            return new ArgumentsBinder(this.BindInfo);
        }

        public GameObjectNameGroupNameScopeArgBinder FromGameObject()
        {
            BindingUtil.AssertIsAbstractOrComponentOrGameObject(this.BindInfo.ContractTypes);
            BindingUtil.AssertIsComponentOrGameObject(this.ConcreteTypes);

            var gameObjectInfo = new GameObjectCreationParameters();

            if (this.ConcreteTypes.All(x => x == typeof(GameObject)))
            {
                this.SubFinalizer = new ScopableBindingFinalizer(
                    this.BindInfo, SingletonTypes.ToGameObject, gameObjectInfo,
                    (container, type) =>
                    {
                        Assert.That(this.BindInfo.Arguments.IsEmpty(), "Cannot inject arguments into empty game object");
                        return new EmptyGameObjectProvider(
                            container, gameObjectInfo);
                    });
            }
            else
            {
                BindingUtil.AssertIsComponent(this.ConcreteTypes);
                BindingUtil.AssertTypesAreNotAbstract(this.ConcreteTypes);

                this.SubFinalizer = new ScopableBindingFinalizer(
                    this.BindInfo, SingletonTypes.ToGameObject, gameObjectInfo,
                    (container, type) => new AddToNewGameObjectComponentProvider(
                        container,
                        type,
                        this.BindInfo.ConcreteIdentifier,
                        this.BindInfo.Arguments,
                        gameObjectInfo));
            }

            return new GameObjectNameGroupNameScopeArgBinder(this.BindInfo, gameObjectInfo);
        }

        public GameObjectNameGroupNameScopeArgBinder FromPrefab(UnityEngine.Object prefab)
        {
            BindingUtil.AssertIsValidPrefab(prefab);
            BindingUtil.AssertIsAbstractOrComponentOrGameObject(this.AllParentTypes);

            var gameObjectInfo = new GameObjectCreationParameters();

            this.SubFinalizer = new PrefabBindingFinalizer(
                this.BindInfo, gameObjectInfo, prefab);

            return new GameObjectNameGroupNameScopeArgBinder(this.BindInfo, gameObjectInfo);
        }

        public GameObjectNameGroupNameScopeArgBinder FromPrefabResource(string resourcePath)
        {
            BindingUtil.AssertIsValidResourcePath(resourcePath);
            BindingUtil.AssertIsAbstractOrComponentOrGameObject(this.AllParentTypes);

            var gameObjectInfo = new GameObjectCreationParameters();

            this.SubFinalizer = new PrefabResourceBindingFinalizer(
                this.BindInfo, gameObjectInfo, resourcePath);

            return new GameObjectNameGroupNameScopeArgBinder(this.BindInfo, gameObjectInfo);
        }

        public ScopeBinder FromResource(string resourcePath)
        {
            BindingUtil.AssertDerivesFromUnityObject(this.ConcreteTypes);

            this.SubFinalizer = new ScopableBindingFinalizer(
                this.BindInfo,
                SingletonTypes.ToResource,
                resourcePath.ToLower(),
                (_, type) => new ResourceProvider(resourcePath, type));

            return new ScopeBinder(this.BindInfo);
        }

#endif

        public ScopeArgBinder FromMethod(Func<InjectContext, object> method)
        {
            this.SubFinalizer = new ScopableBindingFinalizer(
                this.BindInfo,
                SingletonTypes.ToMethod, new SingletonImplIds.ToMethod(method),
                (container, type) => new MethodProviderUntyped(method, container));

            return this;
        }

        protected ScopeArgBinder FromMethodBase<TConcrete>(Func<InjectContext, TConcrete> method)
        {
            BindingUtil.AssertIsDerivedFromTypes(typeof(TConcrete), this.AllParentTypes);

            this.SubFinalizer = new ScopableBindingFinalizer(
                this.BindInfo,
                SingletonTypes.ToMethod, new SingletonImplIds.ToMethod(method),
                (container, type) => new MethodProvider<TConcrete>(method, container));

            return this;
        }

        protected ScopeArgBinder FromFactoryBase<TConcrete, TFactory>()
            where TFactory : IFactory<TConcrete>
        {
            BindingUtil.AssertIsDerivedFromTypes(typeof(TConcrete), this.AllParentTypes);

            this.SubFinalizer = new ScopableBindingFinalizer(
                this.BindInfo,
                SingletonTypes.ToFactory, typeof(TFactory),
                (container, type) => new FactoryProvider<TConcrete, TFactory>(container, this.BindInfo.Arguments));

            return new ScopeArgBinder(this.BindInfo);
        }

        protected ScopeBinder FromResolveGetterBase<TObj, TResult>(
            object identifier, Func<TObj, TResult> method)
        {
            BindingUtil.AssertIsDerivedFromTypes(typeof(TResult), this.AllParentTypes);

            this.SubFinalizer = new ScopableBindingFinalizer(
                this.BindInfo,
                SingletonTypes.ToGetter,
                new SingletonImplIds.ToGetter(identifier, method),
                (container, type) => new GetterProvider<TObj, TResult>(identifier, method, container));

            return new ScopeBinder(this.BindInfo);
        }

        protected ScopeBinder FromInstanceBase(object instance, bool allowNull)
        {
            if (!allowNull)
            {
                Assert.That(!ZenUtilInternal.IsNull(instance),
                    "Found null instance for type '{0}' in FromInstance method",
                    this.ConcreteTypes.First().Name());
            }

            BindingUtil.AssertInstanceDerivesFromOrEqual(instance, this.AllParentTypes);

            this.SubFinalizer = new ScopableBindingFinalizer(
                this.BindInfo, SingletonTypes.ToInstance, instance,
                (container, type) => new InstanceProvider(container, type, instance));

            return new ScopeBinder(this.BindInfo);
        }
    }
}
