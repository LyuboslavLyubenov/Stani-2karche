using UnityEngine;

namespace Assets.Tests
{

    using Assets.Scripts.Controllers.Jokers;
    using Assets.Scripts.Utils;

    public class Test_SelectRandomJoker : MonoBehaviour
    {
        public SelectRandomJokerUIController SelectRandomJokerController;

        void Start()
        {
        }

        public void Test()
        {
            this.SelectRandomJokerController.gameObject.SetActive(true);
            this.SelectRandomJokerController.PlaySelectRandomJokerAnimation(JokerUtils.AllJokersTypes, 0);
        }
    }

}