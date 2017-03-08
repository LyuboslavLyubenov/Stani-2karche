#if !NOT_UNITY3D

namespace Assets.Zenject.Source.Binding.Binders.GameObject
{

    using System;

    using Assets.Zenject.Source.Binding.BindInfo;
    using Assets.Zenject.Source.Main;

    using UnityEngine;

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
