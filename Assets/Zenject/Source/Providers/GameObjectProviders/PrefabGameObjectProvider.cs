#if !NOT_UNITY3D

namespace Zenject.Source.Providers.GameObjectProviders
{

    using System;
    using System.Collections.Generic;

    using UnityEngine;

    using Zenject.Source.Injection;
    using Zenject.Source.Providers.PrefabCreators;

    public class PrefabGameObjectProvider : IProvider
    {
        readonly IPrefabInstantiator _prefabCreator;

        public PrefabGameObjectProvider(
            IPrefabInstantiator prefabCreator)
        {
            this._prefabCreator = prefabCreator;
        }

        public Type GetInstanceType(InjectContext context)
        {
            return typeof(GameObject);
        }

        public IEnumerator<List<object>> GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args)
        {
            var runner = this._prefabCreator.Instantiate(args);

            // First get instance
            bool hasMore = runner.MoveNext();

            var instance = runner.Current;

            yield return new List<object>() { instance };

            // Now do injection
            while (hasMore)
            {
                hasMore = runner.MoveNext();
            }
        }
    }
}

#endif
