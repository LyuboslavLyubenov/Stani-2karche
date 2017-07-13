namespace Assets.Tests.Utils.UnityTimer
{

    using Assets.Scripts.Interfaces.Utils;

    using UnityEngine;

    using Zenject;

    public class CantStopIfNotStarted : MonoBehaviour
    {
        [Inject]
        private IUnityTimer timer;

        void Start()
        {
            this.timer.StopTimer();
        }
    }
}