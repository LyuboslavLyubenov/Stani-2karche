namespace Assets.Tests.Network.RemoteStateActivator
{
    using Assets.Scripts.Interfaces.Network;
    using Assets.Tests.DummyObjects.States.EveryBodyVsTheTeacher;

    using UnityEngine;

    using Zenject.Source.Usage;

    public class CantUnBindNonExistingState : MonoBehaviour
    {
        [Inject]
        private IRemoteStateActivator stateActivator;

        void Start()
        {
            var dummyState = new DummyRoundState();
            this.stateActivator.UnBind(dummyState);
        }
    }
}