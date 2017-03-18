#if !NOT_UNITY3D

namespace Zenject.Source.Binding.Binders.GameObject
{

    using Zenject.Source.Binding.BindInfo;

    public class GameObjectNameGroupNameBinder : GameObjectGroupNameBinder
    {
        public GameObjectNameGroupNameBinder(
            BindInfo bindInfo, GameObjectCreationParameters gameObjectInfo)
            : base(bindInfo, gameObjectInfo)
        {
        }

        public GameObjectGroupNameBinder WithGameObjectName(string gameObjectName)
        {
            this.GameObjectInfo.Name = gameObjectName;
            return this;
        }
    }
}

#endif
