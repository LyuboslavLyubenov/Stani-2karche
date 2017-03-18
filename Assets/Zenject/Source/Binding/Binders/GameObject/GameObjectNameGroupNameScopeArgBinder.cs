#if !NOT_UNITY3D

namespace Zenject.Source.Binding.Binders.GameObject
{

    using Zenject.Source.Binding.BindInfo;

    public class GameObjectNameGroupNameScopeArgBinder : GameObjectGroupNameScopeArgBinder
    {
        public GameObjectNameGroupNameScopeArgBinder(
            BindInfo bindInfo,
            GameObjectCreationParameters gameObjectInfo)
            : base(bindInfo, gameObjectInfo)
        {
        }

        public GameObjectGroupNameScopeArgBinder WithGameObjectName(string gameObjectName)
        {
            this.GameObjectInfo.Name = gameObjectName;
            return this;
        }
    }
}

#endif
