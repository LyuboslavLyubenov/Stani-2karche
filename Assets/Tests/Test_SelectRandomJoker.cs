namespace Tests
{

    using Controllers.Jokers;

    using UnityEngine;

    using Utils;

    public class Test_SelectRandomJoker : MonoBehaviour
    {
        public SelectRandomJokerUIController SelectRandomJokerController;

        private void Start()
        {
        }

        public void Test()
        {
            this.SelectRandomJokerController.gameObject.SetActive(true);
            this.SelectRandomJokerController.PlaySelectRandomJokerAnimation(JokerUtils.AllJokersTypes, 0);
        }
    }

}