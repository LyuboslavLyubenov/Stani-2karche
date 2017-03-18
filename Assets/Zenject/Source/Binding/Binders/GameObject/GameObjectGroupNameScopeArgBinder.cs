#if !NOT_UNITY3D

namespace Zenject.Source.Binding.Binders.GameObject
{

    using System;

    using UnityEngine;

    using Zenject.Source.Binding.BindInfo;
    using Zenject.Source.Main;

    public class GameObjectGroupNameScopeArgBinder : ScopeArgBinder
    {
        public GameObjectGroupNameScopeArgBinder(
            BindInfo bindInfo,
            GameObjectCreationParameters gameObjectInfo)
            : base(bindInfo)
        {
            this.GameObjectInfo = gameObjectInfo;
        }

        protected GameObjectCreationParameters GameObjectInfo
        {
            get;
            private set;
        }

        public ScopeArgBinder UnderTransform(Transform parent)
        {
            this.GameObjectInfo.ParentTransform = parent;
            return this;
        }

        public ScopeArgBinder UnderTransform(Func<DiContainer, Transform> parentGetter)
        {
            this.GameObjectInfo.ParentTransformGetter = parentGetter;
            return this;
        }

        public ScopeArgBinder UnderTransformGroup(string transformGroupname)
        {
            this.GameObjectInfo.GroupName = transformGroupname;
            return this;
        }
    }
}

#endif
