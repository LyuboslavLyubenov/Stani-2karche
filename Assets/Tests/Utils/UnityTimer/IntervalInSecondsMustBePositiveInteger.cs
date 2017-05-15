﻿namespace Assets.Tests.Utils.UnityTimer
{

    using Assets.Scripts.Interfaces.Utils;

    using UnityEngine;

    using Zenject.Source.Usage;

    public class IntervalInSecondsMustBePositiveInteger : MonoBehaviour
    {
        [Inject]
        private IUnityTimer timer;

        void Start()
        {
            this.timer.InvervalInSeconds = -2;
            this.timer.StartTimer();
        }
    }

}