namespace Tests
{

    using Assets.Scripts.Interfaces;

    using Interfaces;

    using StateMachine;

    using UnityEngine;

    using Zenject.Source.Usage;

    public class StateActivator : MonoBehaviour
    {
        [Inject]
        private IState state;

        private StateMachine StateMachine = new StateMachine();

        void Start()
        {
            this.StateMachine.SetCurrentState(this.state);
        }
    }

}
