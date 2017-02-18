namespace Assets.Tests
{

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.StateMachine;

    using UnityEngine;

    using Zenject;

    public class StateActivator : MonoBehaviour
    {
        [Inject]
        private IState state;

        private SimpleFiniteStateMachine stateMachine = new SimpleFiniteStateMachine();

        void Start()
        {
            this.stateMachine.SetCurrentState(this.state);
        }
    }

}
