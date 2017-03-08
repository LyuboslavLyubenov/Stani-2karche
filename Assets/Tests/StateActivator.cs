﻿namespace Assets.Tests
{

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.StateMachine;
    using Assets.Zenject.Source.Usage;

    using UnityEngine;

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
