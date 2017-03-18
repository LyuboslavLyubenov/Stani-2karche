#if !NOT_UNITY3D

namespace Zenject.Source.Providers.PrefabCreators
{

    using System.Collections.Generic;

    using UnityEngine;

    using Zenject.Source.Binding.BindInfo;
    using Zenject.Source.Injection;

    public interface IPrefabInstantiator
    {
        List<TypeValuePair> ExtraArguments
        {
            get;
        }

        GameObjectCreationParameters GameObjectCreationParameters
        {
            get;
        }

        IEnumerator<GameObject> Instantiate(List<TypeValuePair> args);

        UnityEngine.Object GetPrefab();
    }
}

#endif
