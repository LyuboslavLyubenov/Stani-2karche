#if !NOT_UNITY3D

namespace Assets.Zenject.Source.Providers.ComponentProviders
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Assets.Zenject.Source.Injection;
    using Assets.Zenject.Source.Internal;
    using Assets.Zenject.Source.Providers.PrefabCreators;

    public class GetFromPrefabComponentProvider : IProvider
    {
        readonly IPrefabInstantiator _prefabInstantiator;
        readonly Type _componentType;

        // if concreteType is null we use the contract type from inject context
        public GetFromPrefabComponentProvider(
            Type componentType,
            IPrefabInstantiator prefabInstantiator)
        {
            this._prefabInstantiator = prefabInstantiator;
            this._componentType = componentType;
        }

        public Type GetInstanceType(InjectContext context)
        {
            return this._componentType;
        }

        public IEnumerator<List<object>> GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args)
        {
            Assert.IsNotNull(context);

            var gameObjectRunner = this._prefabInstantiator.Instantiate(args);

            // First get instance
            bool hasMore = gameObjectRunner.MoveNext();

            var gameObject = gameObjectRunner.Current;

            var allComponents = gameObject.GetComponentsInChildren(this._componentType);

            Assert.That(allComponents.Length >= 1,
                "Expected to find at least one component with type '{0}' on prefab '{1}'",
                this._componentType, this._prefabInstantiator.GetPrefab().name);

            yield return allComponents.Cast<object>().ToList();

            // Now do injection
            while (hasMore)
            {
                hasMore = gameObjectRunner.MoveNext();
            }
        }
    }
}

#endif
