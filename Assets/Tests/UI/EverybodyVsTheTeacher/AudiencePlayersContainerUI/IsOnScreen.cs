using ObjectsPool = Utils.Unity.ObjectsPool;

namespace Assets.Tests.UI.EverybodyVsTheTeacher.AudiencePlayersContainerUI
{
    using System.Collections;

    using Interfaces.Controllers;

    using UnityEngine;
    using UnityEngine.UI;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class IsOnScreen : MonoBehaviour
    {
        [Inject]
        private IAudiencePlayersContainerUIController audiencePlayersContainerUIController;

        void Start()
        {
            this.StartCoroutine(this.StartTestCoroutine());
        }

        IEnumerator StartTestCoroutine()
        {
            this.audiencePlayersContainerUIController.ShowAudiencePlayer(1, "Player 1");

            yield return new WaitForSeconds(2f);

            if (!this.audiencePlayersContainerUIController.IsOnScreen(1))
            {
                IntegrationTest.Fail();
                yield break;
            }

            var objectsPool = GameObject.FindObjectOfType<ObjectsPool>()
                .gameObject
                .transform;

            for (int i = 0; i < objectsPool.childCount; i++)
            {
                var audiencePlayerObject = objectsPool.GetChild(i).gameObject;

                if (!audiencePlayerObject.activeSelf)
                {
                    continue;
                }

                var audiencePlayerUsername = audiencePlayerObject.GetComponentInChildren<Text>().text;

                if (audiencePlayerUsername != "Player 1")
                {
                    IntegrationTest.Fail();
                    yield break;
                }
            }

            IntegrationTest.Pass();
        }
    }
}