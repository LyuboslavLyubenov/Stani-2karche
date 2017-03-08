#if !NOT_UNITY3D

namespace Assets.Zenject.Source.Providers.PrefabProviders
{

    using Assets.Zenject.Source.Internal;

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


