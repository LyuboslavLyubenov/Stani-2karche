#if !NOT_UNITY3D

namespace Zenject.Source.Providers.PrefabProviders
{

    using Zenject.Source.Internal;

    public class PrefabProvider : IPrefabProvider
    {
        readonly UnityEngine.Object _prefab;

        public PrefabProvider(UnityEngine.Object prefab)
        {
            Assert.IsNotNull(prefab);
            this._prefab = prefab;
        }

        public UnityEngine.Object GetPrefab()
        {
            return this._prefab;
        }
    }
}

#endif


