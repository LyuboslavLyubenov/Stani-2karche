#if !NOT_UNITY3D

namespace Zenject.Source.Providers.PrefabCreators
{

    using System.Collections.Generic;

    using UnityEngine;

    using Zenject.Source.Binding.BindInfo;
    using Zenject.Source.Injection;
    using Zenject.Source.Internal;

    public class PrefabInstantiatorCached : IPrefabInstantiator
    {
        readonly IPrefabInstantiator _subInstantiator;

        GameObject _gameObject;

        public PrefabInstantiatorCached(IPrefabInstantiator subInstantiator)
        {
            this._subInstantiator = subInstantiator;
        }

        public List<TypeValuePair> ExtraArguments
        {
            get
            {
                return this._subInstantiator.ExtraArguments;
            }
        }

        public GameObjectCreationParameters GameObjectCreationParameters
        {
            get
            {
                return this._subInstantiator.GameObjectCreationParameters;
            }
        }

        public UnityEngine.Object GetPrefab()
        {
            return this._subInstantiator.GetPrefab();
        }

        public IEnumerator<GameObject> Instantiate(List<TypeValuePair> args)
        {
            // We can't really support arguments if we are using the cached value since
            // the arguments might change when called after the first time
            Assert.IsEmpty(args);

            if (this._gameObject != null)
            {
                yield return this._gameObject;
                yield break;
            }

            var runner = this._subInstantiator.Instantiate(new List<TypeValuePair>());

            // First get instance
            bool hasMore = runner.MoveNext();

            this._gameObject = runner.Current;

            yield return this._gameObject;

            // Now do injection
            while (hasMore)
            {
                hasMore = runner.MoveNext();
            }
        }
    }
}

#endif
