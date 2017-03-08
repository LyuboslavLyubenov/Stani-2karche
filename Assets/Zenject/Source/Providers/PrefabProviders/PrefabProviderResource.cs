#if !NOT_UNITY3D

namespace Assets.Zenject.Source.Providers.PrefabProviders
{

    using Assets.Zenject.Source.Internal;

    using UnityEngine;

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

