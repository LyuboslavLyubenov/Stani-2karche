#if !NOT_UNITY3D

namespace Zenject.Source.Binding.Binders.GameObject
{

    using Zenject.Source.Binding.BindInfo;

    public class GameObjectNameGroupNameScopeBinder : GameObjectGroupNameScopeBinder
    {
        public GameObjectNameGroupNameScopeBinder(
            BindInfo bindInfo,
            GameObjectCreationParameters gameObjectInfo)
            : base(bindInfo, gameObjectInfo)
        {
        }

        public GameObjectGroupNameScopeBinder WithGameObjectName(string gameObjectName)
        {
            this.GameObjectInfo.Name = gameObjectName;
            return this;
        }
    }
}

#endif
