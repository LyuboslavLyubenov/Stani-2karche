#if !NOT_UNITY3D

namespace Zenject.Source.Binding.Binders.GameObject
{

    using System;

    using UnityEngine;

    using Zenject.Source.Binding.BindInfo;
    using Zenject.Source.Main;

    public class GameObjectGroupNameBinder : ConditionBinder
    {
        public GameObjectGroupNameBinder(BindInfo bindInfo, GameObjectCreationParameters gameObjInfo)
            : base(bindInfo)
        {
            this.GameObjectInfo = gameObjInfo;
        }

        protected GameObjectCreationParameters GameObjectInfo
        {
            get;
            private set;
        }

        public ConditionBinder UnderTransform(Transform parent)
        {
            this.GameObjectInfo.ParentTransform = parent;
            return this;
        }

        public ConditionBinder UnderTransform(Func<DiContainer, Transform> parentGetter)
        {
            this.GameObjectInfo.ParentTransformGetter = parentGetter;
            return this;
        }

        public ConditionBinder UnderTransformGroup(string transformGroupname)
        {
            this.GameObjectInfo.GroupName = transformGroupname;
            return this;
        }
    }
}

#endif
