#if !NOT_UNITY3D

#endif

namespace Zenject.Source.Binding.Binders.Factory
{

    using System;
    using System.Collections.Generic;

    using UnityEngine;

    using Zenject.Source.Binding.Binders.GameObject;
    using Zenject.Source.Binding.BindInfo;
    using Zenject.Source.Binding.Finalizers;
    using Zenject.Source.Factories;
    using Zenject.Source.Injection;
    using Zenject.Source.Internal;
    using Zenject.Source.Main;
    using Zenject.Source.Providers;
    using Zenject.Source.Providers.ComponentProviders;
    using Zenject.Source.Providers.ComponentProviders.AddToGameObjectComponentProviders;
    using Zenject.Source.Providers.GameObjectProviders;
    using Zenject.Source.Providers.PrefabCreators;
    using Zenject.Source.Providers.PrefabProviders;

    public class FactoryFromBinderBase<TContract> : ConditionBinder
    {
        public FactoryFromBinderBase(
            BindInfo bindInfo,
            Type factoryType,
            BindFinalizerWrapper finalizerWrapper)
            : base(bindInfo)
        {
            // Note that it doesn't derive from Factory<TContract>
            // when used with To<>, so we can only check IDynamicFactory
            Assert.That(factoryType.DerivesFrom<IDynamicFactory>());

            this.FactoryType = factoryType;
            this.FinalizerWrapper = finalizerWrapper;

            // Default to just creating it using new
            finalizerWrapper.SubFinalizer = this.CreateFinalizer(
                (container) => new TransientProvider(this.ContractType, container));
        }

        protected Type FactoryType
        {
            get;
            private set;
        }

        protected Type ContractType
        {
            get
            {
                return typeof(TContract);
            }
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

        public IEnumerable<Type> AllParentTypes
        {
            get
            {
                yield return this.ContractType;

                foreach (var type in this.BindInfo.ToTypes)
                {
                    yield return type;
                }
            }
        }

        protected IBindingFinalizer CreateFinalizer(Func<DiContainer, IProvider> providerFunc)
        {
            return new DynamicFactoryBindingFinalizer<TContract>(
                this.BindInfo, this.FactoryType, providerFunc);
        }

        // Note that this isn't necessary to call since it's the default
        public ConditionBinder FromNew()
        {
            BindingUtil.AssertIsNotComponent(this.ContractType);
            BindingUtil.AssertIsNotAbstract(this.ContractType);

            return this;
        }

        public ConditionBinder FromResolve()
        {
            return this.FromResolve(null);
        }

        public ConditionBinder FromResolve(object subIdentifier)
        {
            this.SubFinalizer = this.CreateFinalizer(
                (container) => new ResolveProvider(
                    this.ContractType, container, subIdentifier, false));

            return this;
        }

#if !NOT_UNITY3D

        public GameObjectNameGroupNameBinder FromGameObject()
        {
            var gameObjectInfo = new GameObjectCreationParameters();

            if (this.ContractType == typeof(GameObject))
            {
                this.SubFinalizer = this.CreateFinalizer(
                    (container) => new EmptyGameObjectProvider(
                        container, gameObjectInfo));
            }
            else
            {
                BindingUtil.AssertIsComponent(this.ContractType);
                BindingUtil.AssertIsNotAbstract(this.ContractType);

                this.SubFinalizer = this.CreateFinalizer(
                    (container) => new AddToNewGameObjectComponentProvider(
                        container, this.ContractType, null,
                        new List<TypeValuePair>(), gameObjectInfo));
            }

            return new GameObjectNameGroupNameBinder(this.BindInfo, gameObjectInfo);
        }

        public ConditionBinder FromComponent(GameObject gameObject)
        {
            BindingUtil.AssertIsValidGameObject(gameObject);
            BindingUtil.AssertIsComponent(this.ContractType);
            BindingUtil.AssertIsNotAbstract(this.ContractType);

            this.SubFinalizer = this.CreateFinalizer(
                (container) => new AddToExistingGameObjectComponentProvider(
                    gameObject, container, this.ContractType,
                    null, new List<TypeValuePair>()));

            return this;
        }

        public GameObjectNameGroupNameBinder FromPrefab(UnityEngine.Object prefab)
        {
            BindingUtil.AssertIsValidPrefab(prefab);

            var gameObjectInfo = new GameObjectCreationParameters();

            if (this.ContractType == typeof(GameObject))
            {
                this.SubFinalizer = this.CreateFinalizer(
                    (container) => new PrefabGameObjectProvider(
                        new PrefabInstantiator(
                            container, gameObjectInfo, 
                            new List<TypeValuePair>(), new PrefabProvider(prefab))));
            }
            else
            {
                BindingUtil.AssertIsAbstractOrComponent(this.ContractType);

                this.SubFinalizer = this.CreateFinalizer(
                    (container) => new GetFromPrefabComponentProvider(
                        this.ContractType,
                        new PrefabInstantiator(
                            container, gameObjectInfo, 
                            new List<TypeValuePair>(), new PrefabProvider(prefab))));
            }

            return new GameObjectNameGroupNameBinder(this.BindInfo, gameObjectInfo);
        }

        public GameObjectNameGroupNameBinder FromPrefabResource(string resourcePath)
        {
            BindingUtil.AssertIsValidResourcePath(resourcePath);

            var gameObjectInfo = new GameObjectCreationParameters();

            if (this.ContractType == typeof(GameObject))
            {
                this.SubFinalizer = this.CreateFinalizer(
                    (container) => new PrefabGameObjectProvider(
                        new PrefabInstantiator(
                            container, gameObjectInfo, 
                            new List<TypeValuePair>(), new PrefabProviderResource(resourcePath))));
            }
            else
            {
                BindingUtil.AssertIsAbstractOrComponent(this.ContractType);

                this.SubFinalizer = this.CreateFinalizer(
                    (container) => new GetFromPrefabComponentProvider(
                        this.ContractType,
                        new PrefabInstantiator(
                            container, gameObjectInfo, 
                            new List<TypeValuePair>(), new PrefabProviderResource(resourcePath))));
            }

            return new GameObjectNameGroupNameBinder(this.BindInfo, gameObjectInfo);
        }
#endif
    }
}
