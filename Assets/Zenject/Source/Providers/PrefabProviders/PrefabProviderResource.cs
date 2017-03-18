#if !NOT_UNITY3D

namespace Zenject.Source.Providers.PrefabProviders
{

    using UnityEngine;

    using Zenject.Source.Internal;

    public class PrefabProviderResource : IPrefabProvider
    {
        readonly string _resourcePath;

        public PrefabProviderResource(string resourcePath)
        {
            this._resourcePath = resourcePath;
        }

        public UnityEngine.Object GetPrefab()
        {
            var prefab = (GameObject)Resources.Load(this._resourcePath);

            Assert.IsNotNull(prefab,
                "Expected to find prefab at resource path '{0}'", this._resourcePath);

            return prefab;
        }
    }
}

#endif

