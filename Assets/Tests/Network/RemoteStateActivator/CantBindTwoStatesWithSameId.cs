namespace Assets.Tests.Network.RemoteStateActivator
{
    using Assets.Scripts.Interfaces.Network;
    using Assets.Tests.DummyObjects.States.EveryBodyVsTheTeacher;

    using UnityEngine;

    using Zenject.Source.Usage;

    public class CantBindTwoStatesWithSameId : MonoBehaviour
    {
        [Inject]
        private IRemoteStateActivator stateActivator;

        void Start()
        {
            this.stateActivator.Bind("State", new DummyRoundState());
            this.stateActivator.Bind("State", new DummyRoundState());
        }
    }
}