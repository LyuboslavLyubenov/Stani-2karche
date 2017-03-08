#if !NOT_UNITY3D

namespace Assets.Zenject.Source.Providers
{

    using System;
    using System.Collections.Generic;

    using Assets.Zenject.Source.Injection;
    using Assets.Zenject.Source.Internal;

    using UnityEngine;

    public class ResourceProvider : IProvider
    {
        readonly Type _resourceType;
        readonly string _resourcePath;

        public ResourceProvider(
            string resourcePath, Type resourceType)
        {
            this._resourceType = resourceType;
            this._resourcePath = resourcePath;
        }

        public Type GetInstanceType(InjectContext context)
        {
            return this._resourceType;
        }

        public IEnumerator<List<object>> GetAllInstancesWithInjectSplit(InjectContext context, List<TypeValuePair> args)
        {
            Assert.IsEmpty(args);

            Assert.IsNotNull(context);

            var obj = Resources.Load(this._resourcePath, this._resourceType);

            Assert.IsNotNull(obj,
                "Could not find resource at path '{0}' with type '{1}'", this._resourcePath, this._resourceType);

            yield return new List<object>() { obj };

            // Are there any resource types which can be injected?
        }
    }
}

#endif



